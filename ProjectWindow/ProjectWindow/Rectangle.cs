using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace ProjectWindow
{
    public class Window
    {
        PointF[] rectOut;
        PointF[] rectIn;
        public float Up, Down, Left, Right;
        
        public Window(PointF begin, float width, float height, float up, float down, float left, float right)
        {
            rectOut = new PointF[4];
            rectIn = new PointF[4];
            rectOut[0].X = begin.X; rectOut[0].Y = begin.Y;
            rectOut[1].X = begin.X + width; rectOut[1].Y = begin.Y;
            rectOut[2].X = begin.X + width; rectOut[2].Y = begin.Y + height;
            rectOut[3].X = begin.X; rectOut[3].Y = begin.Y + height;

            rectIn[0].X = rectOut[0].X + left; rectIn[0].Y = rectOut[0].Y + up;
            rectIn[1].X = rectOut[1].X - right; rectIn[1].Y = rectOut[1].Y + up;
            rectIn[2].X = rectOut[2].X - right; rectIn[2].Y = rectOut[2].Y - down;
            rectIn[3].X = rectOut[3].X + left; rectIn[3].Y = rectOut[3].Y - down;
            Up = up; Down = down; Left = left; Right = right;
        }

        public bool Intersect(Window other)
        {
            if (rectOut[0].X > other.InRect[0].X && rectOut[0].Y > other.InRect[0].Y
                && rectOut[2].X < other.InRect[2].X && rectOut[2].Y < other.InRect[2].Y)
                return false;
            if (rectIn[0].X < other.OutRect[0].X && rectIn[0].Y < other.OutRect[0].Y
                && rectIn[2].X > other.OutRect[2].X && rectIn[2].Y > other.OutRect[2].Y)
                return false;
            if (((rectOut[0].X > other.OutPoint1.X + other.Width) || (other.OutPoint1.X > rectOut[0].X + Width) || (rectOut[0].Y > other.OutPoint1.Y + other.Height) || (other.OutPoint1.Y > rectOut[0].Y + Height)))
                return false;
            return true;
        }

        public void Rotate()
        {
            float width = rectOut[3].Y - rectOut[0].Y;
            float height = rectOut[1].X - rectOut[0].X;
            rectOut[1].X = rectOut[0].X + width; rectOut[1].Y = rectOut[0].Y;
            rectOut[2].X = rectOut[0].X + width; rectOut[2].Y = rectOut[0].Y + height;
            rectOut[3].X = rectOut[0].X; rectOut[3].Y = rectOut[0].Y + height;
            float tmp = Up; Up = Right; Right = Down; Down = Left; Left = tmp;
            rectIn[0].X = rectOut[0].X + Left; rectIn[0].Y = rectOut[0].Y + Up;
            rectIn[1].X = rectOut[1].X - Right; rectIn[1].Y = rectOut[1].Y + Up;
            rectIn[2].X = rectOut[2].X - Right; rectIn[2].Y = rectOut[2].Y - Down;
            rectIn[3].X = rectOut[3].X + Left; rectIn[3].Y = rectOut[3].Y - Down;
        }
        
        public void DrawWindow(Graphics gr)
        {
            Pen pen = new Pen(Color.Black, 1);
            SolidBrush brush = new SolidBrush(Color.FromName("Control"));
            gr.DrawLine(pen, rectOut[0].X, rectOut[0].Y, rectOut[1].X, rectOut[1].Y);
            gr.DrawLine(pen, rectOut[2].X, rectOut[2].Y, rectOut[1].X, rectOut[1].Y);
            gr.DrawLine(pen, rectOut[2].X, rectOut[2].Y, rectOut[3].X, rectOut[3].Y);
            gr.DrawLine(pen, rectOut[0].X, rectOut[0].Y, rectOut[3].X, rectOut[3].Y);
            gr.FillPolygon(Brushes.BurlyWood, rectOut);
            gr.FillPolygon(brush, rectIn);
            gr.DrawLine(pen, rectIn[0].X, rectIn[0].Y, rectIn[1].X, rectIn[1].Y);
            gr.DrawLine(pen, rectIn[2].X, rectIn[2].Y, rectIn[1].X, rectIn[1].Y);
            gr.DrawLine(pen, rectIn[2].X, rectIn[2].Y, rectIn[3].X, rectIn[3].Y);
            gr.DrawLine(pen, rectIn[0].X, rectIn[0].Y, rectIn[3].X, rectIn[3].Y);
        }

        public void MoveTo(PointF point)
        {
            float delX = rectOut[0].X - point.X;
            float delY = rectOut[0].Y - point.Y;
            for (int i = 0; i < 4; i++)
            {
                rectOut[i].X = rectOut[i].X - delX;
                rectOut[i].Y = rectOut[i].Y - delY;
                rectIn[i].X = rectIn[i].X - delX;
                rectIn[i].Y = rectIn[i].Y - delY;
            }
        }

        public PointF OutPoint1
        {
            get { return rectOut[0]; }
            set { rectOut[0] = value; }
        }

        public PointF OutPoint2
        {
            get { return rectOut[1]; }
            set { rectOut[1] = value; }
        }

        public PointF OutPoint3
        {
            get { return rectOut[2]; }
            set { rectOut[2] = value; }
        }

        public PointF OutPoint4
        {
            get { return rectOut[3]; }
            set { rectOut[3] = value; }
        }

        public PointF InPoint1
        {
            get { return rectIn[0]; }
            set { rectIn[0] = value; }
        }

        public PointF InPoint2
        {
            get { return rectIn[1]; }
            set { rectIn[1] = value; }
        }

        public PointF InPoint3
        {
            get { return rectIn[2]; }
            set { rectIn[2] = value; }
        }

        public PointF InPoint4
        {
            get { return rectIn[3]; }
            set { rectIn[3] = value; }
        }

        public PointF[] OutRect
        {
            get { return rectOut; }
            set { if(value.Length == 4) rectOut = value; }
        }

        public PointF[] InRect
        {
            get { return rectIn; }
            set { if (value.Length == 4) rectIn = value; }
        }

        public float Square
        {
            get { return (rectOut[1].X - rectOut[0].X) * (rectOut[3].Y - rectOut[0].Y); }
        }

        public float Width
        {
            get { return rectOut[1].X - rectOut[0].X; }
        }

        public float Height
        {
            get { return rectOut[3].Y - rectOut[0].Y; }
        }

        public Window Copy()
        {
            return new Window(new PointF(rectOut[0].X, rectOut[0].Y), Width, Height, Up, Down, Left, Right);
        }
    }
}
