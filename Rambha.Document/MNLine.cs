using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Xml;

namespace Rambha.Document
{
    public class MNLine: SMControl
    {
        public MNPagePoint StartPoint { get; set; }
        public MNPagePoint EndPoint { get; set; }

        public int StartCap { get; set; }
        public int EndCap { get; set; }
        public int DashType { get; set; }
        public int Width { get; set; }
        public Color LineColor { get; set; }
        public int EndingSize { get; set; }

        public Pen LinePen { get; set; }

        public MNLine(MNPage p)
            : base(p)
        {
            StartPoint = new MNPagePoint(p);
            StartPoint.Radius = 0;
            StartPoint.EditParentProperties = true;
            StartPoint.ParentObject = this;

            EndPoint = new MNPagePoint(p);
            EndPoint.Radius = 0;
            EndPoint.EditParentProperties = true;
            EndPoint.ParentObject = this;

            StartCap = 0;
            EndCap = 0;
            DashType = 0;
            Width = 1;
            EndingSize = 30;
            LineColor = Color.Black;

            LinePen = null;
        }

        public override void Paint(MNPageContext context)
        {
            Point A = StartPoint.GetLogicalCenter(context);
            Point B = EndPoint.GetLogicalCenter(context);

            /*if (StartPoint.Selected && context.TrackedObjects.Count > 0)
                A.Offset(context.TrackedDrawOffset);

            if (EndPoint.Selected && context.TrackedObjects.Count > 0)
                B.Offset(context.TrackedDrawOffset);*/

            if (LinePen == null)
            {
                LinePen = new Pen(LineColor);
                LinePen.Width = Width;
            }

            context.g.DrawLine(LinePen, A, B);

            switch (StartCap)
            {
                case 0: break;
                case 1: PaintArrow(context.g, A, B); break;
                case 2: PaintEnd(context.g, A, B); break;
                default: break;
            }

            switch (EndCap)
            {
                case 0: break;
                case 1: PaintArrow(context.g, B, A); break;
                case 2: PaintEnd(context.g, B, A); break;
                default: break;
            }


            base.Paint(context);
        }

        public void PaintArrow(Graphics g, Point A, Point B)
        {
            double cx = B.X - A.X;
            double cy = B.Y - A.Y;
            double d = Math.Sqrt(cx * cx + cy * cy);
            cx /= d;
            cy /= d;
            cx *= EndingSize;
            cy *= EndingSize;

            Point C = new Point(Convert.ToInt32(B.X - cx * 2 + cy), Convert.ToInt32(B.Y - cy * 2 - cx));
            Point D = new Point(Convert.ToInt32(B.X - cx * 2 - cy), Convert.ToInt32(B.Y - cy * 2 + cx));

            g.DrawLine(LinePen, C, B);
            g.DrawLine(LinePen, B, D);
        }

        public void PaintEnd(Graphics g, Point A, Point B)
        {
            double cx = B.X - A.X;
            double cy = B.Y - A.Y;
            double d = Math.Sqrt(cx * cx + cy * cy);
            cx /= d;
            cy /= d;
            cx *= EndingSize;
            cy *= EndingSize;

            Point C = new Point(Convert.ToInt32(B.X + cy), Convert.ToInt32(B.Y - cx));
            Point D = new Point(Convert.ToInt32(B.X - cy), Convert.ToInt32(B.Y + cx));

            g.DrawLine(LinePen, C, D);
        }

    }
}
