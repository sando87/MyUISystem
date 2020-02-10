using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class uiViewImage : uiView
    {
        public override void OnLoad(int depth)
        {
            base.OnLoad(depth);

            PropImage prop = JsonNode as PropImage;
            prop.ImgNormal.LoadTexture();
            RenderParam.texID = prop.ImgNormal.GetTexID();
            RenderParam.rectImg.rect = RectAbsolute;
            RenderParam.rectImg.uv = new RectangleF(prop.ImgNormal.left, prop.ImgNormal.top, prop.ImgNormal.right - prop.ImgNormal.left, prop.ImgNormal.bottom - prop.ImgNormal.top);
            RenderParam.rectImg.bright = 1;
            RenderParam.rectImg.Clip(Parent.RectAbsolute);
        }
        public override void OnDraw()
        {
            uiViewManager.Inst.InvokeDrawBitmap?.Invoke(RenderParam);
        }
        public void ClipRight(float rate)
        {
            Rectangle clipedRect = RectAbsolute;
            clipedRect.Width = (int)(RectAbsolute.Width * rate);
            RenderParam.rectImg.Clip(clipedRect);
        }
        public void ClipTop(float rate)
        {
            int newHeight = (int)(RectAbsolute.Height * rate);
            int newTop = RectAbsolute.Bottom - newHeight;
            Rectangle clipedRect = new Rectangle(RectAbsolute.Left, newTop, RectAbsolute.Width, newHeight);
            RenderParam.rectImg.Clip(clipedRect);
        }
    }
}
