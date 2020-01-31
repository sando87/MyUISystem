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
        public override void OnLoad(int depth)
        {
            base.OnLoad(depth);

            string Text = (JsonNode as PropButton).ButtonText;
            InvokeMouseClick += (pt) => { Console.WriteLine("Click " + Text); };
            InvokeMouseEnter += (pt) => { Console.WriteLine("Enter " + Text); };
            InvokeMouseLeave += (pt) => { Console.WriteLine("Leave " + Text); };
        }
        public override void OnDraw()
        {
            uiViewManager.Inst.InvokeDrawRectFill?.Invoke(RenderParam);
        }
    }
}
