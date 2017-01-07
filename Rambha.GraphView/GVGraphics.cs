using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace Rambha.GraphView
{
    public class GVGraphics
    {
        private static Dictionary<Color, Brush> p_brushes = new Dictionary<Color, Brush>();

        public static Brush GetBrush(Color c)
        {
            if (p_brushes.ContainsKey(c))
            {
                return p_brushes[c];
            }
            else
            {
                Brush b = new SolidBrush(c);
                p_brushes.Add(c, b);
                return b;
            }
        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static void DrawRoundedRectangle(Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        public static void FillRoundedRectangle(Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawFillRoundedRectangle(Graphics graphics, Pen pen, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
                graphics.DrawPath(pen, path);
            }
        }

        public static PointF GetCenterPoint(RectangleF rect)
        {
            return new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static PointF GetCenterPoint(PointF p1, PointF p2)
        {
            return new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        /// <summary>
        /// Calculates intersection point between rectangle and line connecting center of rectangle
        /// with target point
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="target"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static PointF GetConnectorPoint(RectangleF rect, PointF target, out RectSide rs)
        {
            PointF center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            SizeF vector = new SizeF(target.X - center.X, target.Y - center.Y);
            PointF border = new PointF();
            rs = RectSide.A;
            bool b;

            if (vector.Width == 0 && vector.Height == 0)
                return center;
            b = Math.Abs(vector.Width / rect.Width) > Math.Abs(vector.Height / rect.Height);

            rs = b ? (vector.Width < 0 ? RectSide.C : RectSide.A)
                : (vector.Height < 0 ? RectSide.D : RectSide.B);

            switch (rs)
            {
                case RectSide.A:
                    border.X = rect.Right;
                    border.Y = center.Y + vector.Height / vector.Width * (rect.Width / 2);
                    break;
                case RectSide.B:
                    border.X = center.X + vector.Width / vector.Height * (rect.Height / 2);
                    border.Y = rect.Bottom;
                    break;
                case RectSide.C:
                    border.X = rect.Left;
                    border.Y = center.Y - vector.Height / vector.Width * (rect.Width / 2);
                    break;
                case RectSide.D:
                    border.X = center.X - vector.Width / vector.Height * (rect.Height / 2);
                    border.Y = rect.Top;
                    break;
            }

            return border;
        }

        /// <summary>
        /// Draws line bewteen point and rectangle, but line ends on the border of rectangle
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="rect"></param>
        /// <param name="target"></param>
        public static void DrawRectangleLine(Graphics g, Pen pen, RectangleF rect, PointF target)
        {
            RectSide rs;
            PointF border = GetConnectorPoint(rect, target, out rs);
            g.DrawLine(pen, border, target);
        }

        public static void DrawRectangleLine(Graphics g, Pen pen, RectangleF rectA, RectangleF rectB)
        {
            RectSide rsa, rsb;
            PointF border1 = GetConnectorPoint(rectA, GetCenterPoint(rectB), out rsa);
            PointF border2 = GetConnectorPoint(rectB, GetCenterPoint(rectA), out rsb);
            g.DrawLine(pen, border1, border2);
        }

        public static void DrawRectangleLine(Graphics g, Pen pen, RectangleF rectA, RectangleF rectB, float arrowSize)
        {
            RectSide rsa, rsb;
            PointF border1 = GetConnectorPoint(rectA, GetCenterPoint(rectB), out rsa);
            PointF border2 = GetConnectorPoint(rectB, GetCenterPoint(rectA), out rsb);
            g.DrawLine(pen, border1, border2);
            DrawArrowEnd(g, border1, border2, pen.Color, arrowSize);
        }

        public static void DrawArrowEnd(Graphics g, PointF a1, PointF a2, Color clr, float arrowSize)
        {
            Brush b = GVGraphics.GetBrush(clr);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            double angle = -Math.Atan2(a2.Y - a1.Y, a2.X - a1.X);
            PointF p1 = new PointF(a2.X - (float)(Math.Cos(angle - Math.PI / 8.0) * arrowSize), a2.Y + (float)(Math.Sin(angle - Math.PI / 8.0) * arrowSize));
            PointF p2 = a2;
            PointF p3 = new PointF(a2.X - (float)(Math.Cos(angle + Math.PI / 8.0) * arrowSize), a2.Y + (float)(Math.Sin(angle + Math.PI / 8.0) * arrowSize));

            g.FillPolygon(b, new PointF[] { p1, p2, p3 });
        }

        private static Font p_actionTitleFont = null;
        private static Font p_actionPropertyFont = null;

        public static Font ActionTitleFont
        {
            get
            {
                if (p_actionTitleFont == null)
                    p_actionTitleFont = new Font(SystemFonts.CaptionFont, FontStyle.Bold);
                return p_actionTitleFont;
            }
        }

        public static Font ActionPropertyFont
        {
            get
            {
                if (p_actionPropertyFont == null)
                    p_actionPropertyFont = SystemFonts.MenuFont;
                return p_actionPropertyFont;
            }
        }
    }
    public enum RectSide
    {
        A, B, C, D
    }
}
