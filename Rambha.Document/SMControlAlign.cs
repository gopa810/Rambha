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
        All = Top | Left | Right | Bottom,

        LeftBoundary = 1 << 4,
        TopBoundary = 1 << 5,
        RightBoundary = 1 << 6,
        BottomBoundary = 1 << 7,
        AnyBoundary = LeftBoundary | RightBoundary | TopBoundary | BottomBoundary
    }

    public enum SMContentAlign
    {
        TopLeft, TopCenter, TopRight,
        CenterLeft, Center, CenterRight,
        BottomLeft, BottomCenter, BottomRight
    }

    public enum SMVerticalAlign
    {
        Top = 0,
        Center = 1,
        Bottom = 2,
        Undefined = 3
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
            return "...";
        }

        public void Save(RSFileWriter bw)
        {
            bw.WriteInt32(Left);
            bw.WriteInt32(Top);
            bw.WriteInt32(Right);
            bw.WriteInt32(Bottom);
        }

        public void Load(RSFileReader br)
        {
            Left = br.ReadInt32();
            Top = br.ReadInt32();
            Right = br.ReadInt32();
            Bottom = br.ReadInt32();
        }
    }

    public enum SMContentScaling
    {
        Normal,
        Fit,
        Stretch
    }

    public enum SMLineStyle
    {
        None,
        Plain,
        Dashed,
        ZigZag,
        Doted
    }

    public enum SMBorderStyle
    {
        None,
        Rectangle,
        RoundRectangle,
        Elipse
    }

    public enum SMTextDirection
    {
        Vertical,
        Horizontal
    }

    public enum SMContentType
    {
        Undefined,
        Text,
        Audio,
        AudioText,
        Image
    }

    public enum SMDragResponse
    {
        Undef,
        None,
        Drag,
        Line
    }

    public enum SMDropResponse
    {
        Undef,
        None,
        One,
        Many
    }

    public enum SMRunningLine
    {
        SingleWord,
        Justify,
        Natural
    }

    public enum SMConnectionStyle
    {
        Invisible,
        DirectLine
    }
}
