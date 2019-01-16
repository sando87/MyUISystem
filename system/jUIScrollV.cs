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
        jUIButton mBtnUP = new jUIButton();
        jUIButton mBtnScroll = new jUIButton();
        jUIButton mBtnDown = new jUIButton();

        private jUIControl mParent = null;
        private Rectangle mRectAll = new Rectangle();
        private const int mSquareSize = 20;
        public jUIScrollV()
        {
            mNodes.Add(mBtnUP);
            mNodes.Add(mBtnDown);
            mNodes.Add(mBtnScroll);
        }

        public void Init(jUIControl _parent)
        {
            int cnt = _parent.mNodes.Count;
            for (int i = 0; i < cnt; ++i)
                mRectAll.Inflate(_parent.mNodes[i].Size);

            SetSize(new Size(mSquareSize, _parent.Size.Height - mSquareSize));

            float rate = Size.Height / mRectAll.Height;
            if (rate >= 1.0f)
                return;

            mParent = _parent;
            int scrollHeight = (int)(rate * (float)(Size.Height - (mSquareSize * 2)));
            mBtnUP.SetSize(new Size(mSquareSize, mSquareSize));
            mBtnScroll.SetSize(new Size(mSquareSize, scrollHeight));
            mBtnDown.SetSize(new Size(mSquareSize, mSquareSize));
            

            mBtnUP.SetPos(new Point(0, 0));
            mBtnScroll.SetPos(new Point(0, mSquareSize));
            mBtnDown.SetPos(new Point(0, Size.Height - mSquareSize));
            
            SetPos(new Point(_parent.Size.Height - mSquareSize, 0));

            mBtnUP.OnMouseClick += (ctrl, args) =>
            {
                Point curPt = mBtnScroll.Point_R;
                int newYPos = Math.Max(0, curPt.Y - 10);
                mBtnScroll.SetPos(new Point(curPt.X, newYPos));
            };

            mBtnDown.OnMouseClick += (ctrl, args) =>
            {
                Point curPt = mBtnScroll.Point_R;
                int newYPos = Math.Min(Size.Height - mSquareSize - mBtnScroll.Size.Height, curPt.Y + 10);
                mBtnScroll.SetPos(new Point(curPt.X, newYPos));

            };
        }

        public float GetCurrentPos()
        {
            int total = Size.Height - mBtnUP.Size.Height - mBtnDown.Size.Height - mBtnScroll.Size.Height;
            int current = mBtnScroll.Point_R.Y - mBtnUP.Size.Height;
            return current / total;
        }

        private void UpdateControls()
        {
            float rate = GetCurrentPos();
            int off = (int)(rate * (mRectAll.Height - Size.Height));

            int cnt = mParent.mNodes.Count;
            for (int i = 0; i < cnt; ++i)
            {
                Point pt = mParent.mNodes[i].Point_R;
                mParent.mNodes[i].SetPos(new Point(pt.X, pt.Y - off));
            }
        }

        public override void Draw()
        {
            base.Draw();
            mBtnUP.Draw();
            mBtnDown.Draw();
            mBtnScroll.Draw();
        }

    }
}
