using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Rambha.Script;

namespace Rambha.Document
{
    public class MNPageContext: GSCore
    {
        public Graphics g;
        public int PageWidth;
        public int PageHeight;
        public bool drawSelectionMarks = true;
        public float zoom = 1.0f;
        public bool isTracking = false;

        public int selectedMenuItem = -1;

        public MNPage CurrentPage { get; set; }

        public PageEditDisplaySize DisplaySize { get; set; }

        public static Image[] HandImages = null;

        public Matrix LastMatrix { get; set; }

        public Matrix LastInvertMatrix { get; set; }

        public List<SMControl> TrackedObjects = new List<SMControl>();
        public Point TrackedStartLogical = new Point();
        public Point TrackedDrawOffset = new Point();

        public Point TrackedBoundaryStart = new Point();
        public Point TrackedBoundaryEnd = new Point();
        public SMRuler TrackedBoundaryRuler = null;
        public SMRuler TrackedBoundaryEndRuler = null;

        public Cursor cursorBoundaryReady;
        public Cursor cursorBoundaryAnchor;
        public Cursor cursorBoundaryMoving;

        public Pen redConstraintTrack;

        public Font HintFont = null;
        public Font MenuFont = null;
        public Font MenuTitleFont = null;
        public Brush semitransparentGrayBrush = null;

        public GSCore ViewController = null;

        public MNPageContext()
        {
            redConstraintTrack = new Pen(Color.Red, 3);
            LastMatrix = new Matrix();
            LastInvertMatrix = new Matrix();
            CurrentPage = null;
            HintFont = new Font(FontFamily.GenericSansSerif, 9);
            MenuFont = SystemFonts.MenuFont;
            MenuTitleFont = new Font(FontFamily.GenericSansSerif, 13);
            semitransparentGrayBrush = new SolidBrush(Color.FromArgb(128,Color.Gray));
        }

        public int DimensionForAxis(SMAxis axis)
        {
            if (axis == SMAxis.X) return PageWidth;
            return PageHeight;
        }

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

        public Size PageSize
        {
            get
            {
                switch (DisplaySize)
                {
                    case PageEditDisplaySize.LandscapeBig: return new Size(1024, 768);
                    case PageEditDisplaySize.LandscapeSmall: return new Size(800, 600);
                    case PageEditDisplaySize.PortaitBig: return new Size(768, 1024);
                    case PageEditDisplaySize.PortaitSmall: return new Size(600, 800);
                    default: return new Size(1024, 768);
                }
            }
        }

    }
}
