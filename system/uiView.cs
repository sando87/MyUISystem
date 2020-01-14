using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{

    public class uiView
    {
        internal List<uiView> Childs = new List<uiView>();
        internal uiView Parent { get; set; }
        internal uiViewProperties JsonNode { get; set; }

        internal bool Visiable { get; set; }
        internal bool Enable { get; set; }
        internal int Depth { get; set; }
        internal Rectangle RectRelative { get; set; }
        internal Rectangle RectAbsolute { get; set; }

        public delegate void DelMouseEvent(Point pt);
        public DelMouseEvent InvokeMouseDown = null;
        public DelMouseEvent InvokeMouseUp = null;
        public DelMouseEvent InvokeMouseClick = null;
        public DelMouseEvent InvokeMouseEnter = null;
        public DelMouseEvent InvokeMouseMove = null;
        public DelMouseEvent InvokeMouseLeave = null;

        public virtual void OnDraw()
        {
            DrawingParams info = new DrawingParams();
            info.rect = RectAbsolute;
            info.color = Color.Gray;
            uiViewManager.Inst.InvokeDrawRectFill?.Invoke(info);
        }
        public virtual void OnLoad(int depth)
        {
            Depth = depth;
            Visiable = true;
            Enable = true;

            Point localPt = new Point(JsonNode.LocalX, JsonNode.LocalY);
            Size size = new Size(JsonNode.Width, JsonNode.Height);
            RectRelative = new Rectangle(localPt, size);

            Point parentAbPt = Parent == null ? new Point() : Parent.RectAbsolute.Location;
            RectAbsolute = new Rectangle(new Point(parentAbPt.X + localPt.X, parentAbPt.Y + localPt.Y), size);
        }

        internal void LoadAll(int depth)
        {
            OnLoad(depth);
            depth++;
            for (int i = 0; i < Childs.Count; ++i)
                Childs[i].LoadAll(depth);
        }
        internal void DrawAll()
        {
            if (!Visiable)
                return;

            OnDraw();
            for (int i = 0; i < Childs.Count; ++i)
                Childs[i].DrawAll();
        }
        internal uiView BornChild(uiViewType type)
        {
            uiView view = null;
            switch (type)
            {
                case uiViewType.View: view = new uiView(); break;
                case uiViewType.Button: view = new uiViewButton(); break;
                case uiViewType.Checkbox: view = new uiView(); break;
                case uiViewType.EditBox: view = new uiView(); break;
                case uiViewType.ComboBox: view = new uiView(); break;
                default: break;
            }
            view.Parent = this;
            Childs.Add(view);
            return view;
        }
        internal void MakeTree(uiViewProperties json)
        {
            JsonNode = json;
            if (JsonNode.Childs == null)
                return;

            foreach(uiViewProperties child in JsonNode.Childs)
            {
                uiView view = BornChild(child.Type);
                view.MakeTree(child);
            }
        }
        internal uiView FindTopView(int worldX, int worldY)
        {
            if (!RectAbsolute.Contains(worldX, worldY))
                return null;

            uiView findView = this;
            for (int i = Childs.Count - 1; i >= 0; --i)
            {
                uiView child = Childs[i].FindTopView(worldX, worldY);
                if (child != null)
                {
                    findView = child;
                    break;
                }
            }
            return findView;
        }
        
        
    }
}
