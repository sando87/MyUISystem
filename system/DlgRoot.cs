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
    public partial class DlgRoot : Form
    {
        static public int ScreenWidth = 640;
        static public int ScreenHeight = 480;
        static private DlgRoot mInst = null;
        static public DlgRoot GetInst() { if (mInst == null) mInst = new DlgRoot(); return mInst; }
        static public Renderer mRender = new Renderer();
        static public jUISystem mUISystem = new jUISystem(ScreenWidth, ScreenHeight);
        private jUIControl mCurCtrl = null;
        private int mEditMode = 0;
        private bool mIsDown = false;
        private bool mIsFixed = false;
        private Point mDownPt;

        private DlgRoot()
        {
            InitializeComponent();
            chkItemButton.Tag = UIControlType.Button;
            chkItemCheckBox.Tag = UIControlType.CheckBox;
            chkItemComboBox.Tag = UIControlType.ComboBox;
        }


        private void btnTest_Click(object sender, EventArgs e)
        {
            mUISystem.OnDrawRequest += (param) => { mRender.Draw(); };
            mUISystem.OnDrawRectFill += (param) => { mRender.DrawRect(param.rect, param.color); };
            mUISystem.OnDrawRectOutline += (param) => { mRender.DrawOutline(param.rect, param.color, param.lineWidth); };

            mRender.Initialize(pnGLView, ScreenWidth, ScreenHeight);

            mRender.mGlView.MouseClick += MGlView_MouseClick;
            mRender.mGlView.MouseMove += MGlView_MouseMove;
            mRender.mGlView.MouseDown += MGlView_MouseDown;
            mRender.mGlView.MouseUp += MGlView_MouseUp;
            mRender.OnDraw += () =>
            {
                DlgRoot.mUISystem.Draw();

                if (mCurCtrl != null)
                {
                    mRender.DrawOutline(mCurCtrl.Rect, Color.Blue);
                }
            };
        }

        private void MGlView_MouseClick(object sender, MouseEventArgs e)
        {
            CheckBox chk = FindAddingItem();
            if (chk != null) //컨트롤 추가을 위한 경우
            {
                jUIControl control = new jUIControl();
                control.mType = (UIControlType)chk.Tag;
                control.SetSize(new Size(100, 40));
                mUISystem.Add(control, e.X, e.Y);
                UnCheckAllItems();
                mRender.Draw();
                mCurCtrl = null;
                mEditMode = 0;
                mIsDown = false;
                mIsFixed = false;
                return;
            }

            if(!mIsFixed)
            {
                mIsFixed = false;
                jUIControl ctrl = mUISystem.SelectControl(e.X, e.Y);
                if (ctrl.mID == "0")
                    mCurCtrl = null;
                else
                    mCurCtrl = ctrl;

                mRender.Draw();
            }
            
        }
        private void MGlView_MouseUp(object sender, MouseEventArgs e)
        {
            mIsDown = false;
            mIsFixed = false;
        }
        private void MGlView_MouseDown(object sender, MouseEventArgs e)
        {
            if(mEditMode != 0)
            {
                mIsDown = true;
                mIsFixed = false;
                mDownPt.X = e.X;
                mDownPt.Y = e.Y;
            }
        }
        private void MGlView_MouseMove(object sender, MouseEventArgs e)
        {
            if (mCurCtrl == null)
            {
                Cursor = Cursors.Default;
                mEditMode = 0;
                return;
            }

            if(mIsDown)
            {
                mIsFixed = true;
                switch (mEditMode)
                {
                    case 0: break;
                    case 1:
                        {
                            Point pt = mCurCtrl.Point;
                            mCurCtrl.SetSize(new Size(e.X - pt.X, e.Y - pt.Y));
                        }
                        break;
                    case 2:
                        {
                            mCurCtrl.SetSize(new Size(e.X - mCurCtrl.Point.X, mCurCtrl.Size.Height));
                        }
                        break;
                    case 3:
                        {
                            mCurCtrl.SetSize(new Size(mCurCtrl.Size.Width, e.Y - mCurCtrl.Point.Y));
                        }
                        break;
                    case 4:
                        {
                            int deltaX = e.X - mDownPt.X;
                            int deltaY = e.Y - mDownPt.Y;

                            mCurCtrl.SetPos(new Point(mCurCtrl.Point_R.X + deltaX, mCurCtrl.Point_R.Y + deltaY));

                            mDownPt.X = e.X;
                            mDownPt.Y = e.Y;
                        }
                        break;
                }
                mRender.Draw();
                return;
            }

            if (!mCurCtrl.Rect.Contains(e.X, e.Y))
            {
                Cursor = Cursors.Default;
                mEditMode = 0;
                return;
            }

            int termX = Math.Abs(mCurCtrl.Rect.Right - e.X);
            int termY = Math.Abs(mCurCtrl.Rect.Bottom - e.Y);
            if (termX < 5 && termY < 5)
            {
                Cursor = Cursors.SizeNWSE;
                mEditMode = 1;
            }
            else if (termX < 5)
            {
                Cursor = Cursors.SizeWE;
                mEditMode = 2;
            }
            else if (termY < 5)
            {
                Cursor = Cursors.SizeNS;
                mEditMode = 3;
            }
            else
            {
                Cursor = Cursors.SizeAll;
                mEditMode = 4;
            }

        }

        private void chkItem_Click(object sender, EventArgs e)
        {
            int cnt = pnControlItems.Controls.Count;
            for (int i = 0; i < cnt; ++i)
            {
                CheckBox chk = (CheckBox)pnControlItems.Controls[i];
                if (chk.Name != ((CheckBox)sender).Name)
                    chk.Checked = false;
            }
        }

        private CheckBox FindAddingItem()
        {
            int cnt = pnControlItems.Controls.Count;
            for (int i = 0; i < cnt; ++i)
            {
                if (((CheckBox)pnControlItems.Controls[i]).Checked)
                    return (CheckBox)pnControlItems.Controls[i];
            }
            return null;
        }

        private void UnCheckAllItems()
        {
            int cnt = pnControlItems.Controls.Count;
            for (int i = 0; i < cnt; ++i)
                ((CheckBox)pnControlItems.Controls[i]).Checked = false;
        }

        private void btnTest_Click_1(object sender, EventArgs e)
        {
            DlgTest dlg = new DlgTest(ScreenWidth, ScreenHeight);
            dlg.ShowDialog();
        }
    }
}
