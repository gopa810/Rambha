using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

using Rambha.Script;

namespace Rambha.Document
{
    public class SMTextContainer: SMControl
    {
        [Browsable(true), Category("Layout")]
        public int Columns { get; set; }

        [Browsable(true), Category("Layout")]
        public float ColumnSeparatorWidth { get; set; }

        [Browsable(true), Category("Layout")]
        public SMLineStyle ColumnSeparatorStyle { get; set; }

        [Browsable(true), Category("Layout")]
        public bool ShowNavigationButtons { get; set; }


        public List<SMWordBase> drawWords = new List<SMWordBase>();
        public List<SMWordLine> drawLines = null;
        public bool drawWordsModified = false;

        private SMRunningLine p_runline = SMRunningLine.SingleWord;

        private string p_prevText = "";

        public int CurrentPage = 0;
        public int PageCount = 1;

        private bool prevBtnPressed = false;
        private bool nextBtnPressed = false;
        private int navigButtonsHeight = 48;
        private Rectangle prevBtnRect;
        private Rectangle nextBtnRect;


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
            Columns = 1;
            ColumnSeparatorStyle = SMLineStyle.Plain;
            ColumnSeparatorWidth = 20;
            ShowNavigationButtons = false;
        }

        public override bool Load(Serializer.RSFileReader br)
        {
            if (base.Load(br))
            {
                SetText(Text);

                byte b;
                while ((b = br.ReadByte()) != 0)
                {
                    switch (b)
                    {
                        case 10:
                            Columns = br.ReadInt32();
                            break;
                        case 11:
                            ColumnSeparatorStyle = (SMLineStyle)br.ReadInt32();
                            break;
                        case 12:
                            ColumnSeparatorWidth = br.ReadFloat();
                            break;
                        case 13:
                            ShowNavigationButtons = br.ReadBool();
                            break;
                    }
                }
                return true;
            }

            return false;
        }

        public override void Save(Serializer.RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteInt32(Columns);

            bw.WriteByte(11);
            bw.WriteInt32((Int32)ColumnSeparatorStyle);
            
            bw.WriteByte(12);
            bw.WriteFloat(ColumnSeparatorWidth);

            bw.WriteByte(13);
            bw.WriteBool(ShowNavigationButtons);

            bw.WriteByte(0);
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

            if (ShowNavigationButtons)
                textBounds.Height -= navigButtonsHeight;

            PrepareBrushesAndPens();

            DrawStyledBorder(context, bounds);

            string stext = Text;

            if (Content != null)
            {
                if (Content is MNReferencedText)
                {
                    stext = (Content as MNReferencedText).Text;
                }
            }

            if (!p_prevText.Equals(stext))
            {
                SetText(stext);
                p_prevText = stext;
            }

            if (drawWordsModified || area.Changed)
            {
                Rectangle layoutBounds = textBounds;
                if (ShowNavigationButtons)
                    layoutBounds.Height -= navigButtonsHeight;
                drawLines = RecalculateWordsLayout(context, layoutBounds, drawWords, this, RunningLine, Columns, ColumnSeparatorWidth, out PageCount);

                drawWordsModified = false;
                area.Changed = false;
            }

            PaintPageNo(context, CurrentPage);

            if (ShowNavigationButtons)
            {
                textBounds = DrawNavigationButtons(context, textBounds);
            }

            if (Columns > 1)
            {
                textBounds = DrawColumnSeparators(context, textBounds);
            }

            // draw selection marks
            base.Paint(context);
        }

        private Rectangle DrawNavigationButtons(MNPageContext context, Rectangle textBounds)
        {
            context.g.DrawLine(SMGraphics.GetPen(Style.ForeColor, 1), textBounds.Left, textBounds.Bottom - navigButtonsHeight,
                textBounds.Right, textBounds.Bottom - navigButtonsHeight);
            if (HasPrevPage())
            {
                prevBtnRect = new Rectangle(textBounds.Left, textBounds.Bottom - navigButtonsHeight, textBounds.Width / 2 - 32, navigButtonsHeight);
                if (prevBtnPressed)
                    context.g.FillRectangle(SMGraphics.GetBrush(Color.LightGreen), prevBtnRect);
                context.g.DrawString("< PREV", SMGraphics.GetFontVariation(SystemFonts.MenuFont, 20f), SMGraphics.GetBrush(Color.Gray), prevBtnRect, SMGraphics.StrFormatCenter);
            }
            if (HasNextPage())
            {
                nextBtnRect = new Rectangle(textBounds.Left + textBounds.Width / 2 + 32, textBounds.Bottom - navigButtonsHeight, textBounds.Width / 2 - 32, navigButtonsHeight);
                if (nextBtnPressed)
                    context.g.FillRectangle(SMGraphics.GetBrush(Color.LightGreen), nextBtnRect);
                context.g.DrawString("NEXT >", SMGraphics.GetFontVariation(SystemFonts.MenuFont, 20f), SMGraphics.GetBrush(Color.Gray), nextBtnRect, SMGraphics.StrFormatCenter);
            }

            context.g.DrawString(string.Format("{0}/{1}", CurrentPage + 1, PageCount), SMGraphics.GetFontVariation(SystemFonts.MenuFont, 20f),
                SMGraphics.GetBrush(Color.Gray), new Rectangle(textBounds.Left + textBounds.Width / 2 - 48, textBounds.Bottom - navigButtonsHeight, 96, navigButtonsHeight),
                SMGraphics.StrFormatCenter);
            return textBounds;
        }

