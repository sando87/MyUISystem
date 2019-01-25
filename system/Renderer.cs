using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace system
{
    public class Renderer
    {
        //static private Renderer mInst = null;
        //static public Renderer GetInst() { if (mInst == null) mInst = new Renderer(); return mInst; }
        public Renderer() { }

        bool loaded = false;
        public OpenTK.GLControl mGlView = new OpenTK.GLControl();
        public delegate void DelDraw();
        public DelDraw OnDraw;

        private int MIN(int _a, int _b) { return (_a < _b) ? _a : _b; }
        private int MAX(int _a, int _b) { return (_a > _b) ? _a : _b; }

        public void Initialize(Panel _panel, int _w, int _h)
        {
            if (loaded)
                return;

            mGlView.BackColor = System.Drawing.Color.Black;
            mGlView.Location = new System.Drawing.Point(0, 0);
            mGlView.Size = new System.Drawing.Size(_w, _h);
            mGlView.VSync = true;
            mGlView.Load += new EventHandler(this.glControl1_Load);
            mGlView.Paint += new PaintEventHandler(this.glControl1_Paint);

            _panel.Controls.Add(mGlView);

        }
        public static int InitTexture(string _filename)
        {
            int texID = 0;
            Bitmap bitmap = new Bitmap(_filename);
            GL.GenTextures(1, out texID);
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            BitmapData data = bitmap.LockBits(new Rectangle(0,0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            return texID;
        }

        public void ReleaseTexture(int _texID)
        {
            GL.DeleteTextures(1, ref _texID);
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.Black);

            int w = mGlView.Width;
            int h = mGlView.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area


            //FontTextureID = LoadTexture(Settings.FontBitmapFilename);
            //GL.Enable(EnableCap.Texture2D);
            //GL.ClearColor(Color.ForestGreen);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.Ortho(0, Width, Height, 0, 0, 1);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            OnDraw?.Invoke();

            //GL.Clear(ClearBufferMask.ColorBufferBit);
            //GL.Disable(EnableCap.Blend);
            //Blt(10, 40, TextureWidth, TextureHeight);
            //GL.Enable(EnableCap.Blend);
            //DrawText(10, 10, Settings.Text);
            //SwapBuffers();

            mGlView.SwapBuffers();
        }


        public void Draw()
        {
            mGlView.Invalidate();
        }

        public void DrawOutline(Rectangle _rect, Color _color, float _lineWidth = 2.0f)
        {
            int left = _rect.Left;
            int right = _rect.Right;
            int top = mGlView.Height - _rect.Top;
            int bottom = mGlView.Height - _rect.Bottom;

            GL.Begin(PrimitiveType.LineStrip);
            GL.LineWidth(2.0f);
            GL.Color3(_color);
            GL.Vertex2(left, top); //lt
            GL.Vertex2(left, bottom); //lb
            GL.Vertex2(right, bottom);//rb
            GL.Vertex2(right, top);//rt
            GL.Vertex2(left, top); //lt
            GL.End();
        }
        public void DrawRect(Rectangle _rect, Color _color)
        {
            int left = _rect.Left;
            int right = _rect.Right;
            int top = mGlView.Height - _rect.Top;
            int bottom = mGlView.Height - _rect.Bottom;

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Color3(_color);
            GL.Vertex2(left, top); //lt
            GL.Vertex2(left, bottom); //lb
            GL.Vertex2(right, top);//rt
            GL.Vertex2(right, bottom);//rb
            GL.End();
        }

        public void DrawTexture(Rectangle _rect, int _texID)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _texID);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            int left = _rect.Left;
            int right = _rect.Right;
            int top = mGlView.Height - _rect.Top;
            int bottom = mGlView.Height - _rect.Bottom;

            GL.Begin(PrimitiveType.TriangleStrip);

            GL.TexCoord2(0, 0);
            GL.TexCoord2(0, 1);
            GL.TexCoord2(1, 0);
            GL.TexCoord2(1, 1);

            GL.Vertex2(left, top); //lt
            GL.Vertex2(left, bottom); //lb
            GL.Vertex2(right, bottom);//rb
            GL.Vertex2(right, top);//rt

            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
        }
        public void DrawTextureRect(Rectangle _rect, int _texID, RectangleF _uv)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _texID);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            int left = _rect.Left;
            int right = _rect.Right;
            int top = mGlView.Height - _rect.Top;
            int bottom = mGlView.Height - _rect.Bottom;

            GL.Begin(PrimitiveType.TriangleStrip);

            GL.TexCoord2(_uv.Left, _uv.Top);
            GL.TexCoord2(_uv.Left, _uv.Bottom);
            GL.TexCoord2(_uv.Right, _uv.Top);
            GL.TexCoord2(_uv.Right, _uv.Bottom);

            GL.Vertex2(left, top); //lt
            GL.Vertex2(left, bottom); //lb
            GL.Vertex2(right, top);//rt
            GL.Vertex2(right, bottom);//rb

            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
        }
        public void DrawText(int x, int y, string text)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, FontManager.Settings.TextureID);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.Begin(PrimitiveType.Quads);
        
            float u_step = (float)FontManager.Settings.GlyphWidth / (float)FontManager.Settings.TextureWidth;
            float v_step = (float)FontManager.Settings.GlyphHeight / (float)FontManager.Settings.TextureHeight;
        
            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                float u = (float)(idx % FontManager.Settings.GlyphsPerLine) * u_step;
                float v = (float)(idx / FontManager.Settings.GlyphsPerLine) * v_step;
        
                GL.TexCoord2(u, v);
                GL.Vertex2(x, y);
                GL.TexCoord2(u + u_step, v);
                GL.Vertex2(x + FontManager.Settings.GlyphWidth, y);
                GL.TexCoord2(u + u_step, v + v_step);
                GL.Vertex2(x + FontManager.Settings.GlyphWidth, y + FontManager.Settings.GlyphHeight);
                GL.TexCoord2(u, v + v_step);
                GL.Vertex2(x, y + FontManager.Settings.GlyphHeight);
        
                x += FontManager.Settings.CharXSpacing;
            }
        
            GL.End();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
        }


    }
}
