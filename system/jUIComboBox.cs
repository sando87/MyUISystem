using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class jUIComboBox : jUIControl
    {
        public jUIComboBox()
        {
            OnMouseEnter += (ctrl, args) =>
            {
                Console.WriteLine("Enter, cb, " + mID.ToString());
            };

            OnMouseLeave += (ctrl, args) =>
            {
                Console.WriteLine("Leave, cb, " + mID.ToString());
            };
        }
    }
}
