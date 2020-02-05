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

namespace system
{
    public partial class uiViewEditor : Form
    {
        public class PropFieldInfo
        {
            public FieldInfo field;
            public object msg;
            public int depth;
            public PropFieldInfo(FieldInfo _field, object _msg, int _depth) { field = _field;  msg = _msg; depth = _depth; }
        };
        public Renderer mRender = new Renderer();
        public uiViewManager mUIMgr = uiViewManager.Inst;

        public const int DetectPixelSize = 5; //선택된 view의 외곽선 감지 범위
        public uiView EditableView = null; //선택된 view
        public uiView TempDownView = null; //단순히 다운된 view(업시 null)
        public Point PreviousPt = new Point(); //마우스 다운시 클릭지점 백업

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
            timer.Tick += (ss, ee) => { mRender.mGlView.Invalidate(); };
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
            mUIMgr.InvokeDrawRectFill += (param) => { mRender.DrawRect(param); };
            mUIMgr.InvokeDrawRectOutline += (param) => { mRender.DrawOutline(param); };
            mUIMgr.InvokeDrawBitmap += (param) => { mRender.DrawTexture(param); };
            mUIMgr.InvokeDrawText += (param) => { mRender.DrawText(param); };
        }

        //단순히 마우스 Down상태 해제(view클릭으로 간주되면 선택된view를 활성화시킴)
        private void MGlView_MouseUp(object sender, MouseEventArgs e)
        {
            uiView view = FindTopView(e.X, e.Y);
            if (view == TempDownView)
            {
                EditableView = view;
                CheckParentLink();
                panel2.Controls.Clear();
                if (EditableView != null)
                {
                    Control[] ctrls = ToControls(EditableView.JsonNode);
                    AddControlsToPanel(panel2, ctrls);
                }
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
                Rectangle rect_out = ExpandRect_FixedCenter(EditableView.RectAbsolute, DetectPixelSize);
                if (rect_out.Contains(new Point(e.X, e.Y)))
                {
                    Rectangle rect_in = ExpandRect_FixedCenter(EditableView.RectAbsolute, -DetectPixelSize);
                    if (rect_in.Contains(new Point(e.X, e.Y)))
                        Cursor = Cursors.SizeAll;
                    else
                    {
                        int dtX = Math.Abs(EditableView.RectAbsolute.Right - e.X);
                        int dtY = Math.Abs(EditableView.RectAbsolute.Bottom - e.Y);
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
            mUIMgr.Draw();
            DrawEditableView();
        }
        //선탠된 view의 크기나 위치를 커서 상태에 따라 조정한다.
        public void DragView(uiView view, int dx, int dy)
        {
            if (view != EditableView)
                return;

            if(Cursor == Cursors.SizeAll)
            {
                view.JsonNode.LocalX += dx;
                view.JsonNode.LocalY += dy;
            }
            else if (Cursor == Cursors.SizeNWSE)
            {
                view.JsonNode.Width += dx;
                view.JsonNode.Height += dy;
            }
            else if (Cursor == Cursors.SizeWE)
            {
                view.JsonNode.Width += dx;
            }
            else if (Cursor == Cursors.SizeNS)
            {
                view.JsonNode.Height += dy;
            }
            view.LoadAll();
        }

        //MouseUp.. 즉 놓는 순간 부모 자식 관계를 Rect의 모퉁이 기준으로 재정립한다.
        void CheckParentLink()
        {
            if (EditableView == null || Cursor != Cursors.SizeAll)
                return;

            Point pt = EditableView.RectAbsolute.Location;
            bool backEn = EditableView.Enable;
            EditableView.Enable = false;
            uiView view = mUIMgr.RootView.FindTopView(pt.X, pt.Y);
            EditableView.Enable = backEn;
            if (view != null && view != EditableView.Parent)
                EditableView.ChangeParent(view);
        }
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
        uiView FindTopView(int x, int y)
        {
            if (EditableView == null)
                return mUIMgr.RootView.FindTopView(x, y);

            Rectangle rect_out = ExpandRect_FixedCenter(EditableView.RectAbsolute, DetectPixelSize);
            if (rect_out.Contains(new Point(x, y)))
                return EditableView;

            return mUIMgr.RootView.FindTopView(x, y);
        }
        void DrawEditableView()
        {
            if (EditableView == null)
                return;

            DrawingParams param = new DrawingParams();
            param.rect = EditableView.RectAbsolute;
            param.color = Color.Blue;
            param.lineWidth = 3;
            mRender.DrawOutline(param);
        }
        private void CreateNewView(int clickPtX, int clickPtY)
        {
            uiView parentView = FindTopView(clickPtX, clickPtY);
            if (parentView == null)
                return;

            ViewProperty defaultProp = new ViewProperty();
            string viewName = cbViewType.SelectedItem.ToString();
            uiViewType viewType = (uiViewType)Enum.Parse(typeof(uiViewType), viewName);
            uiView view = parentView.BornChild(viewType);

            ViewPropertiesTree newProp = new ViewPropertiesTree();
            newProp.Me.Name = mUIMgr.AutoName(viewType.ToString());
            newProp.Me.Type = viewType;
            newProp.Me.LocalX = clickPtX - parentView.RectAbsolute.Location.X;
            newProp.Me.LocalY = clickPtY - parentView.RectAbsolute.Location.Y;
            newProp.Me.Width = 80;
            newProp.Me.Height = 30;
            newProp.Me.Color = "255.160.160.160";
            string jsonString = newProp.ToJSON();
            newProp.Parse(jsonString);
            view.JsonNode = newProp.Me;
            view.OnLoad(parentView.Depth + 1);
            mUIMgr.RegisterView(view);
        }

        private Control[] ToControls(object _msg, int depth = 0)
        {
            List<Control> ctrls = new List<Control>();
            FieldInfo[] fields = _msg.GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    ComboBox cb = new ComboBox();
                    cb.Tag = new PropFieldInfo(field, _msg, depth);
                    cb.FormattingEnabled = true;
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                    cb.Name = field.Name;
                    cb.SelectedIndexChanged += new EventHandler(PropertyEditorHandler);
                    string[] enums = System.Enum.GetNames(field.FieldType);
                    cb.Items.AddRange(enums);
                    cb.SelectedIndex = (int)field.GetValue(_msg);
                    if(field.FieldType == typeof(uiViewType))
                        cb.Enabled = false;
                    ctrls.Add(cb);
                }
                else if (field.FieldType.Name == "Boolean")
                {
                    CheckBox chk = new CheckBox();
                    chk.Tag = new PropFieldInfo(field, _msg, depth);
                    chk.AutoSize = true;
                    chk.Name = field.Name;
                    chk.UseVisualStyleBackColor = true;
                    chk.Checked = (bool)field.GetValue(_msg);
                    chk.CheckedChanged += new EventHandler(PropertyEditorHandler);
                    ctrls.Add(chk);
                }
                else if (field.FieldType.IsClass && field.FieldType.Name != "String")
                {
                    Label dummy = new Label();
                    dummy.Tag = new PropFieldInfo(field, _msg, depth);
                    dummy.Name = field.Name;
                    dummy.BackColor = Color.Gray;
                    dummy.Text = "▼";
                    dummy.TextAlign = ContentAlignment.MiddleLeft;
                    ctrls.Add(dummy);

                    var subObject = field.GetValue(_msg);
                    Control[] subCtrls = ToControls(subObject, depth + 1);
                    ctrls.AddRange(subCtrls);
                }
                else
                {
                    string value = "";
                    try { value = field.GetValue(_msg).ToString(); }
                    catch (Exception) { }
                    TextBox tb = new TextBox();
                    tb.Tag = new PropFieldInfo(field, _msg, depth);
                    tb.Name = field.Name;
                    tb.Text = value;
                    tb.Leave += new EventHandler(PropertyEditorHandler);
                    ctrls.Add(tb);
                }
            }
            return ctrls.ToArray();
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
                int depth = (ctrl.Tag as PropFieldInfo).depth;
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
                ctrl.TabIndex = i;
                panel.Controls.Add(ctrl);
            }
        }
        private void PropertyEditorHandler(object sender, EventArgs e)
        {
            if (EditableView == null)
                return;

            Control ctrl = sender as Control;
            PropFieldInfo propInfo = ctrl.Tag as PropFieldInfo;
            FieldInfo info = propInfo.field;
            object msg = propInfo.msg;
            if (ctrl.GetType() == typeof(ComboBox))
            {
                info.SetValue(msg, (ctrl as ComboBox).SelectedIndex);
            }
            else if (ctrl.GetType() == typeof(CheckBox))
            {
                bool state = (ctrl as CheckBox).Checked;
                info.SetValue(msg, state);
            }
            else if (ctrl.GetType() == typeof(TextBox))
            {
                try
                {
                    string data = (ctrl as TextBox).Text;
                    Type type = Type.GetType(info.FieldType.FullName);
                    var value = Convert.ChangeType(data, type);
                    info.SetValue(msg, value);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                Console.WriteLine(ctrl.Name);

            EditableView.LoadAll();
        }

        private void btnSaveJson_Click(object sender, EventArgs e)
        {
            string json = mUIMgr.ToJsonString();
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

            ViewPropertiesTree obj = new ViewPropertiesTree();
            obj.Parse(text);
            bool ret = mUIMgr.Load(obj);
            if (!ret)
                MessageBox.Show(mUIMgr.ErrorMessage);
        }
    }
}

