using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class uiViewFont : uiView
    {
        public override void OnLoad(int depth)
        {
            base.OnLoad(depth);

            PropFont node = JsonNode as PropFont;
            Rectangle textRect = new Rectangle();
            textRect.Size = node.GetTextSize;
            int centerOffX = RectAbsolute.Left + (int)(RectAbsolute.Width * 0.5) - (int)(textRect.Width * 0.5);
            int rightOffX = RectAbsolute.Right - textRect.Width;
            int centerOffY = RectAbsolute.Top + (int)(RectAbsolute.Height * 0.5) - (int)(textRect.Height * 0.5);
            int bottomOffY = RectAbsolute.Bottom - textRect.Height;
            switch (node.align)
            {
                case uiViewAlign.TopLeft:       textRect.Location = new Point(RectAbsolute.Left, RectAbsolute.Top); break;
                case uiViewAlign.Top:           textRect.Location = new Point(centerOffX, RectAbsolute.Top); break;
                case uiViewAlign.TopRight:      textRect.Location = new Point(rightOffX, RectAbsolute.Top); break;
                case uiViewAlign.Left:            textRect.Location = new Point(RectAbsolute.Left, centerOffY); break;
                case uiViewAlign.Center:         textRect.Location = new Point(centerOffX, centerOffY); break;
                case uiViewAlign.Right:           textRect.Location = new Point(rightOffX, centerOffY); break;
                case uiViewAlign.BottomLeft:    textRect.Location = new Point(RectAbsolute.Left, bottomOffY); break;
                case uiViewAlign.Bottom:         textRect.Location = new Point(centerOffX, bottomOffY); break;
                case uiViewAlign.BottomRight:  textRect.Location = new Point(rightOffX, bottomOffY); break;
                default:
                    break;
            }
            RenderParam.rectText = textRect;
            RenderParam.colorText = Color.Black;
            RenderParam.font = node.Font;
            RenderParam.text = node.text;
            RenderParam.gapRate = node.gapRate;
        }
        public override void OnDraw()
        {
            if(RenderParam.font != null)
                uiViewManager.Inst.InvokeDrawText?.Invoke(RenderParam);
        }
    }
}
