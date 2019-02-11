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
        public class CharInfo
        {
            public char ch;
            public RectangleF uv;
            public bool isSeleted;
        }
        public enum Align
        {
            TopLeft,      Top,              TopRight,
            Left,           Center,         Right,
            BottomLeft, Bottom,         BottomRight,
        }

        public Align mAlign = Align.Center;
        List<CharInfo> mListChars = new List<CharInfo>();
        Rectangle mBlicker;
        bool mIsBlinker = false;
        jUIControl mParent = null;

        public jUIFontBox()
        {
        }

        public void Initialize(jUIControl _ctrl)
        {
            mText = _ctrl.mText;
            mParent = _ctrl;
            SetSize(FontManager.GetStringSize(mText));
            SetPos(GetStringPosition());

            int cnt = mText.Length;
            for(int i = 0; i<cnt; ++i)
            {
                CharInfo info = new CharInfo();
                info.ch = mText[i];
                info.uv = FontManager.GetCharUV(mText[i]);
                info.isSeleted = false;
                mListChars.Add(info);
            }
            OnFocus += (ctrl, args) => {
                GetBlinkerRect(args.x, args.y);
                mIsBlinker = true;
                mSystem.OnTimerEverySecond += TimerEverySec;
                mSystem.OnDrawRequest(null);
            };
            OnFocusOut += (ctrl, args) => {
                mIsBlinker = false;
                mSystem.OnTimerEverySecond -= TimerEverySec;
                mSystem.OnDrawRequest(null);
            };
        }
        public override void Draw()
        {
            jUIControl parentCtrl = mParent;
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
                RectangleF charUV = mListChars[i].uv;

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

            if(mIsFocused && mIsBlinker)
            {
                DrawInfo drawInfo = new DrawInfo();
                drawInfo.rect = mBlicker;
                drawInfo.color = Color.DarkBlue;
                mSystem.OnDrawRectFill(drawInfo);
            }
        }

        private void TimerEverySec()
        {
            mIsBlinker = !mIsBlinker;
            mSystem.OnDrawRequest(null);
        }
        private void GetBlinkerRect(int _x, int _y)
        {
            mBlicker.Size = new Size(2, 16);
            for(int i = 0; i<mText.Length; ++i)
            {
                Rectangle chRect = GetCharPosition(i);
                if (Math.Abs(chRect.X - _x) < 4)
                {
                    mBlicker.Location = new Point(chRect.X, chRect.Y + 3);
                    return;
                }
            }

            if (Rect.Right < _x)
                mBlicker.Location = new Point(Rect.Right, Rect.Top + 3);
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
                    x = (mParent.Size.Width - Size.Width) / 2;
                    break;
                case Align.TopRight:
                case Align.Right:
                case Align.BottomRight:
                    x = mParent.Size.Width - Size.Width;
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
                    y = (mParent.Size.Height - Size.Height) / 2;
                    break;
                case Align.BottomLeft:
                case Align.Bottom:
                case Align.BottomRight:
                    y = mParent.Size.Height - Size.Height;
                    break;
            }
            return new Point(x, y);
        }
    }
}
