using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace system
{
    public struct EventParams
    {
        public int mouseX;
        public int mouseY;
        public bool mouseDown;
        public char key;
        public bool keyDown;
        public uiView viewDown;
        public uiView viewHovered;
    }
    public class DrawingParams
    {
        public RectangleF uv;
        public Rectangle rect;
        public Color color;
        public int texID;
        public Bitmap bitmap;
        public int lineWidth;
        public string text;
    }

    public class uiViewManager
    {
        static public uiViewManager Inst = new uiViewManager();

        public delegate void DelegateRender(DrawingParams info);
        public DelegateRender InvokeDrawRefresh;
        public DelegateRender InvokeDrawRectFill;
        public DelegateRender InvokeDrawRectOutline;
        public DelegateRender InvokeDrawBitmap;
        public DelegateRender InvokeDrawBitmapRect;

        internal EventParams CurrentEventInfo;
        internal EventParams PreviousEventInfo;
        internal uiView RootView { get; set; }

        internal uiView FindTopView(int _worldX, int _worldY)
        {
            return RootView.FindTopView(_worldX, _worldY);
        }
        internal void ProcMouseUpDownClick(EventParams eventInfo)
        {
            Point pt = new Point(eventInfo.mouseX, eventInfo.mouseY);
            if (!PreviousEventInfo.mouseDown && eventInfo.mouseDown) //Mouse Down Triggered
            {
                if (PreviousEventInfo.viewDown != null)
                    PreviousEventInfo.viewDown.InvokeMouseUp?.Invoke(pt);

                uiView downView = RootView.FindTopView(eventInfo.mouseX, eventInfo.mouseY);
                if (downView != null)
                    downView.InvokeMouseDown?.Invoke(pt);

                PreviousEventInfo = eventInfo;
                PreviousEventInfo.mouseDown = true;
                PreviousEventInfo.viewDown = downView;
            }
            else if (PreviousEventInfo.mouseDown && !eventInfo.mouseDown) //Mouse Up Triggered
            {
                if (PreviousEventInfo.viewDown != null)
                    PreviousEventInfo.viewDown.InvokeMouseUp?.Invoke(pt);

                uiView upView = RootView.FindTopView(eventInfo.mouseX, eventInfo.mouseY);
                if (upView != null && upView == PreviousEventInfo.viewDown)
                    upView.InvokeMouseClick?.Invoke(pt); //Mouse Click Invoked in case of same view...

                PreviousEventInfo = eventInfo;
                PreviousEventInfo.mouseDown = false;
                PreviousEventInfo.viewDown = null;
            }
        }
        internal void ProcMouseEnterLeaveMove(EventParams eventInfo)
        {
            Point pt_pre = new Point(PreviousEventInfo.mouseX, PreviousEventInfo.mouseY);
            Point pt = new Point(eventInfo.mouseX, eventInfo.mouseY);
            if (pt.X == pt_pre.X && pt.Y == pt_pre.Y)
                return;

            uiView hoverView = RootView.FindTopView(eventInfo.mouseX, eventInfo.mouseY);
            if(hoverView == null)
            {
                if (PreviousEventInfo.viewHovered != null)
                    PreviousEventInfo.viewHovered.InvokeMouseLeave?.Invoke(pt);

                PreviousEventInfo = eventInfo;
                PreviousEventInfo.viewHovered = null;
            }
            else
            {
                if (PreviousEventInfo.viewHovered == null)
                    hoverView.InvokeMouseEnter?.Invoke(pt);
                else if (PreviousEventInfo.viewHovered == hoverView)
                    PreviousEventInfo.viewHovered.InvokeMouseMove?.Invoke(pt);
                else if (PreviousEventInfo.viewHovered != null)
                {
                    PreviousEventInfo.viewHovered.InvokeMouseLeave?.Invoke(pt);
                    hoverView.InvokeMouseEnter?.Invoke(pt);
                }

                PreviousEventInfo = eventInfo;
                PreviousEventInfo.viewHovered = hoverView;
            }
        }

        public uiViewManager()
        {
            RootView = new uiView();
            CurrentEventInfo = new EventParams();
            PreviousEventInfo = new EventParams();
        }
        public void Load(uiViewProperties json)
        {
            RootView.MakeTree(json);
            RootView.LoadAll(0);
        }
        public void MouseEventCall()
        {
            ProcMouseUpDownClick(CurrentEventInfo);
            ProcMouseEnterLeaveMove(CurrentEventInfo);
        }
        public void Draw()
        {
            RootView.DrawAll();
        }
        public void SetMouseEvent(Point pt, bool? down)
        {
            CurrentEventInfo.mouseX = pt.X;
            CurrentEventInfo.mouseY = pt.Y;
            if(down != null)
                CurrentEventInfo.mouseDown = down.Value;
        }
        public string ToJsonString()
        {
            return RootView.JsonNode.ToJSON();
        }
    }
}
