using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class uiViewManager
    {
        internal uiView mRootView { get; set; }

        private uiView FindTopView(int _worldX, int _worldY)
        {
            return mRootView.FindTopView(_worldX, _worldY);
        }
    }
}
