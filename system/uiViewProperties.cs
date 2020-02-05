using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace system
{
    public enum uiViewType
    { View, Button, Image, Font }
    public enum uiViewAlign
    {
        TopLeft,      Top,              TopRight,
        Left,           Center,         Right,
        BottomLeft, Bottom,         BottomRight,
    }
    public enum uiViewStyle
    {
        Regular,
        Bold,
        Italic,
        Underline,
        Strikeout
    }

    public class RscImage
    {
        public string filename;
        public float left;
        public float right;
        public float top;
        public float bottom;
        private int texID = -1;
        public bool LoadTexture()
        {
            if(texID < 0)
                texID = Renderer.InitTexture(filename);
            return true;
        }
        public int GetTexID() { return texID; }
    }

    public class PropFont : ViewProperty
    {
        public string text;
        public string fontName;
        public int fontSize;
        public uiViewStyle style;
        public uiViewAlign align;
        public float gapRate;
        private FontCapture font;
        private System.Drawing.Size textSize;
        public override bool Load() {
            if(fontName != null && fontName.Length > 0 && fontSize > 0)
            {
                if (font != null)
                    font.ReleaseTexture();

                font = FontCapture.Capture(fontName, fontSize, style);
                int chWidth = (int)(font.SizeChar.Width * gapRate);
                int chHeight = (int)font.SizeChar.Height; // (int)(font.SizeChar.Height * gapRate);
                textSize = new System.Drawing.Size(chWidth * text.Length, chHeight);
            }
            return true;
        }
        public FontCapture Font { get { return font; } }
        public System.Drawing.Size GetTextSize { get { return textSize; } }
    }
    public class PropImage : ViewProperty
    {
        public RscImage ImgNormal;
        public float bright;
        public PropImage() {
            ImgNormal = new RscImage();
        }
        public override bool Load() { return ImgNormal.LoadTexture(); }
    }
    public class PropButton : ViewProperty
    {
        public string ButtonText;
    }
    public class ViewProperty
    {
        public string Name;
        public uiViewType Type;
        public int LocalX;
        public int LocalY;
        public int Width;
        public int Height;
        public string Color;
        public virtual bool Load() { return true; }
    }

    public class ViewPropertiesTree
    {
        public ViewProperty Me = new ViewProperty();
        public List<ViewPropertiesTree> Childs = new List<ViewPropertiesTree>();
        public string ToJSON()
        {
            string ret =  SerializeJson();
            var tmp = JObject.Parse(ret);
            return JsonConvert.SerializeObject(tmp, Formatting.Indented);
        }
        public void Parse(string json)
        {
            JObject obj = JObject.Parse(json);
            Parse(obj);
        }

        private string SerializeJson()
        {
            string me = JsonConvert.SerializeObject(Me);
            string childs = "";
            foreach (ViewPropertiesTree child in Childs)
            {
                childs += child.SerializeJson();
                childs += ",";
            }

            string subString = me.Substring(0, me.Length - 1);
            string result = "";
            if (childs.Length > 0)
                result = subString + ",\"Childs\": [" + childs + "]}";
            else
                result = subString + ",\"Childs\": null}";

            return result;
        }
        private void Parse(JObject obj)
        {
            Me = ToClassObject(obj);
            var childs = obj["Childs"];
            if (childs == null)
                return;

            foreach(JObject child in childs)
            {
                ViewPropertiesTree node = new ViewPropertiesTree();
                node.Parse(child);
                Childs.Add(node);
            }
        }
        private ViewProperty ToClassObject(JObject obj)
        {
            uiViewType type = obj["Type"].ToObject<uiViewType>();
            switch (type)
            {
                case uiViewType.View: return obj.ToObject<ViewProperty>();
                case uiViewType.Button: return obj.ToObject<PropButton>();
                case uiViewType.Image: return obj.ToObject<PropImage>();
                case uiViewType.Font: return obj.ToObject<PropFont>();
                default: break;
            }
            return null;
        }

        static public uiView CreateView(uiViewType type)
        {
            uiView view = null;
            switch (type)
            {
                case uiViewType.View: view = new uiView(); break;
                case uiViewType.Button: view = new uiViewButton(); break;
                case uiViewType.Image: view = new uiViewImage(); break;
                case uiViewType.Font: view = new uiViewFont(); break;
                default: break;
            }
            return view;
        }
    }
}
