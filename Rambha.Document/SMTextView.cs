using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;

namespace Rambha.Document
{
    public class SMTextView: SMControl
    {
        private Size p_textSize = Size.Empty;

        public SMTextView(MNPage p)
            : base(p)
        {
            Text = "Label";
        }

        public override void Paint(MNPageContext context)
        {
            SMRectangleArea area = context.CurrentPage.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);

            PrepareBrushesAndPens();
            DrawStyledBorder(context, bounds);

            Rectangle textBounds = Style.ApplyPadding(bounds);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
            format.Trimming = StringTrimming.EllipsisWord;

            context.g.DrawString(Text, Style.Font, tempForeBrush, textBounds, format);

            // draw selection marks
            base.Paint(context);
        }

    }
}
