using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Document
{
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
        Circle
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
}
