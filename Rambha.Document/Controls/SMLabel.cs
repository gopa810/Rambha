using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;

using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMLabel: SMControl
    {

        [Browsable(true), Category("Content")]
        public bool RichContent { get; set; }

        [Browsable(true), Category("Layout")]
        public SMRunningLine RunningLine { get; set; }

        private Size p_textSize = Size.Empty;

        private string p_prevText = "";
        private List<SMWordBase> drawWords = null;

        public SMLabel(MNPage p)
            : base(p)
        {
            Text = "Label";
            RichContent = false;
            RunningLine = SMRunningLine.Natural;
        }

        public override void Paint(MNPageContext context)
        {
            SMRectangleArea area = context.CurrentPage.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);

            PrepareBrushesAndPens();

            DrawStyledBorder(context, bounds);

            Rectangle textBounds = Style.ApplyPadding(bounds);

            if (Text != null && Text.Contains("\\n"))
                Text = Text.Replace("\\n", "\n");
            string plainText = Text;
            MNReferencedAudioText runningText = null;

            if (Content != null)
            {
                plainText = null;
                if (Content is MNReferencedText)
                    plainText = ((MNReferencedText)Content).Text;
                else if (Content is MNReferencedAudioText)
                    runningText = Content as MNReferencedAudioText;
                else if (Content is MNReferencedSound)
                    plainText = Text;
            }

            float fontSize = FontSize > 5 ? FontSize : Style.Font.Size;
            Font usedFont = GetUsedFont();

            if (plainText != null)
            {
                if (RichContent)
                {
                    if (!p_prevText.Equals(Text))
                    {
                        p_prevText = Text;
                        drawWords = SMWordToken.WordListFromString(Text, this);
                    }
                    int pages = 0;
                    SMTextContainer.RecalculateWordsLayout(context, textBounds, drawWords, this,
                        RunningLine, 1, 0, out pages);
                    foreach (SMWordBase wt in drawWords)
                    {
                        wt.Paint(context);
                    }
                }
                else
                {
                    SizeF sf = context.g.MeasureString(Text, usedFont);
                    if (Style.SizeToFit)
                    {
                        float cx = Math.Max(sf.Width / textBounds.Width, sf.Height / textBounds.Height);
                        usedFont = (cx > 1f ? SMGraphics.GetFontVariation(usedFont, usedFont.Size / cx) : usedFont);
                    }

                    p_textSize = new Size((int)sf.Width, (int)sf.Height);

                    StringFormat format = Style.GetAlignmentStringFormat();

                    context.g.DrawString(Text, usedFont, tempForeBrush, textBounds, format);
                }
            }
            else if (runningText != null)
            {
                Point curr = new Point(textBounds.Left, textBounds.Top);
                int index = 0;
                foreach (GOFRunningTextItem w in runningText.Words)
                {
                    Brush currBrush = (runningText.currentWord >= index ? Brushes.Red : tempForeBrush);
                    SizeF textSize = context.g.MeasureString(w.Text, usedFont);
                    if (curr.X + textSize.Width > textBounds.Right)
                    {
                        curr.X = textBounds.Left;
                        curr.Y += (int)textSize.Height;
                    }
                    context.g.DrawString(w.Text, usedFont, currBrush, curr);
                    curr.X += (int)textSize.Width;

                    /*textSize = context.g.MeasureString(" ", Style.Font);
                    if (curr.X + textSize.Width > textBounds.Right)
                    {
                        curr.X = textBounds.Left;
                        curr.Y += (int)textSize.Height;
                    }
                    else
                    {
                        curr.X += (int)textSize.Width;
                    }*/

                    index++;
                }
            }

            // draw selection marks
            base.Paint(context);
        }

        public override bool Load(RSFileReader br)
        {
            if (base.Load(br))
            {
                byte tag;
                while ((tag = br.ReadByte()) != 0)
                {
                    switch (tag)
                    {
                        case 10:
                            RunningLine = (SMRunningLine)br.ReadInt32();
                            break;
                        case 11:
                            RichContent = br.ReadBool();
                            break;
                        default:
                            return false;
                    }

                }

                return true;
            }

            return false;
        }


        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteInt32((int)RunningLine);

            bw.WriteByte(11);
            bw.WriteBool(RichContent);

            bw.WriteByte(0);
        }

        public override SMControl Duplicate()
        {
            SMLabel label = new SMLabel(Page);

            CopyContentTo(label);

            // copy label
            label.RichContent = this.RichContent;
            label.RunningLine = this.RunningLine;



            return label;
        }
    }
}
