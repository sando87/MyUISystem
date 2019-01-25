using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace system
{
    public partial class DlgTest : Form
    {
        Renderer mRenderTest = new Renderer();
        jUISystem mSystemTest = new jUISystem(DlgRoot.ScreenWidth, DlgRoot.ScreenHeight);

        public DlgTest(int _w, int _h)
        {
            InitializeComponent();
            Init(_w, _h);
        }

        public void Init(int _w, int _h)
        {
            Size = new Size(_w, _h);

            InitUISystems();

            mSystemTest.OnDrawRequest += (param) => { mRenderTest.Draw(); };
            mSystemTest.OnDrawRectFill += (param) => { mRenderTest.DrawRect(param.rect, param.color); };
            mSystemTest.OnDrawRectOutline += (param) => { mRenderTest.DrawOutline(param.rect, param.color, param.lineWidth); };
            mSystemTest.OnDrawBitmapRect += (param) => { mRenderTest.DrawTextureRect(param.rect, param.texID, param.uv); };
            mSystemTest.OnDrawText += (param) => { mRenderTest.DrawText(param.rect.X, param.rect.Y, param.text); };

            mRenderTest.Initialize(panel1, _w, _h);

            mRenderTest.mGlView.MouseMove += MGlView_MouseMove;
            mRenderTest.mGlView.MouseDown += MGlView_MouseDown;
            mRenderTest.mGlView.MouseUp += MGlView_MouseUp;
            mRenderTest.OnDraw += () =>
            {
                mSystemTest.Draw();
            };
        }

        private void MGlView_MouseUp(object sender, MouseEventArgs e)
        {
            jMouseEventArgs args = new jMouseEventArgs();
            args.x = e.X;
            args.y = e.Y;
            mSystemTest.ProcMouseUp(args);
        }

        private void MGlView_MouseDown(object sender, MouseEventArgs e)
        {
            jMouseEventArgs args = new jMouseEventArgs();
            args.x = e.X;
            args.y = e.Y;
            mSystemTest.ProcMouseDown(args);
        }

        private void MGlView_MouseMove(object sender, MouseEventArgs e)
        {
            jMouseEventArgs args = new jMouseEventArgs();
            args.x = e.X;
            args.y = e.Y;
            mSystemTest.ProcMouseMove(args);
        }

        private void InitUISystems()
        {
            jUISystem baseSystems = DlgRoot.mUISystem;
            baseSystems.LoopControls(baseSystems.GetRoot(), (ctrl) =>
            {
                // 심플 초기값 적용 하는 곳
                switch (ctrl.mType)
                {
                    case UIControlType.Button:
                        jUIButton btnCtrl = new jUIButton();
                        btnCtrl.mType = UIControlType.Button;
                        btnCtrl.mID = ctrl.mID;
                        btnCtrl.mParentID = ctrl.mParentID;
                        btnCtrl.SetSize(ctrl.Size);
                        btnCtrl.SetPos(ctrl.Point_R);
                        btnCtrl.mText = "Test";// ctrl.mText;
                        mSystemTest.Registor(btnCtrl);
                        break;
                    case UIControlType.CheckBox:
                        break;
                        jUICheckBox chkCtrl = new jUICheckBox();
                        chkCtrl.mType = UIControlType.CheckBox;
                        chkCtrl.SetSize(ctrl.Size);
                        mSystemTest.Registor(chkCtrl);
                        break;
                    case UIControlType.ComboBox:
                        break;
                        jUIComboBox cbCtrl = new jUIComboBox();
                        cbCtrl.mType = UIControlType.ComboBox;
                        cbCtrl.SetSize(ctrl.Size);
                        mSystemTest.Registor(cbCtrl);
                        break;
                }

                return false;
            }, null);

            mSystemTest.BuildUpTree();
            mSystemTest.mRoot.CalcAbsolutePostion();

            foreach (var item in mSystemTest.mDicControls)
                item.Value.Init();
        }
    }
}
