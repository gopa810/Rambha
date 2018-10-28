using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMRectangleArea: GSCore
    {
        public SMScreen Screen = SMScreen.Screen_1024_768__4_3;

        /// <summary>
        /// Relative area means that coordinates of rectangle are as if from 
        /// screen of size 1024x768 points
        /// </summary>
        public Rectangle RelativeArea = Rectangle.Empty;

        [Browsable(true), Category("Layout")]
        public SMControlSelection Dock 
        {
            get
            {
                return p_dock;
            }
            set
            {
                p_dock = value;
                DockModified = false;
            }
        }
        private SMControlSelection p_dock = SMControlSelection.None;

        public bool DockModified { get; set; }

        [Browsable(true), Category("Layout")]
        public SMBackgroundType BackType { get; set; }

        public Image BackgroundImage = null;
        public Point BackgroundImageOffset = Point.Empty;


        [Browsable(false)]
        public bool Selected { get; set; }

        public void Clear()
        {
            RelativeArea = Rectangle.Empty;
            Dock = SMControlSelection.None;
        }

        public void Save(RSFileWriter bw)
        {
            bw.Log("* * * AREA * * *\n");
            bw.WriteByte(11);
            bw.WriteBool(Selected);

            bw.WriteByte(12);
            bw.WriteInt32(RelativeArea.X);
            bw.WriteInt32(RelativeArea.Y);
            bw.WriteInt32(RelativeArea.Width);
            bw.WriteInt32(RelativeArea.Height);

            bw.WriteByte(13);
            bw.WriteInt32((int)Dock);

            bw.WriteByte(14);
            bw.WriteInt32((int)BackType);

            if (BackgroundImage != null)
            {
                bw.WriteByte(15);
                bw.WriteImage(BackgroundImage);
            }

            bw.WriteByte(16);
            bw.WriteInt32(BackgroundImageOffset.X);
            bw.WriteInt32(BackgroundImageOffset.Y);

            bw.WriteByte(17);
            bw.WriteInt32((int)Screen);

            bw.WriteByte(18);
            bw.WriteBool(DockModified);

            // end of object
            bw.WriteByte(0);
        }

        public bool Load(RSFileReader br)
        {
            br.Log("* * * AREA * * *\n");
            byte tag;

            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        {
                            int left = LoadRuler(br, 1024);
                            int top = LoadRuler(br, 768);
                            int right = LoadRuler(br, 1024);
                            int bottom = LoadRuler(br, 768);
                            RelativeArea = new Rectangle(left, top, right - left, bottom - top);
                        }
                        break;
                    case 11:
                        Selected = br.ReadBool();
                        break;
                    case 12:
                        RelativeArea.X = br.ReadInt32();
                        RelativeArea.Y = br.ReadInt32();
                        RelativeArea.Width = br.ReadInt32();
                        RelativeArea.Height = br.ReadInt32();
                        break;
                    case 13:
                        Dock = (SMControlSelection)br.ReadInt32();
                        if (Dock != SMControlSelection.None)
                            BackType = SMBackgroundType.Solid;
                        break;
                    case 14:
                        BackType = (SMBackgroundType)br.ReadInt32();
                        break;
                    case 15:
                        BackgroundImage = br.ReadImage();
                        break;
                    case 16:
                        BackgroundImageOffset = new Point(br.ReadInt32(), br.ReadInt32());
                        break;
                    case 17:
                        Screen = (SMScreen)br.ReadInt32();
                        break;
                    case 18:
                        DockModified = br.ReadBool();
                        break;
                }
            }

            return true;
        }

        public int LoadRuler(RSFileReader br, int dim)
        {
            double d = 0;
            byte tag;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10:
                        br.ReadBool();
                        br.ReadBool();
                        br.ReadBool();
                        br.ReadBool();
                        d = br.ReadDouble();
                        br.ReadDouble();
                        br.ReadDouble();
                        br.ReadDouble();
                        break;
                    case 11: br.ReadInt32(); break;
                    case 12: br.ReadBool(); break;
                    case 13: br.ReadBool(); break;
                }
            }

            return Convert.ToInt32(d * dim);
        }

        public SMRectangleArea()
        {
            Selected = false;
            RelativeArea = Rectangle.Empty;
            Dock = SMControlSelection.None;
            DockModified = false;
        }

        public SMRectangleArea(SMRectangleArea source)
        {
            if (source == null)
            {
                RelativeArea = Rectangle.Empty;
                Selected = false;
                Dock = SMControlSelection.None;
                DockModified = false;
            }
            else
            {
                Copy(source, this);
            }
        }

        public static void Copy(SMRectangleArea source, SMRectangleArea target)
        {
            target.RelativeArea = source.RelativeArea;
            target.Selected = source.Selected;
            target.Dock = source.Dock;
            target.BackType = source.BackType;
            target.BackgroundImage = source.BackgroundImage;
            target.BackgroundImageOffset = source.BackgroundImageOffset;
            target.Screen = source.Screen;
            target.DockModified = source.DockModified;
        }

        public virtual void MoveRaw(int rx, int ry)
        {
            RelativeArea.X += rx;
            RelativeArea.Y += ry;
        }

        public void SetCenterSize(Point center, Size defSize)
        {
            RelativeArea = new Rectangle(center.X - defSize.Width / 2, center.Y - defSize.Height / 2,
                defSize.Width, defSize.Height);
        }

        public int Left
        {
            get { return RelativeArea.Left; }
            set { RelativeArea.Width -= (value - RelativeArea.X); RelativeArea.X = value; }
        }

        public int Right
        {
            get { return RelativeArea.Right; }
            set { RelativeArea.Width = value - RelativeArea.X; }
        }

        public int Top
        {
            get { return RelativeArea.Top; }
            set { RelativeArea.Height -= (value - RelativeArea.Y); RelativeArea.Y = value; }
        }

        public int Bottom
        {
            get { return RelativeArea.Bottom; }
            set { RelativeArea.Height = value - RelativeArea.Y; }
        }

        public int Width
        {
            get { return RelativeArea.Width; }
            set { RelativeArea.Width = value; }
        }

        public int Height
        {
            get { return RelativeArea.Height;  }
            set { RelativeArea.Height = value; }
        }

        public int CenterX
        {
            get { return RelativeArea.X + RelativeArea.Width / 2; }
            set { RelativeArea.X = value - RelativeArea.Width / 2; }
        }

        public int CenterY
        {
            get { return RelativeArea.Y + RelativeArea.Height / 2; }
            set { RelativeArea.Y = value - RelativeArea.Height / 2; }
        }

        public virtual bool TestHitLogical(MNPageContext context, Point logicalPoint)
        {
            return RelativeArea.Contains(logicalPoint);
        }

        public virtual SMControlSelection TestHitLogical(MNPageContext context, Rectangle logRect)
        {
            return RelativeArea.IntersectsWith(logRect) ? SMControlSelection.All : SMControlSelection.None;
        }

        /// <summary>
        /// returns bounds of control in logical coordinate system
        /// if control is tracked, then returns modified bounds according tracking values
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public Rectangle GetBounds(MNPageContext ctx)
        {
            return RelativeArea;
        }

        public Rectangle GetBounds(PageEditDisplaySize dsp)
        {
            return RelativeArea;
        }

        public Rectangle GetBoundsRecalc(MNPageContext ctx)
        {
            return RelativeArea;
        }

        public void SetRawRectangle(PageEditDisplaySize ds, Rectangle r)
        {
            RelativeArea = r;
        }

        public void Set(SMRectangleArea area)
        {
            Copy(area, this);
        }

        public byte[] GetBytes()
        {
            byte[] buffer = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    RSFileWriter fw = new RSFileWriter(bw);
                    Save(fw);
                    buffer = ms.GetBuffer();
                }
            }

            return buffer;
        }

        public static SMRectangleArea FromBytes(byte[] buffer)
        {
            SMRectangleArea area = null;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    RSFileReader fr = new RSFileReader(br);
                    area = new SMRectangleArea();
                    area.Load(fr);
                }
            }
            return area;
        }

        public virtual void PaintSelectionMarks(MNPageContext context)
        {
            int x = RelativeArea.X;
            int y = RelativeArea.Y;
            int r = RelativeArea.Right;
            int b = RelativeArea.Bottom;

            int markWidth = context.PhysicalToLogical(3);
            if (Dock == SMControlSelection.None || Dock == SMControlSelection.Bottom)
                context.g.FillRectangle(Brushes.DarkBlue, (x + r) / 2 - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
            if (Dock == SMControlSelection.None || Dock == SMControlSelection.Top)
                context.g.FillRectangle(Brushes.DarkBlue, (x + r) / 2 - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
            if (Dock == SMControlSelection.None || Dock == SMControlSelection.Right)
                context.g.FillRectangle(Brushes.DarkBlue, x - markWidth, (y + b) / 2 - markWidth, 2 * markWidth, 2 * markWidth);
            if (Dock == SMControlSelection.None || Dock == SMControlSelection.Left)
                context.g.FillRectangle(Brushes.DarkBlue, r - markWidth, (y + b) / 2 - markWidth, 2 * markWidth, 2 * markWidth);

            if (Dock == SMControlSelection.None)
            {
                context.g.FillRectangle(Brushes.DarkBlue, x - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
                context.g.FillRectangle(Brushes.DarkBlue, r - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
                context.g.FillRectangle(Brushes.DarkBlue, x - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
                context.g.FillRectangle(Brushes.DarkBlue, r - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
            }
        }

        public static Size _size_4_3 = new Size(1024, 768);
        public static Size _size_3_2 = new Size(1152, 768);
        public static Size _size_16_9 = new Size(1376, 774);
        public static Size _size_3_4 = new Size(768, 1024);

        public static Size GetPageSize(SMScreen screen)
        {
            switch (screen)
            {
                case SMScreen.Screen_1152_768__3_2:
                    return _size_3_2;
                case SMScreen.Screen_1376_774__16_9:
                    return _size_16_9;
                case SMScreen.Screen_768_1024__3_4:
                    return _size_3_4;
                default:
                    return _size_4_3;
            }
        }

        public const int PADDING_DOCK_TOP = 33;
        public const int PADDING_DOCK_BOTTOM = 44;
        public const int PADDING_DOCK_LEFT = 66;
        public const int PADDING_DOCK_RIGHT = 88;

        public Rectangle GetDockedRectangle(Size screenSize, Size contentSize)
        {
            if (Dock == SMControlSelection.Top)
            {
                RelativeArea.X = 0;
                RelativeArea.Y = MNPage.HEADER_HEIGHT;
                if (!DockModified)
                    RelativeArea.Height = contentSize.Height + PADDING_DOCK_BOTTOM + PADDING_DOCK_TOP;
                RelativeArea.Width = screenSize.Width;
            }
            else if (Dock == SMControlSelection.Bottom)
            {
                if (!DockModified)
                {
                    RelativeArea.Y = screenSize.Height - contentSize.Height - PADDING_DOCK_TOP - PADDING_DOCK_BOTTOM;
                    RelativeArea.Height = screenSize.Height - RelativeArea.Y;
                }
                else
                {
                    RelativeArea.Y = screenSize.Height - RelativeArea.Height;
                }
                RelativeArea.X = 0;
                RelativeArea.Width = screenSize.Width;
            }
            else if (Dock == SMControlSelection.Right)
            {
                if (!DockModified)
                {
                    RelativeArea.X = screenSize.Width - contentSize.Width - PADDING_DOCK_LEFT - PADDING_DOCK_RIGHT;
                    RelativeArea.Width = screenSize.Width - RelativeArea.X;
                }
                else
                {
                    RelativeArea.X = screenSize.Width - RelativeArea.Width;
                }
                RelativeArea.Y = MNPage.HEADER_HEIGHT;
                RelativeArea.Height = screenSize.Height - MNPage.HEADER_HEIGHT;
            }
            else if (Dock == SMControlSelection.Left)
            {
                RelativeArea.X = 0;
                RelativeArea.Y = MNPage.HEADER_HEIGHT;
                RelativeArea.Height = screenSize.Height - MNPage.HEADER_HEIGHT;
                if (!DockModified)
                    RelativeArea.Width = contentSize.Width + PADDING_DOCK_RIGHT + PADDING_DOCK_LEFT;
            }

            return RelativeArea;
        }

        public string HtmlLT()
        {
            if (Dock == SMControlSelection.None)
                return SMRectangleArea.HtmlLT(RelativeArea);
            return HtmlDock();
        }

        public string HtmlLTR()
        {
            if (Dock == SMControlSelection.None)
                return SMRectangleArea.HtmlLTR(RelativeArea);
            return HtmlDock();
        }
        public string HtmlLTRB()
        {
            if (Dock == SMControlSelection.None)
                return SMRectangleArea.HtmlLTRB(RelativeArea);
            return HtmlDock();
        }

        private string HtmlDock()
        {
            if (Dock == SMControlSelection.Top)
            {
                return "left:0;top:" + MNPage.HEADER_HEIGHT.ToString() + ";right:0;";
            }
            else if (Dock == SMControlSelection.Left)
            {
                return "left:0;top:" + MNPage.HEADER_HEIGHT.ToString() + ";bottom:0%;";
            }
            else if (Dock == SMControlSelection.Bottom)
            {
                return "left:0;bottom:0;right:0;";
            }
            else if (Dock == SMControlSelection.Right)
            {
                return "right:0;top:" + MNPage.HEADER_HEIGHT.ToString() + ";bottom:0";
            }

            return "";
        }

        public static int AbsToPercX(int x) { return 25 * x / 256; }
        public static int AbsToPercY(int y) { return 25 * y / 192; }

        public static string HtmlLT(Rectangle r)
        {
            return string.Format("left:{0}%;top:{1}%;", 25 * r.Left / 256, 25 * r.Top / 192);
        }
        public static string HtmlLTR(Rectangle r)
        {
            return string.Format("left:{0}%;top:{1}%;width:{2}%;", 25 * r.Left / 256, 25 * r.Top / 192, 25 * r.Width / 256);
        }
        public static string HtmlLTRB(Rectangle r)
        {
            return string.Format("left:{0}%;top:{1}%;width:{2}%;height:{3}%;", 25 * r.Left / 256, 25 * r.Top / 192, 25 * r.Width / 256, 25 * r.Height / 192);
        }
    }
}
