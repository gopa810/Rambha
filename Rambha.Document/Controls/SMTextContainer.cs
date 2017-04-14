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

        private SMRunningLine p_runline = SMRunningLine.Natural;

        private string p_prevText = "";

        public int CurrentPage = 0;
        public int PageCount = 1;

        private bool prevBtnPressed = false;
        private bool nextBtnPressed = false;
        private int navigButtonsHeight = 32;
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
                        case 14:
                            RunningLine = (SMRunningLine)br.ReadInt32();
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

            bw.WriteByte(14);
            bw.WriteInt32((int)RunningLine);

            bw.WriteByte(0);
        }

        public override System.Drawing.Size GetDefaultSize()
        {
            return new Size(256,196);
        }

        public override void Paint(MNPageContext context)
        {
            Rectangle bounds = Area.GetBounds(context);

            Rectangle textBounds = ContentPadding.ApplyPadding(bounds);

            if (ShowNavigationButtons)
                textBounds.Height -= navigButtonsHeight;

            PrepareBrushesAndPens();

            DrawStyledBackground(context, bounds);
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
                Text = stext;
                p_prevText = stext;
            }

            if (drawWordsModified)
            {
                Rectangle layoutBounds = textBounds;
                if (ShowNavigationButtons)
                    layoutBounds.Height -= navigButtonsHeight;
                SMRichLayout lay = null;
                lay = RecalculateWordsLayout(context, layoutBounds, drawWords, this, RunningLine, Columns);
                drawLines = lay.Lines;
                PageCount = lay.Pages;

                drawWordsModified = false;
            }

            PaintPageNo(context, CurrentPage, textBounds.X, textBounds.Y);

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
            context.g.DrawLine(SMGraphics.GetPen(NormalState.ForeColor, 1), textBounds.Left, textBounds.Bottom - navigButtonsHeight,
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
            Pen pen = SMGraphics.GetPen(NormalState.ForeColor, 1);
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

        public void PaintPageNo(MNPageContext context, int pageNo, int X, int Y)
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
                        wt.Paint(context, X, Y);
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

        public override void StyleDidChange()
        {
            p_prevText = "";
            base.StyleDidChange();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textBounds"></param>
        /// <param name="drawWords"></param>
        /// <param name="control"></param>
        /// <param name="RunningLine"></param>
        /// <param name="Columns">Value -1 means, that no paging is done, normaly columns are 1,2,3...</param>
        /// <param name="ColumnSeparatorWidth"></param>
        /// <param name="PageCount"></param>
        /// <returns></returns>
        public static SMRichLayout RecalculateWordsLayout(MNPageContext context, Rectangle textBounds, List<SMWordBase> drawWords, SMControl control, SMRunningLine RunningLine,
            int Columns)
        {
            textBounds.X = 0;
            textBounds.Y = 0;
            float lineY = textBounds.Y;
            float lineX = textBounds.X;
            float lineEnd = textBounds.Right;
            float lineHeight = 0f;
            float lineWidth = textBounds.Width;
            float columnWidth = textBounds.Width;
            int lineNo = 0;
            int columnNo = 0;
            int pageNo = 0;
            int rightX = textBounds.X;
            bool writeLineNo = false;
            bool isNewLine = false;
            bool isNewColumn = false;
            SMWordLine currLine = new SMWordLine();
            SMRichLayout richLayout = new SMRichLayout();
            richLayout.Lines = new List<SMWordLine>();
            richLayout.Lines.Add(currLine);
            //context.g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            if (Columns > 1)
            {
                columnWidth = textBounds.Width / Columns;
                lineWidth = textBounds.Width / Columns - control.ContentPadding.Left - control.ContentPadding.Right;
                lineX = textBounds.X + columnNo * columnWidth + control.ContentPadding.Left;
                lineEnd = lineX + lineWidth;
            }

            float bottom = textBounds.Bottom;
            bool isSpaceText = false;
            int startsWithParentheses = 0;
            bool isNewPage = false;

            // first placement of word tokens
            foreach (SMWordBase wt in drawWords)
            {
                isSpaceText = false;
                writeLineNo = true;
                isNewLine = false;
                isNewColumn = false;
                isNewPage = false;
                startsWithParentheses = 0;
                if (wt is SMWordSpecial)
                {
                    SMWordSpecial spwt = (SMWordSpecial)wt;
                    if (spwt.Type == SMWordSpecialType.Newline)
                    {
                        isNewLine = true;
                        writeLineNo = false;
                    }
                    else if (spwt.Type == SMWordSpecialType.NewColumn)
                    {
                        isNewLine = false;
                        writeLineNo = false;
                        isNewColumn = true;
                    }
                    else if (spwt.Type == SMWordSpecialType.HorizontalLine)
                    {
                        wt.rect.Width = lineEnd - lineX - 1;
                        wt.rect.Height = 20;
                    }
                    else if (spwt.Type == SMWordSpecialType.NewPage)
                    {
                        isNewPage = true;
                        writeLineNo = false;
                    }
                }
                else if (wt is SMWordToken)
                {
                    SMWordToken wtk = (SMWordToken)wt;
                    string s = wtk.GetCurrentText();
                    wt.rect.Size = context.g.MeasureString(s, wtk.Font.Font, textBounds.Width, StringFormat.GenericTypographic);
                }
                else if (wt is SMWordText)
                {
                    SMWordText wtt = wt as SMWordText;
                    if (wtt.text.StartsWith("\"") || wtt.text.StartsWith("\u201c"))
                    {
                        SizeF sf = context.g.MeasureString("\u201c", wtt.Font.Font, textBounds.Width, StringFormat.GenericTypographic);
                        startsWithParentheses = (int)sf.Width;
                    }
                    if (wtt.text.Equals(" "))
                    {
                        wtt.rect.Size = context.g.MeasureString(wtt.text, wtt.Font.Font);
                        isSpaceText = true;
                    }
                    else
                        wtt.rect.Size = context.g.MeasureString(wtt.text, wtt.Font.Font, textBounds.Width, StringFormat.GenericTypographic);
                }
                else if (wt is SMWordImage)
                {
                    SMWordImage wti = wt as SMWordImage;
                    wti.rect.Size = wti.imageSize;
                }

                if (writeLineNo && !control.Autosize)
                {
                    if ((lineX + wt.rect.Width > lineEnd) || RunningLine == SMRunningLine.SingleWord)
                    {
                        if (currLine.Count > 0)
                            isNewLine = true;
                    }
                }

                if (isNewLine)
                {
                    if (currLine.Count == 0)
                        lineY += lineHeight * control.Paragraph.LineSpacing / 2;
                    else
                        lineY += lineHeight * control.Paragraph.LineSpacing;

                    currLine = new SMWordLine();
                    richLayout.Lines.Add(currLine);

                    lineHeight = context.g.MeasureString("M", control.GetUsedFont()).Height / 2;
                    lineNo++;

                    if (Columns != -1 && !control.Autosize)
                    {
                        if (lineY + lineHeight > textBounds.Bottom)
                        {
                            isNewColumn = true;
                        }
                    }
                }

                if (isNewPage)
                {
                    lineNo = 0;
                    columnNo = 0;
                    pageNo++;
                    lineY = textBounds.Top;
                }


                if (isNewColumn)
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

                if (isNewLine || isNewColumn || isNewPage)
                {
                    lineX = textBounds.X + columnNo * columnWidth + control.ContentPadding.Left;
                    lineEnd = lineX + lineWidth;
                }

                if (writeLineNo)
                {
                    if (currLine.Count == 0 && startsWithParentheses > 0)
                    {
                        wt.rect.X -= startsWithParentheses;
                        lineX -= startsWithParentheses;
                    }
                    if (currLine.Count > 0 || !isSpaceText)
                    {
                        currLine.Add(wt);

                        wt.LineNo = lineNo;
                        wt.ColumnNo = columnNo;
                        wt.PageNo = pageNo;
                        wt.rect.Location = new PointF(lineX, lineY);
                        lineX += wt.rect.Width;
                        rightX = Math.Max(rightX, (int)lineX);
                    }

                    lineHeight = Math.Max(lineHeight, wt.rect.Height);
                    writeLineNo = false;
                }
            }

            lineY += lineHeight * control.Paragraph.LineSpacing;

            // vertical alignment
            AdjustVerticaly(textBounds, richLayout.Lines, control.GetVerticalAlign());

            // horizontal aligment
            AdjustLinesHorizontaly((int)lineWidth, control.GetHorizontalAlign(), richLayout.Lines);

            richLayout.Pages = pageNo + 1;
            richLayout.bottomY = (int)lineY + 1;
            richLayout.rightX = rightX + 1;

            return richLayout;
        }

        private static void AdjustVerticaly(Rectangle textBounds, List<SMWordLine> drawWords, SMVerticalAlign valign)
        {
            int col = -1;
            int pg = -1;

            List<SMWordLine> lines = new List<SMWordLine>();
            foreach (SMWordLine wl in drawWords)
            {
                if (wl.Count > 0)
                {
                    if (col < 0) col = wl[0].ColumnNo;
                    if (pg < 0) pg = wl[0].PageNo;
                    if (wl[0].ColumnNo != col || wl[0].PageNo != pg)
                    {
                        AdjustLinesVerticaly(textBounds, lines, valign);
                        col = wl[0].ColumnNo;
                        pg = wl[0].PageNo;
                        lines.Clear();
                    }
                    lines.Add(wl);
                }
            }
            if (lines.Count > 0)
            {
                AdjustLinesVerticaly(textBounds, lines, valign);
            }
        }

        private static void AdjustLinesVerticaly(Rectangle textBounds, List<SMWordLine> drawWords, SMVerticalAlign valign)
        {
            float diff = 0f;
            float lineY = 0;
            float lineTop = 10000;
            foreach (SMWordLine wl in drawWords)
            {
                foreach (SMWordBase wb in wl)
                {
                    lineTop = Math.Min(wb.rect.Top, lineTop);
                    lineY = Math.Max(wb.rect.Bottom, lineY);
                }
            }
            switch (valign)
            {
                case SMVerticalAlign.Center:
                    diff = (textBounds.Bottom + textBounds.Top ) / 2 - (lineY + lineTop) / 2;
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

            if (this.Cardinality == SMConnectionCardinality.One && dc.draggedItem != null)
            {
                drawWords.Clear();
                DroppedItems.Clear();
                drawWordsModified = true;
                needUpdateDrawWords = true;
            }
            else if (this.Cardinality == SMConnectionCardinality.Many)
            {
                needUpdateDrawWords = true;
            }
            else
            {
                if (dc.draggedItem != null)
                {
                    foreach (SMWordBase wt in drawWords)
                    {
                        if (wt.rect.Contains(dc.lastPoint))
                        {
                            if (wt is SMWordToken)
                            {
                                SMWordToken wtk = (SMWordToken)wt;
                                if (wtk.Editable)
                                {
                                    wtk.AcceptString(dc.draggedItem.Tag);
                                    wtk.UIStateHover = false;
                                    drawWordsModified = true;
                                    return true;
                                }
                                else if (wtk.Cardinality == SMConnectionCardinality.One || wtk.Cardinality == SMConnectionCardinality.Many)
                                {
                                    wtk.UIStateHover = false;
                                    if (HasImmediateEvaluation && !wtk.tag.Equals(dc.draggedItem.Tag))
                                        return false;

                                    wtk.droppedItem = dc.draggedItem;
                                    drawWordsModified = true;
                                    return true;
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
                    wr = new SMWordImage(this.NormalState, this.HighlightState, this.Font, dc.draggedItem);
                }
                else
                {
                    wr = new SMWordText(this.NormalState, this.HighlightState, this.Font, dc.draggedItem);
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

        public override void TextDidChange()
        {
            drawWords = SMWordToken.WordListFromString(Text, this);
            drawWordsModified = true;
        }

        public List<string> ExpectedTags()
        {
            string[] s =  Tag.ToLower().Split(',');
            List<string> et = new List<string>();
            et.AddRange(s);
            foreach (SMWordBase wd in drawWords)
            {
                if (wd is SMWordToken)
                {
                    SMWordToken wt = (SMWordToken)wd;
                    if (wt.Cardinality == SMConnectionCardinality.One && wd.tag != null && wd.tag.Length > 0)
                        et.Add(wd.tag);
                }
            }
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
                SMWordToken item = new SMWordToken(this.NormalState, this.HighlightState, this.Font);
                item.text = w;
                item.tag = w.ToLower();
                item.Draggable = SMDragResponse.Drag;
                item.Cardinality = SMConnectionCardinality.None;
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
            else if (token.Equals("gotoNextEdit"))
            {
                SetNextEditableField();
            }
            else if (token.Equals("acceptString"))
            {
                SMWordToken edit = GetFocusedEditField();
                if (edit != null) edit.AcceptString(args.getSafe(0).getStringValue());
                drawWordsModified = true;
            }
            else if (token.Equals("acceptEnter"))
            {
                SetNextEditableField();
            }
            else if (token.Equals("acceptBack"))
            {
                SMWordToken edit = GetFocusedEditField();
                if (edit != null) edit.AcceptBack();
                drawWordsModified = true;
            }

            return base.ExecuteMessage(token, args);
        }

        public SMWordToken GetFocusedEditField()
        {
            foreach (SMWordBase wb in this.drawWords)
            {
                if (!(wb is SMWordToken))
                    continue;

                SMWordToken wt = (SMWordToken)wb;
                if (wt.Editable && wt.Focused)
                    return wt;
            }

            return null;

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

        public void SetNextEditableField()
        {
            SMWordToken firstEdit = null;
            bool catchEdit = false;
            SMWordToken nextEdit = null;
            foreach (SMWordBase wb in this.drawWords)
            {
                if (!(wb is SMWordToken))
                    continue;

                SMWordToken wt = (SMWordToken)wb;
                if (!wt.Editable) continue;


                if (firstEdit == null)
                    firstEdit = wt;
                
                if (catchEdit)
                {
                    catchEdit = false;
                    nextEdit = wt;
                    break;
                }

                if (wt.Focused)
                {
                    wt.Focused = false;
                    catchEdit = true;
                    continue;
                }
            }

            if (catchEdit)
                nextEdit = firstEdit;

            if (nextEdit != null)
                nextEdit.Focused = true;

        }

    }
}
