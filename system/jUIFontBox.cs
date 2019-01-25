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
        public Align mAlign = Align.Center;

        public jUIFontBox()
        {
        }

        public void Initialize(jUIControl _ctrl)
        {
            mSystem = _ctrl.mSystem;
            mText = _ctrl.mText;
            mType = UIControlType.FontBox;
            mID = "";
            mParentID = _ctrl.mID;
            SetSize(FontManager.GetStringSize(mText));
            SetPos(GetStringPosition());
            mSystem.Registor(this);
            CalcAbsolutePostion();
            _ctrl.mNodes.Add(this);
        }
        public override void Draw()
        {
            jUIControl parentCtrl = Parent;
            if (parentCtrl == null)
                return;

            if (!parentCtrl.Rect.IntersectsWith(Rect))
                return;

            for (int i = 0; i<mText.Length; ++i)
            {
                char ch = mText[i];
                Rectangle chRect = GetCharPosition(i);
                if (!parentCtrl.Rect.IntersectsWith(chRect))
                    continue;

                Rectangle interRect = new Rectangle(chRect.Location, chRect.Size);
                interRect.Intersect(parentCtrl.Rect);
                RectangleF charUV = FontManager.GetCharUV(ch);

                //adjust new U float
                float rateWidthL = (float)(interRect.Left - chRect.Left) / chRect.Width;
                float newLeftU = charUV.Left + (rateWidthL * charUV.Width);
                float rateWidthR = (float)(chRect.Right - interRect.Right) / chRect.Width;
                float newRightU = charUV.Right - (rateWidthR * charUV.Width);

                //adjust new V float
                float rateHeightT = (float)(interRect.Top - chRect.Top) / chRect.Height;
                float newTopV = charUV.Top + (rateHeightT * charUV.Height);
                float rateHeightB = (float)(chRect.Bottom - interRect.Bottom) / chRect.Height;
                float newBottomV = charUV.Bottom - (rateHeightB * charUV.Height);

                RectangleF charNewUV = new RectangleF(newLeftU, newTopV, newRightU - newLeftU, newBottomV - newTopV);
                DrawInfo drawInfo = new DrawInfo();
                drawInfo.texID = FontManager.Settings.TextureID;
                drawInfo.rect = interRect;
                drawInfo.uv = charNewUV;
                mSystem.OnDrawBitmapRect(drawInfo);
            }
        }

        private Rectangle GetCharPosition(int _idx)
        {
            int x_step = Size.Width / mText.Length;
            return new Rectangle(new Point(Point.X + _idx * x_step, Point.Y), new Size(x_step, Size.Height));
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
                    x = (Parent.Size.Width - Size.Width) / 2;
                    break;
                case Align.TopRight:
                case Align.Right:
                case Align.BottomRight:
                    x = Parent.Size.Width - Size.Width;
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
                    y = (Parent.Size.Height - Size.Height) / 2;
                    break;
                case Align.BottomLeft:
                case Align.Bottom:
                case Align.BottomRight:
                    y = Parent.Size.Height - Size.Height;
                    break;
            }
            return new Point(x, y);
        }
    }
}
