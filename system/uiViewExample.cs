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

namespace system
{
    public partial class uiViewExample : Form
    {
        public Renderer mRender = new Renderer();
        public uiViewManager mUIMgr = uiViewManager.Inst;
        public uiViewExample()
        {
            InitializeComponent();
            InitRenderer();
            InitUIManager();

            Timer timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 20;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            mUIMgr.MouseEventCall();
            mRender.mGlView.Invalidate();
        }

        public void InitRenderer()
        {
            mRender.Initialize(panel1, panel1.Width, panel1.Height);

            mRender.mGlView.MouseMove += (obj, args) => { mUIMgr.SetMouseEvent(new Point(args.X, args.Y), null); };
            mRender.mGlView.MouseDown += (obj, args) => { mUIMgr.SetMouseEvent(new Point(args.X, args.Y), true); };
            mRender.mGlView.MouseUp += (obj, args) => { mUIMgr.SetMouseEvent(new Point(args.X, args.Y), false); };
            mRender.OnDraw += () => { mUIMgr.Draw(); };
        }
        public void InitUIManager()
        {
            mUIMgr.InvokeDrawRectFill += (param) => { mRender.DrawRect(param); };
            mUIMgr.InvokeDrawRectOutline += (param) => { mRender.DrawOutline(param); };
            mUIMgr.InvokeDrawBitmap += (param) => { mRender.DrawTexture(param); };
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

            ViewPropertiesTree obj = new ViewPropertiesTree();
            obj.Parse(text);
            bool ret = mUIMgr.Load(obj);
            if (!ret)
                MessageBox.Show(mUIMgr.ErrorMessage);
        }
    }
}
