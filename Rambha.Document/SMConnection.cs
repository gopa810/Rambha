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
    public class SMConnection: GSCore, IRSObjectOrigin
    {
        public MNDocument Document { get; set; }
        public SMControl Source { get; set; }
        public SMControl Target { get; set; }

        public SMConnectionStyle ConnectionStyle { get; set; }

        public SMConnection(MNDocument doc)
            : base()
        {
            Document = doc;
            ConnectionStyle = SMConnectionStyle.DirectLine;
            Source = null;
            Target = null;
        }

        public void Paint(MNPageContext context)
        {
            if (Source == null || Target == null)
                return;

            SMRectangleArea sa = context.CurrentPage.GetArea(Source.Id);
            SMRectangleArea ta = context.CurrentPage.GetArea(Target.Id);

            Rectangle rs = sa.GetBounds(context);
            Rectangle rt = ta.GetBounds(context);

            Point sc = Point.Empty;
            Point tc = Point.Empty;

            if (Source.Style.BorderStyle == SMBorderStyle.Elipse)
                sc = IntersectionPointLineElipse(rs, CenterPoint(rt));
            else
                sc = IntersectionPointLineRect(rs, CenterPoint(rt));

            if (Target.Style.BorderStyle == SMBorderStyle.Elipse)
                tc = IntersectionPointLineElipse(rt, CenterPoint(rs));
            else
                tc = IntersectionPointLineRect(rt, CenterPoint(rs));


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

        public Point IntersectionPointLineRect(Rectangle rect, Point tar)
        {
            Point src = CenterPoint(rect);
            Size vector = Vector(tar, src);
            int ax = src.X;
            int ay = src.Y;
            int df = 0;
            double t = 0.0;

            if (tar.X > rect.Right || tar.X < rect.Left)
            {
                df = tar.X - src.X;
                t = (rect.Width / 2) / df;
                ax = Convert.ToInt32(t * df);
                ay = Convert.ToInt32(t * df);
            }

            if (ay < rect.Bottom || ay > rect.Top)
            {
                df = ay - src.Y;
                t = (rect.Height / 2) / df;
                ax = Convert.ToInt32(t * df);
                ay = Convert.ToInt32(t * df);
            }

            return new Point(ax, ay);
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
                        br.AddReference(Document, "SMControl", br.ReadInt64(), 10, this);
                        br.AddReference(Document, "SMControl", br.ReadInt64(), 11, this);
                        ConnectionStyle = (SMConnectionStyle)br.ReadInt32();
                        break;
                }
            }
        }

        public void setReference(int tag, object obj)
        {
            switch (tag)
            {
                case 10:
                    if (obj is SMControl) Source = (SMControl)obj;
                    break;
                case 11:
                    if (obj is SMControl) Target = (SMControl)obj;
                    break;
            }
        }
    }
}
