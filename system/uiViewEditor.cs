using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CliLib;
using System.Runtime.InteropServices;

namespace system
{
    public partial class uiViewEditor : Form
    {
        public Renderer mRender = new Renderer();
        //public uiViewManager mUIMgr = uiViewManager.Inst;
        public UIEngine mUIEngine;

        public const int DetectPixelSize = 5; //선택된 view의 외곽선 감지 범위
        public ViewInfo EditableView = null; //선택된 view
        public ViewInfo TempDownView = null; //단순히 다운된 view(업시 null)
        public Point PreviousPt = new Point(); //마우스 다운시 클릭지점 백업
        public JObject EditableJson;

        public uiViewEditor()
        {
            InitializeComponent();

            string[] enums = Enum.GetNames(typeof(uiViewType));
            cbViewType.Items.AddRange(enums);
            cbViewType.SelectedIndex = 0;

            InitRenderer();
            InitUIManager();

            Timer timer = new Timer();
            timer.Interval = 20;
            timer.Tick += (ss, ee) => {
                //mUIEngine.DoMouseEvent();
                mRender.mGlView.Invalidate();
            };
            timer.Start();
        }

        public void InitRenderer()
        {
            mRender.Initialize(panel1, panel1.Width, panel1.Height);

            mRender.mGlView.MouseMove += MGlView_MouseMove;
            mRender.mGlView.MouseDown += MGlView_MouseDown;
            mRender.mGlView.MouseUp += MGlView_MouseUp;
            mRender.OnDraw += MGlView_Draw;
        }
        public void InitUIManager()
        {
            mUIEngine = UIEngine.GetInst();
            mUIEngine.SetResourcePath("../../res/");
            mUIEngine.Init(new MyUIEngineCallbacks(mRender));
        }

        //단순히 마우스 Down상태 해제(view클릭으로 간주되면 선택된view를 활성화시킴)
        private void MGlView_MouseUp(object sender, MouseEventArgs e)
        {
            ViewInfo view = FindTopView(e.X, e.Y);
            if(view == null)
            {
                EditableView = null;
                EditableJson = null;
                panel2.Controls.Clear();
            }
            else if (view.Equal(TempDownView))
            {
                SelectView(view);

                int id = (int)EditableJson["#ID"];
                SelectTreeNode(id);
            }

            TempDownView = null;
        }
        //단순히 마우스 Down상태 진입(클릭지점 백업)
        private void MGlView_MouseDown(object sender, MouseEventArgs e)
        {
            if(chkNewView.Checked)
            {
                CreateNewView(e.X, e.Y);
                chkNewView.Checked = false;
                EditableView = null;
                EditableJson = null;
                TempDownView = null;
                return;
            }
            TempDownView = FindTopView(e.X, e.Y);
            PreviousPt.X = e.X;
            PreviousPt.Y = e.Y;
        }
        //MouseDown상태에서 이동시 DragView를 호출하고 그이외에는 커서 위치에 따른 커서상태(모양) 변경
        private void MGlView_MouseMove(object sender, MouseEventArgs e)
        {
            if (TempDownView != null)
            {
                int dx = e.X - PreviousPt.X;
                int dy = e.Y - PreviousPt.Y;
                DragView(TempDownView, dx, dy);
                PreviousPt.X = e.X;
                PreviousPt.Y = e.Y;
                return;
            }

            if (EditableView != null)
            {
                Rectangle editAbsRect = (Rectangle)EditableView.GetRectAbsolute();
                Rectangle rect_out = ExpandRect_FixedCenter(editAbsRect, DetectPixelSize);
                if (rect_out.Contains(new Point(e.X, e.Y)))
                {
                    Rectangle rect_in = ExpandRect_FixedCenter(editAbsRect, -DetectPixelSize);
                    if (rect_in.Contains(new Point(e.X, e.Y)))
                        Cursor = Cursors.SizeAll;
                    else
                    {
                        int dtX = Math.Abs(editAbsRect.Right - e.X);
                        int dtY = Math.Abs(editAbsRect.Bottom - e.Y);
                        if(dtX < DetectPixelSize && dtY < DetectPixelSize)
                            Cursor = Cursors.SizeNWSE;
                        else if(dtX < DetectPixelSize)
                            Cursor = Cursors.SizeWE;
                        else if (dtY < DetectPixelSize)
                            Cursor = Cursors.SizeNS;
                    }
                    return;
                }
            }
            Cursor = Cursors.Default;
        }
        //기본 UI view들을 그린 후 선택된 view의 외곽선을 그린다.
        private void MGlView_Draw()
        {
            mUIEngine.Draw();
            DrawEditableView();
        }
        //선탠된 view의 크기나 위치를 커서 상태에 따라 조정한다.
        public void DragView(ViewInfo view, int dx, int dy)
        {
            if (view != null && !view.Equal(EditableView))
                return;

            if(Cursor == Cursors.SizeAll)
            {
                int newLocalX = (int)EditableJson["LocalX"] + dx;
                int newLocalY = (int)EditableJson["LocalY"] + dy;
                EditableJson["LocalX"] = newLocalX;
                EditableJson["LocalY"] = newLocalY;
            }
            else if (Cursor == Cursors.SizeNWSE)
            {
                int newWidth = (int)EditableJson["Width"] + dx;
                int newHeight = (int)EditableJson["Height"] + dy;
                EditableJson["Width"] = newWidth;
                EditableJson["Height"] = newHeight;
            }
            else if (Cursor == Cursors.SizeWE)
            {
                int newWidth = (int)EditableJson["Width"] + dx;
                EditableJson["Width"] = newWidth;
            }
            else if (Cursor == Cursors.SizeNS)
            {
                int newHeight = (int)EditableJson["Height"] + dy;
                EditableJson["Height"] = newHeight;
            }

            view.Update(EditableJson.ToString());
        }

