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
        public static StringFormat StrFormatCenter;

        public static Dictionary<MNFontName, Font> BuiltInFonts = new Dictionary<MNFontName, Font>();
        public static StringFormat StrFormatLeftCenter;

        static SMGraphics()
        {
            StrFormatCenter = new StringFormat();
            StrFormatCenter.Alignment = StringAlignment.Center;
            StrFormatCenter.LineAlignment = StringAlignment.Center;

            StrFormatLeftCenter = new StringFormat();
            StrFormatLeftCenter.Alignment = StringAlignment.Near;
            StrFormatLeftCenter.LineAlignment = StringAlignment.Center;

            Font defaultFont = new Font(FontFamily.GenericSansSerif, 14f, FontStyle.Regular);

            BuiltInFonts[MNFontName.AdobeGaramondPro] = defaultFont;
            BuiltInFonts[MNFontName.Balaram] = defaultFont;
            BuiltInFonts[MNFontName.BerlinSansFB] = defaultFont;
            BuiltInFonts[MNFontName.ChaparralPro] = defaultFont;
            BuiltInFonts[MNFontName.Devanagari] = defaultFont;
            BuiltInFonts[MNFontName.FranklinGothicCondensed] = defaultFont;
            BuiltInFonts[MNFontName.GilSansMurari] = defaultFont;
            BuiltInFonts[MNFontName.LucidaSans] = defaultFont;
            BuiltInFonts[MNFontName.OddsAndSods] = defaultFont;
            BuiltInFonts[MNFontName.PenmanshipPrint] = defaultFont;
            BuiltInFonts[MNFontName.Sabon] = defaultFont;
            BuiltInFonts[MNFontName.Shanti] = defaultFont;
            BuiltInFonts[MNFontName.TektonPro] = defaultFont;
            BuiltInFonts[MNFontName.VagRounded] = defaultFont;
            BuiltInFonts[MNFontName.Times] = new Font("Times New Roman", 14f, FontStyle.Regular);
            BuiltInFonts[MNFontName.Default] = BuiltInFonts[MNFontName.LucidaSans];
            BuiltInFonts[MNFontName.WWDesigns] = defaultFont;
        }

        public static Font GetFont(MNFontName fontNameEnum)
        {
            return BuiltInFonts[fontNameEnum];
        }

        public static Font GetFontVariation(MNFontName fontName, float size)
        {
            return GetFontVariation(BuiltInFonts[fontName], size);
        }

        public static Font GetFontVariation(Font font, float size)
        {
            string descr = String.Format("{0}_{1}_{2}", Convert.ToInt32(size * 10), font.Style, font.FontFamily);
            if (FontDict.ContainsKey(descr))
                return FontDict[descr];
            Font ni = new Font(font.FontFamily, size, font.Style);
            FontDict.Add(descr, ni);
            return ni;
        }

        public static Font GetFontVariation(MNFontName font, FontStyle style)
        {
            return GetFontVariation(BuiltInFonts[font], style);
        }

        public static Font GetFontVariation(Font font, FontStyle style)
        {
            string descr = String.Format("{0}_{1}_{2}", Convert.ToInt32(font.Size * 10), style, font.FontFamily);
            if (FontDict.ContainsKey(descr))
                return FontDict[descr];
            Font ni = new Font(font.FontFamily, font.Size, style);
            FontDict.Add(descr, ni);
            return ni;
        }

        public static Font GetFontVariation(MNFontName fontName, float size, FontStyle style)
        {
            return GetFontVariation(BuiltInFonts[fontName], size, style);
        }

        public static Font GetFontVariation(Font font, float size, FontStyle style)
        {
            string descr = String.Format("{0}_{1}_{2}", Convert.ToInt32(size * 10), style, font.FontFamily);
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

        public static Size GetMaximumSize(Rectangle bounds, Size imageSize)
        {
            double d = Math.Min(Convert.ToDouble(bounds.Width)/imageSize.Width, Convert.ToDouble(bounds.Height)/imageSize.Height);
            return new Size(Convert.ToInt32(d*imageSize.Width), Convert.ToInt32(d*imageSize.Height));
        }
    }
}
