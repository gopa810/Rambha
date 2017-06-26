using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMTextContainer: SMControl
    {
        public List<SMTextContainerWord> drawWords = new List<SMTextContainerWord>();
        public List<SMTextContainerLine> drawLines = new List<SMTextContainerLine>();
        public bool drawWordsModified = false;


        public SMStatusLayout ItemLayout { get; set; }
        public SMContentPadding ItemPadding { get; set; }
        public SMContentPadding ItemMargin { get; set; }

        private string p_prevText = "";


        public SMTextContainer(MNPage p)
            : base(p)
        {
            Text = "Text Container";
            Evaluation = MNEvaluationType.Inherited;
            ItemLayout = new SMStatusLayout();
            ItemMargin = new SMContentPadding();
            ItemMargin.Left = ItemMargin.Right = 8;
            ItemMargin.Top = ItemMargin.Bottom = 8;
            ItemPadding = new SMContentPadding();
            ItemPadding.Left = ItemPadding.Right = 8;
            ItemPadding.Top = ItemPadding.Bottom = 8;
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
                            br.ReadInt32();
                            break;
                        case 11:
                            br.ReadInt32();
                            break;
                        case 12:
                            br.ReadFloat();
                            break;
                        case 13:
                            br.ReadBool();
                            break;
                        case 14:
                            br.ReadInt32();
                            break;
                        case 15:
                            ItemLayout.Load(br);
                            break;
                        case 16:
                            ItemMargin.Load(br);
                            break;
                        case 17:
                            ItemPadding.Load(br);
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

            bw.WriteByte(15);
            ItemLayout.Save(bw);

            bw.WriteByte(16);
            ItemMargin.Save(bw);

            bw.WriteByte(17);
            ItemPadding.Save(bw);

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

            SMStatusLayout layout;

            if (IsDraggable())
            {
                SMDragResponse dr = Draggable;
                Draggable = SMDragResponse.None;
                layout = PrepareBrushesAndPens();
                Draggable = dr;
            }
            else
            {
                layout = PrepareBrushesAndPens();
            }

            DrawStyledBackground(context, layout, bounds);
            DrawStyledBorder(context, layout, bounds);

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

                SplitTextToWords(stext);
            }

            if (drawWordsModified)
            {
                RecalculateWordsLayout(context, textBounds.Size, drawWords);

                drawWordsModified = false;
            }

            context.g.DrawRectangle(Pens.Black, textBounds);
            PaintPageNo(context, IsDraggable() ? SMGraphics.draggableLayoutN : layout, textBounds.X, textBounds.Y);

            // draw selection marks
            base.Paint(context);
        }

        private void SplitTextToWords(string stext)
        {
            StringBuilder sb = new StringBuilder();
            int mode = 0;

            drawWords.Clear();
            drawWordsModified = true;

            foreach (char c in stext)
            {
                if (mode == 0)
                {
                    if (c == '\"')
                        mode = 1;
                    else if (Char.IsSeparator(c))
                    {
                        if (sb.Length > 0)
                            AddWord(sb.ToString());
                        sb.Clear();
                    }
                    else
                        sb.Append(c);
                }
                else if (mode == 1)
                {
                    if (c == '\"')
                    {
                        AddWord(sb.ToString());
                        sb.Clear();
                        mode = 0;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            if (sb.Length > 0)
                AddWord(sb.ToString());
        }

        public void AddWord(string s)
        {
            drawWords.Add(new SMTextContainerWord(this, s));
        }

        public void PaintPageNo(MNPageContext context, SMStatusLayout layout, int X, int Y)
        {
            Brush backgroundBrush = SMGraphics.GetBrush(layout.BackColor);
            Brush highBackgroundBrush = SMGraphics.GetBrush(Color.LightGray);
            Brush textBrush = SMGraphics.GetBrush(layout.ForeColor);

            foreach (SMTextContainerLine wline in drawLines)
            {
                foreach (SMTextContainerWord wt in wline)
                {
                    Rectangle r = wt.rect;
                    r.Offset(X, Y);
                    context.g.DrawFillRoundedRectangle(Pens.Black, wt.Used ? highBackgroundBrush : backgroundBrush, r, 5);
                    context.g.DrawString(wt.text, Font.Font, textBrush, r, SMGraphics.StrFormatCenter);
                }
            }

        }


        public override SMTokenItem GetDraggableItem(Point p)
        {
            Point point = p;
            point.X -= this.Area.Left;
            point.Y -= this.Area.Top;
            foreach (SMTextContainerWord wt in drawWords)
            {
                if (wt.rect.Contains(point))
                {
                    SMTokenItem ti = new SMTokenItem();
                    ti.Text = wt.text;
                    ti.Tag = wt.tag;
                    return ti;
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
        public void RecalculateWordsLayout(MNPageContext context, Size textBoundsSize, List<SMTextContainerWord> drawWords)
        {
            Rectangle textBounds = Rectangle.Empty;
            textBounds.Size = textBoundsSize;
            float lineY = 0;
            float lineX = ItemMargin.Left;
            float maxX = textBoundsSize.Width - ItemMargin.Right;

            SMTextContainerLine currLine = new SMTextContainerLine();
            drawLines.Clear();
            drawLines.Add(currLine);

            // first placement of word tokens
            foreach (SMTextContainerWord wt in drawWords)
            {
                SizeF szf = context.g.MeasureString(wt.text, Font.Font, textBounds.Width, StringFormat.GenericDefault);

                wt.rect.Width = (int)szf.Width + ItemPadding.Left + ItemPadding.Right;
                wt.rect.Height = (int)szf.Height + ItemPadding.Top + ItemPadding.Bottom;

                if (((lineX + wt.rect.Width + ItemMargin.Left) > maxX) 
                    && currLine.Count > 0)
                {
                    lineX = ItemMargin.Left;
                    lineY += wt.rect.Height + ItemMargin.Top + ItemMargin.Bottom;
                    currLine = new SMTextContainerLine();
                    drawLines.Add(currLine);
                }

                wt.rect.X = (int)lineX;
                wt.rect.Y = (int)lineY;
                lineX += wt.rect.Width + ItemMargin.Left + ItemMargin.Right;
                currLine.Add(wt);
            }

            // vertical alignment
            AdjustLinesVerticaly(textBounds, drawLines, GetVerticalAlign());

            // horizontal aligment
            AdjustLinesHorizontaly((int)textBounds.Width, GetHorizontalAlign(), drawLines);

        }

        private static void AdjustLinesVerticaly(Rectangle textBounds, List<SMTextContainerLine> drawWords, SMVerticalAlign valign)
        {
            float diff = 0f;
            float textBottom = 0;
            float textTop = 10000;
            foreach (SMTextContainerLine line in drawWords)
            {
                foreach (SMTextContainerWord wb in line)
                {
                    textTop = Math.Min(wb.rect.Top, textTop);
                    textBottom = Math.Max(wb.rect.Bottom, textBottom);
                }
            }

            switch (valign)
            {
                case SMVerticalAlign.Center:
                    diff = (textBounds.Bottom + textBounds.Top ) / 2 - (textBottom + textTop) / 2;
                    break;
                case SMVerticalAlign.Bottom:
                    diff = (textBounds.Bottom - textBottom);
                    break;
            }

            if (diff != 0)
            {
                foreach (SMTextContainerLine wt in drawWords)
                {
                    foreach (SMTextContainerWord wbase in wt)
                    {
                        wbase.rect.Y += (int)diff;
                    }
                }
            }
        }

        private void AdjustLinesHorizontaly(int areaWidth, SMHorizontalAlign align, List<SMTextContainerLine> lines)
        {
            int i = 0;
            int max = lines.Count;
            for (i = 0; i < max; i++)
            {
                AdjustLineWords(lines[i], areaWidth, align, i < max -1);
            }

        }

        private void AdjustLineWords(SMTextContainerLine line, float areaWidth, 
            SMHorizontalAlign align, bool lastLine)
        {
            float totalWidth = 0;
            foreach (SMTextContainerWord wb in line)
            {
                totalWidth += wb.rect.Width + ItemMargin.LeftRight;
            }

            if (line.Count > 0)
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
                    adjustmentStep = (areaWidth - totalWidth) / (line.Count + 1);
                }
                if (doAdjust)
                {
                    foreach (SMTextContainerWord wat in line)
                    {
                        wat.rect.X += (int)adjustment;
                        wat.rect.Width += (int)adjustmentStep;
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
                if (et.Count > 0 && et.IndexOf(dc.draggedItem.Tag.ToLower()) < 0)
                {
                    return false;
                }

                foreach (SMTextContainerWord tcw in drawWords)
                {
                    if (tcw.text.Equals(dc.draggedItem.Text, StringComparison.CurrentCultureIgnoreCase))
                        return false;
                }
            }

            if (this.Cardinality == SMConnectionCardinality.One && dc.draggedItem != null)
            {
                drawWords.Clear();
                drawWordsModified = true;
                needUpdateDrawWords = true;
            }
            else if (this.Cardinality == SMConnectionCardinality.Many)
            {
                needUpdateDrawWords = true;
            }

            if (base.OnDropFinished(dc) && needUpdateDrawWords)
            {
                SMTextContainerWord wr = null;
                if (dc.draggedItem.Text != null && dc.draggedItem.Text.Length > 0)
                {
                    wr = new SMTextContainerWord(this, dc.draggedItem.Text);
                }
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
                drawWords[idx].Used = true;
                //drawWords.RemoveAt(idx);
                //drawWordsModified = true;
            }
        }

        public int IndexOfDroppedItem(string text, string tag)
        {
            int index = 0;
            foreach (SMTextContainerWord word in drawWords)
            {
                if (word.text.Equals(text, StringComparison.CurrentCultureIgnoreCase)
                    && word.tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase))
                {
                    return index;
                }
                index++;
            }

            return -1;
        }


        public override void ResetStatus()
        {
            drawWords.Clear();
            drawLines.Clear();
            p_prevText = "";
            base.ResetStatus();
        }

        /*public override void OnDropMove(PVDragContext context)
        {
            if (context.draggedItem != null)
            {
                foreach (SMWordBase wt in drawWords)
                {
                    wt.HoverPoint(context.lastPoint);
                }
            }

            base.OnDropMove(context);
        }*/

        public override void TextDidChange()
        {
            SplitTextToWords(Text);
            drawWordsModified = true;
        }

        public List<string> ExpectedTags()
        {
            string trimmedTag = Tag.Trim();
            string[] s;
            List<string> et = new List<string>();

            if (trimmedTag.Length > 0)
            {
                s = trimmedTag.ToLower().Split('|');
                et.AddRange(s);
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

                    foreach (SMTextContainerWord wt in drawWords)
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

                    if (s.Count > 0)
                        UIStateError = MNEvaluationResult.Incorrect;
                }
            }

            return base.Evaluate();
        }

        public override void SaveStatus(RSFileWriter bw)
        {
            base.SaveStatusCore(bw);

            foreach (SMTextContainerWord tcw in drawWords)
            {
                bw.WriteByte(20);
                bw.WriteString(tcw.text);
            }

            bw.WriteByte(0);
        }

        public override void LoadStatus(RSFileReader br)
        {
            base.LoadStatusCore(br);

            drawWords.Clear();
            drawWordsModified = true;
            byte b = 0;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 20:
                        string word = br.ReadString();
                        SMTextContainerWord tcw = new SMTextContainerWord(this, word);
                        drawWords.Add(tcw);
                        break;
                    default:
                        break;
                }
            }


        }

        public override void DisplayAnswers()
        {
            drawWords.Clear();
            foreach (string w in ExpectedTags())
            {
                SMTextContainerWord item = new SMTextContainerWord(this, w);
                drawWords.Add(item);
            }
            base.DisplayAnswers();
        }

        public override GSCore GetPropertyValue(string s)
        {
            return base.GetPropertyValue(s);
        }


        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            return base.ExecuteMessage(token, args);
        }

        protected override GSCore ExecuteMessageSet(GSCore a1, GSCore a2, GSCoreCollection args)
        {
            return base.ExecuteMessageSet(a1, a2, args);
        }

        public override void OnTapBegin(PVDragContext dc)
        {
            base.OnTapBegin(dc);
        }

        public override void OnTapEnd(PVDragContext dc)
        {
            base.OnTapEnd(dc);
        }

        public override void OnTapCancel(PVDragContext dc)
        {
            base.OnTapCancel(dc);
        }

        public void ContentToTags()
        {
            StringBuilder sb = new StringBuilder();
            Text = "";
            foreach (SMTextContainerWord stw in drawWords)
            {
                if (stw.text.Length > 0)
                {
                    if (sb.Length > 0)
                        sb.Append("|");
                    sb.Append(stw.text);
                }
            }
            Tag = sb.ToString();
        }
    }

    public class SMTextContainerWord
    {
        public SMTextContainer Parent = null;
        public int LineNo = 0;
        public int ColumnNo = 0;
        public int PageNo = 0;
        public Rectangle rect = Rectangle.Empty;
        private string p_tag = string.Empty;
        public int lineOffset = 0;
        public string text = string.Empty;

        public string tag
        {
            get
            {
                if ((p_tag == null || p_tag.Length == 0) && text.Length > 0)
                    p_tag = text.ToLower();
                return p_tag;
            }
            set
            {
                p_tag = value;
            }
        }

        public SMTextContainerWord(SMTextContainer parent, string tx)
        {
            Parent = parent;
            text = tx;
            Used = false;
        }

        public bool Used { get; set; }
    }

    public class SMTextContainerLine : List<SMTextContainerWord>
    {
    }
}
