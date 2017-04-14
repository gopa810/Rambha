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
    public enum SMControlSelection
    {
        None = 0,
        Left = 1, 
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3,
        All = Top | Left | Right | Bottom
    }

    public enum SMVerticalAlign
    {
        Top = 0,
        Center = 1,
        Bottom = 2,
        Undefined = 3
    }

    public enum SMBackgroundType
    {
        None = 0,
        Solid = 1,
        Shadow = 2
    }

    public enum SMHorizontalAlign
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Justify = 3,
        Undefined = 4
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class SMParaFormat
    {
        [Browsable(true), Category("Appearance")]
        public bool SizeToFit { get; set; }

        [Browsable(true), Category("Appearance")]
        public float LineSpacing { get; set; }

        [Browsable(true), DisplayName("Vertical Align"), Category("Appearance")]
        public SMVerticalAlign VertAlign { get; set; }

        [Browsable(true), DisplayName("Horizontal Align"), Category("Appearance")]
        public SMHorizontalAlign Align { get; set; }

        public SMParaFormat()
        {
            VertAlign = SMVerticalAlign.Center;
            Align = SMHorizontalAlign.Left;
            LineSpacing = 1.2f;
            SizeToFit = true;
        }

        public override string ToString()
        {
            return Align.ToString() + ", " + VertAlign.ToString() + ", " + LineSpacing;
        }

        public void Set(SMParaFormat p)
        {
            this.SizeToFit = p.SizeToFit;
            this.LineSpacing = p.LineSpacing;
            this.VertAlign = p.VertAlign;
            this.Align = p.Align;
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10); bw.WriteBool(SizeToFit);
            bw.WriteByte(11); bw.WriteInt32((Int32)Align);
            bw.WriteByte(12); bw.WriteInt32((Int32)VertAlign);
            bw.WriteByte(13); bw.WriteFloat(LineSpacing);

            bw.WriteByte(0);
        }

        public void Load(RSFileReader br)
        {
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10: SizeToFit = br.ReadBool(); break;
                    case 11: Align = (SMHorizontalAlign)br.ReadInt32(); break;
                    case 12: VertAlign = (SMVerticalAlign)br.ReadInt32(); break;
                    case 13: LineSpacing = br.ReadFloat(); break;
                }
            }
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
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class SMFont
    {
        public MNFontName Name { get; set; }

        public bool Italic { get; set; }

        public bool Bold { get; set; }

        public bool Underline { get; set; }

        public float Size { get; set; }

        public SMFont()
        {
            Name = MNFontName.LucidaSans;
            Size = 14f;
            Italic = false;
            Bold = false;
            Underline = false;
        }

        public override string ToString()
        {
            return Name.ToString() + ", " + Size + "pt";
        }

        [Browsable(false)]
        public FontStyle Style
        {
            get
            {
                if (!Italic && !Bold && !Underline) return System.Drawing.FontStyle.Regular;
                return (Italic ? FontStyle.Italic : 0) | (Bold ? FontStyle.Bold : 0) | (Underline ? FontStyle.Underline : 0);
            }
            set
            {
                Italic = ((value & FontStyle.Italic) != 0);
                Bold = ((value & FontStyle.Bold) != 0);
                Underline = ((value & FontStyle.Underline) != 0);
            }
        }

        [Browsable(false)]
        public Font Font
        {
            get
            {
                return SMGraphics.GetFontVariation(Name, Size, Style);
            }
        }

        public void Set(SMFont f)
        {
            this.Bold = f.Bold;
            this.Italic = f.Italic;
            this.Underline = f.Underline;
            this.Name = f.Name;
            this.Size = f.Size;
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteInt32((Int32)Name);
            
            bw.WriteByte(11);
            bw.WriteFloat(Size);

            bw.WriteByte(13);
            bw.WriteBool(Italic);

            bw.WriteByte(14);
            bw.WriteBool(Bold);
            
            bw.WriteByte(15);
            bw.WriteBool(Underline);

            bw.WriteByte(0);
        }

        public void Load(RSFileReader br)
        {
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        Name = (MNFontName)br.ReadInt32();
                        break;
                    case 11:
                        Size = br.ReadFloat();
                        break;
                    case 13:
                        Italic = br.ReadBool();
                        break;
                    case 14:
                        Bold = br.ReadBool();
                        break;
                    case 15:
                        Underline = br.ReadBool();
                        break;
                }
            }
        }

    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class SMStatusLayout
    {
        [Browsable(true), Category("Highlight State")]
        public Color BackColor { get; set; }

        [Browsable(true), Category("Highlight State")]
        public Color ForeColor { get; set; }

        [Browsable(true), Category("Highlight State")]
        public Color BorderColor { get; set; }

        [Browsable(true), Category("Highlight State")]
        public SMBorderStyle BorderStyle { get; set; }

        [Browsable(true), Category("Highlight State")]
        public float BorderWidth { get; set; }

        [Browsable(true), Category("Border")]
        public int CornerRadius { get; set; }

        public SMStatusLayout()
        {
            BackColor = Color.White;
            ForeColor = Color.Black;
            BorderColor = Color.Black;
            BorderStyle = SMBorderStyle.None;
            BorderWidth = 1f;
            CornerRadius = 5;
        }

        public void Set(SMStatusLayout s)
        {
            BackColor = s.BackColor;
            ForeColor = s.ForeColor;
            BorderColor = s.BorderColor;
            BorderStyle = s.BorderStyle;
            BorderWidth = s.BorderWidth;
            CornerRadius = s.CornerRadius;
        }

        public override string ToString()
        {
            return "Type:" + BorderStyle.ToString() + " Width:" + BorderWidth;
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10); bw.WriteColor(BackColor);
            bw.WriteByte(11); bw.WriteColor(ForeColor);
            bw.WriteByte(12); bw.WriteInt32((int)BorderStyle);
            bw.WriteByte(13); bw.WriteInt32(Convert.ToInt32(BorderWidth * 100));
            bw.WriteByte(14); bw.WriteColor(BorderColor);
            bw.WriteByte(15); bw.WriteInt32(CornerRadius);

            bw.WriteByte(0);
        }

        public void Load(RSFileReader br)
        {
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10: BackColor = br.ReadColor(); break;
                    case 11: ForeColor = br.ReadColor(); break;
                    case 12: BorderStyle = (SMBorderStyle)br.ReadInt32(); break;
                    case 13: BorderWidth = br.ReadInt32() / 100f; break;
                    case 14: BorderColor = br.ReadColor(); break;
                    case 15: CornerRadius = br.ReadInt32(); break;
                }
            }
        }

    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class SMContentPadding
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public string All
        {
            get
            {
                if (Top == Bottom && Top == Left && Top == Right)
                    return Top.ToString();
                else
                    return "...";
            }
            set
            {
                int a = 0;
                if (int.TryParse(value, out a))
                {
                    Top = Bottom = Right = Left = a;
                }
            }
        }

        public SMContentPadding()
        {
            Top = 0;
            Bottom = 0;
            Left = 0;
            Right = 0;
        }

        public override string ToString()
        {
            return All;
        }

        public void Set(SMContentPadding p)
        {
            Top = p.Top;
            Bottom = p.Bottom;
            Left = p.Left;
            Right = p.Right;
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteByte(10);
            bw.WriteInt32(Left);
            
            bw.WriteByte(11);
            bw.WriteInt32(Top);
            
            bw.WriteByte(12);
            bw.WriteInt32(Right);

            bw.WriteByte(13);
            bw.WriteInt32(Bottom);

            bw.WriteByte(0);
        }

        public void Load(RSFileReader br)
        {
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 10:
                        Left = br.ReadInt32();
                        break;
                    case 11:
                        Top = br.ReadInt32();
                        break;
                    case 12:
                        Right = br.ReadInt32();
                        break;
                    case 13:
                        Bottom = br.ReadInt32();
                        break;
                }
            }
        }

        public Rectangle ApplyPadding(Rectangle rect)
        {
            rect.X += Left;
            rect.Y += Top;
            rect.Width -= (Left + Right);
            rect.Height -= (Top + Bottom);
            return rect;
        }

    }

    public enum SMContentScaling
    {
        Normal = 0,
        Fit = 1,
        Stretch = 2,
        Fill = 3
    }

    public enum SMContentArangement
    {
        TextOnly = 0,
        ImageOnly = 1,
        ImageAbove = 2,
        ImageBelow = 3,
        ImageOnLeft = 4,
        ImageOnRight = 5
    }

    public enum SMLineStyle
    {
        None = 0,
        Plain = 1,
        Dashed = 2,
        ZigZag = 3,
        Doted = 4
    }

    public enum SMBorderStyle
    {
        None = 0,
        Rectangle = 1,
        RoundRectangle = 2,
        Elipse = 3
    }

    public enum SMTextDirection
    {
        Vertical = 0,
        Horizontal = 1
    }

    public enum SMContentType
    {
        Undefined = 0,
        Text = 1,
        Audio = 2,
        AudioText = 3,
        Image = 4
    }

    public enum SMDragResponse
    {
        Undef = 0,
        None = 1,
        Drag = 2,
        Line = 3
    }

    public enum SMConnectionCardinality
    {
        Undef = 0,
        None = 1,
        One = 2,
        Many = 3
    }

    public enum SMRunningLine
    {
        SingleWord = 0,
        Justify = 1,
        Natural = 2
    }

    public enum SMConnectionStyle
    {
        Invisible = 0,
        DirectLine = 1
    }
}
