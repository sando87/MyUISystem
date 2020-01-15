﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class uiViewButton : uiView
    {
        ViewPropButton JsonNodeEnd;
        public override void OnLoad(int depth)
        {
            base.OnLoad(depth);
            JsonNodeEnd = JsonNode as ViewPropButton;
            string Text = JsonNodeEnd.ButtonText;
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
