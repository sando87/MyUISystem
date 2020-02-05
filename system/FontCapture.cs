using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    public class FontCapture
    {
        public int TextureID { get { return mTextureID; } }
        public SizeF SizeChar { get { return mSizeChar; } }
        public RectangleF UVChar(int ch)
        {
            float u = (ch % mGlyphsPerLine) * mSizeUV.Width;
            float v = (ch / mGlyphsPerLine) * mSizeUV.Height;
            return new RectangleF(new PointF(u,v), mSizeUV);
        }
        public void ReleaseTexture()
        {
            if (TextureID > 0)
                Renderer.ReleaseTexture(TextureID);
        }
        static public FontCapture Capture(string fontname, int size, uiViewStyle style)
        {
            FontCapture font = new FontCapture();
            int n = (int)style;
            if (n > 0)
                n = (int)Math.Pow(2, n - 1);
            font.LoadFont(fontname, size, n);
            return font;
        }

        private const int mGlyphsPerLine = 16;
        private const int mGlyphLineCount = 16;
        private SizeF mSizeChar;
        private SizeF mSizeUV;
        private int mTextureID;
        private Font mFont;
        private Bitmap mBitmap;

        private FontCapture() { }
        private void LoadFont(string fontname, int size, int style)
        {
            mFont = new Font(fontname, size, (FontStyle)style);
            int width = mFont.Height * mGlyphsPerLine;
            int height = mFont.Height * mGlyphLineCount;
            mBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var graphics = Graphics.FromImage(mBitmap))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                mSizeChar = graphics.MeasureString("A", mFont);
                for (int p = 0; p < mGlyphLineCount; p++)
                {
                    for (int n = 0; n < mGlyphsPerLine; n++)
                    {
                        char c = (char)(n + p * mGlyphsPerLine);
                        graphics.DrawString(c.ToString(), mFont, Brushes.White, n * mSizeChar.Width, p * mSizeChar.Height);
                    }
                }
            }

            //mBitmap.Save("test.png");
            mSizeUV = new SizeF(mSizeChar.Width / (float)width, mSizeChar.Height / (float)height); 
            mTextureID = Renderer.InitTexture(mBitmap);
        }
    }
}
