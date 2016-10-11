using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SlideMaker.Document
{
    public enum SMControlSelection
    {
        None = 0x0000, All = 0x1111,
        TopLeft = 0x1100, TopCenter = 0x0100, TopRight = 0x0110,
        CenterLeft = 0x1000, Center = 0x2222, CenterRight = 0x0010,
        BottomLeft = 0x1001, BottomCenter = 0x0001, BottomRight = 0x0011
    }
}
