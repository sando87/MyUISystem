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
    public partial class uiViewEditor : Form
    {
        public Renderer mRender = new Renderer();
        public uiViewManager mUIMgr = uiViewManager.Inst;
        public Timer mTimer = new Timer();

        public uiViewEditor(int _w, int _h)
        {
            InitializeComponent();

            InitRenderer();
            //InitUIManager("test.json");

            mTimer.Tick += Timer_Tick;
            mTimer.Interval = 20;
            mTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            mUIMgr.MouseEventCall();
            mRender.Draw();
        }

        public void InitRenderer()
        {
            mRender.Initialize(panel1, panel1.Width, panel1.Height);

            mRender.mGlView.MouseMove += (obj, args) => { mUIMgr.SetMouseEvent(new Point(args.X, args.Y), null); };
            mRender.mGlView.MouseDown += (obj, args) => { mUIMgr.SetMouseEvent(new Point(args.X, args.Y), true); };
            mRender.mGlView.MouseUp += (obj, args) => { mUIMgr.SetMouseEvent(new Point(args.X, args.Y), false); };
            mRender.OnDraw += () => { mUIMgr.Draw(); };
        }
        public void InitUIManager(string jsonFullname)
        {
            string jsonString = File.ReadAllText(jsonFullname);
            uiViewProperties obj = uiViewProperties.Parse(jsonString);
            mUIMgr.Load(obj);

            mUIMgr.InvokeDrawRefresh += (param) => { mRender.Draw(); };
            mUIMgr.InvokeDrawRectFill += (param) => { mRender.DrawRect(param.rect, param.color); };
            mUIMgr.InvokeDrawRectOutline += (param) => { mRender.DrawOutline(param.rect, param.color, param.lineWidth); };
            mUIMgr.InvokeDrawBitmapRect += (param) => { mRender.DrawTextureRect(param.rect, param.texID, param.uv); };
        }

    }
}
