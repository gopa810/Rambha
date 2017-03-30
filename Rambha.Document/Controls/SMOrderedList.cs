using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using Rambha.Serializer;
using Rambha.Script;

namespace Rambha.Document
{
    public class SMOrderedList: SMControl
    {

        public SMTextDirection Orientation { get; set; }

        public List<object> Objects = new List<object>();

        public List<int> DrawnObjects = new List<int>();

        public SMOrderedList(MNPage p)
            : base(p)
        {
            Orientation = SMTextDirection.Vertical;
        }


        public override bool Load(RSFileReader br)
        {
            if (base.Load(br))
            {
                Objects.Clear();
                byte b;
                while ((b = br.ReadByte()) != 0)
                {
                    switch (b)
                    {
                        case 10: 
                            Orientation = (SMTextDirection)br.ReadInt32();
                            break;
                        case 20:
                            MNLazyImage li = new MNLazyImage(Document);
                            li.ImageId = br.ReadInt64();
                            Objects.Add(li);
                            break;
                        case 21:
                            string txt = br.ReadString();
                            Objects.Add(txt);
                            break;
                        default:
                            break;
                    }
                }
                return true;
            }

            return false;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteInt32((int)Orientation);

            foreach (object obj in Objects)
            {
                if (obj is string)
                {
                    bw.WriteByte(21);
                    bw.WriteString(obj as string);
                }
                else if (obj is MNLazyImage)
                {
                    bw.WriteByte(20);
                    bw.WriteInt64((obj as MNLazyImage).ImageId);
                }
            }
            // ending tag
            bw.WriteByte(0);
        }

        public override void Paint(MNPageContext context)
        {
            Rectangle rect = Area.GetBounds(context);
            SizeF offset = SizeF.Empty;

            PrepareBrushesAndPens();

            CalcCells(ref rect, ref offset);

            if (DrawnObjects.Count < Objects.Count)
            {
                MixObjects(context.drawSelectionMarks);
            }

            Font usedFont = GetUsedFont();

            bool enabledBorder = true;

            List<int> drw = DrawnObjects;

            if (moveTapIndex >= 0)
            {
                drw = new List<int>();
                drw.AddRange(DrawnObjects);

                int a = drw[startTapIndex];
                drw.RemoveAt(startTapIndex);
                drw.Insert(moveTapIndex, a);
            }

            for (int i = 0; i < drw.Count; i++)
            {
                Rectangle r = rect;
                Rectangle showRect;
                r.Inflate(-3, -3);
                r.Offset((int)(offset.Width*i), (int)(offset.Height*i));
                showRect = r;
                showRect.Inflate(-3, -3);

                int objectIndex = drw[i];
                if (objectIndex == i && enabledBorder && startTapIndex < 0)
                {
                    context.g.FillRectangle(Brushes.Green, r.X, r.Y, 3, r.Height);
                    context.g.FillRectangle(Brushes.Green, r.X, r.Y, r.Width, 3);
                    context.g.FillRectangle(Brushes.Green, r.Right - 3, r.Y, 3, r.Height);
                    context.g.FillRectangle(Brushes.Green, r.X, r.Bottom - 3, r.Width, 3);
                }
                else
                {
                    enabledBorder = false;
                }

                object obj = Objects[objectIndex];
                // draw moved item near the cursor
                if (objectIndex == startObjectIndex)
                {
                    Point lastPoint = context.PhysicalToLogical(context.lastClientPoint);

                    showRect.X = lastPoint.X - startTapOffset.X;
                    showRect.Y = lastPoint.Y - startTapOffset.Y;
                }
                if (obj is String)
                {
                    string plainText = obj as string;
                    StringFormat format = Paragraph.GetAlignmentStringFormat();
                    context.g.DrawString(plainText, usedFont, tempForeBrush, showRect, format);
                }
                else if (obj is MNLazyImage)
                {
                    DrawImage(context, showRect, (obj as MNLazyImage).ImageData, SMContentScaling.Fit);
                }
            }

            base.Paint(context);
        }

