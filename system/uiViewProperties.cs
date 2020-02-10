﻿using System;
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
        private int texID;
        public RscImage()
        {
            filename = "defaultImage.png";
            left = 0.0f;
            right = 1.0f;
            top = 0.0f;
            bottom = 1.0f;
            texID = -1;
        }
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
        public PropFont()
        {
            text = "text";
            fontName = "굴림체";
            fontSize = 20;
            style = uiViewStyle.Regular;
            align = uiViewAlign.Center;
            gapRate = 0.6f;
        }
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
        public PropImage() {
            ImgNormal = new RscImage();
        }
        public override bool Load() { return ImgNormal.LoadTexture(); }
    }
    public class PropButton : PropFont
    {
        public RscImage ImgNormal;
        public string ButtonColor;
        public PropButton() {
            ImgNormal = new RscImage();
            ButtonColor = "255.225.225.225";
        }
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
        public ViewProperty()
        {
            Name = "Name";
            Type = uiViewType.View;
            LocalX = 0;
            LocalY = 0;
            Width = 80;
            Height = 30;
            Color = "255.240.240.240";
        }
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
                case uiViewType.View:
                    view = new uiView();
                    view.JsonNode = new ViewProperty();
                    view.JsonNode.Type = type;
                    break;
                case uiViewType.Button:
                    view = new uiViewButton();
                    view.JsonNode = new PropButton();
                    view.JsonNode.Type = type;
                    break;
                case uiViewType.Image:
                    view = new uiViewImage();
                    view.JsonNode = new PropImage();
                    view.JsonNode.Type = type;
                    break;
                case uiViewType.Font:
                    view = new uiViewFont();
                    view.JsonNode = new PropFont();
                    view.JsonNode.Type = type;
                    break;
                default: break;
            }
            return view;
        }
    }
}
