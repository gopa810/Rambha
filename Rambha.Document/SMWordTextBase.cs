using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMWordTextBase : SMWordBase
    {
        private Brush textBrush = null;

        public Font WinFont
        {
            get
            {
                Font f = Font.Font;
                return f;
            }
        }

        public Brush TextBrush
        {
            set
            {
                textBrush = value;
            }
        }

        public override Brush GetCurrentTextBrush(SMStatusLayout layout)
        {
            if (textBrush != null)
                return textBrush;
            return base.GetCurrentTextBrush(layout);
        }

        public SMWordTextBase(SMFont f)
            : base(f)
        {
        }
    }

}
