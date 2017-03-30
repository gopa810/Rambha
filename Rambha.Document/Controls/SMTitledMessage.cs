using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;

using Rambha.Serializer;
using Rambha.Script;

namespace Rambha.Document
{
    public class SMTitledMessage: SMControl
    {
        private SMRichText titleRichText = null;
        private SMRichText messageRichText = null;

        public bool Visible { get; set; }

        public int CurrentTop { get; set; }

        public SMTitledMessage(MNPage p)
            : base(p)
        {
            Visible = true;
            titleRichText = new SMRichText();
            titleRichText.Font.Size = 17;
            titleRichText.Font.Name = MNFontName.ChaparralPro;
            titleRichText.NormalState.ForeColor = Color.White;

            messageRichText = new SMRichText(this);
            messageRichText.Font.Name = MNFontName.ChaparralPro;
            messageRichText.Font.Size = 14;
        }

        public override void StyleDidChange()
        {
            // we need to reformat text according new style
            messageRichText.ForceRecalc();
            titleRichText.ForceRecalc();
        }

        public override void Paint(MNPageContext context)
        {
            if (!Visible || context.CurrentPage == null)
                return;

            //Rectangle bounds = Area.GetBounds(context);
            Rectangle textBounds = new Rectangle(32, 0, context.PageWidth - 64, context.PageHeight);

            string plainText = context.CurrentPage.MessageText;
            string titleText = context.CurrentPage.MessageTitle;

            Font usedFont = GetUsedFont();
            int titleHeight = 0;
            int textHeight = 0;

            if (titleText != null)
            {
                Size size = titleRichText.MeasureString(context, titleText, textBounds.Width);
                titleHeight = size.Height;
            }

            if (plainText != null)
            {
                Size size = messageRichText.MeasureString(context, plainText, textBounds.Width);
                textHeight = size.Height;
            }

            int cy = context.PageHeight;

            if (textHeight > 0)
            {
                cy = cy - textHeight - 32;
                context.g.FillRectangle(Brushes.LightBlue, 0, cy, context.PageWidth, textHeight + 32);
                textBounds.Y = cy;
                textBounds.Height = textHeight + 32;
                messageRichText.DrawString(context, plainText, textBounds);
            }

            if (titleHeight > 0)
            {
                cy = cy - titleHeight - 16;
                context.g.FillRectangle(Brushes.CadetBlue, 0, cy, context.PageWidth, titleHeight + 16);
                textBounds.Y = cy;
                textBounds.Height = titleHeight + 16;
                titleRichText.DrawString(context, titleText, textBounds);
            }

            if (titleHeight + textHeight > 0)
            {
                cy -= 4;
                context.g.FillRectangle(Brushes.Black, 0, cy, context.PageWidth, 4);
            }

            CurrentTop = cy;
        }
    }
}
