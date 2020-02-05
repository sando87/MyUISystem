using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class FontManager
    {
        public static class Settings
        {
            public static string FontBitmapFilename = "test.png";
            public static int GlyphsPerLine = 16;
            public static int GlyphLineCount = 16;
            public static int GlyphWidth = 11;
            public static int GlyphHeight = 22;

            public static int CharXSpacing = 11;

            public static string Text = "leesungju";

            // Used to offset rendering glyphs to bitmap
            public static int AtlasOffsetX = -3, AtlassOffsetY = -1;
            public static int FontSize = 14;
            public static bool BitmapFont = false;
            public static string FromFile; //= "joystix monospace.ttf";
            public static string FontName = "굴림체";


            public static int TextureWidth = 0;
            public static int TextureHeight = 0;
            public static int TextureID = -1;
        }

        public static Size GetStringSize(string _name)
        {
            int max = 0;
            string[] lines = _name.Split('\n');
            for (int i = 0; i < lines.Length; ++i)
            {
                max = Math.Max(lines[i].Length, max);
            }
            int width = max * Settings.GlyphWidth;
            int height = lines.Length * Settings.GlyphHeight;
            return new Size(width, height);
        }

        public static Size GetCharSize(char _ch)
        {
            return new Size(Settings.GlyphWidth, Settings.GlyphHeight);
        }

        public static RectangleF GetCharUV(char _ch)
        {
            char idx = _ch;
            float u_step = (float)Settings.GlyphWidth / (float)Settings.TextureWidth;
            float v_step = (float)Settings.GlyphHeight / (float)Settings.TextureHeight;
            float u = (float)(idx % Settings.GlyphsPerLine) * u_step;
            float v = (float)(idx / Settings.GlyphsPerLine) * v_step;
            return new RectangleF(u, v, u_step, v_step);
        }

        public static void GenerateFontImage()
        {
            int bitmapWidth = Settings.GlyphsPerLine * Settings.GlyphWidth;
            int bitmapHeight = Settings.GlyphLineCount * Settings.GlyphHeight;

            using (Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                Settings.TextureWidth = bitmap.Width;
                Settings.TextureHeight = bitmap.Height;
                Font font;
                if (!String.IsNullOrWhiteSpace(Settings.FromFile))
                {
                    var collection = new PrivateFontCollection();
                    collection.AddFontFile(Settings.FromFile);
                    var fontFamily = new FontFamily(Path.GetFileNameWithoutExtension(Settings.FromFile), collection);
                    font = new Font(fontFamily, Settings.FontSize);
                }
                else
                {
                    font = new Font(new FontFamily(Settings.FontName), Settings.FontSize);
                }

                using (var g = Graphics.FromImage(bitmap))
                {
                    if (Settings.BitmapFont)
                    {
                        g.SmoothingMode = SmoothingMode.None;
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                    }
                    else
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        //g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    }

                    SizeF size = g.MeasureString("A", font);
                    //size = new SizeF(size.Width * 0.6f, size.Height * 0.9f);
                    for (int p = 0; p < Settings.GlyphLineCount; p++)
                    {
                        for (int n = 0; n < Settings.GlyphsPerLine; n++)
                        {
                            char c = (char)(n + p * Settings.GlyphsPerLine);
                            g.DrawString(c.ToString(), font, Brushes.White,
                                n * size.Width, p * size.Height);
                        }
                    }
                }
                bitmap.Save(Settings.FontBitmapFilename);
            }
            //Process.Start(Settings.FontBitmapFilename);
        }

    }
}
