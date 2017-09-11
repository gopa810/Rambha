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
        public int xoffset;
        public int yoffset;

        public bool isTracking { get { return trackingType != SMControlSelection.None;  } }
        public SMControlSelection trackingType = SMControlSelection.None;

        public int selectedMenuItem = -1;

        public MNPage CurrentPage { get; set; }

        public SMScreen DisplaySize { get; set; }

        public static Image[] HandImages = null;

        public Matrix LastMatrix { get; set; }

        public Matrix LastInvertMatrix { get; set; }

        public Point TrackedStartLogical = new Point();
        public Point TrackedDrawOffset = new Point();
        public Point startClientPoint = new Point();
        public Point lastClientPoint = new Point();

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
        public Font DragItemFont = null;
        public Font PageTitleFont = null;
        public Brush semitransparentGrayBrush = null;

        public GSCore ViewController = null;
        public bool isMarkingArea = false;

        public Image navigIconBack = null;
        public Image navigIconFwd = null;
        public Image navigIconMenu = null;
        public Image navigIconHelp = null;
        public Image navigArrowBack = null;
        public Image navigArrowFwd = null;
        public Image navigSpeakerOn = null;
        public Image navigSpeakerOff = null;

        public SMTitledMessage messageBox = null;
        public Pen SpotAreaBorderPen = null;

        public Rectangle TopRectL = new Rectangle();
        public Rectangle SideRectLeftL = new Rectangle();
        public Rectangle SideRectRightL = new Rectangle();
        public Rectangle BackButtonRectL = new Rectangle();
        public Rectangle FwdButtonRectL = new Rectangle();
        public Rectangle BackAreaRectL = new Rectangle();
        public Rectangle FwdAreaRectL = new Rectangle();
        public Rectangle MenuButtonRectL = new Rectangle();
        public Rectangle HelpButtonRectL = new Rectangle();
        public Rectangle AudioButtonRectL = new Rectangle();

        public MNPageContext()
        {
            redConstraintTrack = new Pen(Color.Red, 3);
            LastMatrix = new Matrix();
            LastInvertMatrix = new Matrix();
            CurrentPage = null;
            HintFont = new Font(FontFamily.GenericSansSerif, 9);
            MenuFont = SMGraphics.GetFontVariation(MNFontName.GilSansMurari, 18);
            MenuTitleFont = new Font(FontFamily.GenericSansSerif, 24);
            DragItemFont = new Font(FontFamily.GenericSansSerif, 35);
            PageTitleFont = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Italic);
            semitransparentGrayBrush = new SolidBrush(Color.FromArgb(128, Color.Gray));
            messageBox = new SMTitledMessage(CurrentPage);
            SpotAreaBorderPen = new Pen(Color.LightBlue, 3);
        }

        public void ShowMessageBox()
        {
            messageBox.Visible = true;
        }

        public void PaintMessageBox(bool grayOutFreeSpace)
        {
            messageBox.Paint(this);
            if (grayOutFreeSpace)
                g.FillRectangle(semitransparentGrayBrush, 0, MNPage.HEADER_HEIGHT, PageWidth, messageBox.CurrentTop - MNPage.HEADER_HEIGHT);
        }

        public int DimensionForAxis(SMAxis axis)
        {
            if (axis == SMAxis.X) return PageWidth;
            return PageHeight;
        }

        public Point PointToRelative(Point clientPoint)
        {
            Point point = new Point();
            point.X = Convert.ToInt32((float)clientPoint.X / PageWidth * 1024);
            point.Y = Convert.ToInt32((float)clientPoint.Y / PageHeight * 768);
            return point;
        }

        public Size SizeToRelative(Size clientSize)
        {
            Size point = new Size();
            point.Width = Convert.ToInt32((float)clientSize.Width / PageWidth * 1024);
            point.Height = Convert.ToInt32((float)clientSize.Height / PageHeight * 768);
            return point;
        }


        public Point PhysicalToRelative(Point clientPoint)
        {
            clientPoint = PhysicalToLogical(clientPoint);
            clientPoint.X = clientPoint.X * 1024 / PageWidth;
            clientPoint.Y = clientPoint.Y * 768 / PageHeight;
            return clientPoint;
        }

        public Point PhysicalToLogical(Point clientPoint)
        {
            Point[] a = new Point[] { clientPoint };
            if (LastInvertMatrix != null)
                LastInvertMatrix.TransformPoints(a);
            return a[0];
        }

        public Rectangle PhysicalToLogical(Rectangle r)
        {
            Point[] a = new Point[] { r.Location, new Point(r.Right, r.Bottom) };
            if (LastInvertMatrix != null)
                LastInvertMatrix.TransformPoints(a);
            return new Rectangle(a[0].X, a[0].Y, a[1].X - a[0].X, a[1].Y - a[0].Y);
        }

        public int PhysicalToLogical(int clientDistance)
        {
            Point[] a = new Point[] { new Point(0, 0), new Point(clientDistance, 0) };
            if (LastInvertMatrix != null)
                LastInvertMatrix.TransformPoints(a);
            return a[1].X - a[0].X;
        }

        public Rectangle GetMarkingRect()
        {
            Rectangle r = Rectangle.Empty;
            if (startClientPoint.X < lastClientPoint.X)
            {
                r.X = startClientPoint.X;
                r.Width = lastClientPoint.X - startClientPoint.X;
            }
            else
            {
                r.X = lastClientPoint.X;
                r.Width = startClientPoint.X - lastClientPoint.X;
            }
            if (startClientPoint.Y < lastClientPoint.Y)
            {
                r.Y = startClientPoint.Y;
                r.Height = lastClientPoint.Y - startClientPoint.Y;
            }
            else
            {
                r.Y = lastClientPoint.Y;
                r.Height = startClientPoint.Y - lastClientPoint.Y;
            }
            return r;
        }



        public Size PageSize
        {
            get
            {
                switch (DisplaySize)
                {
                    case SMScreen.Screen_1024_768__4_3: return SMRectangleArea._size_4_3;
                    case SMScreen.Screen_1152_768__3_2: return SMRectangleArea._size_3_2;
                    case SMScreen.Screen_1376_774__16_9: return SMRectangleArea._size_16_9;
                    case SMScreen.Screen_768_1024__3_4: return SMRectangleArea._size_3_4;
                    default: return SMRectangleArea._size_4_3;
                }
            }
        }


        public bool isMovingTag { get; set; }

        public int hitHeaderButton { get; set; }


        public int LogicalToRelative(int p, int p_2)
        {
            return p*1000 / p_2;
        }
    }
}
