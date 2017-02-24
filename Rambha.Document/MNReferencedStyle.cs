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
    public class MNReferencedStyle: MNReferencedCore
    {
        [Browsable(true), Category("Text")]
        public MNFontName FontName { get; set; }

        [Browsable(true), Category("Text")]
        public FontStyle FontStyle { get; set; }
        
        [Browsable(true), Category("Text")]
        public float FontSize { get; set; }

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

        [Browsable(true), DisplayName("Vertical Align"), Category("Appearance")]
        public SMVerticalAlign VertAlign { get; set; }

        [Browsable(true), DisplayName("Horizontal Align"), Category("Appearance")]
        public SMHorizontalAlign Align { get; set; }

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

        [Browsable(true), Category("Border")]
        public int CornerRadius { get; set; }

        [Browsable(true), Category("Appearance")]
        public bool SizeToFit { get; set; }

        public MNReferencedStyle()
        {
            Name = "";
            FontName = MNFontName.LucidaSans;
            FontSize = 14f;
            FontStyle = System.Drawing.FontStyle.Regular;
            ContentPadding = new SMContentPadding();
            BorderWidth = 1f;
            HighBorderWidth = 1f;
            CornerRadius = 5;
            SizeToFit = true;
        }

        public MNReferencedStyle CreateCopy()
        {
            MNReferencedStyle ns = new MNReferencedStyle();

            ns.Name = Name;
            ns.FontName = FontName;
            ns.FontStyle = FontStyle;
            ns.FontSize = FontSize;
            ns.ForeColor = ForeColor;
            ns.BackColor = BackColor;
            ns.HighBackColor = HighBackColor;
            ns.HighForeColor = HighForeColor;
            ns.VertAlign = VertAlign;
            ns.Align = Align;
            ns.BorderStyle = BorderStyle;
            ns.BorderWidth = BorderWidth;
            ns.HighBorderWidth = HighBorderWidth;

            return ns;
        }

        public override string ToString()
        {
            return Name;
        }

        public Font Font
        {
            get
            {
                return SMGraphics.GetFontVariation(FontName, FontSize, FontStyle);
            }
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
            switch (Align)
            {
                case SMHorizontalAlign.Left:
                    format.Alignment = StringAlignment.Near;
                    break;
                case SMHorizontalAlign.Center:
                case SMHorizontalAlign.Justify:
                    format.Alignment = StringAlignment.Center;
                    break;
                case SMHorizontalAlign.Right:
                    format.Alignment = StringAlignment.Far;
                    break;
            }
            switch (VertAlign)
            {
                case SMVerticalAlign.Top:
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case SMVerticalAlign.Center:
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case SMVerticalAlign.Bottom:
                    format.LineAlignment = StringAlignment.Far;
                    break;
            }
            return format;
        }

        public override void Save(RSFileWriter bw)
        {
            bw.WriteByte(11); bw.WriteString(Name);
            bw.WriteByte(12); 
            bw.WriteInt32((Int32)FontName);
            bw.WriteFloat(FontSize);
            bw.WriteInt32((Int32)FontStyle);
            bw.WriteByte(13); bw.WriteColor(BackColor);
            bw.WriteByte(14); bw.WriteColor(ForeColor);
            bw.WriteByte(15); bw.WriteInt32((int)BorderStyle);
            bw.WriteByte(16); bw.WriteInt32(Convert.ToInt32(BorderWidth * 100));
            bw.WriteByte(17); bw.WriteColor(BorderColor);
            bw.WriteByte(19); bw.WriteColor(HighBackColor);
            bw.WriteByte(20); bw.WriteColor(HighForeColor);
            bw.WriteByte(21); bw.WriteColor(HighBorderColor);
            bw.WriteByte(22); bw.WriteInt32((Int32)HighBorderStyle);
            bw.WriteByte(23); bw.WriteInt32(Convert.ToInt32(HighBorderWidth * 100));
            bw.WriteByte(24); ContentPadding.Save(bw);
            bw.WriteByte(28); bw.WriteInt32((Int32)CornerRadius);
            bw.WriteByte(29); bw.WriteBool(SizeToFit);
            bw.WriteByte(30); bw.WriteInt32((Int32)Align);
            bw.WriteByte(31); bw.WriteInt32((Int32)VertAlign);

            // end of object
            bw.WriteByte(0);
        }

        public override void Load(RSFileReader br)
        {
            byte tag;
            while ((tag = br.ReadByte()) != 0)
            {
                switch (tag)
                {
                    case 11: Name = br.ReadString(); break;
                    case 12:
                        FontName = (MNFontName)br.ReadInt32();
                        FontSize = br.ReadFloat();
                        FontStyle = (System.Drawing.FontStyle)br.ReadInt32();
                        break;
                    case 13: BackColor = br.ReadColor(); break;
                    case 14: ForeColor = br.ReadColor(); break;
                    case 15: BorderStyle = (SMBorderStyle)br.ReadInt32(); break;
                    case 16: BorderWidth = br.ReadInt32() / 100f; break;
                    case 17: BorderColor = br.ReadColor(); break;
                    case 19: HighBackColor = br.ReadColor(); break;
                    case 20: HighForeColor = br.ReadColor(); break;
                    case 21: HighBorderColor = br.ReadColor(); break;
                    case 22: HighBorderStyle = (SMBorderStyle)br.ReadInt32(); break;
                    case 23: HighBorderWidth = br.ReadInt32() / 100f; break;
                    case 24: ContentPadding.Load(br); break;
                    case 28: CornerRadius = br.ReadInt32(); break;
                    case 29: SizeToFit = br.ReadBool(); break;
                    case 30: Align = (SMHorizontalAlign)br.ReadInt32(); break;
                    case 31: VertAlign = (SMVerticalAlign)br.ReadInt32(); break;
                    default: break;
                }
            }
        }
    }
}
