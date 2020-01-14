using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class uiViewButton : uiView
    {
        internal string Text { get; set; }
        public override void OnLoad()
        {
            base.OnLoad();
            Text = JsonNode["Text"].ToString();
            InvokeMouseClick += (pt) => { Console.WriteLine("Click " + Text); };
            InvokeMouseEnter += (pt) => { Console.WriteLine("Enter " + Text); };
            InvokeMouseLeave += (pt) => { Console.WriteLine("Leave " + Text); };
        }
        public override void OnDraw()
        {
            DrawingParams info = new DrawingParams();
            info.rect = RectAbsolute;
            info.color = Color.Green;
            uiViewManager.Inst.InvokeDrawRectFill?.Invoke(info);
        }
    }
}
