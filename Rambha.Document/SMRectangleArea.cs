using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.IO;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMRectangleArea: GSCore
    {
        /// <summary>
        /// Relative area means that coordinates of rectangle are as if from 
        /// screen of size 1024x768 points
        /// </summary>
        public Rectangle RelativeArea = Rectangle.Empty;

        [Browsable(false)]
        public bool Selected { get; set; }

        public void Clear()
        {
            RelativeArea = Rectangle.Empty;
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


        public Rectangle[] LastBounds = new Rectangle[9];

        public SMRectangleArea()
        {
            Selected = false;
            RelativeArea = Rectangle.Empty;
        }

        public SMRectangleArea(SMRectangleArea source)
        {
            if (source == null)
            {
                RelativeArea = Rectangle.Empty;
                Selected = false;
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

        public void RecalcAllBounds(MNPageContext ctx)
        {
        }

        public Rectangle GetRawRectangle(PageEditDisplaySize ds)
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
            context.g.FillRectangle(Brushes.DarkBlue, x - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
            context.g.FillRectangle(Brushes.DarkBlue, (x + r) / 2 - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
            context.g.FillRectangle(Brushes.DarkBlue, r - markWidth, y - markWidth, 2 * markWidth, 2 * markWidth);
            context.g.FillRectangle(Brushes.DarkBlue, x - markWidth, (y + b) / 2 - markWidth, 2 * markWidth, 2 * markWidth);
            context.g.FillRectangle(Brushes.DarkBlue, r - markWidth, (y + b) / 2 - markWidth, 2 * markWidth, 2 * markWidth);
            context.g.FillRectangle(Brushes.DarkBlue, x - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
            context.g.FillRectangle(Brushes.DarkBlue, (x + r) / 2 - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
            context.g.FillRectangle(Brushes.DarkBlue, r - markWidth, b - markWidth, 2 * markWidth, 2 * markWidth);
        }
    }
}
