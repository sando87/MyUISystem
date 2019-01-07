using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    class jUISystem
    {
        const int screenWidth = 640;
        const int screenHeight = 480;
        
        static private jUISystem mInst = null;
        static public jUISystem GetInst() { if (mInst == null) mInst = new jUISystem(); return mInst; }
        private jUISystem() { mRoot.mID = GetNextID().ToString(); mRoot.SetSize(new Size(screenWidth, screenHeight)); }

        Dictionary<string, jUIControl> mDicControls = new Dictionary<string, jUIControl>();
        public jUIControl mRoot = new jUIControl();
        int mNextContrlID = 0;

        //true반환시 거기서 loop stop, false 반환시 계속 loop
        public delegate bool DelCtrls(jUIControl control);

        private int GetNextID() { return mNextContrlID++; }

        public void Add(jUIControl control, int _x_a, int _y_a)
        {
            jUIControl node = SelectControl(mRoot, _x_a, _y_a);
            control.mParentControl = node;
            control.mID = GetNextID().ToString();
            control.SetPos(new Point(_x_a - node.Point.X, _y_a - node.Point.Y));
            mDicControls[control.mID] = control;
            node.mNodes.Add(control);
        }
        public void TrigMouseDown(jMouseEventArgs args)
        {
            jUIControl node = SelectControl(mRoot, args.x, args.y);
            node.OnMouseDown?.Invoke(node, args);
        }
        public void TrigMouseUp(jMouseEventArgs args)
        {
            jUIControl node = SelectControl(mRoot, args.x, args.y);
            node.OnMouseUp?.Invoke(node, args);
        }
        public void TrigMouseHover(jMouseEventArgs args)
        {
            jUIControl node = SelectControl(mRoot, args.x, args.y);
            node.OnMouseHover?.Invoke(node, args);
        }
        public void TrigMouseClick(jMouseEventArgs args)
        {
            jUIControl node = SelectControl(mRoot, args.x, args.y);
            node.OnMouseClick?.Invoke(node, args);
        }


        public void Draw()
        {
            LoopControls(mRoot, (ctrl) =>
            {
                ctrl.Draw();
                return false;
            }, null);
        }

        public jUIControl SelectControl(int _x_a, int _y_a)
        {
            return SelectControl(mRoot, _x_a, _y_a);
        }

        private bool LoopControls(jUIControl _node, DelCtrls _funcPre, DelCtrls _funcPost)
        {
            if (_funcPre != null && _funcPre.Invoke(_node))
                return true;

            int cnt = _node.mNodes.Count;
            for (int i = 0; i < cnt; ++i)
            {
                if (LoopControls(_node.mNodes[i], _funcPre, _funcPost))
                    return true;
            }

            if (_funcPost != null && _funcPost.Invoke(_node))
                return true;

            return false;
        }
        private bool LoopControls_Reverse(jUIControl _node, DelCtrls _funcPre, DelCtrls _funcPost)
        {
            if (_funcPre != null && _funcPre.Invoke(_node))
                return true;

            int cnt = _node.mNodes.Count;
            for (int i = cnt-1; i >= 0; --i)
            {
                if (LoopControls_Reverse(_node.mNodes[i], _funcPre, _funcPost))
                    return true;
            }

            if (_funcPost != null && _funcPost.Invoke(_node))
                return true;

            return false;
        }
        private jUIControl SelectControl(jUIControl _node, int _x_a, int _y_a)
        {
            int cnt = _node.mNodes.Count;
            for (int i = cnt - 1; i >= 0; --i)
            {
                jUIControl subCtrl = _node.mNodes[i];
                if (subCtrl.Rect.Contains(_x_a, _y_a))
                {
                    return SelectControl(subCtrl, _x_a, _y_a);
                }
            }
            return _node;
        }

    }
}
