using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class jUIButton : jUIControl
    {
        public jUIButton()
        {
            OnMouseEnter += (ctrl, args) =>
            {
                mColor = System.Drawing.Color.Yellow;
                Console.WriteLine("Enter, btn, " + mID.ToString());
                mSystem.OnDrawRequest(null);
            };

            OnMouseLeave += (ctrl, args) =>
            {
                mColor = System.Drawing.Color.Purple;
                Console.WriteLine("Leave, btn, " + mID.ToString());
                mSystem.OnDrawRequest(null);
            };
        }
    }
}
