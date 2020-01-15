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
    { None, View, Button, Checkbox, Label, Image, EditBox, ComboBox }

    public class ViewPropComboBox : ViewProperty
    {
        public string items;
    }
    public class ViewPropEditBox : ViewProperty
    {
        public string EditText;
    }
    public class ViewPropImage : ViewProperty
    {
        public bool AlphaOn;
    }
    public class ViewPropLabel : ViewProperty
    {
        public string LabelName;
    }
    public class ViewPropCheckBox : ViewProperty
    {
        public bool Checked;
        public string ImgHover;
        public string ImgDown;
        public string ImgNormalChecked;
    }
    public class ViewPropButton : ViewProperty
    {
        public string ButtonText;
        public string ImgHover;
        public string ImgDown;
    }
    public class ViewProperty
    {
        public string Name;
        public uiViewType Type;
        public int LocalX;
        public int LocalY;
        public int Width;
        public int Height;
        public string ImgNormal;
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
            string me = JsonConvert.SerializeObject(Me, Formatting.Indented);
            string childs = "";
            foreach (ViewPropertiesTree child in Childs)
            {
                childs += child.SerializeJson();
                childs += ",";
            }

            me = me.Replace("{", "");
            me = me.Replace("}", "");
            me = me.Trim();
            string result = "";
            if (childs.Length > 0)
                result = "{" + me + ",\"Childs\": [" + childs + "]}";
            else
                result = "{" + me + ",\"Childs\": null}";

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
            switch(type)
            {
                case uiViewType.View: return obj.ToObject<ViewProperty>();
                case uiViewType.Button: return obj.ToObject<ViewPropButton>();
                case uiViewType.Checkbox: return obj.ToObject<ViewPropCheckBox>();
                case uiViewType.Label: return obj.ToObject<ViewPropLabel>();
                case uiViewType.Image: return obj.ToObject<ViewPropImage>();
                case uiViewType.EditBox: return obj.ToObject<ViewPropEditBox>();
                case uiViewType.ComboBox: return obj.ToObject<ViewPropComboBox>();
                default: break;
            }
            return null;
        }
    }
}
