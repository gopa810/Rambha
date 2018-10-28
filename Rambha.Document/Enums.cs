using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Document
{
    public enum Bool3
    {
        False = 0,
        True = 1,
        Undef = 2,
        Both = 3,
    }

    public enum PageEditDisplaySize
    {
        LandscapeBig = 0,   // 0,0 
        PortaitBig = 1,     // 1,0
        LandscapeSmall = 2, // 0,1
        PortaitSmall = 3,   // 1,1
    }

    public enum MNRefSpotShape
    {
        Rectangle,
        Circle,
        Triangle
    }

    public class MNFontName
    {
        public static string IntToString(int fontId)
        {
            switch (fontId)
            {
                case 1: return "Garamond";
                case 2: return "Balaram";
                case 3: return "Berlin";
                case 4: return "Chaparral";
                case 5: return "Devanagari";
                case 6: return "Franklin";
                case 7: return "GilSans";
                case 8: return "Lucida";
                case 9: return "Odds";
                case 10: return "Penmanship";
                case 11: return "Sabon";
                case 12: return "Shanti";
                case 13: return "Tekton";
                case 14: return "VagRounded";
                case 15: return "WW";
                case 16: return "Times";
            }
            return "GilSans";
        }

        public const string AdobeGaramondPro = "Garamond";
        public const string Balaram = "Balaram";
        public const string BerlinSansFB = "Berlin";
        public const string ChaparralPro = "Chaparral";
        public const string Devanagari = "Devanagari";
        public const string FranklinGothicCondensed = "Franklin";
        public const string GilSansMurari = "GilSans";
        public const string LucidaSans = "Lucida";
        public const string OddsAndSods = "Odds";
        public const string PenmanshipPrint = "Penmanship";
        public const string Sabon = "Sabon";
        public const string Shanti = "Shanti";
        public const string TektonPro = "Tekton";
        public const string VagRounded = "VagRounded";
        public const string WWDesigns = "WW";
        public const string Times = "Times";
        public const string Default = "Default";
    }

    public enum SMScreen
    {
        Screen_1024_768__4_3 = 0,
        Screen_1152_768__3_2 = 1,
        Screen_1376_774__16_9 = 2,
        Screen_768_1024__3_4 = 3
    }
}
