using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class jUIFontBox : jUIControl
    {
        public enum Align
        {
            TopLeft,      Top,              TopRight,
            Left,           Center,         Right,
            BottomLeft, Bottom,         BottomRight,
        }
        public string mText = "test";
        public Align mAlign = Align.Center;

        public jUIFontBox()
        {
        }

        public override void Init()
        {
            Size size = GetStringSize(mText);
            SetSize(size);
            Point rPos = GetStringPosition();
            SetPos(rPos);
        }
        public override void Draw()
        {
            if (mParentControl == null)
                return;

            if (!mParentControl.Rect.IntersectsWith(Rect))
                return;

            for(int i = 0; i<mText.Length; ++i)
            {
                char ch = mText[i];
                //adjust XY, UV;
                //draw Text;
            }
        }

        private Size GetStringSize(string _name)
        {
            int max = 0;
            string[] lines = _name.Split('\n');
            for(int i=0; i<lines.Length; ++i)
            {
                max = Math.Max(lines[i].Length, max);
            }
            int width = max * Settings.GlyphWidth;
            int height = lines.Length * Settings.GlyphHeight;
            return new Size(width,height);
        }
        private Point GetStringPosition()
        {
            int x = 0;
            int y = 0;
            switch(mAlign)
            {
                case Align.TopLeft:
                case Align.Left:
                case Align.BottomLeft:
                    x = 0;
                    break;
                case Align.Top:
                case Align.Center:
                case Align.Bottom:
                    x = (mParentControl.Size.Width - Size.Width) / 2;
                    break;
                case Align.TopRight:
                case Align.Right:
                case Align.BottomRight:
                    x = mParentControl.Size.Width - Size.Width;
                    break;
            }
            switch (mAlign)
            {
                case Align.TopLeft:
                case Align.Top:
                case Align.TopRight:
                    y = 0;
                    break;
                case Align.Left:
                case Align.Center:
                case Align.Right:
                    y = (mParentControl.Size.Height - Size.Height) / 2;
                    break;
                case Align.BottomLeft:
                case Align.Bottom:
                case Align.BottomRight:
                    y = mParentControl.Size.Height - Size.Height;
                    break;
            }
            return new Point(x, y);
        }
    }
}
