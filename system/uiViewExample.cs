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
        public Timer mTimer = new Timer();
        public uiViewExample()
        {
            //CreateTestJson();

            InitializeComponent();
            InitRenderer();
            InitUIManager("test.json");

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

        public void CreateTestJson()
        {
            uiViewProperties prop = new uiViewProperties();
            prop.Name = "RootView";
            prop.Type = uiViewType.View;
            prop.LocalX = "10";
            prop.LocalY = "10";
            prop.Width = "300";
            prop.Height = "200";
            prop.Childs = new uiViewProperties[2];

            prop.Childs[0].Name = "Button1";
            prop.Childs[0].Type = uiViewType.Button;
            prop.Childs[0].LocalX = "10";
            prop.Childs[0].LocalY = "10";
            prop.Childs[0].Width = "80";
            prop.Childs[0].Height = "30";
            prop.Childs[0].Text = "Button_Left";

            prop.Childs[1].Name = "Button2";
            prop.Childs[1].Type = uiViewType.Button;
            prop.Childs[1].LocalX = "100";
            prop.Childs[1].LocalY = "10";
            prop.Childs[1].Width = "80";
            prop.Childs[1].Height = "30";
            prop.Childs[1].Text = "Button_Right";

            string rets = prop.ToJSON();
            File.WriteAllText("test.json", rets);
        }
    }
}