        //MouseUp.. 즉 놓는 순간 부모 자식 관계를 Rect의 모퉁이 기준으로 재정립한다.
        //void CheckParentLink()
        //{
        //    if (EditableView == null || Cursor != Cursors.SizeAll)
        //        return;
        //
        //    Point pt = EditableView.RectAbsolute.Location;
        //    bool backEn = EditableView.Enable;
        //    EditableView.Enable = false;
        //    uiView view = mUIMgr.RootView.FindTopView(pt.X, pt.Y);
        //    EditableView.Enable = backEn;
        //    if (view != null && view != EditableView.Parent)
        //        EditableView.ChangeParent(view);
        //}
        Rectangle ExpandRect_FixedCenter(Rectangle rect, int range)
        {
            Point newPos = rect.Location;
            newPos.X -= range;
            newPos.Y -= range;
            Size newSize = rect.Size;
            newSize.Width += range * 2;
            newSize.Height += range * 2;
            return new Rectangle(newPos, newSize);
        }
        ViewInfo FindTopView(int x, int y)
        {
            if (EditableView != null)
            {
                Rectangle abRect = (Rectangle)EditableView.GetRectAbsolute();
                Rectangle rect_out = ExpandRect_FixedCenter(abRect, DetectPixelSize);
                if (rect_out.Contains(new Point(x, y)))
                    return EditableView;
            }
            return mUIEngine.FindTopView(x, y);
        }
        void DrawEditableView()
        {
            if (EditableView == null)
                return;

            DrawArgs param = new DrawArgs();
            param.rect = (Rectangle)EditableView.GetRectAbsolute();
            param.color = Color.Blue;
            mRender.DrawOutline(param);
        }
        private void CreateNewView(int clickPtX, int clickPtY)
        {
            string viewName = cbViewType.SelectedItem.ToString();
            uiViewType viewType = (uiViewType)Enum.Parse(typeof(uiViewType), viewName);
            mUIEngine.CreateView(clickPtX, clickPtY, (int)viewType);
        }

