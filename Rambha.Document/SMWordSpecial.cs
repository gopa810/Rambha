using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMWordSpecial : SMWordBase
    {
        public SMWordSpecial(SMFont f)
            : base(f)
        {
        }

        public SMWordSpecialType Type { get; set; }

        public override void Paint(MNPageContext context, SMStatusLayout layout, int X, int Y)
        {
            if (Type == SMWordSpecialType.HorizontalLine)
            {
                context.g.DrawLine(SMGraphics.GetPen(layout.ForeColor, 1), rect.X + X, rect.Y + Y + rect.Height / 2, rect.Right + X, rect.Y + Y + rect.Height / 2);
            }
            base.Paint(context, layout, X, Y);
        }
    }

    public enum SMWordSpecialType
    {
        Newline = 0,
        HorizontalLine = 1,
        NewColumn = 2,
        NewPage = 3
    }


}
