using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class jUIScrollV : jUIControl
    {
        jUIButton mBtnUP = null;
        jUIButton mBtnScroll = null;
        jUIButton mBtnDown = null;

        private Size mParentSizeAll = new Size();
        private const int mSquareSize = 20;
        public jUIScrollV()
        {
            mType = UIControlType.ScrollV;
        }

        public void Initialize(jUIControl _parent)
        {
            int cnt = _parent.mNodes.Count;
            if (cnt == 0)
                return;

            for (int i = 0; i < cnt; ++i)
            {
                mParentSizeAll.Width = Math.Max(mParentSizeAll.Width, _parent.mNodes[i].Rect_R.Right);
                mParentSizeAll.Height = Math.Max(mParentSizeAll.Height, _parent.mNodes[i].Rect_R.Bottom);
            }

            SetSize(new Size(mSquareSize, _parent.Size.Height - mSquareSize));
            SetPos(new Point(_parent.Size.Width - mSquareSize, 0));
            _parent.Add(this);

            mBtnUP = new jUIButton();
            mBtnScroll = new jUIButton();
            mBtnDown = new jUIButton();

            float rate = (float)Size.Height / mParentSizeAll.Height;
            if (rate >= 1.0f)
                return;

            int scrollHeight = (int)(rate * (float)(Size.Height - (mSquareSize * 2)));
            mBtnUP.SetSize(new Size(mSquareSize, mSquareSize));
            mBtnScroll.SetSize(new Size(mSquareSize, scrollHeight));
            mBtnDown.SetSize(new Size(mSquareSize, mSquareSize));

            mBtnUP.SetPos(new Point(0, 0));
            mBtnScroll.SetPos(new Point(0, mSquareSize));
            mBtnDown.SetPos(new Point(0, Size.Height - mSquareSize));

            Add(mBtnDown);
            Add(mBtnUP);
            Add(mBtnScroll);

            mBtnUP.OnMouseClick += (ctrl, args) =>
            {
                Point curPt = mBtnScroll.Point_R;
                int newYPos = Math.Max(mBtnUP.Rect_R.Bottom, curPt.Y - 10);
                mBtnScroll.SetPos(new Point(curPt.X, newYPos));
                //UpdateControls();
                Parent.CalcAbsolutePostion();
                mSystem.OnDrawRequest(null);
            };

            mBtnDown.OnMouseClick += (ctrl, args) =>
            {
                Point curPt = mBtnScroll.Point_R;
                int newYPos = Math.Min(Size.Height - mSquareSize - mBtnScroll.Size.Height, curPt.Y + 10);
                mBtnScroll.SetPos(new Point(curPt.X, newYPos));
                //UpdateControls();
                Parent.CalcAbsolutePostion();
                mSystem.OnDrawRequest(null);
            };

            _parent.CalcAbsolutePostion();
        }

        public float GetCurrentPos()
        {
            int total = Size.Height - mBtnUP.Size.Height - mBtnDown.Size.Height - mBtnScroll.Size.Height;
            int current = mBtnScroll.Point_R.Y - mBtnUP.Size.Height;
            return (float)current / total;
        }

        public int GetCurrentOffset()
        {
            int total = Size.Height - mBtnUP.Size.Height - mBtnDown.Size.Height - mBtnScroll.Size.Height;
            int current = mBtnScroll.Point_R.Y - mBtnUP.Size.Height;
            float rate = (float)current / total;
            int off = (int)(rate * (mParentSizeAll.Height - Size.Height));
            return off;
        }

        private void UpdateControls()
        {
            float rate = GetCurrentPos();
            int off = (int)(rate * (mParentSizeAll.Height - Size.Height));

            int cnt = Parent.mNodes.Count;
            for (int i = 0; i < cnt; ++i)
            {
                if (Parent.mNodes[i] == this)
                    break;

                Point pt = Parent.mNodes[i].Point_R;
                Parent.mNodes[i].SetPos(new Point(pt.X, pt.Y - off));
            }
        }

        //public override void Draw()
        //{
        //    base.Draw();
        //    mBtnUP.Draw();
        //    mBtnDown.Draw();
        //    mBtnScroll.Draw();
        //}

    }
}
