using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMConnection: GSCore
    {
        public MNPage Page { get; set; }
        public SMControl Source
        {
            get
            {
                if (p_source == null && p_source_lazy > 0)
                {
                    p_source = Page.FindObject(p_source_lazy);
                    p_source_lazy = -1;
                }
                return p_source;
            }
            set
            {
                p_source = value;
            }
        }
        public long SourceId
        {
            get { SMControl t = Source; return (t != null ? t.Id : p_source_lazy); }
            set { p_source_lazy = value; }
        }
        private SMControl p_source = null;
        private long p_source_lazy = -1;

        public SMControl Target
        {
            get
            {
                if (p_target == null && p_target_lazy > 0)
                {
                    p_target = Page.FindObject(p_target_lazy);
                    p_target_lazy = -1;
                }
                return p_target;
            }
            set
            {
                p_target = value;
            }
        }
        public long TargetId
        {
            get { SMControl t = Target; return (t != null ? t.Id : p_target_lazy); }
            set { p_target_lazy = value; }
        }
        private SMControl p_target = null;
        private long p_target_lazy = -1;

        public SMConnectionStyle ConnectionStyle { get; set; }

        public SMConnection(MNPage doc)
            : base()
        {
            Page = doc;
            ConnectionStyle = SMConnectionStyle.DirectLine;
            Source = null;
            Target = null;
        }

        public void Paint(MNPageContext context)
        {
            if (Source == null || Target == null)
                return;

            SMRectangleArea sa = Source.Area;
            SMRectangleArea ta = Target.Area;

            Rectangle rs = sa.GetBounds(context);
            Rectangle rt = ta.GetBounds(context);

            Point sc = Point.Empty;
            Point tc = Point.Empty;

            if (Source.DragLineAlign == SMDragLineAlign.Undef)
            {
                if (Source.NormalState.BorderStyle == SMBorderStyle.Elipse)
                    sc = IntersectionPointLineElipse(rs, CenterPoint(rt));
                else
                    sc = IntersectionPointLineRect(rs, CenterPoint(rt));
            }
            else if (Source.DragLineAlign == SMDragLineAlign.LeftRight)
            {
                if (rs.Left > rt.Right)
                    sc = new Point(rs.Left, rs.Top + rs.Height / 2);
                else
                    sc = new Point(rs.Right, rs.Top + rs.Height/2);
            }
            else if (Source.DragLineAlign == SMDragLineAlign.TopBottom)
            {
                if (rs.Top > rt.Bottom)
                    sc = new Point(rs.Left + rs.Width / 2, rs.Top);
                else
                    sc = new Point(rs.Left + rs.Width / 2, rs.Bottom);
            }

            if (Target.DragLineAlign == SMDragLineAlign.LeftRight)
            {
                if (rs.Left < rt.Right)
                    tc = new Point(rt.Left, rt.Top + rt.Height / 2);
                else
                    tc = new Point(rt.Right, rt.Top + rt.Height / 2);
            }
            else if (Target.DragLineAlign == SMDragLineAlign.TopBottom)
            {
                if (rs.Top < rt.Bottom)
                    tc = new Point(rt.Left + rt.Width / 2, rt.Top);
                else
                    tc = new Point(rt.Left + rt.Width / 2, rt.Bottom);
            }
            else
            {
                if (Target.NormalState.BorderStyle == SMBorderStyle.Elipse)
                    tc = IntersectionPointLineElipse(rt, CenterPoint(rs));
                else
                    tc = IntersectionPointLineRect(rt, CenterPoint(rs));
            }


            switch (ConnectionStyle)
            {
                default:
                    context.g.DrawLine(Pens.Black, sc, tc);
                    break;
            }
        }

        public Point CenterPoint(Rectangle r)
        {
            return new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
        }

        public Size Vector(Point s, Point t)
        {
            return new Size(t.X - s.X, t.Y - s.Y);
        }

        public Point IntersectionPointLineElipse(Rectangle elipse, Point tar)
        {
            Point src = CenterPoint(elipse);
            Size vector = Vector(tar, src);
            double angle = Math.Atan2(vector.Height, vector.Width);
            return new Point(Convert.ToInt32(src.X + Math.Cos(angle) * elipse.Width / 2),
                Convert.ToInt32(src.Y + Math.Sin(angle) * elipse.Height / 2));
        }

        public Point IntersectionPointLineRect(Rectangle bounds, Point tar)
        {
            Point cp = new Point((bounds.Left + bounds.Right) / 2, (bounds.Top + bounds.Bottom) / 2);
            Point intPoint = new Point();
            double Xc = tar.X - cp.X;
            double Yc = tar.Y - cp.Y;
            double rat = Convert.ToDouble(bounds.Height) / Convert.ToDouble(bounds.Width);
            Xc *= rat;

            if (Xc * Xc + Yc * Yc == 0)
                return cp;

            if (Xc + Yc > 0) // B or C
            {
                if (Xc < Yc) // C
                {
                    intPoint.X = cp.X + Convert.ToInt32((bounds.Bottom - cp.Y) * Xc / (Yc * rat));
                    intPoint.Y = bounds.Bottom;
                }
                else // D
                {
                    intPoint.X = bounds.Right;
                    intPoint.Y = cp.Y + Convert.ToInt32((bounds.Right - cp.X) * Yc * rat / Xc);
                }
            }
            else // A or D
            {
                if (Xc < Yc) // B
                {
                    intPoint.X = bounds.Left;
                    intPoint.Y = cp.Y + Convert.ToInt32((bounds.Left - cp.X) * Yc * rat / Xc);
                }
                else// A
                {
                    intPoint.X = cp.X + Convert.ToInt32((bounds.Top - cp.Y) * Xc / (Yc * rat));
                    intPoint.Y = bounds.Top;
                }
            }

            return intPoint;
        }

        public static float IntersectionLines(int sX, int sY, int tX, int tY, int s2X, int s2Y, int t2X, int t2Y, out Point intersection)
        {
            float x = 0, y = 0;

            float A1 = tY - sY;
            float B1 = sX - tX;
            float C1 = -B1 * sY - A1 * sX;

            float A2 = t2Y - s2Y;
            float B2 = s2X - t2X;
            float C2 = -B2 * s2Y - A2 * s2X;

            y = (A1*C2/A2 - C1) / (B1 - A1*B2/A2);
            x = (-C2 - B2 * y) / A2;

            intersection = new Point((int)x, (int)y);
            if (tX != sX)
            {
                return (x - sX) / (tX - sX);
            }
            else if (tY != sY)
            {
                return (y - sY) / (tY - sY);
            }
            else
            {
                return 0;
            }
        }


        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteInt64(Source != null ? Source.Id : -1);
            bw.WriteInt64(Target != null ? Target.Id : -1);
            bw.WriteInt32((Int32)ConnectionStyle);

            bw.WriteByte(0);
        }

        public void Load(RSFileReader br)
        {
            byte tag;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        /*Source = new SMControlLoadingPlaceholder() { controlId = br.ReadInt32() };
                        Target = new SMControlLoadingPlaceholder() { controlId = br.ReadInt32() };*/
                        SourceId = br.ReadInt64();
                        TargetId = br.ReadInt64();
                        ConnectionStyle = (SMConnectionStyle)br.ReadInt32();
                        break;
                }
            }
        }

    }
}
