using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class MNReferencedSpot
    {
        public string Name { get; set; }

        public SMContentType ContentType { get; set; }

        public string ContentId { get; set; }

        public MNRefSpotShape Shape { get; set; }
        /// <summary>
        /// Relative bounds of rectangle
        /// </summary>
        public Point Center { get; set; }
        public Point AnchorA { get; set; }
        public Point AnchorB { get; set; }

        public MNReferencedSpot()
        {
            Name = string.Empty;
            Shape = MNRefSpotShape.Rectangle;
            Center = new Point(50, 50);
            AnchorA = new Point(10, 0);
            AnchorB = new Point(0, 10);
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteString(Name);

            bw.WriteByte(11);
            bw.WriteString(ContentId);

            bw.WriteByte(12);
            bw.WriteInt32((Int32)ContentType);

            bw.WriteByte(13);
            bw.WriteInt32((Int32)Shape);

            bw.WriteByte(14);
            bw.WriteInt32(Center.X);
            bw.WriteInt32(Center.Y);

            bw.WriteByte(15);
            bw.WriteInt32(AnchorA.X);
            bw.WriteInt32(AnchorA.Y);

            bw.WriteByte(16);
            bw.WriteInt32(AnchorB.X);
            bw.WriteInt32(AnchorB.Y);

            bw.WriteByte(0);
        }

        public void Load(RSFileReader br)
        {
            byte b;

            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10: Name = br.ReadString(); break;
                    case 11: ContentId = br.ReadString(); break;
                    case 12: ContentType = (SMContentType)br.ReadInt32(); break;
                    case 13: Shape = (MNRefSpotShape)br.ReadInt32(); break;
                    case 14: Center = new Point(br.ReadInt32(), br.ReadInt32()); break;
                    case 15: AnchorA = new Point(br.ReadInt32(), br.ReadInt32()); break;
                    case 16: AnchorB = new Point(br.ReadInt32(), br.ReadInt32()); break;
                }
            }
        }

        private static double[,] rectBorderPoints = new double[,] {
            {1, 0}, {0, 1}, {1, 1}, {1, 0.5}, {1, -0.5},
            {1, -1}, {0.5, -1}, {0, -1}, {-0.5, -1},
            {-1, -1}, {-1, -0.5}, {-1, 0}, {-1, 0.5},
            {-1, 1}, {-0.5, 1}
        };

        public void Paint(Graphics graphics, Rectangle showRect, bool bActive, Pen penA, Pen penB)
        {
            // draw title in the center
            SizeF szTitle = graphics.MeasureString(ContentId, SystemFonts.MenuFont);
            Rectangle r = new Rectangle(Convert.ToInt32(showRect.X + showRect.Width*Center.X/100 - szTitle.Width / 2 - 1),
                Convert.ToInt32(showRect.Y + showRect.Height*Center.Y/100 - szTitle.Height/2 - 1),
                Convert.ToInt32(szTitle.Width + 2), 
                Convert.ToInt32(szTitle.Height + 2));
            graphics.FillRectangle(Brushes.White, r);
            graphics.DrawRectangle(Pens.Black, r);
            graphics.DrawString(ContentId, SystemFonts.MenuFont, Brushes.Black, r.X + 1, r.Y + 1);

            // draw points
            if (Shape == MNRefSpotShape.Circle)
            {
                Point[] pa = new Point[17];
                for (int i = 0; i < 17; i++)
                {
                    pa[i] = RecalcPoint(ref showRect, CalculateX(Math.Cos(i * Math.PI / 8), Math.Sin(i * Math.PI / 8)),
                        CalculateY(Math.Cos(i * Math.PI / 8), Math.Sin(i * Math.PI / 8)));
                }
                graphics.DrawLines(penA, pa);
                graphics.DrawLines(penB, pa);
                PaintPoint(graphics, showRect, CalculateX(1, 0), CalculateY(1, 0), (bActive ? Color.Red : Color.Blue));
                PaintPoint(graphics, showRect, CalculateX(0, 1), CalculateY(0, 1), (bActive ? Color.Red : Color.Blue));
            }
            else if (Shape == MNRefSpotShape.Rectangle)
            {
                Point[] pa = new Point[5];
                pa[0] = RecalcPoint(ref showRect, CalculateX(1, 1), CalculateY(1, 1));
                pa[1] = RecalcPoint(ref showRect, CalculateX(-1, 1), CalculateY(-1, 1));
                pa[2] = RecalcPoint(ref showRect, CalculateX(-1, -1), CalculateY(-1, -1));
                pa[3] = RecalcPoint(ref showRect, CalculateX(1, -1), CalculateY(1, -1));
                pa[4] = pa[0];
                graphics.DrawLines(penA, pa);
                graphics.DrawLines(penB, pa);

                PaintPoint(graphics, showRect, CalculateX(1, 0), CalculateY(1, 0), (bActive ? Color.Red : Color.Blue));
                PaintPoint(graphics, showRect, CalculateX(0, 1), CalculateY(0, 1), (bActive ? Color.Red : Color.Blue));
            }
            /*if (Shape == MNRefSpotShape.Circle)
            {
                for (int i = 0; i < 16; i++)
                {
                    PaintPoint(graphics, showRect,
                        CalculateX(Math.Cos(i * Math.PI / 8), Math.Sin(i * Math.PI / 8)),
                        CalculateY(Math.Cos(i * Math.PI / 8), Math.Sin(i * Math.PI / 8)),
                        ((i == 0 || i == 4) ? (bActive ? Color.Red : Color.Blue) : (bActive ? Color.Black : Color.Gray)));
                }
            }
            else if (Shape == MNRefSpotShape.Rectangle)
            {
                for (int i = 0; i < rectBorderPoints.GetLength(0); i++)
                {
                    PaintPoint(graphics, showRect, CalculateX(rectBorderPoints[i, 0], rectBorderPoints[i, 1]),
                        CalculateY(rectBorderPoints[i, 0], rectBorderPoints[i, 1]),
                        (i < 2 ? (bActive ? Color.Red : Color.Blue) : (bActive ? Color.Black : Color.Gray)));
                }
            }*/
        }


        public int CalculateX(double a, double b)
        {
            return Center.X + Convert.ToInt32(a * AnchorA.X) + Convert.ToInt32(b * AnchorB.X);
        }

        public int CalculateY(double a, double b)
        {
            return Center.Y + Convert.ToInt32(a*AnchorA.Y) + Convert.ToInt32(b*AnchorB.Y);
        }

        public Point AbsoluteAnchor(Rectangle rect, int a)
        {
            Point p = (a == 0 ? AnchorA : AnchorB);
            int xa = rect.X + Convert.ToInt32(rect.Width * (Center.X + p.X) / 100.0);
            int ya = rect.Y + Convert.ToInt32(rect.Height * (Center.Y + p.Y) / 100.0);
            return new Point(xa, ya);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect">Absolute value</param>
        /// <param name="x">Relative value</param>
        /// <param name="y">Relative value</param>
        /// <param name="c"></param>
        public void PaintPoint(Graphics graphics, Rectangle rect, int x, int y, Color c)
        {
            int xa = rect.X + Convert.ToInt32(rect.Width * x / 100.0);
            int ya = rect.Y + Convert.ToInt32(rect.Height * y / 100.0);
            graphics.FillRectangle(SMGraphics.GetBrush(c), xa - 6, ya - 6, 12, 12);
        }

        public Point RecalcPoint(ref Rectangle rect, int x, int y)
        {
            int xa = rect.X + Convert.ToInt32(rect.Width * x / 100.0);
            int ya = rect.Y + Convert.ToInt32(rect.Height * y / 100.0);
            return new Point(xa, ya);
        }

        public bool Contains(Point p)
        {
            double cx = Center.X;
            double cy = Center.Y;
            double ax = AnchorA.X;
            double ay = AnchorA.Y;
            double bx = AnchorB.X;
            double by = AnchorB.Y;

            double R = ax * by - ay * bx;
            if (Math.Abs(R) == 0.0)
                return false;
            if (ax == 0.0)
                return false;

            double N = (ax*p.Y - ax*cy - ay*p.X + ay*cx) / R;
            double M = (p.X - cx - N * bx) / ax;

            if (Shape == MNRefSpotShape.Rectangle)
            {
                return (Math.Abs(M) <= 1.0) && (Math.Abs(N) <= 1.0);
            }
            else if (Shape == MNRefSpotShape.Circle)
            {
                return (M * M + N * N) <= 1.0;
            }

            return false;
        }

        public Point AbsoluteCenter(Rectangle rect)
        {
            int xa = rect.X + Convert.ToInt32(rect.Width * Center.X / 100.0);
            int ya = rect.Y + Convert.ToInt32(rect.Height * Center.Y / 100.0);
            return new Point(xa, ya);
        }
    }
}
