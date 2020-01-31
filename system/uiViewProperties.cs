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
    { None, View, Button, Image }

    public class RscImage
    {
        public string filename;
        public float left;
        public float right;
        public float top;
        public float bottom;
        private int texID = -1;
        public void LoadTexture()
        {
            if(texID < 0)
                texID = Renderer.InitTexture(filename);
        }
        public int GetTexID() { return texID; }
    }
    public class PropImage : ViewProperty
    {
        public RscImage ImgNormal;
        public float bright;
        public PropImage() {
            ImgNormal = new RscImage();
        }
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
        public string SerializeJson()
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
        public void Parse(string json)
        {
            JObject obj = JObject.Parse(json);
            Parse(obj);
        }
        public void Parse(JObject obj)
        {
            Me = ToClassObject(obj);
            var childs = obj["Childs"];
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
                //case uiViewType.Label: return obj.ToObject<ViewPropLabel>();
                //case uiViewType.Checkbox: return obj.ToObject<ViewPropCheckBox>();
                //case uiViewType.EditBox: return obj.ToObject<ViewPropEditBox>();
                //case uiViewType.ComboBox: return obj.ToObject<ViewPropComboBox>();
                default: break;
            }
            return null;
        }
    }
}