        private void SelectView(ViewInfo view)
        {
            EditableView = view;
            EditableJson = JObject.Parse(EditableView.jsonString);
            panel2.Controls.Clear();
            Control[] ctrls = ToControls(EditableJson);
            AddControlsToPanel(panel2, ctrls);
        }
        private Control[] ToControls(JObject jsonNode)
        {
            List<Control> ctrls = new List<Control>();
            foreach(var node in jsonNode)
            {
                if (node.Key[0] == '#')
                    continue;

                if (jsonNode.ContainsKey("#" + node.Key))
                {
                    string fieldInfo = jsonNode["#" + node.Key].ToString();
                    string typename = fieldInfo.Split('&')[0];
                    if (typename == "Color")
                    {
                        Color color = Color.FromArgb((int)node.Value[3], (int)node.Value[0], (int)node.Value[1], (int)node.Value[2]);
                        Button btn = new Button();
                        btn.Tag = jsonNode;
                        btn.Text = ColorTranslator.ToHtml(color);
                        btn.Name = node.Key;
                        btn.Click += new EventHandler(PropertyEditorHandler);
                        ctrls.Add(btn);
                    }
                    else if (typename == "Enum")
                    {
                        string typeInfo = fieldInfo.Split('&')[1];
                        string[] enums = typeInfo.Split(',');

                        ComboBox cb = new ComboBox();
                        cb.Tag = jsonNode;
                        cb.FormattingEnabled = true;
                        cb.DropDownStyle = ComboBoxStyle.DropDownList;
                        cb.Name = node.Key;
                        cb.SelectedIndexChanged += new EventHandler(PropertyEditorHandler);
                        cb.Items.AddRange(enums);
                        cb.SelectedIndex = (int)node.Value;
                        if (node.Key == "Type")
                            cb.Enabled = false;
                        ctrls.Add(cb);
                    }
                }
                else if (node.Value.Type == JTokenType.Boolean)
                {
                    CheckBox chk = new CheckBox();
                    chk.Tag = jsonNode;
                    chk.AutoSize = true;
                    chk.Name = node.Key;
                    chk.UseVisualStyleBackColor = true;
                    chk.Checked = (bool)node.Value;
                    chk.CheckedChanged += new EventHandler(PropertyEditorHandler);
                    ctrls.Add(chk);
                }
                else if (node.Value.Type == JTokenType.Object)
                {
                    Panel panel = new Panel();
                    panel.Name = node.Key;
                    panel.AutoScroll = true;
                    panel.BackColor = Color.WhiteSmoke;
                    panel.Paint += (s,e)=>
                    {
                        Panel _pn = (Panel)s;
                        ControlPaint.DrawBorder(e.Graphics, _pn.ClientRectangle, Color.DarkBlue, ButtonBorderStyle.Solid);
                    };
                    JObject subObj = (JObject)jsonNode[node.Key];
                    Control[] subCtrls = ToControls(subObj);
                    AddControlsToPanel(panel, subCtrls);

                    Button btn = new Button();
                    btn.Tag = panel;
                    btn.Text = "▼";
                    btn.Name = node.Key;
                    btn.Click += Btn_Click;
                    ctrls.Add(btn);
                }
                else if (node.Value.Type == JTokenType.Array)
                {
                    //skip if it's child node
                }
                else if(node.Value.Type == JTokenType.Float || node.Value.Type == JTokenType.Integer || node.Value.Type == JTokenType.String)
                {
                    TextBox tb = new TextBox();
                    tb.Tag = jsonNode;
                    tb.Name = node.Key;
                    tb.Text = node.Value.ToString();
                    tb.Leave += new EventHandler(PropertyEditorHandler);
                    ctrls.Add(tb);
                }

            }
            return ctrls.ToArray();
        }
        private void UpdateTreeView()
        {
            string jsonStr = mUIEngine.ToJsonString();
            JObject obj = JObject.Parse(jsonStr);
            TreeNode rootNode = CreateNodeTree(obj);
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(rootNode);
        }
        private TreeNode CreateNodeTree(JObject jsonNode)
        {
            string name = jsonNode["Name"].ToString();
            int id = (int)jsonNode["#ID"];
            TreeNode node = new TreeNode();
            node.Name = name;
            node.Text = name;
            node.Tag = id;
            if (!jsonNode.ContainsKey("Childs"))
                return node;

            if (jsonNode["Childs"].Type != JTokenType.Array)
                return node;

            JArray childs = (JArray)jsonNode["Childs"];
            int cnt = childs.Count;
            for (int i = 0; i < cnt; ++i)
            {
                TreeNode childNode = CreateNodeTree((JObject)childs[i]);
                node.Nodes.Add(childNode);
            }

            return node;
        }
        private void SelectTreeNode(int id)
        {
            TreeNode findNode = null;
            foreach (TreeNode node in treeView1.Nodes)
            {
                findNode = FromID(id, node);
                if (findNode != null)
                    break;
            }
            treeView1.SelectedNode = findNode;
            treeView1.Select();
        }
        public TreeNode FromID(int id, TreeNode rootNode)
        {
            if (rootNode.Tag.Equals(id))
                return rootNode;

            foreach (TreeNode node in rootNode.Nodes)
            {
                TreeNode next = FromID(id, node);
                if (next != null)
                    return next;
            }
            return null;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Panel pn = (Panel)btn.Tag;
            Control[] ctrls = (Control[])pn.Tag;
            if (btn.Text == "▼")
            {
                btn.Text = "▲";
                Point pos = new Point(10, btn.Location.Y + btn.Size.Height);
                pn.Location = pos;
                Size size = new Size(panel2.Size.Width, panel2.Size.Height / 2);
                pn.Size = size;
                panel2.Controls.Add(pn);
                pn.BringToFront();
            }
            else
            {
                btn.Text = "▼";
                panel2.Controls.Remove(pn);
            }
        }

