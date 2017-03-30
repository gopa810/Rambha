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
    public class SMLabel : SMControl
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
            RichContent = true;
            RunningLine = SMRunningLine.Natural;
            richText = new SMRichText(this);
        }

        protected override GSCore ExecuteMessageSet(GSCore a1, GSCore a2, GSCoreCollection args)
        {
            switch (a1.getStringValue())
            {
                case "rich":
                    RichContent = a2.getBooleanValue();
                    break;
                default:
                    return base.ExecuteMessageSet(a1, a2, args);
            }
            return a2;
        }

        public override void StyleDidChange()
        {
            // we need to reformat text according new style
            //if (RichContent)
            if (richText != null)
            {
                //drawWords = SMWordToken.WordListFromString(Text, this);
                richText.ForceRecalc();
            }
        }

        public override void TextDidChange()
        {
            if (richText != null)
                richText.ForceRecalc();
        }

        public SMRichText richText = null;

        public const int PADDING_DOCK_TOP = 33;
        public const int PADDING_DOCK_BOTTOM = 44;
        public const int PADDING_DOCK_LEFT = 66;
        public const int PADDING_DOCK_RIGHT = 88;
        public override void Paint(MNPageContext context)
        {
            SMRectangleArea area = this.Area;
            Rectangle bounds = area.GetBounds(context);

            PrepareBrushesAndPens();

            Rectangle textBounds = ContentPadding.ApplyPadding(bounds);

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

            if (plainText.StartsWith("$"))
            {
                plainText = Document.ResolveProperty(plainText.Substring(1));
            }

            Font usedFont = GetUsedFont();

            if (plainText != null)
            {
                Rectangle r = Rectangle.Empty;
                Size textSize = richText.MeasureString(context, plainText, textBounds.Width);
                if (Dock == SMControlSelection.Top)
                {
                    r.X = 0;
                    r.Y = MNPage.HEADER_HEIGHT;
                    r.Height = textSize.Height + PADDING_DOCK_BOTTOM + PADDING_DOCK_TOP;
                    r.Width = context.PageWidth;
                    textBounds.X = PADDING_DOCK_LEFT;
                    textBounds.Y = r.Y + PADDING_DOCK_TOP;
                    context.g.FillRectangle(SMGraphics.GetBrush(Page.BackgroundColor), r);
                    richText.Paragraph.VertAlign = SMVerticalAlign.Top;
                    richText.DrawString(context, textBounds);
                    Area.RelativeArea = r;
                }
                else if (Dock == SMControlSelection.Bottom)
                {
                    r.Y = context.PageHeight - textSize.Height - PADDING_DOCK_TOP - PADDING_DOCK_BOTTOM;
                    r.X = 0;
                    r.Height = textSize.Height + PADDING_DOCK_BOTTOM + PADDING_DOCK_TOP;
                    r.Width = context.PageWidth;
                    textBounds.X = PADDING_DOCK_LEFT;
                    textBounds.Y = r.Y + PADDING_DOCK_TOP;
                    context.g.FillRectangle(SMGraphics.GetBrush(Page.BackgroundColor), r);
                    richText.Paragraph.VertAlign = SMVerticalAlign.Top;
                    richText.DrawString(context, textBounds);
                    Area.RelativeArea = r;
                }
                else
                {
                    DrawStyledBorder(context, bounds);

                    richText.DrawString(context, textBounds);
                }

                /*if (RichContent)
                {
                    if (!p_prevText.Equals(plainText) || drawWords == null)
                    {
                        p_prevText = plainText;
                        drawWords = SMWordToken.WordListFromString(plainText, this);
                    }
                    SMRichLayout lay = SMTextContainer.RecalculateWordsLayout(context, textBounds, drawWords, this,
                        RunningLine, -1);
                    foreach (SMWordBase wt in drawWords)
                    {
                        wt.Paint(context, textBounds.X, textBounds.Y);
                    }
                }
                else
                {
                    SizeF sf = context.g.MeasureString(plainText, usedFont);
                    if (!Autosize && Paragraph.SizeToFit)
                    {
                        float cx = Math.Max(sf.Width / textBounds.Width, sf.Height / textBounds.Height);
                        usedFont = (cx > 1f ? SMGraphics.GetFontVariation(usedFont, usedFont.Size / cx) : usedFont);
                    }

                    p_textSize = new Size((int)sf.Width, (int)sf.Height);

                    StringFormat format = Paragraph.GetAlignmentStringFormat();

                    context.g.DrawString(plainText, usedFont, tempForeBrush, textBounds, format);
                }*/
            }
            else if (runningText != null)
            {
                DrawStyledBorder(context, bounds);

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

        public override bool OnDropFinished(PVDragContext dc)
        {
            if (HasImmediateEvaluation)
            {
                if (!SafeTag.Equals(dc.draggedItem.Tag.ToLower()))
                {
                    return false;
                }
            }

            return base.OnDropFinished(dc);
        }

        public Rectangle FindOptimalSize(MNPageContext context, string str)
        {
            Rectangle t1, t2, t3, res;
            SMRichLayout lay = null;
            int s1, s2, s3, ses;
            double scrRatio = Convert.ToDouble(context.PageWidth) / context.PageHeight;
            double r1, r2, r3;
            Text = str;
            p_prevText = Text;
            drawWords = SMWordToken.WordListFromString(Text, this);
            if (drawWords.Count == 0)
                return Rectangle.Empty;

            t1 = new Rectangle(context.PageWidth/30, context.PageHeight/30, context.PageWidth*28/30, context.PageHeight*28/32);
            lay = SMTextContainer.RecalculateWordsLayout(context, t1, drawWords, this, RunningLine, -1);
            s1 = lay.bottomY - t1.Top;
            r1 = Math.Abs(Convert.ToDouble(t1.Width) / s1 - scrRatio);

            t2 = new Rectangle(context.PageWidth * 5 / 32, context.PageHeight * 5 / 32, context.PageWidth * 20 / 32, context.PageHeight * 20 / 32);
            lay = SMTextContainer.RecalculateWordsLayout(context, t2, drawWords, this, RunningLine, -1);
            s2 = lay.bottomY - t2.Top;
            r2 = Math.Abs(Convert.ToDouble(t2.Width) / s2 - scrRatio);

            t3 = new Rectangle(context.PageWidth / 4, context.PageHeight / 4, context.PageWidth / 2, context.PageHeight / 2);
            lay = SMTextContainer.RecalculateWordsLayout(context, t3, drawWords, this, RunningLine, -1);
            s3 = lay.bottomY - t3.Top;
            r3 = Math.Abs(Convert.ToDouble(t3.Width) / s3 - scrRatio);

            if (r1 < r2)
            {
                if (r1 < r3)
                {
                    res = t1;
                    ses = s1;
                }
                else
                {
                    res = t3;
                    ses = s3;
                }
            }
            else if (r2 < r3)
            {
                res = t2;
                ses = s2;
            }
            else
            {
                res = t3;
                ses = s3;
            }

            ses = Math.Abs(ses);

            res.Height = ses + ContentPadding.Top + ContentPadding.Bottom;
            res.Y = context.PageHeight / 2 - ses / 2 - ContentPadding.Top;
            res.X -= ContentPadding.Left;
            res.Width += ContentPadding.Left + ContentPadding.Right;

            return res;
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

    }
}
