using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    public class jMouseEventArgs
    {
        public int x;
        public int y;
        public int delta;
        public string type;
    }
    public class DrawInfo
    {
        public RectangleF uv;
        public Rectangle rect;
        public Color color;
        public int texID;
        public Bitmap bitmap;
        public int lineWidth;
        public string text;
    }
    public class jUISystem
    {
        //static private jUISystem mInst = null;
        //static public jUISystem GetInst() { if (mInst == null) mInst = new jUISystem(); return mInst; }
        public static string ID_ROOT_CONTROL = "";
        public jUISystem(int _w, int _h)
        {
            mRoot.SetUISystem(this);
            mRoot.mID = GetNextID().ToString();
            ID_ROOT_CONTROL = mRoot.mID;
            mDicControls[mRoot.mID] = mRoot;
            mRoot.SetSize(new Size(_w, _h));
            mMousedControl = mRoot;
            mFocusedControl = mRoot;
            
        }

        public Dictionary<string, jUIControl> mDicControls = new Dictionary<string, jUIControl>();
        public jUIControl mRoot = new jUIControl();
        int mNextContrlID = 0;
        private Point mPreMousePoint;
        private jUIControl mMousedControl;
        private jUIControl mFocusedControl;
        private jUIControl mMouseDownControl;

        //true반환시 거기서 loop stop, false 반환시 계속 loop
        public delegate bool DelCtrls(jUIControl control);
        public delegate void DelDraw(DrawInfo info);
        public DelDraw OnDrawRequest;
        public DelDraw OnDrawRectFill;
        public DelDraw OnDrawRectOutline;
        public DelDraw OnDrawBitmap;
        public DelDraw OnDrawBitmapRect;

        private int GetNextID() { return mNextContrlID++; }


        //심플 초기값 세팅 부분과 부모값 세팅 분리 및 계산값 정리 할 것
        public void Add(jUIControl control, int _x_a, int _y_a)
        {
            jUIControl node = SelectControl(mRoot, _x_a, _y_a);
            control.mParentID = node.mID;
            control.mID = GetNextID().ToString();
            control.SetPos(new Point(_x_a - node.Point.X, _y_a - node.Point.Y));

            control.SetUISystem(this);
            mDicControls[control.mID] = control;

            node.mNodes.Add(control);

            control.CalcAbsolutePostion();
            
        }
        public void Registor(jUIControl control)
        {
            if (mDicControls.ContainsKey(control.mID))
                return;

            if (control.mID.Length == 0)
                return; // control.mID = GetNextID().ToString();

            control.SetUISystem(this);
            mDicControls[control.mID] = control;
        }
        public void BuildUpTree()
        {
            foreach (var item in mDicControls)
            {
                jUIControl ctrl = item.Value;
                string parentID = ctrl.mParentID;
                if (mDicControls.ContainsKey(parentID))
                {
                    mDicControls[parentID].mNodes.Add(ctrl);
                }
            }
        }
        public jUIControl GetControl(string _id)
        {
            return mDicControls.ContainsKey(_id) ? mDicControls[_id] : null;
        }
        public void ProcMouseMove(jMouseEventArgs args)
        {
            if (mPreMousePoint == new Point(args.x, args.y))
                return;

            mPreMousePoint.X = args.x;
            mPreMousePoint.Y = args.y;
            jUIControl node = SelectControl(mRoot, args.x, args.y);

            if(mMousedControl != node)
            {
                mMousedControl.OnMouseLeave?.Invoke(mMousedControl, args);
                node.OnMouseEnter?.Invoke(node, args);
                mMousedControl = node;
            }

            node.OnMouseMove?.Invoke(node, args);
        }
        public void ProcMouseDown(jMouseEventArgs args)
        {
            jUIControl node = SelectControl(mRoot, args.x, args.y);
            mMouseDownControl = node;
            node.OnMouseDown?.Invoke(node, args);
        }
        public void ProcMouseUp(jMouseEventArgs args)
        {
            jUIControl node = SelectControl(mRoot, args.x, args.y);
            node.OnMouseUp?.Invoke(node, args);
            if(mMouseDownControl == node)
            {
                node.OnMouseClick?.Invoke(node, args);

                if (mFocusedControl != node)
                {
                    mFocusedControl.OnFocusOut?.Invoke(mFocusedControl, args);
                    node.OnFocus?.Invoke(node, args);
                    mFocusedControl = node;
                }
            }
            mMouseDownControl = null;
        }


        public void Draw()
        {
            LoopControls(mRoot, (ctrl) =>
            {
                if(ctrl.mIsVisiable)
                    ctrl.Draw();

                return false;
            }, null);
        }

        public jUIControl SelectControl(int _x_a, int _y_a)
        {
            return SelectControl(mRoot, _x_a, _y_a);
        }

        public jUIControl GetRoot()
        {
            return mRoot;
        }

        public bool LoopControls(jUIControl _node, DelCtrls _funcPre, DelCtrls _funcPost)
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
        public bool LoopControls_Reverse(jUIControl _node, DelCtrls _funcPre, DelCtrls _funcPost)
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
