using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Rambha.Document
{
    public class SMGraphics
    {
        public static Dictionary<String,Font> FontDict = new Dictionary<string,Font>();
        public static Dictionary<Color, Brush> BrushDict = new Dictionary<Color, Brush>();
        public static Dictionary<String, Pen> PenDict = new Dictionary<string, Pen>();
        public static StringFormat StrFormatCenter;
        public static Font DefaultFont;
        public static string CustomFontsDir;

        public static Dictionary<string, Font> BuiltInFonts = new Dictionary<string, Font>();
        public static List<string> AddedFonts = new List<string>();
        public static StringFormat StrFormatLeftCenter;

        public static SMStatusLayout clickableLayoutN = new SMStatusLayout();
        public static SMStatusLayout clickableLayoutH = new SMStatusLayout();
        public static SMStatusLayout draggableLayoutN = new SMStatusLayout();
        public static SMStatusLayout draggableLayoutH = new SMStatusLayout();
        public static SMStatusLayout dropableLayoutN = new SMStatusLayout();
        public static SMStatusLayout dropableLayoutH = new SMStatusLayout();

        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
                                     string lpFileName);

        [DllImport("gdi32.dll", EntryPoint = "RemoveFontResourceW", SetLastError = true)]
        public static extern bool RemoveFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
                                     string lpFileName);

        static SMGraphics()
        {
            DefaultFont = new Font(FontFamily.GenericSansSerif, 18);

            StrFormatCenter = new StringFormat();
            StrFormatCenter.Alignment = StringAlignment.Center;
            StrFormatCenter.LineAlignment = StringAlignment.Center;

            StrFormatLeftCenter = new StringFormat();
            StrFormatLeftCenter.Alignment = StringAlignment.Near;
            StrFormatLeftCenter.LineAlignment = StringAlignment.Center;

            // clickable
            clickableLayoutN.BorderColor = Color.Brown;
            clickableLayoutN.BackColor = Color.NavajoWhite;
            clickableLayoutN.ForeColor = Color.Brown;
            clickableLayoutN.BorderWidth = 1;
            clickableLayoutN.BorderStyle = SMBorderStyle.RoundRectangle;

            clickableLayoutH.BorderColor = Color.Brown;
            clickableLayoutH.BackColor = Color.Brown;
            clickableLayoutH.ForeColor = Color.White;
            clickableLayoutH.BorderWidth = 1;
            clickableLayoutH.BorderStyle = SMBorderStyle.RoundRectangle;

            // draggable
            draggableLayoutN.BorderColor = Color.Black;
            draggableLayoutN.BackColor = Color.LightYellow;
            draggableLayoutN.ForeColor = Color.Black;
            draggableLayoutN.BorderWidth = 1;
            draggableLayoutN.BorderStyle = SMBorderStyle.RoundRectangle;

            draggableLayoutH.BorderColor = Color.Black;
            draggableLayoutH.BackColor = Color.Orange;
            draggableLayoutH.ForeColor = Color.Black;
            draggableLayoutH.BorderWidth = 1;
            draggableLayoutH.BorderStyle = SMBorderStyle.RoundRectangle;

            // dropable
            dropableLayoutN.BorderColor = Color.Yellow;
            dropableLayoutN.BackColor = Color.Transparent;
            dropableLayoutN.ForeColor = Color.Brown;
            dropableLayoutN.BorderWidth = 4;
            dropableLayoutN.BorderStyle = SMBorderStyle.RoundRectangle;

            dropableLayoutH.BorderColor = Color.Orange;
            dropableLayoutH.BackColor = Color.Transparent;
            dropableLayoutH.ForeColor = Color.Black;
            dropableLayoutH.BorderWidth = 4;
            dropableLayoutH.BorderStyle = SMBorderStyle.RoundRectangle;



            string fontsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fonts");
            CustomFontsDir = fontsDir;

            InstalledFontCollection installed = new InstalledFontCollection();
            FontFamily[] inst = installed.Families;

            InstallAppFont(inst, MNFontName.TektonPro, "Tekton Pro", new string[] { "TektonPro-Bold.otf" });
            InstallAppFont(inst, MNFontName.Shanti, "Shanti", new string[] { "SHANT_.TTF", "SHANT_B.TTF", "SHANT_BI.TTF", "SHANT_I.TTF" });
            //InstallAppFont(MNFontName.PenmanshipPrint, "Penmanship Print", new string[] { "PENMP___.TTF" });
            InstallAppFont(inst, MNFontName.Sabon, "Sabon", new string[] { "Sabon-Bold.otf", "Sabon-BoldItalic.otf", "Sabon-Italic.otf", "Sabon-Roman.otf" });
            InstallAppFont(inst, MNFontName.GilSansMurari, "Gill Sans MT Murari", new string[] { "GILMUBD.ttf", "GILMUBDI.ttf", "GILMUIT.ttf", "GILMURG.ttf" });
            InstallAppFont(inst, MNFontName.ChaparralPro, "Chaparral Pro", new string[] { "ChaparralPro-Bold.otf", "ChaparralPro-BoldIt.otf", "ChaparralPro-Italic.otf", "ChaparralPro-Regular.otf" });
            InstallAppFont(inst, MNFontName.BerlinSansFB, "Berlin Sans FB", new string[] { "BRLNSB.TTF", "BRLNSDB.TTF" });
            InstallAppFont(inst, MNFontName.AdobeGaramondPro, "Garamond", new string[] { "AGaramondPro-Bold.otf", "AGaramondPro-Italic.otf", "AGaramondPro-Regular.otf" });
            InstallAppFont(inst, MNFontName.VagRounded, "Arial Rounded MT Bold", new string[] { "vag_rounded_bold.ttf" });
            InstallAppFont(inst, MNFontName.Times, "Times New Roman", new string[] { });
            InstallAppFont(inst, MNFontName.LucidaSans, "Lucida Sans", new string[] { });

            BuiltInFonts[MNFontName.Devanagari] = DefaultFont;
            BuiltInFonts[MNFontName.FranklinGothicCondensed] = DefaultFont;
            BuiltInFonts[MNFontName.OddsAndSods] = DefaultFont;
            BuiltInFonts[MNFontName.Default] = DefaultFont;
            BuiltInFonts[MNFontName.WWDesigns] = DefaultFont;
            BuiltInFonts[MNFontName.Balaram] = DefaultFont;

        }

        public static FontFamily FindFontFamily(FontFamily[] inst, string familyFont)
        {
            foreach(FontFamily ff in inst)
            {
                if (ff.Name == familyFont)
                    return ff;
            }
            return null;
        }

        private static void InstallAppFont(FontFamily[] inst, string fontName, string familyName, string[] filesToAdd)
        {
            if (!BuiltInFonts.ContainsKey(fontName))
            {
                FontFamily family = FindFontFamily(inst, familyName);
                if (family != null)
                {
                    Font font = new Font(family, 14f, FontStyle.Regular);
                    BuiltInFonts[fontName] = font;
                }
            }
            if (!BuiltInFonts.ContainsKey(fontName))
            {
                foreach (string fileRaw in filesToAdd)
                {
                    string file = Path.Combine(CustomFontsDir, fileRaw);
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".ttf" || ext == ".otf")
                    {
                        int a = AddFontResource(file);
                        if (a > 0)
                        {
                            AddedFonts.Add(file);
                        }
                    }
                }

                try
                {
                    FontFamily family = new FontFamily(familyName);
                    Font font = new Font(family, 14f, FontStyle.Regular);
                    BuiltInFonts[fontName] = font;
                }
                catch
                {
                    BuiltInFonts.Remove(fontName);
                }
            }
            if (!BuiltInFonts.ContainsKey(fontName))
            {
                BuiltInFonts[fontName] = DefaultFont;
            }
        }

        public static void RemoveAddedFonts()
        {
            foreach(string af in AddedFonts)
            {
                RemoveFontResource(af);
            }
        }

        public static Font GetFont(string fontNameEnum)
        {
            return BuiltInFonts[fontNameEnum];
        }

        public static Font GetFontVariation(string fontName, float size)
        {
            return GetFontVariation(BuiltInFonts[fontName], size);
        }

        public static Font GetFontVariation(Font font, float size)
        {
            return GetFontVariation(font, size, font.Style);
        }

        public static Font GetFontVariation(string font, FontStyle style)
        {
            return GetFontVariation(BuiltInFonts[font], style);
        }

        public static Font GetFontVariation(Font font, FontStyle style)
        {
            return GetFontVariation(font, font.Size, style);
        }

        public static Font GetFontVariation(string fontName, float size, FontStyle style)
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

        public static string GetVirtFontCode(string fontName, float fontSize, bool Bold, bool Italic, bool Underline, bool Strikeout)
        {
            return string.Format("{0}-{1}-{2}{3}{4}{5}", fontName, fontSize, Bold ? 1 : 0, Italic ? 1 : 0, Underline ? 1 : 0, Strikeout ? 1 : 0);
        }

        public static SMFont GetVirtFontVariation(string fontName, float fontSize, bool Bold, bool Italic, bool Underline, bool Strikeout)
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

        public static Size RestrictSize(Size size, int dim)
        {
            if (size.Width > dim && size.Height > dim)
            {
                if (size.Width > size.Height)
                {
                    if (size.Width > dim)
                    {
                        return new Size(32, Convert.ToInt32(size.Height * 32.0 / size.Width));
                    }
                }
                else
                {
                    if (size.Height > 32)
                    {
                        return new Size(Convert.ToInt32(size.Width * 32.0 / size.Height), 32);
                    }
                }
            }

            return new Size(dim, dim);
        }
    }
}
