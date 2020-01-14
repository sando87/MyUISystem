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
    { None, View, Button, Checkbox, Image, EditBox, ComboBox }

    public class uiViewProperties
    {
        public string Name;
        public uiViewType Type;
        public int LocalX;
        public int LocalY;
        public int Width;
        public int Height;
        public string Text;
        public uiViewProperties[] Childs;
        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
        static public uiViewProperties Parse(string json)
        {
            return JsonConvert.DeserializeObject<uiViewProperties>(json);
        }
    }
}
