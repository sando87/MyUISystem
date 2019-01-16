using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class jUIButton : jUIControl
    {
        jUIScrollV mScrollV = null;

        public jUIButton()
        {
            mColor = Color.Purple;

            OnMouseEnter += (ctrl, args) =>
            {
                mColor = System.Drawing.Color.Yellow;
                Console.WriteLine("Enter, btn, " + mID.ToString());
                mSystem.OnDrawRequest(null);
            };

            OnMouseLeave += (ctrl, args) =>
            {
                mColor = System.Drawing.Color.Purple;
                Console.WriteLine("Leave, btn, " + mID.ToString());
                mSystem.OnDrawRequest(null);
            };

        }

        public override void Init()
        {
            mScrollV = new jUIScrollV();
            mScrollV.Init(this);
        }

        public override void Draw()
        {
            if (mParentControl == null)
                return;

            if (!mParentControl.Rect.IntersectsWith(Rect))
                return;

            int left = Math.Max(mParentControl.Rect.Left, Rect.Left);
            int right = Math.Min(mParentControl.Rect.Right, Rect.Right);
            int top = Math.Max(mParentControl.Rect.Top, Rect.Top);
            int bottom = Math.Min(mParentControl.Rect.Bottom, Rect.Bottom);

            DrawInfo info = new DrawInfo();
            info.rect = new Rectangle(left, top, right - left, bottom - top);
            info.color = mColor;
            mSystem.OnDrawRectFill?.Invoke(info);

            info.color = Color.DarkGray;
            info.lineWidth = 2;
            mSystem.OnDrawRectOutline?.Invoke(info);

            if (mScrollV != null)
                mScrollV.Draw();
        }
    }
}
