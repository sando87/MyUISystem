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
        internal ViewProperty JsonNode { get; set; }
        internal DrawingParams RenderParam = new DrawingParams();

        internal bool Downed { get; set; }
        internal bool Hovered { get; set; }
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

            RenderParam.rect = RectAbsolute;
            if (JsonNode.Color != null && JsonNode.Color.Length != 0)
            {
                string[] argb = JsonNode.Color.Split('.');
                UInt32 col = 0;
                for (int i = 0; i < argb.Length; ++i)
                {
                    int shift = (argb.Length - i - 1) * 8;
                    col |= (UInt32.Parse(argb[i]) << shift);
                }
                RenderParam.color = Color.FromArgb((int)col);
            }
        }

        internal void LoadAll(int depth = -1)
        {
            if (depth < 0)
                depth = Depth;

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
                case uiViewType.Image: view = new uiViewImage(); break;
                //case uiViewType.Checkbox: view = new uiView(); break;
                //case uiViewType.EditBox: view = new uiView(); break;
                //case uiViewType.ComboBox: view = new uiView(); break;
                default: break;
            }
            view.Parent = this;
            Childs.Add(view);
            return view;
        }
        internal void MakeTree(ViewPropertiesTree json)
        {
            JsonNode = json.Me;
            if (json.Childs == null)
                return;

            foreach(ViewPropertiesTree child in json.Childs)
            {
                uiView view = BornChild(child.Me.Type);
                view.MakeTree(child);
                uiViewManager.Inst.RegisterView(view);
            }
        }
        internal uiView FindTopView(int worldX, int worldY)
        {
            if (!Enable || !RectAbsolute.Contains(worldX, worldY))
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
        internal void ChangeParent(uiView newParent)
        {
            if (Parent == newParent)
                return;

            Parent.Childs.Remove(this);
            newParent.Childs.Add(this);
            Parent = newParent;
            int localX = RectAbsolute.Location.X - newParent.RectAbsolute.Location.X;
            int localY = RectAbsolute.Location.Y - newParent.RectAbsolute.Location.Y;
            JsonNode.LocalX = localX;
            JsonNode.LocalY = localY;
            LoadAll();
        }
        internal ViewPropertiesTree ToPropTree()
        {
            ViewPropertiesTree tree = new ViewPropertiesTree();
            tree.Me = JsonNode;
            foreach (uiView child in Childs)
                tree.Childs.Add(child.ToPropTree());
            return tree;
        }
        
        
    }
}
