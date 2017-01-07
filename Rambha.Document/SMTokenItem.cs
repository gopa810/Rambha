using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Script;

namespace Rambha.Document
{
    public class SMTokenItem: GSCore
    {
        public string Tag = "";
        public string Text = null;
        public Font TextFont = SystemFonts.DialogFont;
        public Brush TextBrush = Brushes.Black;
        public Size ContentSize = Size.Empty;
        public Image Image = null;
    }
}
