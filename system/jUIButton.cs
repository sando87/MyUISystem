using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class jUIButton : jUIControl
    {
        jUIFontBox mFont = null;

        public jUIButton()
        {
            mType = UIControlType.Button;
        }


        public override void Init()
        {
            base.Init();

            mColor = Color.Purple;

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

            
            mFont = new jUIFontBox();
            mFont.Initialize(this);
            
        }

        public override void Draw()
        {
            base.Draw();
            if (mFont != null)
                mFont.Draw();
        }
    }
}
