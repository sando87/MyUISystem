using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class uiViewButton : uiViewFont
    {
        private bool mImageMode = false;
        public override void OnLoad(int depth)
        {
            base.OnLoad(depth);

            PropButton prop = JsonNode as PropButton;
            if (prop.ImgNormal.filename != null && prop.ImgNormal.filename.Length > 0)
                mImageMode = true;

            if (mImageMode)
            {
                prop.ImgNormal.LoadTexture();
                RenderParam.texID = prop.ImgNormal.GetTexID();
                RenderParam.rectImg.rect = RectAbsolute;
                float width = prop.ImgNormal.right - prop.ImgNormal.left;
                float height = prop.ImgNormal.bottom - prop.ImgNormal.top;
                RenderParam.rectImg.uv = new RectangleF(prop.ImgNormal.left, prop.ImgNormal.top, width, height);
                RenderParam.rectImg.bright = 1;
            }
            else
            {
                RenderParam.rect = RectAbsolute;
                RenderParam.lineWidth = 2;
                RenderParam.color = Utils.ToColor(prop.ButtonColor);
                RenderParam.colorOutline = Utils.ToColor("255.173.173.173");
            }
        }
        public override void OnDraw()
        {

            if (mImageMode)
            {
                if (Downed)
                    RenderParam.rectImg.bright = 0.8f;
                else if (Hovered)
                    RenderParam.rectImg.bright = 1.0f;
                else
                    RenderParam.rectImg.bright = 0.9f;

                if (RenderParam.texID > 0)
                    uiViewManager.Inst.InvokeDrawBitmap?.Invoke(RenderParam);
            }
            else
            {
                if (Downed)
                {
                    PropButton prop = JsonNode as PropButton;
                    Color btnColor = Utils.ToColor(prop.ButtonColor);
                    int red = (int)Math.Max(btnColor.R * 0.9, 0);
                    int green = (int)Math.Max(btnColor.G * 0.9, 0);
                    int blue = (int)Math.Max(btnColor.B * 0.9, 0);
                    RenderParam.color = Color.FromArgb(red, green, blue);
                    RenderParam.colorOutline = Utils.ToColor("255.0.84.153");
                }
                else if (Hovered)
                {
                    PropButton prop = JsonNode as PropButton;
                    Color btnColor = Utils.ToColor(prop.ButtonColor);
                    int delta = (int)(btnColor.R * 0.05);
                    int red = Math.Min(btnColor.R + 1, 255);
                    int green = Math.Min(btnColor.G + delta, 255);
                    int blue = Math.Min(btnColor.B + delta * 2, 255);
                    RenderParam.color = Color.FromArgb(red, green, blue);
                    RenderParam.colorOutline = Utils.ToColor("255.0.120.215");
                }
                else
                {
                    PropButton prop = JsonNode as PropButton;
                    RenderParam.color = Utils.ToColor(prop.ButtonColor);
                    RenderParam.colorOutline = Utils.ToColor("255.173.173.173");
                }


                uiViewManager.Inst.InvokeDrawRectFill?.Invoke(RenderParam);
                uiViewManager.Inst.InvokeDrawRectOutline?.Invoke(RenderParam);

                if (RenderParam.font != null)
                    uiViewManager.Inst.InvokeDrawText?.Invoke(RenderParam);
            }
        }
    }
}
