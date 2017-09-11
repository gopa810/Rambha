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

    public enum MNFontName
    {
        AdobeGaramondPro = 1,
        Balaram = 2,
        BerlinSansFB = 3,
        ChaparralPro = 4,
        Devanagari = 5,
        FranklinGothicCondensed = 6,
        GilSansMurari = 7,
        LucidaSans = 8,
        OddsAndSods = 9,
        PenmanshipPrint = 10,
        Sabon = 11,
        Shanti = 12,
        TektonPro = 13,
        VagRounded = 14,
        WWDesigns = 15,
        Times = 16,
        Default = 17
    }

    public enum SMScreen
    {
        Screen_1024_768__4_3 = 0,
        Screen_1152_768__3_2 = 1,
        Screen_1376_774__16_9 = 2,
        Screen_768_1024__3_4 = 3
    }
}
