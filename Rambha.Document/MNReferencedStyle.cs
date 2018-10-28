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
        public SMFont Font { get; set; }

        /*[Browsable(true), Category("Text")]
        public MNFontName FontName { get; set; }

        [Browsable(true), Category("Text")]
        public bool Italic { get; set; }

        [Browsable(true), Category("Text")]
        public bool Bold { get; set; }

        [Browsable(true), Category("Text")]
        public bool Underline { get; set; }

        [Browsable(true), Category("Text")]
        public float FontSize { get; set; }*/

        /*[Browsable(true),Category("Normal Colors")]
        public Color BackColor { get; set; }
        [Browsable(true), Category("Normal Colors")]
        public Color ForeColor { get; set; }
        [Browsable(true), Category("Border")]
        public SMBorderStyle BorderStyle { get; set; }
        [Browsable(true), Category("Border")]
        public float BorderWidth { get; set; }
        [Browsable(true), Category("Border")]
        public Color BorderColor { get; set; }*/
        /*
        [Browsable(true), DisplayName("Vertical Align"), Category("Appearance")]
        public SMVerticalAlign VertAlign { get; set; }

        [Browsable(true), DisplayName("Horizontal Align"), Category("Appearance")]
        public SMHorizontalAlign Align { get; set; }*/

        [Browsable(true), DisplayName("Normal State")]
        public SMStatusLayout NormalState { get; set; }

        [Browsable(true), DisplayName("Highlight State")]
        public SMStatusLayout HighlightState { get; set; }

/*        [Browsable(true), Category("Highlight State")]
        public Color HighBackColor { get; set; }

        [Browsable(true), Category("Highlight State")]
        public Color HighForeColor { get; set; }

        [Browsable(true), Category("Highlight State")]
        public Color HighBorderColor { get; set; }

        [Browsable(true), Category("Highlight State")]
        public SMBorderStyle HighBorderStyle { get; set; }

        [Browsable(true), Category("Highlight State")]
        public float HighBorderWidth { get; set; }*/


        [Browsable(true), DisplayName("Paragraph Formating")]
        public SMParaFormat Paragraph { get; set; }

        [Browsable(true),DisplayName("Padding"),Category("Appearance")]
        public SMContentPadding ContentPadding { get; set; }

        /*
        [Browsable(true), Category("Appearance")]
        public bool SizeToFit { get; set; }

        [Browsable(true), Category("Appearance")]
        public float LineSpacing { get; set; }*/

        public MNReferencedStyle()
        {
            Name = "";
            Font = new SMFont();
            ContentPadding = new SMContentPadding();
            Paragraph = new SMParaFormat();
            HighlightState = new SMStatusLayout();
            NormalState = new SMStatusLayout();
        }

        public MNReferencedStyle CreateCopy()
        {
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    RSFileWriter fw = new RSFileWriter(bw);
                    this.Save(fw);
                }
                data = ms.GetBuffer();
            }

            using (MemoryStream ms = new MemoryStream(data))
            {
                MNReferencedStyle ns = new MNReferencedStyle();
                using (BinaryReader br = new BinaryReader(ms))
                {
                    RSFileReader fr = new RSFileReader(br);
                    ns.Load(fr);
                }

                ns.Name += " (copy)";
                return ns;
            }
        }

        public override string ToString()
        {
            return Name;
        }




        public override void Save(RSFileWriter bw)
        {
            bw.WriteByte(11); 
            bw.WriteString(Name);

            bw.WriteByte(35);
            ContentPadding.Save(bw);

            bw.WriteByte(36);
            NormalState.Save(bw);

            bw.WriteByte(37);
            HighlightState.Save(bw);

            bw.WriteByte(38);
            Paragraph.Save(bw);

            bw.WriteByte(39);
            Font.Save(bw);

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
                        Font.Name = MNFontName.IntToString(br.ReadInt32());
                        Font.Size = br.ReadFloat();
                        Font.Style = (System.Drawing.FontStyle)br.ReadInt32();
                        break;
                    case 13: NormalState.BackColor = br.ReadColor(); break;
                    case 14: NormalState.ForeColor = br.ReadColor(); break;
                    case 15: NormalState.BorderStyle = (SMBorderStyle)br.ReadInt32(); break;
                    case 16: NormalState.BorderWidth = br.ReadInt32() / 100f; break;
                    case 17: NormalState.BorderColor = br.ReadColor(); break;
                    case 19: HighlightState.BackColor = br.ReadColor(); break;
                    case 20: HighlightState.ForeColor = br.ReadColor(); break;
                    case 21: HighlightState.BorderColor = br.ReadColor(); break;
                    case 22: HighlightState.BorderStyle = (SMBorderStyle)br.ReadInt32(); break;
                    case 23: HighlightState.BorderWidth = br.ReadInt32() / 100f; break;
                    case 24:
                        ContentPadding.Left = br.ReadInt32();
                        ContentPadding.Top = br.ReadInt32();
                        ContentPadding.Right = br.ReadInt32();
                        ContentPadding.Bottom = br.ReadInt32();
                        break;
                    case 28: NormalState.CornerRadius = br.ReadInt32(); break;
                    case 29: Paragraph.SizeToFit = br.ReadBool(); break;
                    case 30: Paragraph.Align = (SMHorizontalAlign)br.ReadInt32(); break;
                    case 31: Paragraph.VertAlign = (SMVerticalAlign)br.ReadInt32(); break;
                    case 32: Paragraph.LineSpacing = br.ReadFloat(); break;
                    case 33: HighlightState.CornerRadius = br.ReadInt32();
                        NormalState.CornerRadius = HighlightState.CornerRadius;
                        break;

                    case 35:
                        ContentPadding.Load(br);
                        break;
                    case 36:
                        NormalState.Load(br);
                        break;
                    case 37:
                        HighlightState.Load(br);
                        break;
                    case 38:
                        Paragraph.Load(br);
                        break;
                    case 39:
                        Font.Load(br);
                        break;

                    default: break;
                }
            }
        }
    }
}