        private Rectangle DrawColumnSeparators(MNPageContext context, Rectangle textBounds)
        {
            Pen pen = SMGraphics.GetPen(Style.ForeColor, 1);
            for (int i = 1; i < Columns; i++)
            {
                int x = textBounds.Left + i * textBounds.Width / Columns;
                int y = textBounds.Top;
                int y2 = textBounds.Bottom;
                if (ShowNavigationButtons)
                    y2 -= navigButtonsHeight;

                int cy = y;
                switch (ColumnSeparatorStyle)
                {
                    case SMLineStyle.Plain:
                        context.g.DrawLine(pen, x, y, x, y2);
                        break;
                    case SMLineStyle.Dashed:
                        while (cy + 16 < y2)
                        {
                            context.g.DrawLine(pen, x, cy, x, cy + 16);
                            cy += 24;
                        }
                        break;
                    case SMLineStyle.Doted:
                        while (cy + 3 < y2)
                        {
                            context.g.DrawRectangle(pen, x, cy, 2, 2);
                            cy += 8;
                        }
                        break;
                    case SMLineStyle.ZigZag:
                        while (cy + 16 < y2)
                        {
                            context.g.DrawLine(pen, x, cy, x - 4, cy + 4);
                            context.g.DrawLine(pen, x - 4, cy + 4, x + 4, cy + 12);
                            context.g.DrawLine(pen, x + 4, cy + 12, x, cy + 16);
                            cy += 16;
                        }
                        break;
                }
            }
            return textBounds;
        }

        public void PaintPageNo(MNPageContext context, int pageNo)
        {
            if (pageNo >= PageCount)
                pageNo = PageCount - 1;
            if (pageNo < 0)
                pageNo = 0;

            foreach (SMWordLine wline in drawLines)
            {
                foreach (SMWordBase wt in wline)
                {
                    if (wt.PageNo == pageNo)
                        wt.Paint(context);
                }
            }

        }


        public override SMTokenItem GetDraggableItem(Point point)
        {
            foreach (SMWordBase wt in drawWords)
            {
                if (wt.rect.Contains(point))
                {
                    return (wt.IsDraggable ? wt.GetDraggableItem() : null);
                }
            }

            return null;
        }

