﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
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
            Area.BackType = SMBackgroundType.None;
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
                Area.BackgroundImage = null;
            }
        }

        public override void TextDidChange()
        {
            if (richText != null)
            {
                richText.ForceRecalc();
                Area.BackgroundImage = null;
            }
        }

        public SMRichText richText = null;

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
            boundsA.Width = Math.Min(context.PageWidth, bounds.Right);
            boundsA.Height = Math.Min(context.PageHeight, bounds.Bottom);
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
                Size textSize = richText.MeasureString(context, plainText, textBounds.Width);
                Rectangle r = Area.GetDockedRectangle(context.PageSize, textSize);
                if (Area.Dock != SMControlSelection.None)
                {
                    textBounds.X = Area.RelativeArea.X + SMRectangleArea.PADDING_DOCK_LEFT;
                    textBounds.Y = Area.RelativeArea.Y + SMRectangleArea.PADDING_DOCK_TOP;
                    textBounds.Width = Area.RelativeArea.Width - SMRectangleArea.PADDING_DOCK_LEFT 
                        - SMRectangleArea.PADDING_DOCK_RIGHT + 2;
                    textBounds.Height = Area.RelativeArea.Height - SMRectangleArea.PADDING_DOCK_TOP - SMRectangleArea.PADDING_DOCK_BOTTOM + 2;
                    richText.Paragraph.VertAlign = SMVerticalAlign.Top;
                }

                if (Area.BackType == SMBackgroundType.None)
                {
                    DrawStyledBackground(context, layout, bounds);
                }
                else if (Area.BackType == SMBackgroundType.Solid)
                {
                    context.g.FillRectangle(SMGraphics.GetBrush(Page.BackgroundColor), r);
                }
                else if (Area.BackType == SMBackgroundType.Shadow && Area.BackgroundImage != null)
                {
                    context.g.DrawImage(Area.BackgroundImage,
                        textBounds.X + Area.BackgroundImageOffset.X,
                        textBounds.Y + Area.BackgroundImageOffset.Y);
                }

                if (Area.Dock == SMControlSelection.None)
                    DrawStyledBorder(context, layout, bounds);

                richText.DrawString(context, layout, textBounds);
            }
            else if (runningText != null)
            {
                DrawStyledBackground(context, layout, bounds);
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

        public override void ExportToHtml(MNExportContext ctx, int zorder, StringBuilder sbHtml, StringBuilder sbCss, StringBuilder sbJS)
        {
//            SMRectangleArea area = this.Area;
//            Rectangle bounds = area.RelativeArea;


            if (Text != null && Text.Contains("\\n"))
                Text = Text.Replace("\\n", "\n");
            Text = Text.Replace("\n", "<br>");
            string plainText = Text;

            if (Content != null)
            {
                plainText = null;
                if (Content is MNReferencedText)
                    plainText = ((MNReferencedText)Content).Text;
                else if (Content is MNReferencedSound)
                    plainText = Text;
            }

            if (plainText.StartsWith("$"))
            {
                plainText = Document.ResolveProperty(plainText.Substring(1));
            }

            if (plainText != null)
            {
                //Size textSize = richText.MeasureString(context, plainText, textBounds.Width);
                /*Rectangle r = Area.GetDockedRectangle(SMRectangleArea._size_4_3, textSize);
                if (Area.Dock != SMControlSelection.None)
                {
                    textBounds.X = Area.RelativeArea.X + SMRectangleArea.PADDING_DOCK_LEFT;
                    textBounds.Y = Area.RelativeArea.Y + SMRectangleArea.PADDING_DOCK_TOP;
                    textBounds.Width = Area.RelativeArea.Width - SMRectangleArea.PADDING_DOCK_LEFT
                        - SMRectangleArea.PADDING_DOCK_RIGHT + 2;
                    textBounds.Height = Area.RelativeArea.Height - SMRectangleArea.PADDING_DOCK_TOP - SMRectangleArea.PADDING_DOCK_BOTTOM + 2;
                    richText.Paragraph.VertAlign = SMVerticalAlign.Top;
                }*/


                /*if (Area.BackType == SMBackgroundType.Shadow && Area.BackgroundImage != null)
                {
                    sbHtml.AppendFormat("<img style='position:absolute;top:{0}%;left:{1}%;width:{2}%;height:{3}%;' src=\"{4}\" />",
                      SMRectangleArea.AbsToPercX(textBounds.X + Area.BackgroundImageOffset.X),
                      SMRectangleArea.AbsToPercY(textBounds.Y + Area.BackgroundImageOffset.Y),
                      SMRectangleArea.AbsToPercX(Area.BackgroundImage.Size.Width),
                      SMRectangleArea.AbsToPercX(Area.BackgroundImage.Size.Height),
                      ctx.GetFileNameFromImage(Area.BackgroundImage));
                }*/

                //if (Area.Dock == SMControlSelection.None) DrawStyledBorder(context, layout, bounds);

                string onclick = GetOnclickHtml();

                //richText.DrawString(context, layout, textBounds);
                string blockFormat = Font.HtmlString() + Paragraph.Html() + ContentPaddingHtml() + "position:absolute;" + Area.HtmlLTRB();
                sbCss.AppendFormat(".c{0}n {{ {1} {2} }}\n", Id, HtmlFormatColor(false), blockFormat);
                sbCss.AppendFormat(".c{0}h {{ {1} {2} }}\n", Id, HtmlFormatColor(true), blockFormat);
                sbHtml.AppendFormat("<div class=\"c{0}n\" style='display:flex;flex-direction:column;justify-content:{1};cursor:pointer;'", Id, GetVerticalAlignHtml());
                if (onclick.Length > 0) sbHtml.AppendFormat(" onclick=\"{0}\" ", onclick);
                sbHtml.Append(">\n");
                sbHtml.Append("<div>" + plainText + "</div>");
                sbHtml.AppendFormat("\n</div>\n");

                
            }
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
