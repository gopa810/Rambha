using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rambha.Document
{
    public class SMTextContainer: SMControl
    {
        public List<SMWordToken> drawWords = new List<SMWordToken>();
        public bool drawWordsModified = false;

        private SMRunningLine p_runline = SMRunningLine.SingleWord;

        private string p_prevText = "";

        public SMRunningLine RunningLine 
        {
            get 
            {
                return p_runline;
            }
            set 
            {
                p_runline = value;
                drawWordsModified = true;
            }
        }

        public SMTextContainer(MNPage p)
            : base(p)
        {
            Text = "Text Container";
            Evaluation = MNEvaluationType.Inherited;
        }

        public override bool Load(Serializer.RSFileReader br)
        {
            if (base.Load(br))
            {
                SetText(Text);
                return true;
            }

            return false;
        }

        public override System.Drawing.Size GetDefaultSize()
        {
            return new Size(256,196);
        }

        public override void Paint(MNPageContext context)
        {
            SMRectangleArea area = context.CurrentPage.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);

            Rectangle textBounds = Style.ApplyPadding(bounds);

            PrepareBrushesAndPens();

            DrawStyledBorder(context, bounds);

            if (!p_prevText.Equals(Text))
            {
                SetText(Text);
                p_prevText = Text;
            }

            if (drawWordsModified || area.Changed)
            {
                textBounds = RecalculateWordsLayout(context, textBounds);

                drawWordsModified = false;
                area.Changed = false;
            }

            foreach (SMWordToken wt in drawWords)
            {
                wt.Paint(context);
            }


            // draw selection marks
            base.Paint(context);
        }


        public override SMTokenItem GetDraggableItem(Point point)
        {
            foreach (SMWordToken wt in drawWords)
            {
                if (wt.rect.Contains(point))
                {
                    return (wt.IsDraggable ? wt.GetDraggableItem() : null);
                }
            }

            return null;
        }

        private Rectangle RecalculateWordsLayout(MNPageContext context, Rectangle textBounds)
        {
            float lineY = textBounds.Y;
            float lineX = textBounds.X;
            float lineHeight = 0f;
            int lineNo = 0;


            // first placement of word tokens
            foreach (SMWordToken wt in drawWords)
            {
                if (wt.droppedItem != null)
                {
                    wt.rect.Size = context.g.MeasureString(wt.droppedItem.Text, wt.Font);
                }
                else
                {
                    wt.rect.Size = context.g.MeasureString(wt.text, wt.Font);
                }
                if (wt.text.Equals(" ") && RunningLine != SMRunningLine.SingleWord)
                {
                    wt.LineNo = lineNo;
                    wt.rect.Location = new PointF(lineX, lineY);
                    lineX += wt.rect.Width;
                    lineHeight = Math.Max(lineHeight, wt.rect.Height);
                }
                else
                {
                    if ((lineX + wt.rect.Width > textBounds.Right) || RunningLine == SMRunningLine.SingleWord)
                    {
                        lineX = textBounds.Left;
                        lineY += lineHeight;
                        lineHeight = 0f;
                        lineNo++;
                    }

                    wt.LineNo = lineNo;
                    wt.rect.Location = new PointF(lineX, lineY);
                    lineX += wt.rect.Width;
                    lineHeight = Math.Max(lineHeight, wt.rect.Height);
                }
            }

            lineY += lineHeight;

            // vertical alignment
            float diff = 0f;
            switch (Style.ContentAlign)
            {
                case SMContentAlign.TopLeft:
                case SMContentAlign.TopCenter:
                case SMContentAlign.TopRight:
                    // do nothing content is already aligned to top
                    break;
                case SMContentAlign.CenterLeft:
                case SMContentAlign.Center:
                case SMContentAlign.CenterRight:
                    diff = (textBounds.Bottom - lineY) / 2;
                    break;
                case SMContentAlign.BottomLeft:
                case SMContentAlign.BottomCenter:
                case SMContentAlign.BottomRight:
                    diff = (textBounds.Bottom - lineY);
                    break;
            }

            if (diff != 0)
            {
                foreach (SMWordToken wt in drawWords)
                {
                    wt.rect.Y += diff;
                }
            }

            // horizontal aligment
            switch (Style.ContentAlign)
            {
                case SMContentAlign.TopLeft:
                case SMContentAlign.CenterLeft:
                case SMContentAlign.BottomLeft:
                    // do nothing content is already aligned to top
                    break;
                case SMContentAlign.TopCenter:
                case SMContentAlign.Center:
                case SMContentAlign.BottomCenter:
                    Adjust(textBounds.Width, true);
                    break;
                case SMContentAlign.TopRight:
                case SMContentAlign.CenterRight:
                case SMContentAlign.BottomRight:
                    Adjust(textBounds.Width, false);
                    break;
            }
            return textBounds;
        }

        private void Adjust(int areaWidth, bool centered)
        {
            int lastLine = -1;
            float totalWidth = 0f;
            int wordCount = 0;
            foreach (SMWordToken wt in drawWords)
            {
                if (wt.LineNo != lastLine)
                {
                    AdjustWords(lastLine, totalWidth, wordCount, areaWidth, centered);
                    totalWidth = wt.rect.Width;
                    wordCount = 1;
                    lastLine = wt.LineNo;
                }
                else
                {
                    wordCount++;
                    totalWidth += wt.rect.Width;
                }
            }
            AdjustWords(lastLine, totalWidth, wordCount, areaWidth, centered);
        }

        private void AdjustWords(int lastLine, float totalWidth, int wordCount, float areaWidth, bool center)
        {
            if (wordCount > 0)
            {
                float adjustment = center ? (areaWidth - totalWidth) / (wordCount + 1) : (areaWidth - totalWidth);
                float adjustmentStep = center ? (areaWidth - totalWidth) / (wordCount + 1) : 0f;
                foreach (SMWordToken wat in drawWords)
                {
                    if (wat.LineNo == lastLine)
                    {
                        wat.rect.X += adjustment;
                        wat.rect.Width += adjustmentStep;
                        adjustment += adjustmentStep;
                    }
                }
            }
        }

        public override bool OnDropFinished(PVDragContext dc)
        {
            bool needUpdateDrawWords = false;

            if (HasImmediateEvaluation)
            {
                List<string> et = ExpectedTags();
                if (et.IndexOf(dc.draggedItem.Tag.ToLower()) < 0)
                {
                    return false;
                }
            }

            if (Style.Droppable == SMDropResponse.One && dc.draggedItem != null)
            {
                drawWords.Clear();
                DroppedItems.Clear();
                drawWordsModified = true;
                needUpdateDrawWords = true;
            }
            else if (Style.Droppable == SMDropResponse.Many)
            {
                needUpdateDrawWords = true;
            }
            else
            {
                if (dc.draggedItem != null)
                {
                    foreach (SMWordToken wt in drawWords)
                    {
                        if (wt.Droppable == SMDropResponse.One || wt.Droppable == SMDropResponse.Many)
                        {
                            wt.UIStateHover = false;
                            if (wt.rect.Contains(dc.lastPoint))
                            {
                                wt.droppedItem = dc.draggedItem;
                                drawWordsModified = true;
                            }
                        }
                    }
                }
            }

            if (base.OnDropFinished(dc) && needUpdateDrawWords)
            {
                SMWordToken wr = new SMWordToken(dc.draggedItem);
                wr.Evaluation = this.HasImmediateEvaluation ? MNEvaluationType.Immediate : MNEvaluationType.Lazy;
                drawWords.Add(wr);
                drawWordsModified = true;
                return true;
            }

            return false;
        }

        public override void OnDragFinished(PVDragContext context)
        {
            base.OnDragFinished(context);

            SMTokenItem item = context.draggedItem;

            int idx = IndexOfDroppedItem(item.Text, item.Tag);
            if (idx >= 0)
            {
                drawWords.RemoveAt(idx);
                drawWordsModified = true;
            }
        }

        public override void OnDropMove(PVDragContext context)
        {
            if (context.draggedItem != null)
            {
                foreach (SMWordToken wt in drawWords)
                {
                    if (wt.Droppable == SMDropResponse.One || wt.Droppable == SMDropResponse.Many)
                    {
                         wt.UIStateHover = wt.rect.Contains(context.lastPoint);
                    }
                }
            }

            base.OnDropMove(context);
        }

        public void SetText(string text)
        {
            Text = text;
            drawWords = SMWordToken.WordListFromString(text, this);
            drawWordsModified = true;
        }

        public List<string> ExpectedTags()
        {
            string[] s =  Tag.ToLower().Split(',');
            List<string> et = new List<string>();
            et.AddRange(s);
            return et;
        }

        /// <summary>
        /// Only lazy evaluation
        /// </summary>
        /// <returns></returns>
        public override MNEvaluationResult Evaluate()
        {
            if (HasLazyEvaluation)
            {
                if (Tag != null)
                {
                    List<string> s = ExpectedTags();

                    foreach (SMWordToken wt in drawWords)
                    {
                        if (wt.Evaluation == MNEvaluationType.Lazy)
                        {
                            int index = s.IndexOf(wt.tag.ToLower());
                            if (index >= 0)
                            {
                                s.RemoveAt(index);
                                UIStateError = MNEvaluationResult.Correct;
                            }
                            else
                            {
                                UIStateError = MNEvaluationResult.Incorrect;
                                break;
                            }
                        }
                    }

                    if (s.Count > 0)
                        UIStateError = MNEvaluationResult.Incorrect;
                }
            }

            return base.Evaluate();
        }
    }
}
