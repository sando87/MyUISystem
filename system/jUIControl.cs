using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system
{
    enum UIControlType
    {
        Dummy, Dialog, Button, CheckBox, Label, TextBox, ComboBox, ListView, ImageBox, 
    }

    public class jMouseEventArgs
    {
        public int x;
        public int y;
        public int delta;
        public string type;
    }

    class jUIControl
    {
        public string mID;
        public List<jUIControl> mNodes = new List<jUIControl>();
        public jUIControl mParentControl;
        public UIControlType mType;

        protected Rectangle mRect_A;
        protected Rectangle mRect_R;

        public Rectangle Rect_R { get { return mRect_R; } }
        public Rectangle Rect { get { return mRect_A; } }
        public Point Point_R { get { return mRect_R.Location; } }
        public Point Point { get { return mRect_A.Location; } }
        public Size Size { get { return mRect_A.Size; } }
        public Color mColor = Color.Gray;
        public bool mIsFocused;
        public bool mIsEnabled;
        public bool mIsVisiable;
        public bool mIsMouseHover;
        public bool mIsMouseDowned;

        public delegate void MouseEvent(jUIControl control, jMouseEventArgs args);
        public MouseEvent OnMouseDown = null;
        public MouseEvent OnMouseUp = null;
        public MouseEvent OnMouseHover = null;
        public MouseEvent OnMouseClick = null;


        public void SetSize(Size _size)
        {
            _size.Width = Math.Max(0, _size.Width);
            _size.Height = Math.Max(0, _size.Height);
            mRect_R.Size = _size;
            mRect_A.Size = _size;
        }
        public void SetPos(Point _point_r)
        {
            mRect_R.Location = _point_r;
            //mRect_A.Location = (mParentControl!=null)? Point.Add(mParentControl.mRect_A.Location, _point) : _point;
            if (mParentControl != null)
            {
                mRect_A.X = mParentControl.mRect_A.Location.X + _point_r.X;
                mRect_A.Y = mParentControl.mRect_A.Location.Y + _point_r.Y;
            }
            else
            {
                mRect_A.Location = _point_r;
            }
            int cnt = mNodes.Count;
            for(int i=0; i<cnt; ++i)
            {
                mNodes[i].SetPos(mNodes[i].Point_R);
            }
        }
        public void Draw()
        {
            if (mParentControl == null)
                return;

            if (!mParentControl.Rect.IntersectsWith(Rect))
                return;

            int left = Math.Max(mParentControl.Rect.Left, Rect.Left);
            int right = Math.Min(mParentControl.Rect.Right, Rect.Right);
            int top = Math.Max(mParentControl.Rect.Top, Rect.Top);
            int bottom = Math.Min(mParentControl.Rect.Bottom, Rect.Bottom);

            Renderer.GetInst().DrawRect(new Rectangle(left, top, right - left, bottom - top), mColor);
            Renderer.GetInst().DrawOutline(new Rectangle(left, top, right - left, bottom - top), Color.DarkGray);
        }

    }
}
