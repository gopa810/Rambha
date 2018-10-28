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
        public class StringItem
        {
            //
            // text properties
            public StringItem(string text)
            {
                this.Text = text;
            }
            private string _text = null;
            public string Text {
                get { return _text; }
                set { _text = value; TextWidth = -1; ItemWidth = -1; }
            }
            public float TextWidth = -1;


            //
            // image properties
            //
            public StringItem(MNLazyImage lazyImage)
            {
                this.Image = lazyImage;
            }
            public MNLazyImage Image = null;



            // general properties
            public float ItemX = 0;
            public float ItemY = 0;
            public float ItemWidth = -1;
            public float ItemHeight = -1;
            public bool IsImage
            {
                get { return Image != null; }
            }
            public bool IsText
            {
                get { return _text != null; }
            }

            public SizeF SizeF
            {
                get { return new SizeF(ItemWidth, ItemHeight); }
            }
            public Size Size
            {
                get { return new Size((int)ItemWidth, (int)ItemHeight); }
            }
            public Rectangle Rectangle
            {
                get { return new Rectangle(0, 0, (int)ItemWidth, (int)ItemHeight); }
            }
        }

        public SMTextDirection Orientation { get; set; }

        public string EndingChar { get; set; } = "";

        public bool LockLast { get; set; } = false;

        public bool CapitalizeFirst { get; set; } = false;

        public List<StringItem> Objects = new List<StringItem>();

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
                            Objects.Add(new StringItem(li));
                            break;
                        case 21:
                            string txt = br.ReadString();
                            Objects.Add(new StringItem(txt));
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
            //SizeF offset = SizeF.Empty;

            SMStatusLayout layout = PrepareBrushesAndPens();

            Font usedFont = GetUsedFont();

            CalcCells(context.g, usedFont, rect);

            if (DrawnObjects.Count < Objects.Count)
            {
                MixObjects(context.drawSelectionMarks);
            }

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

            float floating_offset = 0;
            Rectangle r, showRect;
            int j = 0;
            Point lastPoint = context.PhysicalToLogical(context.lastClientPoint);

            for (int i = 0; i < drw.Count; i++, j++)
            {
                int objectIndex;

                if (moveTapIndex >= 0 && moveTapIndex == j)
                {
                    objectIndex = startObjectIndex;
                }
                else
                {
                    objectIndex = drw[j];
                    if (objectIndex == startObjectIndex)
                    {
                        j++;
                        objectIndex = drw[j];
                    }
                }


                StringItem obj = Objects[objectIndex];

                r = obj.Rectangle;
                r.Location = rect.Location;
                if (Orientation == SMTextDirection.Horizontal)
                    r.X += (int)floating_offset;
                else
                    r.Y += (int)floating_offset;
                r.Inflate(-3, -3);
                showRect = r;
                showRect.Inflate(-3, -3);

                // draw object
                DrawListItem(context, layout, usedFont, showRect, i == 0, obj);


                // move floating offset
                if (Orientation == SMTextDirection.Horizontal)
                    floating_offset += obj.ItemWidth;
                else
                    floating_offset += obj.ItemHeight;

                // draw a border
                Brush br;

                if (objectIndex == i && enabledBorder && startTapIndex < 0)
                {
                    if (objectIndex == startObjectIndex)
                        br = Brushes.BlueViolet;
                    else
                        br = Brushes.Green;
                }
                else
                {
                    if (objectIndex == startObjectIndex)
                        br = Brushes.BlueViolet;
                    else
                        br = Brushes.Gainsboro;
                    enabledBorder = false;
                }

                context.g.FillRectangle(br, r.X, r.Y, 3, r.Height);
                context.g.FillRectangle(br, r.X, r.Y, r.Width, 3);
                context.g.FillRectangle(br, r.Right - 3, r.Y, 3, r.Height);
                context.g.FillRectangle(br, r.X, r.Bottom - 3, r.Width, 3);
            }

            if (startObjectIndex >= 0)
            {
                showRect = new Rectangle(lastPoint.X - startObjectWidth,
                    lastPoint.Y - startObjectHeight, startObjectWidth * 2, startObjectHeight * 2);
                context.g.DrawRectangle(Pens.BlueViolet, showRect);
                DrawListItem(context, layout, usedFont, showRect, false, Objects[startObjectIndex]);
            }

            base.Paint(context);
        }


        public override void ExportToHtml(MNExportContext ctx, int zorder, StringBuilder sbHtml, StringBuilder sbCss, StringBuilder sbJS)
        {
            bool horz = (Orientation == SMTextDirection.Horizontal);
            string blockFormat = Font.HtmlString() + Paragraph.Html() + ContentPaddingHtml();
            sbCss.AppendFormat(".c{0}n {{ {1} {2} height:{3}%;width:{4}%; }}\n", Id, HtmlFormatColor(false), blockFormat,
                horz ? 100 : 100/Objects.Count, horz ? 100/Objects.Count : 100 );
//            sbCss.AppendFormat(".c{0}h {{ {1} {2} }}\n", Id, HtmlFormatColor(true), blockFormat);

            sbHtml.Append("<div ");
            sbHtml.AppendFormat(" id=\"c{0}\" ", this.Id);
            sbHtml.AppendFormat(" style ='display:flex;flex-direction:{1};position:absolute;z-index:{0};", zorder,
                horz ? "row" : "column");
            SMRectangleArea area = this.Area;
            sbHtml.Append(area.HtmlLTRB());
            sbHtml.Append("'>\n");
            foreach(StringItem si in Objects)
            {
                sbHtml.AppendFormat("<div class=\"c{0}n\">\n", Id);
                if (si.IsText)
                {
                    sbHtml.AppendFormat("<div class=\"vertCenter\"><div>\n");
                    sbHtml.AppendFormat("{0}", si.Text);
                    sbHtml.AppendFormat("</div></div>");
                }
                else if (si.IsImage)
                {
                    sbHtml.AppendFormat("<img src=\"{0}\" style='object-fit:contain;width:100%;height:100%;'>", 
                        ctx.GetFileNameFromImage(si.Image.Image));
                }
                sbHtml.AppendFormat("</div>\n");
            }
            //sbHtml.Append("background:lightyellow;border:1px solid black;'>");
            //sbHtml.Append("<b>" + GetType().Name + "</b><br>" + this.Text);
            sbHtml.Append("</div>\n");
        }

        private void DrawListItem(MNPageContext context, SMStatusLayout layout, Font usedFont, Rectangle showRect, bool isFirst, StringItem obj)
        {
            if (obj.IsText)
            {
                StringFormat format = Paragraph.GetAlignmentStringFormat();
                if (Orientation == SMTextDirection.Horizontal)
                {
                    format.Trimming = StringTrimming.None;
                    format.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
                }
                context.g.DrawString(CapitalizeCond(obj.Text, isFirst), usedFont, tempForeBrush, showRect, format);

            }
            else if (obj.IsImage)
            {
                DrawImage(context, layout, showRect, obj.Image.ImageData, SMContentScaling.Fit);
            }
        }

        private string CapitalizeCond(string plainText, bool isFirst)
        {
            if (!isFirst || !CapitalizeFirst || plainText.Length == 0 || char.IsUpper(plainText[0]))
                return plainText;

            return string.Format("{0}{1}", char.ToUpper(plainText[0]), plainText.Substring(1));
        }

        private void CalcCells(Graphics g, Font usedFont, Rectangle rect)
        {
            if (Orientation == SMTextDirection.Horizontal)
            {
                float total = 0;
                foreach(StringItem obj in Objects)
                {
                    if (obj.IsText)
                    {
                        if (obj.TextWidth == -1)
                        {
                            SizeF sf = g.MeasureString(obj.Text, usedFont);
                            obj.TextWidth = sf.Width;
                        }
                    }
                    else
                    {
                        obj.TextWidth = rect.Width / Objects.Count;
                    }
                    total += obj.TextWidth;
                }

                total = Math.Max(0, (rect.Width - total)/Math.Max(1, Objects.Count));

                foreach (StringItem obj in Objects)
                {
                    obj.ItemWidth = obj.TextWidth  + total;
                    obj.ItemHeight = rect.Height;
                }
            }
            else
            {
                foreach(StringItem si in Objects)
                {
                    si.ItemWidth = rect.Width;
                    si.ItemHeight = rect.Height / Math.Max(1, Objects.Count);
                }
            }
        }

        public int GetCellIndex(ref Rectangle bounds, ref Rectangle cell, Point startPoint, bool moving)
        {
            int index = -1;

            if (Orientation == SMTextDirection.Horizontal)
            {
                float x = bounds.Left;
                float spx = startPoint.X - (moving ? startObjectWidth : 0);
                for (int i = 0; i < DrawnObjects.Count; i++)
                {
                    StringItem si = Objects[DrawnObjects[i]];
                    si.ItemX = x;
                    si.ItemY = bounds.Top;
                    if (spx >= x && spx <= x + si.ItemWidth)
                    {
                        index = i;
                        break;
                    }
                    x += si.ItemWidth;
                }
            }
            else
            {
                float y = bounds.Top;
                float spy = startPoint.Y - (moving ? startObjectHeight : 0);
                for(int i = 0; i < DrawnObjects.Count; i++)
                {
                    StringItem si = Objects[DrawnObjects[i]];
                    si.ItemX = bounds.Left;
                    si.ItemY = y;
                    if (spy >= y && spy <= y + si.ItemHeight)
                    {
                        index = i;
                        break;
                    }
                    y += si.ItemHeight;
                }
            }

            Debugger.Log(0, "", "Determined index: " + index + "\n");
            return index;
        }

        private Point startTapOffset = Point.Empty;
        private int moveTapIndex = -1;
        private int startTapIndex = -1;
        private int startObjectIndex = -1;
        private int startObjectWidth = 0;
        private int startObjectHeight = 0;

        public override void OnTapBegin(PVDragContext dc)
        {
            Rectangle rc = Area.GetBounds(dc.context);
            Rectangle bounds = rc;
            int index;
            Font usedFont = GetUsedFont();
            //CalcCells(dc.context.g, usedFont, rc);
            index = GetCellIndex(ref bounds, ref rc, dc.startPoint, false);
            if (LockLast && index == Objects.Count - 1)
                index = -1;

            if (index >= 0)
            {
                startTapIndex = index;
                moveTapIndex = index;
                startObjectIndex = DrawnObjects[index];
                StringItem si = Objects[startObjectIndex];
                startObjectWidth = (int)si.ItemWidth / 2;
                startObjectHeight = (int)si.ItemHeight / 2;
                startTapOffset.X = dc.startPoint.X - Convert.ToInt32(si.ItemX);
                startTapOffset.Y = dc.startPoint.Y - Convert.ToInt32(si.ItemY);
            }

            base.OnTapBegin(dc);
        }

        public override void OnTapMove(PVDragContext dc)
        {
            Rectangle rc = Area.GetBounds(dc.context);
            Rectangle bounds = rc;
            int index;
            Font usedFont = GetUsedFont();
            //CalcCells(dc.context.g, usedFont, rc);
            index = GetCellIndex(ref bounds, ref rc, dc.lastPoint, false);
            if (LockLast && index == Objects.Count - 1 && index > 1)
                index = Objects.Count - 2;

            if (index >= 0)
            {
                moveTapIndex = index;
            }

            base.OnTapMove(dc);
        }

        public override void OnTapEnd(PVDragContext dc)
        {
            Rectangle rc = Area.GetBounds(dc.context);
            Rectangle bounds = rc;
            int index;
            Font usedFont = GetUsedFont();
            CalcCells(dc.context.g, usedFont, rc);
            index = GetCellIndex(ref bounds, ref rc, dc.lastPoint, false);
            if (LockLast && index == Objects.Count - 1 && index > 1)
                index = Objects.Count - 2;

            if (index >= 0)
            {
                DrawnObjects.RemoveAt(startTapIndex);
                DrawnObjects.Insert(index, startObjectIndex);
            }

            moveTapIndex = -1;
            startTapIndex = -1;
            startObjectIndex = -1;
            startObjectWidth = 0;

            base.OnTapEnd(dc);
        }

        public void MixObjects(bool excludeMixing)
        {
            List<int> a = new List<int>();
            List<int> b = new List<int>();

            int max = Objects.Count - 1;
            if (LockLast)
                max--;

            for (int i = 0; i <= max; i++)
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

            if (LockLast)
                b.Add(max + 1);

            DrawnObjects = b;
        }

        public void AddText(string text)
        {
            Objects.Add(new StringItem(text));
        }

        public void AddImage(MNReferencedImage ri)
        {
            MNLazyImage li = new MNLazyImage(Document);
            li.Image = ri;
            Objects.Add(new StringItem(li));
        }

        public override void ResetStatus()
        {
            MixObjects(false);

            base.ResetStatus();
        }

        public override void SetPropertyValue(string propertyName, string propertyValue)
        {
            switch (propertyName)
            {
                case "ending":
                    EndingChar = propertyValue;
                    break;
                case "initcap":
                    CapitalizeFirst = GSBoolean.StringToBool(propertyValue);
                    break;
                case "locklast":
                    LockLast = GSBoolean.StringToBool(propertyValue);
                    break;
                default:
                    base.SetPropertyValue(propertyName, propertyValue);
                    break;
            }
        }
    }
}