        private void AddControlsToPanel(Panel panel, Control[] controls)
        {
            int gapX = 3;
            int gapY = 5;
            Point baseLocation = new Point(10, 10);
            Size ctrlSize = new Size(100, 20);
            Size labelSize = new Size(70, 20);
            int cnt = controls.Count();
            for(int i = 0; i< cnt; ++i)
            {
                Control ctrl = controls[i];
                int depth = 0;
                int yOff = (ctrlSize.Height + gapY) * i;
                int xOff = depth * 10;

                Label lb = new Label();
                lb.Text = ctrl.Name + ":";
                lb.TextAlign = ContentAlignment.MiddleRight;
                lb.BackColor = Color.Gray;
                lb.Font = new Font("맑은 고딕", 8, FontStyle.Bold);
                lb.Location = new Point(baseLocation.X + xOff, baseLocation.Y + yOff);
                lb.Size = labelSize;
                panel.Controls.Add(lb);

                ctrl.Location = new Point(baseLocation.X + xOff + labelSize.Width + gapX, baseLocation.Y + yOff);
                ctrl.Size = ctrlSize;
                panel.Controls.Add(ctrl);
            }
        }
        private void PropertyEditorHandler(object sender, EventArgs e)
        {
            if (EditableView == null)
                return;

            Control ctrl = sender as Control;
            JObject propInfo = ctrl.Tag as JObject;
            string fieldName = ctrl.Name;
            if (ctrl.GetType() == typeof(ComboBox))
            {
                propInfo[fieldName] = (ctrl as ComboBox).SelectedIndex;
            }
            else if (ctrl.GetType() == typeof(CheckBox))
            {
                bool state = (ctrl as CheckBox).Checked;
                propInfo[fieldName] = state;
            }
            else if(ctrl.GetType() == typeof(Button))
            {
                Button btn = (Button)ctrl;
                Color color = ColorTranslator.FromHtml(btn.Text);
                ColorDialog dlg = new ColorDialog();
                dlg.CustomColors = new int[] { ColorTranslator.ToOle(color) };
                dlg.Color = color;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    btn.Text = ColorTranslator.ToHtml(dlg.Color);
                    propInfo[fieldName][0] = dlg.Color.R;
                    propInfo[fieldName][1] = dlg.Color.G;
                    propInfo[fieldName][2] = dlg.Color.B;
                    propInfo[fieldName][3] = dlg.Color.A;
                }
            }
            else if (ctrl.GetType() == typeof(TextBox))
            {
                try
                {
                    string data = (ctrl as TextBox).Text;
                    if(propInfo[fieldName].Type == JTokenType.String)
                        propInfo[fieldName] = data;
                    else if (propInfo[fieldName].Type == JTokenType.Float)
                        propInfo[fieldName] = float.Parse(data);
                    else if (propInfo[fieldName].Type == JTokenType.Integer)
                        propInfo[fieldName] = int.Parse(data);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                Console.WriteLine(ctrl.Name);
            
            EditableView.Update(EditableJson.ToString());
        }

        private void btnSaveJson_Click(object sender, EventArgs e)
        {
            string json = mUIEngine.ToJsonString();
            Utils.SaveFile(json);
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            string filename = "";
            string text = Utils.LoadTextFile(out filename);

            if (filename.Length == 0)
                return;

            if (Utils.GetFileExt(filename) != "json")
            {
                MessageBox.Show("No Json");
                return;
            }

            mUIEngine.Load(filename);
            UpdateTreeView();
        }


        public class MyUIEngineCallbacks : UIEngineCallbacks
        {
            Renderer mRender;
            DrawArgs mArgs = new DrawArgs();
            int[] mTexIDs = new int[4];
            public MyUIEngineCallbacks(Renderer render) { mRender = render; }
            public override void OnDrawFill(RenderParams param)
            {
                mArgs.rect.Location = new Point((int)param.left, (int)param.top);
                mArgs.rect.Size = new Size((int)(param.right - param.left), (int)(param.bottom - param.top));
                mArgs.color = Color.FromArgb(param.alpha, param.red, param.green, param.blue);
                mRender.DrawRect(mArgs);
            }

            public override void OnDrawOutline(RenderParams param)
            {
                mArgs.rect.Location = new Point((int)param.left, (int)param.top);
                mArgs.rect.Size = new Size((int)(param.right - param.left), (int)(param.bottom - param.top));
                mArgs.color = Color.FromArgb(param.alpha, param.red, param.green, param.blue);
                mRender.DrawOutline(mArgs);
            }

            public override void OnDrawTexture(RenderParams param)
            {
                mArgs.rect.Location = new Point((int)param.left, (int)param.top);
                mArgs.rect.Size = new Size((int)(param.right - param.left), (int)(param.bottom - param.top));
                mArgs.uv.Location = new PointF((float)param.uv_left, (float)param.uv_top);
                mArgs.uv.Size = new SizeF((float)(param.uv_right - param.uv_left), (float)(param.uv_bottom - param.uv_top));
                mArgs.color = Color.FromArgb(param.alpha, param.red, param.green, param.blue);
                Marshal.Copy(param.texture, mTexIDs, 0, 1);
                mArgs.texID = mTexIDs[0];
                mRender.DrawTexture(mArgs);
            }

            public override IntPtr OnLoadTexture(BitmapInfo bitmap)
            {
                IntPtr ptr = Marshal.AllocHGlobal(4);
                int texID = -1;
                if (bitmap.fullname.Length > 0)
                    texID = Renderer.InitTexture(bitmap.fullname);
                else
                    texID = Renderer.InitTexture(bitmap.buf, bitmap.width, bitmap.height);
                Marshal.Copy(BitConverter.GetBytes(texID), 0, ptr, 4);
                return ptr;
            }

            public override void OnReleaseTexture(IntPtr ptr)
            {
                Marshal.Copy(ptr, mTexIDs, 0, 1);
                Renderer.ReleaseTexture(mTexIDs[0]);
                Marshal.FreeHGlobal(ptr);
            }
        };

        private void uiViewEditor_MouseDown(object sender, MouseEventArgs e)
        {
            EditableView = null;
            EditableJson = null;
            panel2.Controls.Clear();
            TempDownView = null;
            treeView1.SelectedNode = null;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int id = (int)e.Node.Tag;
            ViewInfo view = mUIEngine.FindView(id);
            SelectView(view);
        }


        //===========================================================================



        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            Point pos = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = treeView1.GetNodeAt(pos);

            if (targetNode != null)
            {
                //nodeCopy = new TreeNode(sourceNode.Text, sourceNode.ImageIndex, sourceNode.SelectedImageIndex);

                sourceNode.Remove();

                if (sourceNode.Index > targetNode.Index)
                    targetNode.Parent.Nodes.Insert(targetNode.Index, sourceNode);
                else
                    targetNode.Parent.Nodes.Insert(targetNode.Index + 1, sourceNode);


                treeView1.Invalidate();
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;

        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            sourceNode = (TreeNode)e.Item;
            DoDragDrop(e.Item.ToString(), DragDropEffects.Move | DragDropEffects.Copy);

        }

        private TreeNode sourceNode;
        private void Form1_Load(object sender, EventArgs e)
        {
            TreeNode root = null;
            for (int i = 0; i < 101; i++)
            {
                if (i % 10 == 0)
                {
                    if (root != null) treeView1.Nodes.Add(root);
                    root = new TreeNode(i.ToString());
                }
                else
                {
                    TreeNode child = new TreeNode(i.ToString());
                    root.Nodes.Add(child);
                }
            }

        }

        TreeNode preNode = null;
        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            Point pos = treeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = treeView1.GetNodeAt(pos);

            if (preNode != null)
                preNode.BackColor = Color.Transparent;

            if (targetNode != null)
            {
                if (targetNode.Bounds.Contains(pos.X, pos.Y))
                {
                    targetNode.BackColor = Color.Red;
                    preNode = targetNode;
                }

            }

        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            Console.WriteLine(e.Node.Text);
        }
    }
}