        public static List<SMWordLine> RecalculateWordsLayout(MNPageContext context, Rectangle textBounds, List<SMWordBase> drawWords, SMControl control, SMRunningLine RunningLine,
            int Columns, float ColumnSeparatorWidth, out int PageCount)
        {
            float lineY = textBounds.Y;
            float lineX = textBounds.X;
            float lineEnd = 0;
            float lineHeight = 0f;
            float lineWidth = textBounds.Width;
            int lineNo = 0;
            int columnNo = 0;
            int pageNo = 0;
            bool writeLineNo = false;
            bool isNewLine = false;
            List<SMWordLine> lines = new List<SMWordLine>();
            SMWordLine currLine = new SMWordLine();
            lines.Add(currLine);

            if (Columns > 1)
            {
                lineWidth = lineWidth - (Columns - 1) * ColumnSeparatorWidth;
                lineWidth = lineWidth / Columns;
                lineEnd = lineX + lineWidth;
            }

            float bottom = textBounds.Bottom - 32;

            // first placement of word tokens
            foreach (SMWordBase wt in drawWords)
            {
                writeLineNo = true;
                isNewLine = false;
                if (wt is SMWordSpecial)
                {
                    SMWordSpecial spwt = (SMWordSpecial)wt;
                    if (spwt.Type == SMWordSpecialType.Newline)
                    {
                        isNewLine = true;
                        writeLineNo = false;
                    }
                }
                else if (wt is SMWordToken)
                {
                    SMWordToken wtk = (SMWordToken)wt;
                    if (wtk.droppedItem != null)
                        wt.rect.Size = context.g.MeasureString(wtk.droppedItem.Text, wtk.Font);
                    else
                        wt.rect.Size = context.g.MeasureString(wtk.text, wtk.Font);
                }
                else if (wt is SMWordText)
                {
                    SMWordText wtt = wt as SMWordText;
                    wtt.rect.Size = context.g.MeasureString(wtt.text, wtt.Font);
                }
                else if (wt is SMWordImage)
                {
                    SMWordImage wti = wt as SMWordImage;
                    wti.rect.Size = wti.imageSize;
                }

                if (writeLineNo)
                {
                    if ((lineX + wt.rect.Width > lineEnd) || RunningLine == SMRunningLine.SingleWord)
                        isNewLine = true;
                }

                if (isNewLine)
                {
                    isNewLine = false;
                    currLine = new SMWordLine();
                    lines.Add(currLine);

                    lineY += lineHeight;
                    lineHeight = context.g.MeasureString("M", control.GetUsedFont()).Height;
                    lineNo++;

                    if (lineY + lineHeight > textBounds.Bottom)
                    {
                        columnNo++;
                        lineNo = 0;

                        if (columnNo >= Columns)
                        {
                            pageNo++;
                            columnNo = 0;
                        }

                        lineY = textBounds.Top;
                    }

                    lineX = textBounds.Left + columnNo * (lineWidth + ColumnSeparatorWidth);
                    lineEnd = lineX + lineWidth;
                }

                if (writeLineNo)
                {
                    currLine.Add(wt);

                    wt.LineNo = lineNo;
                    wt.ColumnNo = columnNo;
                    wt.PageNo = pageNo;
                    wt.rect.Location = new PointF(lineX, lineY);
                    lineX += wt.rect.Width;
                    lineHeight = Math.Max(lineHeight, wt.rect.Height);
                    writeLineNo = false;
                }
            }

            lineY += lineHeight;
            PageCount = pageNo + 1;

            // vertical alignment
            AdjustVerticaly(textBounds, lines, control.GetVerticalAlign(), lineY);

            // horizontal aligment
            AdjustLinesHorizontaly(textBounds.Width, control.GetHorizontalAlign(), lines);

            return lines;
        }

        private static void AdjustVerticaly(Rectangle textBounds, List<SMWordLine> drawWords, SMVerticalAlign valign, float lineY)
        {
            float diff = 0f;
            switch (valign)
            {
                case SMVerticalAlign.Center:
                    diff = (textBounds.Bottom - lineY) / 2;
                    break;
                case SMVerticalAlign.Bottom:
                    diff = (textBounds.Bottom - lineY);
                    break;
            }

            if (diff != 0)
            {
                foreach (SMWordLine wt in drawWords)
                {
                    foreach (SMWordBase wbase in wt)
                    {
                        wbase.rect.Y += diff;
                    }
                }
            }
        }

        private static void AdjustLinesHorizontaly(int areaWidth, SMHorizontalAlign align, List<SMWordLine> drawWords)
        {
            int i = 0;
            int max = drawWords.Count;
            for (i = 0; i < max; i++)
            {
                AdjustLineWords(drawWords[i], areaWidth, align, i < max -1);
            }

        }

