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
        public bool mouseDownKeep;
        public char key;
        public bool keyDown;
        public uiView viewDown;
        public uiView viewHovered;
    }
    public class DrawingParams
    {
        public RectangleF uv;
        public Rectangle rect;
        public FontCapture font;
        public Color color;
        public int texID;
        public float bright;
        public int lineWidth;
        public float gapRate;
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
        public DelegateRender InvokeDrawText;

        internal Dictionary<string, uiView> Views = new Dictionary<string, uiView>();
        internal EventParams CurrentEventInfo;
        internal EventParams PreviousEventInfo;
        internal uiView RootView { get; set; }
        internal string ErrorMessage { get; set; }

        internal uiView ProcMouseUpDownClick(EventParams eventInfo)
        {
            uiView returnView = PreviousEventInfo.viewDown;
            Point pt = new Point(eventInfo.mouseX, eventInfo.mouseY);
            if (!PreviousEventInfo.mouseDownKeep && eventInfo.mouseDownKeep) //Mouse Down Triggered
            {
                if (PreviousEventInfo.viewDown != null)
                {
                    PreviousEventInfo.viewDown.Downed = false;
                    PreviousEventInfo.viewDown.InvokeMouseUp?.Invoke(pt);
                }

                uiView downView = RootView.FindTopView(eventInfo.mouseX, eventInfo.mouseY);
                if (downView != null)
                {
                    downView.Downed = true;
                    downView.InvokeMouseDown?.Invoke(pt);
                }
                    
                returnView = downView;
            }
            else if (PreviousEventInfo.mouseDownKeep && !eventInfo.mouseDownKeep) //Mouse Up Triggered
            {
                if (PreviousEventInfo.viewDown != null)
                {
                    PreviousEventInfo.viewDown.Downed = false;
                    PreviousEventInfo.viewDown.InvokeMouseUp?.Invoke(pt);
                }

                uiView upView = RootView.FindTopView(eventInfo.mouseX, eventInfo.mouseY);
                if (upView != null && upView == PreviousEventInfo.viewDown)
                    upView.InvokeMouseClick?.Invoke(pt); //Mouse Click Invoked in case of same view...

                returnView = null;
            }

            return returnView;
        }
        internal uiView ProcMouseEnterLeaveMove(EventParams eventInfo)
        {
            uiView returnView = PreviousEventInfo.viewHovered;
            if (eventInfo.mouseDownKeep) //마우스 클릭상태에서 이동시 Mouse Enter/Leave/Move는 안먹히도록 수정
                return returnView;

            Point pt_pre = new Point(PreviousEventInfo.mouseX, PreviousEventInfo.mouseY);
            Point pt = new Point(eventInfo.mouseX, eventInfo.mouseY);
            if (pt.X == pt_pre.X && pt.Y == pt_pre.Y && PreviousEventInfo.mouseDownKeep == eventInfo.mouseDownKeep)
                return returnView;

            uiView hoverView = RootView.FindTopView(eventInfo.mouseX, eventInfo.mouseY);
            if(hoverView == null)
            {
                if (PreviousEventInfo.viewHovered != null)
                {
                    PreviousEventInfo.viewHovered.Hovered = false;
                    PreviousEventInfo.viewHovered.InvokeMouseLeave?.Invoke(pt);
                }

                returnView = null;
            }
            else
            {
                if (PreviousEventInfo.viewHovered == null)
                {
                    hoverView.Hovered = true;
                    hoverView.InvokeMouseEnter?.Invoke(pt);
                }
                else if (PreviousEventInfo.viewHovered == hoverView)
                    PreviousEventInfo.viewHovered.InvokeMouseMove?.Invoke(pt);
                else if (PreviousEventInfo.viewHovered != null)
                {
                    PreviousEventInfo.viewHovered.Hovered = false;
                    PreviousEventInfo.viewHovered.InvokeMouseLeave?.Invoke(pt);
                    hoverView.Hovered = true;
                    hoverView.InvokeMouseEnter?.Invoke(pt);
                }

                returnView = hoverView;
            }

            return returnView;
        }

        public uiViewManager()
        {
            ErrorMessage = "";
            RootView = new uiView();
            CurrentEventInfo = new EventParams();
            PreviousEventInfo = new EventParams();
        }
        public bool Load(ViewPropertiesTree json)
        {
            RootView.MakeTree(json);
            RootView.LoadAll(0);
            return ErrorMessage.Length == 0 ? true : false;
        }
        public void MouseEventCall()
        {
            uiView retViewDown = ProcMouseUpDownClick(CurrentEventInfo);
            uiView retViewHover = ProcMouseEnterLeaveMove(CurrentEventInfo);
            PreviousEventInfo = CurrentEventInfo;
            PreviousEventInfo.viewDown = retViewDown;
            PreviousEventInfo.viewHovered = retViewHover;
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
                CurrentEventInfo.mouseDownKeep = down.Value;
        }
        public string ToJsonString()
        {
            return RootView.ToPropTree().ToJSON();
        }
        public void RegisterView(uiView view)
        {
            if (Views.ContainsKey(view.JsonNode.Name))
                ErrorMessage = "Detected Same Name View";
            Views[view.JsonNode.Name] = view;
        }
        public uiView GetView(string name)
        {
            if (!Views.ContainsKey(name))
                return null;
            return Views[name];
        }
        public string AutoName(string basicName = "Name")
        {
            int cnt = Views.Count;
            string autoName = basicName + cnt.ToString();
            while (Views.ContainsKey(autoName))
            {
                cnt++;
                autoName = basicName + cnt.ToString();
            }
            return autoName;
        }
    }
}
