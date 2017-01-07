using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMGraphics
    {
        public static Dictionary<String,Font> FontDict = new Dictionary<string,Font>();
        public static Dictionary<Color, Brush> BrushDict = new Dictionary<Color, Brush>();
        public static Dictionary<String, Pen> PenDict = new Dictionary<string, Pen>();

        public static Font GetFontVariation(Font font, float size)
        {
            string descr = String.Format("{0}_{1}_{2}", Convert.ToInt32(size), font.Style, font.FontFamily);
            if (FontDict.ContainsKey(descr))
                return FontDict[descr];
            Font ni = new Font(font.FontFamily, size, font.Style);
            FontDict.Add(descr, ni);
            return ni;
        }

        public static Brush GetBrush(Color c)
        {
            if (!BrushDict.ContainsKey(c))
            {
                BrushDict.Add(c, new SolidBrush(c));
            }
            return BrushDict[c];
        }


        public static Pen GetPen(Color color, float p)
        {
            string desc = string.Format("{0}-{1}-{2}-{3}-{4}", color.R, color.G, color.B, color.A, p);
            if (!PenDict.ContainsKey(desc))
            {
                PenDict.Add(desc, new Pen(color, p));
            }
            return PenDict[desc];
        }


    }
}
