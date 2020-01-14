using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace system
{
    enum uiViewType
    { None, View, Button, Checkbox, Image, EditBox, ComboBox }

    public class uiViewProperties
    {
        public string Name;
        public uiViewType Type;
        public string LocalX;
        public string LocalY;
        public string Width;
        public string Height;
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
