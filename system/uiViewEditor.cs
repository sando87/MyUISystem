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

namespace system
{
    public partial class uiViewEditor : Form
    {
        public Renderer mRender = new Renderer();
        public uiViewManager mUIMgr = uiViewManager.Inst;

        public const int DetectPixelSize = 5; //선택된 view의 외곽선 감지 범위
        public uiView EditableView = null; //선택된 view
        public uiView TempDownView = null; //단순히 다운된 view(업시 null)
        public Point PreviousPt = new Point(); //마우스 다운시 클릭지점 백업

        public uiViewEditor()
        {
            InitializeComponent();

            InitRenderer();
            InitUIManager("test.json");
        }

        public void InitRenderer()
        {
            mRender.Initialize(panel1, panel1.Width, panel1.Height);

            mRender.mGlView.MouseMove += MGlView_MouseMove;
            mRender.mGlView.MouseDown += MGlView_MouseDown;
            mRender.mGlView.MouseUp += MGlView_MouseUp;
            mRender.OnDraw += MGlView_Draw;
        }
        public void InitUIManager(string jsonFullname)
        {
            string jsonString = File.ReadAllText(jsonFullname);
            uiViewProperties obj = uiViewProperties.Parse(jsonString);
            mUIMgr.Load(obj);

            mUIMgr.InvokeDrawRectFill += (param) => { mRender.DrawRect(param.rect, param.color); };
            mUIMgr.InvokeDrawRectOutline += (param) => { mRender.DrawOutline(param.rect, param.color, param.lineWidth); };
            mUIMgr.InvokeDrawBitmapRect += (param) => { mRender.DrawTextureRect(param.rect, param.texID, param.uv); };
        }

        //단순히 마우스 Down상태 해제(view클릭으로 간주되면 선택된view를 활성화시킴)
        private void MGlView_MouseUp(object sender, MouseEventArgs e)
        {
            uiView view = FindTopView(e.X, e.Y);
            if (view == TempDownView)
                EditableView = view;

            TempDownView = null;
        }
        //단순히 마우스 Down상태 진입(클릭지점 백업)
        private void MGlView_MouseDown(object sender, MouseEventArgs e)
        {
            TempDownView = FindTopView(e.X, e.Y);
            PreviousPt.X = e.X;
            PreviousPt.Y = e.Y;
        }
        //MouseDown상태에서 이동시 DragView를 호출하고 그이외에는 커서 위치에 따른 상태 변경
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
        //실제 Dragging되는 view의 크기나 위치를 커서 상태에 따라 조정한다.
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
            view.LoadAll(view.Depth);
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
            mRender.DrawOutline(param.rect, param.color, param.lineWidth);
        }

        private void btnSaveJson_Click(object sender, EventArgs e)
        {
            string json = mUIMgr.ToJsonString();
            File.WriteAllText("editorJson.json", json);
        }
    }
}
