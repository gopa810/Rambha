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
    public class SMLabel: SMControl
    {

        private Size p_textSize = Size.Empty;

        public SMLabel(MNPage p)
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

            SizeF sf = context.g.MeasureString(Text, Style.Font);
            Font usedFont = Style.Font;
            if (Style.SizeToFit)
            {
                float fontSize = Style.Font.Size;
                float cx = Math.Max(sf.Width / textBounds.Width, sf.Height / textBounds.Height);
                usedFont = (cx > 1f ? SMGraphics.GetFontVariation(Style.Font, Style.Font.Size / cx) : Style.Font);
            }

            p_textSize = new Size((int)sf.Width, (int)sf.Height);

            StringFormat format = Style.GetAlignmentStringFormat();

            context.g.DrawString(Text, usedFont, tempForeBrush, textBounds, format);

            // draw selection marks
            base.Paint(context);
        }

    }
}
