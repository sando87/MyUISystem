using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    public enum UIControlType
    {
        Dummy, Dialog, Button, CheckBox, Label, TextBox, ComboBox, ListView, ImageBox, FontBox, ScrollV
    }


    public class jUIControl
    {
        public string mParentID = "";
        public string mID = "";
        public List<jUIControl> mNodes = new List<jUIControl>();
        public UIControlType mType;
        public jUISystem mSystem;
        public void SetUISystem(jUISystem _system) { mSystem = _system; }
        public bool mIsVisiable = true;
        public bool mIsEnable = true;
        public bool mIsFocused = false;
        public string mText = "";
        bool mIsScrollV = false;
        jUIScrollV mScrollV = null;

        protected Rectangle mRect_A;
        protected Rectangle mRect_R;

        public jUIControl Parent { get { return mSystem.GetControl(mParentID); } }
        public Rectangle Rect_R { get { return mRect_R; } }
        public Rectangle Rect { get { return mRect_A; } }
        public Point Point_R { get { return mRect_R.Location; } }
        public Point Point { get { return mRect_A.Location; } }
        public Size Size { get { return mRect_A.Size; } }
        public Color mColor = Color.Gray;


        public delegate void MouseEvent(jUIControl control, jMouseEventArgs args);
        public MouseEvent OnMouseDown = null;
        public MouseEvent OnMouseUp = null;
        public MouseEvent OnMouseMove = null;
        public MouseEvent OnMouseClick = null;
        public MouseEvent OnMouseEnter = null;
        public MouseEvent OnMouseLeave = null;
        public MouseEvent OnFocus = null;
        public MouseEvent OnFocusOut = null;

        public void SetScrollV(bool _set) { mIsScrollV = _set; }

        public void Add(jUIControl _child)
        {
            if (_child.mID.Length == 0)
                _child.mID = mID + "." + mNodes.Count.ToString();
            _child.mParentID = mID;
            _child.mSystem = mSystem;
            mSystem.Registor(_child);
            mNodes.Add(_child);
        }

        public virtual void Init()
        {
            if (mIsScrollV)
            {
                mScrollV = new jUIScrollV();
                mScrollV.Initialize(this);
            }
        }

        public void SetSize(Size _size)
        {
            _size.Width = Math.Max(0, _size.Width);
            _size.Height = Math.Max(0, _size.Height);
            mRect_R.Size = _size;
            mRect_A.Size = _size;
        }
        public void SetPos(Point _point_r)
        {
            mRect_R.X = _point_r.X;
            mRect_R.Y = _point_r.Y;
        }
        public void CalcAbsolutePostion()
        {
            if (mID != jUISystem.ID_ROOT_CONTROL)
            {
                jUIControl ctrl = Parent;
                mRect_A.X = ctrl.mRect_A.Location.X + mRect_R.X;
                mRect_A.Y = ctrl.mRect_A.Location.Y + mRect_R.Y;
            }
            else
            {
                mRect_A.X = mRect_R.Location.X;
                mRect_A.Y = mRect_R.Location.Y;
            }
            int cnt = mNodes.Count;
            for (int i = 0; i < cnt; ++i)
            {
                mNodes[i].CalcAbsolutePostion();
            }
        }
        public virtual void Draw()
        {
            jUIControl parentCtrl = Parent;
            if (parentCtrl == null)
                return;

            Rectangle myRect = new Rectangle(Rect.X, Rect.Y, Rect.Width, Rect.Height);
            int offset = 0;
            if (parentCtrl.mIsScrollV && mType != UIControlType.ScrollV)
            {
                offset = parentCtrl.mScrollV.GetCurrentOffset();
                myRect.Y -= offset;
            }

            if (!parentCtrl.Rect.IntersectsWith(myRect))
                return;

            int left = Math.Max(parentCtrl.Rect.Left, myRect.Left);
            int right = Math.Min(parentCtrl.Rect.Right, myRect.Right);
            int top = Math.Max(parentCtrl.Rect.Top, myRect.Top);
            int bottom = Math.Min(parentCtrl.Rect.Bottom, myRect.Bottom);

            DrawInfo info = new DrawInfo();
            info.rect = new Rectangle(left, top, right - left, bottom - top);
            info.color = mColor;
            mSystem.OnDrawRectFill?.Invoke(info);

            info.color = Color.DarkGray;
            info.lineWidth = 2;
            mSystem.OnDrawRectOutline?.Invoke(info);
        }

    }
}
