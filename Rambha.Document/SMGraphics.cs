using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Diagnostics;

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


            InstalledFontCollection inst = new InstalledFontCollection();
            foreach (FontFamily ff in inst.Families)
            {
                Debugger.Log(0,"", string.Format("FontFamily: {0} \n", ff.Name));
                if (ff.Name.Equals("Tekton Pro"))
                    BuiltInFonts[MNFontName.TektonPro] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Shanti"))
                    BuiltInFonts[MNFontName.Shanti] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Penmanship Print"))
                    BuiltInFonts[MNFontName.PenmanshipPrint] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Sabon"))
                    BuiltInFonts[MNFontName.Sabon] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Gill Sans MT Murari"))
                    BuiltInFonts[MNFontName.GilSansMurari] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Chaparral Pro"))
                    BuiltInFonts[MNFontName.ChaparralPro] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Berlin Sans FB"))
                    BuiltInFonts[MNFontName.BerlinSansFB] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Balaram"))
                    BuiltInFonts[MNFontName.Balaram] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Garamond"))
                    BuiltInFonts[MNFontName.AdobeGaramondPro] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Arial Rounded MT Bold"))
                    BuiltInFonts[MNFontName.VagRounded] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Lucida Sans"))
                    BuiltInFonts[MNFontName.LucidaSans] = new Font(ff, 14f, FontStyle.Regular);
                if (ff.Name.Equals("Times New Roman"))
                    BuiltInFonts[MNFontName.Times] = new Font(ff, 14f, FontStyle.Regular);
            }

            Font defaultFont = BuiltInFonts[MNFontName.LucidaSans];
            BuiltInFonts[MNFontName.Devanagari] = defaultFont;
            BuiltInFonts[MNFontName.FranklinGothicCondensed] = defaultFont;
            BuiltInFonts[MNFontName.OddsAndSods] = defaultFont;
            BuiltInFonts[MNFontName.Default] = defaultFont;
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
            return GetFontVariation(font, size, font.Style);
        }

        public static Font GetFontVariation(MNFontName font, FontStyle style)
        {
            return GetFontVariation(BuiltInFonts[font], style);
        }

        public static Font GetFontVariation(Font font, FontStyle style)
        {
            return GetFontVariation(font, font.Size, style);
        }

        public static Font GetFontVariation(MNFontName fontName, float size, FontStyle style)
        {
            return GetFontVariation(BuiltInFonts[fontName], size, style);
        }

        public static Font GetFontVariation(Font font, float size, FontStyle style)
        {
            size = Math.Max(size, 5);
            string descr = String.Format("{0}_{1}_{2}", Convert.ToInt32(size * 10), style, font.FontFamily);
            if (FontDict.ContainsKey(descr))
                return FontDict[descr];
            Font ni = new Font(font.FontFamily, size, style);
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

        private static Dictionary<string, SMFont> p_virtual_fonts = new Dictionary<string, SMFont>();

        public static string GetVirtFontCode(MNFontName fontName, float fontSize, bool Bold, bool Italic, bool Underline, bool Strikeout)
        {
            return string.Format("{0}-{1}-{2}{3}{4}{5}", (int)fontName, fontSize, Bold ? 1 : 0, Italic ? 1 : 0, Underline ? 1 : 0, Strikeout ? 1 : 0);
        }

        public static SMFont GetVirtFontVariation(MNFontName fontName, float fontSize, bool Bold, bool Italic, bool Underline, bool Strikeout)
        {
            string code = GetVirtFontCode(fontName, fontSize, Bold, Italic, Underline, Strikeout);
            if (!p_virtual_fonts.ContainsKey(code))
            {
                SMFont sm = new SMFont();
                sm.Underline = Underline;
                sm.Size = fontSize;
                sm.Name = fontName;
                sm.Italic = Italic;
                sm.Bold = Bold;
                p_virtual_fonts[code] = sm;
            }

            return p_virtual_fonts[code];
        }
    }
}
