using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    public enum uiViewType
    {
        View, Button, Image, Font
    }
    public class DrawArgs
    {
        public Rectangle rect;
        public RectangleF uv;
        public Color color;
        public int texID;
    }
    class Header
    {
    }
}
