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

        public bool SwitchStatus = false;

        public SMLabel(MNPage p)
            : base(p)
        {
            Text = "Label";
            RichContent = true;
            RunningLine = SMRunningLine.Natural;
            richText = new SMRichText(this);
            BackType = SMBackgroundType.None;
        }

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            if (token.Equals("toogleCheck"))
            {
                SwitchStatus = !SwitchStatus;
            }
            return base.ExecuteMessage(token, args);
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
                BackgroundImage = null;
            }
        }

        public override void TextDidChange()
        {
            if (richText != null)
            {
                richText.ForceRecalc();
                BackgroundImage = null;
            }
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

            SMConnection conn = context.CurrentPage.FindConnection(this);
            if (conn != null)
                UIStateHover = true;


            bool b = UIStateHover;
            UIStateHover |= SwitchStatus;
            SMStatusLayout layout = PrepareBrushesAndPens();
            UIStateHover = b;

            Rectangle boundsA = bounds;
            boundsA.Y = Math.Max(0, bounds.Top);
            boundsA.X = Math.Max(0, bounds.Left);
            boundsA.Width = Math.Min(1024,bounds.Right);
            boundsA.Height = Math.Min(768,bounds.Bottom);
            boundsA.Width -= boundsA.X;
            boundsA.Height -= boundsA.Y;

            Rectangle textBounds = ContentPadding.ApplyPadding(boundsA);

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
                    if (BackType == SMBackgroundType.Solid)
                    {
                        context.g.FillRectangle(SMGraphics.GetBrush(Page.BackgroundColor), r);
                    }
                    else if (BackType == SMBackgroundType.Shadow && BackgroundImage != null)
                    {
                        context.g.DrawImage(BackgroundImage, 
                            textBounds.X + BackgroundImageOffset.X, 
                            textBounds.Y + BackgroundImageOffset.Y);
                    }
                    richText.Paragraph.VertAlign = SMVerticalAlign.Top;
                    richText.DrawString(context, layout, textBounds);
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
                    if (BackType == SMBackgroundType.Solid)
                    {
                        context.g.FillRectangle(SMGraphics.GetBrush(Page.BackgroundColor), r);
                    }
                    else if (BackType == SMBackgroundType.Shadow && BackgroundImage != null)
                    {
                        context.g.DrawImage(BackgroundImage,
                            textBounds.X + BackgroundImageOffset.X,
                            textBounds.Y + BackgroundImageOffset.Y);
                    }
                    richText.Paragraph.VertAlign = SMVerticalAlign.Top;
                    richText.DrawString(context, layout, textBounds);
                    Area.RelativeArea = r;
                }
                else if (Dock == SMControlSelection.Right)
                {
                    r.X = context.PageWidth - textSize.Width - PADDING_DOCK_LEFT - PADDING_DOCK_RIGHT;
                    r.Y = MNPage.HEADER_HEIGHT;
                    r.Height = context.PageHeight - MNPage.HEADER_HEIGHT;
                    r.Width = textSize.Width + PADDING_DOCK_RIGHT + PADDING_DOCK_LEFT;
                    textBounds.X = r.X + PADDING_DOCK_LEFT;
                    textBounds.Y = r.Y + PADDING_DOCK_TOP;
                    if (BackType == SMBackgroundType.Solid)
                    {
                        context.g.FillRectangle(SMGraphics.GetBrush(Page.BackgroundColor), r);
                    }
                    else if (BackType == SMBackgroundType.Shadow && BackgroundImage != null)
                    {
                        context.g.DrawImage(BackgroundImage,
                            textBounds.X + BackgroundImageOffset.X,
                            textBounds.Y + BackgroundImageOffset.Y);
                    }
                    richText.Paragraph.VertAlign = SMVerticalAlign.Top;
                    richText.DrawString(context, layout, textBounds);
                    Area.RelativeArea = r;
                }
                else if (Dock == SMControlSelection.Left)
                {
                    r.X = 0;
                    r.Y = MNPage.HEADER_HEIGHT;
                    r.Height = context.PageHeight - MNPage.HEADER_HEIGHT;
                    r.Width = textSize.Width + PADDING_DOCK_RIGHT + PADDING_DOCK_LEFT;
                    textBounds.X = r.X + PADDING_DOCK_LEFT;
                    textBounds.Y = r.Y + PADDING_DOCK_TOP;
                    if (BackType == SMBackgroundType.Solid)
                    {
                        context.g.FillRectangle(SMGraphics.GetBrush(Page.BackgroundColor), r);
                    }
                    else if (BackType == SMBackgroundType.Shadow && BackgroundImage != null)
                    {
                        context.g.DrawImage(BackgroundImage,
                            textBounds.X + BackgroundImageOffset.X,
                            textBounds.Y + BackgroundImageOffset.Y);
                    }
                    richText.Paragraph.VertAlign = SMVerticalAlign.Top;
                    richText.DrawString(context, layout, textBounds);
                    Area.RelativeArea = r;
                }
                else
                {
                    if (BackType == SMBackgroundType.Shadow && BackgroundImage != null)
                    {
                        context.g.DrawImage(BackgroundImage,
                            textBounds.X + BackgroundImageOffset.X,
                            textBounds.Y + BackgroundImageOffset.Y);
                    }
                    else
                    {
                        DrawStyledBackground(context, bounds);
                    }
                    DrawStyledBorder(context, layout, bounds);

                    richText.DrawString(context, layout, textBounds);
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
                DrawStyledBackground(context, bounds);
                DrawStyledBorder(context, layout, bounds);

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
            
            if (UIStateError == MNEvaluationResult.Incorrect && UIStateChecked)
            {
                if (Document.HasViewer)
                {
                    Document.Viewer.ScheduleCall(MNNotificationCenter.RectifyDelay, this, "clearCheck");
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

        public override MNEvaluationResult Evaluate()
        {
            UIStateError = GenericCheckedEvaluation();

            return UIStateError; 
        }

        public override bool OnDropFinished(PVDragContext dc)
        {
            if (HasImmediateEvaluation)
            {
                string tag = SafeTag;
                if (tag.Length > 0 && !tag.Equals(dc.draggedItem.Tag, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }

                // if text is only ____, then replace text with dragged content
                if (Text != null && Text.Replace("_","").Length == 0 && dc.draggedItem.Text != null && dc.draggedItem.Text.Length > 0)
                    Text = dc.draggedItem.Text;
            }


            return base.OnDropFinished(dc);
        }
    }
}
