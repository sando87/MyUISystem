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
        public static int InitTexture(string _fullname)
        {
            if (_fullname == null || _fullname.Length == 0)
                return -1;

            int texID = 0;
            string fullname = _fullname;
            Bitmap bitmap = new Bitmap(fullname);
            GL.GenTextures(1, out texID);
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            BitmapData data = bitmap.LockBits(new Rectangle(0,0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            return texID;
        }
        public static int InitTexture(Bitmap bitmap)
        {
            int texID = 0;
            GL.GenTextures(1, out texID);
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            return texID;
        }
        public static int InitTexture(IntPtr ptr, int width, int height)
        {
            int texID = 0;
            GL.GenTextures(1, out texID);
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, ptr);
            return texID;
        }

        public static void ReleaseTexture(int _texID)
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

        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            OnDraw?.Invoke();

            mGlView.SwapBuffers();
        }


        public void DrawRect(DrawArgs param)
        {
            int left = param.rect.Left;
            int right = param.rect.Right;
            int top = mGlView.Height - param.rect.Top;
            int bottom = mGlView.Height - param.rect.Bottom;

            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Color3(param.color);
            GL.Vertex2(left, top); //lt
            GL.Vertex2(left, bottom); //lb
            GL.Vertex2(right, top);//rt
            GL.Vertex2(right, bottom);//rb
            GL.End();
        }
        public void DrawOutline(DrawArgs param)
        {
            int left = param.rect.Left;
            int right = param.rect.Right;
            int top = mGlView.Height - param.rect.Top;
            int bottom = mGlView.Height - param.rect.Bottom;

            GL.Begin(PrimitiveType.LineStrip);
            GL.LineWidth(2.0f);
            GL.Color3(param.color);
            GL.Vertex2(left, top); //lt
            GL.Vertex2(left, bottom); //lb
            GL.Vertex2(right, bottom);//rb
            GL.Vertex2(right, top);//rt
            GL.Vertex2(left, top); //lt
            GL.End();
        }
        public void DrawTexture(DrawArgs param)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, param.texID);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            //GL.BlendFunc(BlendingFactor.OneMinusSrcAlpha, BlendingFactor.SrcAlpha); //font reverse
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            int left = param.rect.Left;
            int right = param.rect.Right;
            int top = mGlView.Height - param.rect.Top;
            int bottom = mGlView.Height - param.rect.Bottom;

            GL.Begin(PrimitiveType.TriangleStrip);

            GL.Color3(param.color);
            GL.TexCoord2(param.uv.Left, param.uv.Top);
            GL.Vertex2(left, top); //lt
            GL.TexCoord2(param.uv.Left, param.uv.Bottom);
            GL.Vertex2(left, bottom); //lb
            GL.TexCoord2(param.uv.Right, param.uv.Top);
            GL.Vertex2(right, top);//rt
            GL.TexCoord2(param.uv.Right, param.uv.Bottom);
            GL.Vertex2(right, bottom);//rb

            GL.End();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
        }

    }
}
