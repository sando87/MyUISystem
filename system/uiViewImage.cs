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
        Color oriColor;
        public override void OnLoad(int depth)
        {
            base.OnLoad(depth);

            oriColor = RenderParam.color;
            PropImage prop = JsonNode as PropImage;
            prop.ImgNormal.LoadTexture();
            RenderParam.texID = prop.ImgNormal.GetTexID();
            RenderParam.rect = RectAbsolute;
            RenderParam.rect.Intersect(Parent.RectAbsolute);
            float rateL = (RenderParam.rect.Left - RectAbsolute.Left) / (float)RectAbsolute.Width;
            float rateR = (RenderParam.rect.Right - RectAbsolute.Left) / (float)RectAbsolute.Width;
            float rateT = (RenderParam.rect.Top - RectAbsolute.Top) / (float)RectAbsolute.Height;
            float rateB = (RenderParam.rect.Bottom - RectAbsolute.Top) / (float)RectAbsolute.Height;
            float uvL = prop.ImgNormal.left + (prop.ImgNormal.right - prop.ImgNormal.left) * rateL;
            float uvR = prop.ImgNormal.left + (prop.ImgNormal.right - prop.ImgNormal.left) * rateR;
            float uvT = prop.ImgNormal.top + (prop.ImgNormal.bottom - prop.ImgNormal.top) * rateT;
            float uvB = prop.ImgNormal.top + (prop.ImgNormal.bottom - prop.ImgNormal.top) * rateB;
            RenderParam.uv = new RectangleF(uvL, uvT, uvR - uvL, uvB - uvT);

        }
        public override void OnDraw()
        {
            if (Downed)
                RenderParam.color = Color.Blue;
            else if (Hovered)
                RenderParam.color = Color.Red;
            else
                RenderParam.color = oriColor;

            uiViewManager.Inst.InvokeDrawBitmap?.Invoke(RenderParam);
        }
    }
}
