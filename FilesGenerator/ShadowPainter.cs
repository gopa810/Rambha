using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;


namespace FilesGenerator
{
    public class ShadowPainter
    {
        public int ShadowWidth = 32;
        public int ShadowSolidWidth = 16;
        public Color ShadowColor = Color.White;
        private Color[] ca = null;
        private Bitmap b = null;
        private Size BitmapSize = Size.Empty;


        // when bitmap is being drawn, then it should be drawn with this offset
        public Point OffsetBitmap = Point.Empty;

        public ShadowPainter(Rectangle area, Color c, int solidWidth, int gradWidth)
        {
            PrepareBitmap(area);

            ShadowColor = c;
            ShadowSolidWidth = solidWidth;
            ShadowWidth = solidWidth + gradWidth;

            PrepareColors();
        }

        public int ShadowGradientWidth
        {
            get { return ShadowWidth - ShadowSolidWidth; }
        }

        private void PrepareBitmap(Rectangle area)
        {
            OffsetBitmap = new Point(area.Left - ShadowWidth, area.Top - ShadowWidth);
            BitmapSize = new Size(area.Width + 2 * ShadowWidth + 1, area.Height + 2 * ShadowWidth + 1);
            b = new Bitmap(BitmapSize.Width, BitmapSize.Height);
        }

        public Image GetPNGImage()
        {
            byte[] result = null;
            using (MemoryStream stream = new MemoryStream())
            {
                b.Save(stream, ImageFormat.Png);
                result = stream.ToArray();
            }

            Image img = null;
            using (MemoryStream s2 = new MemoryStream(result))
            {
                img = Image.FromStream(s2);
            }

            return img;
        }

        private Color GetColor(int i)
        {
            if (ca == null)
                PrepareColors();

            if (i < 0) i = 0;
            if (i >= ca.Length) i = ca.Length - 1;
            return ca[i];
        }

        public void DrawShadowedRect(Rectangle ro)
        {
            int width = ShadowWidth;
            Rectangle r = ro;
            r.X -= OffsetBitmap.X;
            r.Y -= OffsetBitmap.Y;

            // inside area
            for (int y = r.Top; y <= r.Bottom; y++)
            {
                for (int x = r.Left; x <= r.Right; x++)
                {
                    SetPixel(b, x, y, GetColor(0));
                }
            }

            // left b
            for (int y = r.Top; y <= r.Bottom; y++)
            {
                int i = width;
                for (int x = r.Left - width; x <= r.Left; x++)
                {
                    SetPixel(b, x, y, GetColor(i));
                    i--;
                }
            }

            // right b
            for (int y = r.Top; y <= r.Bottom; y++)
            {
                int i = width;
                for (int x = r.Right + width; x >= r.Right; x--)
                {
                    SetPixel(b, x, y, GetColor(i));
                    i--;
                }
            }

            // top b
            for (int x = r.Left; x <= r.Right; x++)
            {
                int i = width;
                for (int y = r.Top - width; y <= r.Top; y++)
                {
                    SetPixel(b, x, y, GetColor(i));
                    i--;
                }
            }
            // bottom b
            for (int x = r.Left; x <= r.Right; x++)
            {
                int i = width;
                for (int y = r.Bottom + width; y >= r.Bottom; y--)
                {
                    SetPixel(b, x, y, GetColor(i));
                    i--;
                }
            }

            // TL
            DrawGradientCorner(b, r.Left - width, r.Top - width, r.Left, r.Top);
            // TR
            DrawGradientCorner(b, r.Right, r.Top - width, r.Right, r.Top);
            // BL
            DrawGradientCorner(b, r.Left - width, r.Bottom, r.Left, r.Bottom);
            // BR
            DrawGradientCorner(b, r.Right, r.Bottom, r.Right, r.Bottom);

        }

        private void SetPixel(Bitmap b, int x, int y, Color c)
        {
            Color cw = b.GetPixel(x, y);
            if (cw.A < c.A)
                b.SetPixel(x, y, c);
        }

        private void PrepareColors()
        {
            ca = new Color[ShadowWidth];
            for (int i = 0; i < ShadowSolidWidth; i++)
            {
                ca[i] = ShadowColor;
            }
            for (int i = 0; i < ShadowGradientWidth; i++)
            {
                ca[i + ShadowSolidWidth] = Color.FromArgb(Math.Min(255, 256 - i * (256 / ShadowGradientWidth)), ShadowColor);
            }
        }

        private void DrawGradientCorner(Bitmap b, int xs, int ys, int xr, int yr)
        {
            for (int x = xs; x < xs + ShadowWidth; x++)
            {
                for (int y = ys; y < ys + ShadowWidth; y++)
                {
                    int xd = Math.Abs(x - xr);
                    int yd = Math.Abs(y - yr);
                    int dist = Convert.ToInt32(Math.Sqrt(xd * xd + yd * yd));
                    SetPixel(b, x, y, GetColor(dist));
                }
            }
        }


    
    }

    public class RectangleStatistic
    {
        public List<Rectangle> Rects = new List<Rectangle>();

        public Rectangle TotalRectangle
        {
            get
            {
                bool init = false;
                int t = 0, l = 0, r = 0, b = 0;
                foreach (Rectangle rx in Rects)
                {
                    if (!init)
                    {
                        l = rx.Left;
                        t = rx.Top;
                        r = rx.Right;
                        b = rx.Bottom;
                        init = true;
                    }
                    else
                    {
                        l = Math.Min(l, rx.Left);
                        t = Math.Min(t, rx.Top);
                        b = Math.Max(b, rx.Bottom);
                        r = Math.Max(r, rx.Right);
                    }
                }

                return new Rectangle(l, t, r - l, b - t);
            }
        }

        public void DrawInto(ShadowPainter sp)
        {
            foreach (Rectangle r in Rects)
            {
                sp.DrawShadowedRect(r);
            }
        }
    }
}
