using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMStyle
    {
        [Browsable(false)]
        public long Id { get; set; }

        [Browsable(true),Category("Content")]
        public string Name { get; set; }

        [Browsable(true), Category("Text")]
        public Font Font { get; set; }

        [Browsable(true),Category("Normal Colors")]
        public Color BackColor { get; set; }

        [Browsable(true), Category("Normal Colors")]
        public Color ForeColor { get; set; }

        [Browsable(true), Category("Border")]
        public SMBorderStyle BorderStyle { get; set; }

        [Browsable(true), Category("Border")]
        public float BorderWidth { get; set; }

        [Browsable(true), Category("Border")]
        public Color BorderColor { get; set; }

        [Browsable(true), DisplayName("Align"),Category("Appearance")]
        public SMContentAlign ContentAlign { get; set; }

        [Browsable(true), Category("Highlight State")]
        public Color HighBackColor { get; set; }

        [Browsable(true), Category("Highlight State")]
        public Color HighForeColor { get; set; }

        [Browsable(true), Category("Highlight State")]
        public Color HighBorderColor { get; set; }

        [Browsable(true), Category("Highlight State")]
        public SMBorderStyle HighBorderStyle { get; set; }

        [Browsable(true), Category("Highlight State")]
        public float HighBorderWidth { get; set; }

        [Browsable(true),DisplayName("Padding"),Category("Appearance")]
        public SMContentPadding ContentPadding { get; set; }

        [Browsable(true), Category("Behavior")]
        public bool Clickable { get; set; }

        [Browsable(true), Category("Behavior")]
        public SMDragResponse Draggable { get; set; }

        [Browsable(true), Category("Behavior")]
        public SMDropResponse Droppable { get; set; }

        [Browsable(true), Category("Border")]
        public int CornerRadius { get; set; }

        [Browsable(true), Category("Appearance")]
        public bool SizeToFit { get; set; }

        public SMStyle()
        {
            Name = "";
            ContentPadding = new SMContentPadding();
            BorderWidth = 1f;
            HighBorderWidth = 1f;
            Draggable = SMDragResponse.None;
            CornerRadius = 5;
            SizeToFit = true;
            Clickable = false;
        }

        public SMStyle CreateCopy()
        {
            SMStyle ns = new SMStyle();

            ns.Name = Name;
            ns.Font = Font;
            ns.ForeColor = ForeColor;
            ns.BackColor = BackColor;
            ns.HighBackColor = HighBackColor;
            ns.HighForeColor = HighForeColor;
            ns.ContentAlign = ContentAlign;
            ns.BorderStyle = BorderStyle;
            ns.BorderWidth = BorderWidth;
            ns.HighBorderWidth = HighBorderWidth;

            return ns;
        }

        public override string ToString()
        {
            return Name;
        }

        public Rectangle ApplyPadding(Rectangle rect)
        {
            rect.X += ContentPadding.Left;
            rect.Y += ContentPadding.Top;
            rect.Width -= (ContentPadding.Left + ContentPadding.Right);
            rect.Height -= (ContentPadding.Top + ContentPadding.Bottom);
            return rect;
        }

        public StringFormat GetAlignmentStringFormat()
        {
            StringFormat format = new StringFormat();
            switch (ContentAlign)
            {
                case SMContentAlign.TopLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case SMContentAlign.TopCenter:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case SMContentAlign.TopRight:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case SMContentAlign.CenterLeft:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case SMContentAlign.Center:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case SMContentAlign.CenterRight:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case SMContentAlign.BottomLeft:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case SMContentAlign.BottomCenter:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case SMContentAlign.BottomRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Far;
                    break;
            }
            return format;
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10); bw.WriteInt64(Id);
            bw.WriteByte(11); bw.WriteString(Name);
            bw.WriteByte(12); bw.WriteFont(Font);
            bw.WriteByte(13); bw.WriteColor(BackColor);
            bw.WriteByte(14); bw.WriteColor(ForeColor);
            bw.WriteByte(15); bw.WriteInt32((int)BorderStyle);
            bw.WriteByte(16); bw.WriteInt32(Convert.ToInt32(BorderWidth * 100));
            bw.WriteByte(17); bw.WriteColor(BorderColor);
            bw.WriteByte(18); bw.WriteInt32((Int32)ContentAlign);
            bw.WriteByte(19); bw.WriteColor(HighBackColor);
            bw.WriteByte(20); bw.WriteColor(HighForeColor);
            bw.WriteByte(21); bw.WriteColor(HighBorderColor);
            bw.WriteByte(22); bw.WriteInt32((Int32)HighBorderStyle);
            bw.WriteByte(23); bw.WriteInt32(Convert.ToInt32(HighBorderWidth * 100));
            bw.WriteByte(24); ContentPadding.Save(bw);
            bw.WriteByte(25); bw.WriteBool(Clickable);
            bw.WriteByte(26); bw.WriteInt32((Int32)Draggable);
            bw.WriteByte(27); bw.WriteInt32((Int32)Droppable);
            bw.WriteByte(28); bw.WriteInt32((Int32)CornerRadius);
            bw.WriteByte(29); bw.WriteBool(SizeToFit);

            // end of object
            bw.WriteByte(0);
        }

        public bool Load(RSFileReader br)
        {
            byte tag;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 10: Id = br.ReadInt64(); break;
                    case 11: Name = br.ReadString(); break;
                    case 12: Font = br.ReadFont(); break;
                    case 13: BackColor = br.ReadColor(); break;
                    case 14: ForeColor = br.ReadColor(); break;
                    case 15: BorderStyle = (SMBorderStyle)br.ReadInt32(); break;
                    case 16: BorderWidth = br.ReadInt32() / 100f; break;
                    case 17: BorderColor = br.ReadColor(); break;
                    case 18: ContentAlign = (SMContentAlign)br.ReadInt32(); break;
                    case 19: HighBackColor = br.ReadColor(); break;
                    case 20: HighForeColor = br.ReadColor(); break;
                    case 21: HighBorderColor = br.ReadColor(); break;
                    case 22: HighBorderStyle = (SMBorderStyle)br.ReadInt32(); break;
                    case 23: HighBorderWidth = br.ReadInt32() / 100f; break;
                    case 24: ContentPadding.Load(br); break;
                    case 25: Clickable = br.ReadBool(); break;
                    case 26: Draggable = (SMDragResponse)br.ReadInt32(); break;
                    case 27: Droppable = (SMDropResponse)br.ReadInt32(); break;
                    case 28: CornerRadius = br.ReadInt32(); break;
                    case 29: SizeToFit = br.ReadBool(); break;
                    default: break;
                }
            }

            return true;
        }
    }
}
