using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace system
{

    class uiView
    {
        public const string JsonName = "Name";
        public const string JsonType = JsonTypeButton;
        public const string JsonChilds = "Childs";
        public const string JsonLocalX = "LocalX";
        public const string JsonLocalY = "LocalY";
        public const string JsonWidth = "Width";
        public const string JsonHeight = "Height";
        public const string JsonDepth = "Depth";

        public const string JsonTypeButton = "Button";
        public const string JsonTypeCheckBox = "CheckBox";
        public const string JsonTypeEditView = "EditView";
        public const string JsonTypeComboBox = "ComboBox";

        internal List<uiView> Childs = new List<uiView>();
        internal uiView Parent { get; set; }
        internal JObject JsonNode { get; set; }

        internal string Name { get; set; }
        internal string Type { get; set; }
        internal bool Visiable { get; set; }
        internal bool Enable { get; set; }
        internal bool Focused { get; set; }
        internal int Depth { get; set; }
        internal Rectangle RectRelative { get; set; }
        internal Rectangle RectAbsolute { get; set; }


        internal uiView BornChild(string type)
        {
            uiView view = null;
            switch (type)
            {
                case JsonTypeButton: view = new uiView(); break;
                case JsonTypeCheckBox: view = new uiView(); break;
                case JsonTypeEditView: view = new uiView(); break;
                case JsonTypeComboBox: view = new uiView(); break;
                default: break;
            }
            view.Parent = this;
            Childs.Add(view);
            return view;
        }
        public void LoadAndMakeTree(JObject json)
        {
            JsonNode = json;
            var childs = json[JsonChilds];
            foreach(JObject child in childs)
            {
                string type = json[JsonType].ToString();
                uiView view = BornChild(type);
                view.LoadAndMakeTree(child);
            }
        }

        public virtual void UpdateViewFromJson()
        {
            Name = JsonNode[JsonName].ToString();
            Type = JsonNode[JsonType].ToString();
            Depth = int.Parse(JsonNode[JsonDepth].ToString());

            int localX = int.Parse(JsonNode[JsonLocalX].ToString());
            int localY = int.Parse(JsonNode[JsonLocalY].ToString());
            Point localPt = new Point(localX, localY);
            int width = int.Parse(JsonNode[JsonWidth].ToString());
            int height = int.Parse(JsonNode[JsonHeight].ToString());
            Size size = new Size(width, height);
            RectRelative = new Rectangle(localPt, size);

            Point parentAbPt = Parent.RectAbsolute.Location;
            RectAbsolute = new Rectangle(new Point(parentAbPt.X + localPt.X, parentAbPt.Y + localPt.Y), size);

            Visiable = true;
            Enable = true;
            Focused = false;
        }
        public virtual void DrawView()
        {

        }
        public uiView FindTopView(int worldX, int worldY)
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