        private static void AdjustLineWords(List<SMWordBase> drawWords, float areaWidth, 
            SMHorizontalAlign align, bool lastLine)
        {
            int wordCount = 0;
            float totalWidth = 0;
            foreach (SMWordBase wb in drawWords)
            {
                if (!wb.IsSpace())
                {
                    wordCount++;
                }
                totalWidth += wb.rect.Width;
            }

            if (wordCount > 0)
            {
                float adjustment = 0;
                float adjustmentStep = 0;
                bool doAdjust = true;
                if (align == SMHorizontalAlign.Left || (align == SMHorizontalAlign.Justify && lastLine))
                {
                    doAdjust = false;
                }
                else if (align == SMHorizontalAlign.Center)
                {
                    adjustment = (areaWidth - totalWidth) / 2;
                }
                else if (align == SMHorizontalAlign.Right)
                {
                    adjustment = (areaWidth - totalWidth);
                }
                else if (align == SMHorizontalAlign.Justify)
                {
                    adjustmentStep = (areaWidth - totalWidth) / (wordCount + 1);
                }
                if (doAdjust)
                {
                    foreach (SMWordBase wat in drawWords)
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

            if (this.Droppable == SMDropResponse.One && dc.draggedItem != null)
            {
                drawWords.Clear();
                DroppedItems.Clear();
                drawWordsModified = true;
                needUpdateDrawWords = true;
            }
            else if (this.Droppable == SMDropResponse.Many)
            {
                needUpdateDrawWords = true;
            }
            else
            {
                if (dc.draggedItem != null)
                {
                    foreach (SMWordBase wt in drawWords)
                    {
                        if (wt is SMWordToken)
                        {
                            SMWordToken wtk = (SMWordToken)wt;
                            if (wtk.Droppable == SMDropResponse.One || wtk.Droppable == SMDropResponse.Many)
                            {
                                wtk.UIStateHover = false;
                                if (wtk.rect.Contains(dc.lastPoint))
                                {
                                    wtk.droppedItem = dc.draggedItem;
                                    drawWordsModified = true;
                                }
                            }
                        }
                    }
                }
            }

            if (base.OnDropFinished(dc) && needUpdateDrawWords)
            {
                SMWordBase wr = null;
                if (dc.draggedItem.Image != null)
                {
                    wr = new SMWordImage(this, dc.draggedItem);
                }
                else
                {
                    wr = new SMWordText(this, dc.draggedItem);
                }
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
                foreach (SMWordBase wt in drawWords)
                {
                    wt.HoverPoint(context.lastPoint);
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

                    foreach (SMWordBase wt in drawWords)
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

        public override void DisplayAnswers()
        {
            drawWords.Clear();
            foreach (string w in ExpectedTags())
            {
                SMWordToken item = new SMWordToken(this);
                item.text = w;
                item.tag = w.ToLower();
                item.Draggable = SMDragResponse.Drag;
                item.Droppable = SMDropResponse.None;
                drawWords.Add(item);
            }
            base.DisplayAnswers();
        }

        public override GSCore GetPropertyValue(string s)
        {
            if (s.Equals("currentPage"))
            {
                return new GSInt32(CurrentPage);
            }
            else if (s.Equals("pageCount"))
            {
                return new GSInt32(PageCount);
            }
            else if (s.Equals("hasPreviousPage"))
            {
                return new GSBoolean(HasPrevPage());
            }
            else if (s.Equals("hasNextPage"))
            {
                return new GSBoolean(HasNextPage());
            }

            return base.GetPropertyValue(s);
        }

        private bool HasNextPage()
        {
            return CurrentPage < PageCount - 1;
        }

        private bool HasPrevPage()
        {
            return CurrentPage > 0;
        }

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            if (token.Equals("showNextPage"))
            {
                if (HasNextPage()) CurrentPage++;
                PostExecuteEvent("OnCurrentPageChanged");
            }
            else if (token.Equals("showPrevPage"))
            {
                if (HasPrevPage()) CurrentPage--;
                PostExecuteEvent("OnCurrentPageChanged");
            }

            return base.ExecuteMessage(token, args);
        }


        protected override GSCore ExecuteMessageSet(GSCore a1, GSCore a2, GSCoreCollection args)
        {
            string s = a1.getStringValue();
            if (s.Equals("columns"))
            {
                Columns = (int)a2.getIntegerValue();
                return a2;
            }
            else if (s.Equals("navigbuttons"))
            {
                ShowNavigationButtons = a2.getBooleanValue();
                return a2;
            }
            else
            {
                return base.ExecuteMessageSet(a1, a2, args);
            }
        }

        public override void OnTapBegin(PVDragContext dc)
        {
            if (ShowNavigationButtons)
            {
                if (prevBtnRect.Contains(dc.lastPoint))
                    prevBtnPressed = true;
                else if (nextBtnRect.Contains(dc.lastPoint))
                    nextBtnPressed = true;
            }
            base.OnTapBegin(dc);
        }

        public override void OnTapEnd(PVDragContext dc)
        {
            if (prevBtnPressed)
            {
                if (HasPrevPage()) CurrentPage--;
            }

            if (nextBtnPressed)
            {
                if (HasNextPage()) CurrentPage++;
            }

            prevBtnPressed = false;
            nextBtnPressed = false;
            base.OnTapEnd(dc);
        }

        public override void OnTapCancel(PVDragContext dc)
        {
            prevBtnPressed = false;
            nextBtnPressed = false;
            base.OnTapCancel(dc);
        }

    }
}