        private void CalcCells(ref Rectangle rect, ref SizeF offset)
        {
            if (Orientation == SMTextDirection.Horizontal)
            {
                offset.Width = (float)rect.Width / Math.Max(1, Objects.Count);
                rect.Width = (int)offset.Width;
            }
            else
            {
                offset.Height = (float)rect.Height / Math.Max(1, Objects.Count);
                rect.Height = (int)offset.Height;
            }
        }

        public int GetCellIndex(ref Rectangle bounds, ref Rectangle cell, ref Point startPoint)
        {
            int col = (startPoint.X - bounds.X) / cell.Width;
            int row = (startPoint.Y - bounds.Y) / cell.Height;
            int index = (Orientation == SMTextDirection.Horizontal) ? col : row;

            if (index < 0)
                index = -1;
            if (index >= Objects.Count)
                index = -1;

            Debugger.Log(0, "", "Determined index: " + index + "\n");
            return index;
        }

        private Point startTapOffset = Point.Empty;
        private int moveTapIndex = -1;
        private int startTapIndex = -1;
        private int startObjectIndex = -1;

        public override void OnTapBegin(PVDragContext dc)
        {
            Rectangle rc = Area.GetBounds(dc.context);
            SizeF offset = SizeF.Empty;
            Rectangle bounds = rc;
            int index;
            CalcCells(ref rc, ref offset);
            index = GetCellIndex(ref bounds, ref rc, ref dc.startPoint);

            if (index >= 0)
            {
                startTapIndex = index;
                startObjectIndex = DrawnObjects[index];
                startTapOffset.X = dc.startPoint.X - rc.X - Convert.ToInt32(offset.Width * index);
                startTapOffset.Y = dc.startPoint.Y - rc.Y - Convert.ToInt32(offset.Height * index);
            }

            base.OnTapBegin(dc);
        }

        public override void OnTapMove(PVDragContext dc)
        {
            Rectangle rc = Area.GetBounds(dc.context);
            SizeF offset = SizeF.Empty;
            Rectangle bounds = rc;
            int index;
            CalcCells(ref rc, ref offset);
            index = GetCellIndex(ref bounds, ref rc, ref dc.lastPoint);

            if (index >= 0)
            {
                moveTapIndex = index;
            }

            base.OnTapMove(dc);
        }

        public override void OnTapEnd(PVDragContext dc)
        {
            Rectangle rc = Area.GetBounds(dc.context);
            SizeF offset = SizeF.Empty;
            Rectangle bounds = rc;
            int index;
            CalcCells(ref rc, ref offset);
            index = GetCellIndex(ref bounds, ref rc, ref dc.lastPoint);

            if (index >= 0)
            {
                DrawnObjects.RemoveAt(startTapIndex);
                DrawnObjects.Insert(index, startObjectIndex);
            }

            moveTapIndex = -1;
            startTapIndex = -1;
            startObjectIndex = -1;

            base.OnTapEnd(dc);
        }

        public void MixObjects(bool excludeMixing)
        {
            List<int> a = new List<int>();
            List<int> b = new List<int>();

            for (int i = 0; i < Objects.Count; i++)
                a.Add(i);

            if (excludeMixing)
            {
                b.AddRange(a);
            }
            else
            {
                Random r = new Random();
                while (a.Count > 0)
                {
                    int idx = r.Next(a.Count);
                    b.Add(a[idx]);
                    a.RemoveAt(idx);
                }
            }

            DrawnObjects = b;
        }

        public void AddText(string text)
        {
            Objects.Add(text);
        }

        public void AddImage(MNReferencedImage ri)
        {
            MNLazyImage li = new MNLazyImage(Document);
            li.Image = ri;
            Objects.Add(li);
        }
    }
}
