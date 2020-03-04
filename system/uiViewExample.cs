using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CliLib;
using System.Runtime.InteropServices;

namespace system
{
    public partial class uiViewExample : Form
    {
        public Renderer mRender = new Renderer();
        public UIEngine mUIEngine;
        public uiViewExample()
        {
            InitializeComponent();
            InitRenderer();
            InitUIEngine();

            Timer timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 20;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            mUIEngine.DoMouseEvent();
            mRender.mGlView.Invalidate();
        }

        public void InitRenderer()
        {
            mRender.Initialize(panel1, panel1.Width, panel1.Height);

            mRender.mGlView.MouseMove += (obj, args) => { mUIEngine.SetMouseEvent(args.X, args.Y, false, false); };
            mRender.mGlView.MouseDown += (obj, args) => { mUIEngine.SetMouseEvent(args.X, args.Y, true, true); };
            mRender.mGlView.MouseUp += (obj, args) => { mUIEngine.SetMouseEvent(args.X, args.Y, false, true); };
            mRender.OnDraw += () => { mUIEngine.Draw(); };
        }
        public void InitUIEngine()
        {
            mUIEngine = UIEngine.GetInst();
            mUIEngine.SetResourcePath("../../res/");
            mUIEngine.Init(new MyUIEngineCallbacks(mRender));
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string filename = "";
            string text = Utils.LoadTextFile(out filename);

            if (filename.Length == 0)
                return;

            if (Utils.GetFileExt(filename) != "json")
            {
                MessageBox.Show("No Json");
                return;
            }

            mUIEngine.Load(filename);
        }

        public class MyUIEngineCallbacks : UIEngineCallbacks
        {
            Renderer mRender;
            DrawArgs mArgs = new DrawArgs();
            int[] mTexIDs = new int[4];
            public MyUIEngineCallbacks(Renderer render) { mRender = render; }
            public override void OnDrawFill(RenderParams param)
            {
                mArgs.rect.Location = new Point((int)param.left, (int)param.top);
                mArgs.rect.Size = new Size((int)(param.right - param.left), (int)(param.bottom - param.top));
                mArgs.color = Color.FromArgb(param.alpha, param.red, param.green, param.blue);
                mRender.DrawRect(mArgs);
            }

            public override void OnDrawOutline(RenderParams param)
            {
                mArgs.rect.Location = new Point((int)param.left, (int)param.top);
                mArgs.rect.Size = new Size((int)(param.right - param.left), (int)(param.bottom - param.top));
                mArgs.color = Color.FromArgb(param.alpha, param.red, param.green, param.blue);
                mRender.DrawOutline(mArgs);
            }

            public override void OnDrawTexture(RenderParams param)
            {
                mArgs.rect.Location = new Point((int)param.left, (int)param.top);
                mArgs.rect.Size = new Size((int)(param.right - param.left), (int)(param.bottom - param.top));
                mArgs.uv.Location = new PointF((float)param.uv_left, (float)param.uv_top);
                mArgs.uv.Size = new SizeF((float)(param.uv_right - param.uv_left), (float)(param.uv_bottom - param.uv_top));
                mArgs.color = Color.FromArgb(param.alpha, param.red, param.green, param.blue);
                Marshal.Copy(param.texture, mTexIDs, 0, 1);
                mArgs.texID = mTexIDs[0];
                mRender.DrawTexture(mArgs);
            }

            public override IntPtr OnLoadTexture(BitmapInfo bitmap)
            {
                IntPtr ptr = Marshal.AllocHGlobal(4);
                int texID = -1;
                if (bitmap.fullname.Length > 0)
                    texID = Renderer.InitTexture(bitmap.fullname);
                else
                    texID = Renderer.InitTexture(bitmap.buf, bitmap.width, bitmap.height);
                Marshal.Copy(BitConverter.GetBytes(texID), 0, ptr, 4);
                return ptr;
            }

            public override void OnReleaseTexture(IntPtr ptr)
            {
                Marshal.Copy(ptr, mTexIDs, 0, 1);
                Renderer.ReleaseTexture(mTexIDs[0]);
                Marshal.FreeHGlobal(ptr);
            }
        };
    }
}
