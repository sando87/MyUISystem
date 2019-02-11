using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class jUICheckBox : jUIControl
    {
        public jUICheckBox()
        {
            mType = UIControlType.CheckBox;

            OnMouseEnter += (ctrl, args) =>
            {
                Console.WriteLine("Enter, chk, " + mID.ToString());
            };

            OnMouseLeave += (ctrl, args) =>
            {
                Console.WriteLine("Leave, chk, " + mID.ToString());
            };
        }
    }
}
