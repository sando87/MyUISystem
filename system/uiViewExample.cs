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
            //InitUIManager("editorJson.json"); 

            mTimer.Tick += Timer_Tick;
            mTimer.Interval = 20;
            mTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            mUIMgr.MouseEventCall();
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
            ViewPropertiesTree obj = new ViewPropertiesTree();
            obj.Parse(jsonString);
            mUIMgr.Load(obj);

            mUIMgr.InvokeDrawRectFill += (param) => { mRender.DrawRect(param.rect, param.color); };
            mUIMgr.InvokeDrawRectOutline += (param) => { mRender.DrawOutline(param.rect, param.color, param.lineWidth); };
            mUIMgr.InvokeDrawBitmapRect += (param) => { mRender.DrawTextureRect(param.rect, param.texID, param.uv); };
        }

        public void CreateTestJson()
        {
            ViewProperty prop = new ViewProperty();
            prop.Name = "RootView";
            prop.Type = uiViewType.View;
            prop.LocalX = 10;
            prop.LocalY = 10;
            prop.Width = 300;
            prop.Height = 200;

            ViewProperty child1 = new ViewProperty();
            child1.Name = "Button1";
            child1.Type = uiViewType.View;
            child1.LocalX = 10;
            child1.LocalY = 10;
            child1.Width = 80;
            child1.Height = 30;

            ViewProperty child2 = new ViewProperty();
            child2.Name = "Button2";
            child2.Type = uiViewType.View;
            child2.LocalX = 100;
            child2.LocalY = 10;
            child2.Width = 80;
            child2.Height = 30;

            ViewPropertiesTree ppp = new ViewPropertiesTree();
            ppp.Me = prop;
            ppp.Childs.Add(new ViewPropertiesTree());
            ppp.Childs.Add(new ViewPropertiesTree());

            ppp.Childs[0].Me = child1;
            ppp.Childs[1].Me = child2;

            string rets = ppp.ToJSON();
            File.WriteAllText("test.json", rets);
            ViewPropertiesTree ppprets = new ViewPropertiesTree();
            ppprets.Parse(rets);
        }
    }
}
