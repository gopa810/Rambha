using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using SlideMaker.Document;

namespace SlideMaker
{
    public class MNPageContext
    {
        public Graphics g;
        public int PageWidth;
        public int PageHeight;
        public bool drawSelectionMarks = true;
        public float zoom = 1.0f;
        public bool isTracking = false;

        public static Image[] HandImages = null;

        public Matrix LastMatrix { get; set; }

        public Matrix LastInvertMatrix { get; set; }

        public List<SMControl> TrackedObjects = new List<SMControl>();
        public Point TrackedStartLogical = new Point();
        public Point TrackedDrawOffset = new Point();

        public Point PointToRelative(Point clientPoint)
        {
            Point point = new Point();
            point.X = Convert.ToInt32((float)clientPoint.X / PageWidth * 1000);
            point.Y = Convert.ToInt32((float)clientPoint.Y / PageHeight * 1000);
            return point;
        }

        public Size SizeToRelative(Size clientSize)
        {
            Size point = new Size();
            point.Width = Convert.ToInt32((float)clientSize.Width / PageWidth * 1000);
            point.Height = Convert.ToInt32((float)clientSize.Height / PageHeight * 1000);
            return point;
        }


        public Point PhysicalToRelative(Point clientPoint)
        {
            clientPoint = PhysicalToLogical(clientPoint);
            clientPoint.X = clientPoint.X * 1000 / PageWidth;
            clientPoint.Y = clientPoint.Y * 1000 / PageHeight;
            return clientPoint;
        }

        public Point PhysicalToLogical(Point clientPoint)
        {
            Point[] a = new Point[] { clientPoint };
            if (LastInvertMatrix != null)
                LastInvertMatrix.TransformPoints(a);
            return a[0];
        }

        public int PhysicalToLogical(int clientDistance)
        {
            Point[] a = new Point[] { new Point(0, 0), new Point(clientDistance, 0) };
            if (LastInvertMatrix != null)
                LastInvertMatrix.TransformPoints(a);
            return a[1].X - a[0].X;
        }


        public Image GetHandImage(int ImageCode)
        {
            if (ImageCode < 0 || ImageCode > 32)
                ImageCode = 0;

            if (HandImages == null)
            {
                HandImages = new Image[33];
            }

            return HandImages[ImageCode];
        }
    }
}
