using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMRichText
    {
        //private SMControl p_privControl = null;
        private string p_prevText = "";
        private int p_prevWidth = -1;
        private List<SMWordBase> drawWords = null;
        private List<SMWordLine> drawLines = null;
        private SMRichLayout richLayout = null;
        private SMRunningLine runLine = SMRunningLine.Natural;
        public int Columns { get; set; }
        public int ColumnSeparatorWidth { get; set; }


        public SMRunningLine RunningLine
        {
            get
            {
                return runLine;
            }
            set
            {
                runLine = value;
                p_prevText = "";
                p_prevWidth = -1;
            }
        }

        public SMFont Font = new SMFont();
        public SMParaFormat Paragraph = new SMParaFormat();
        public MNEvaluationType EvaluationType = MNEvaluationType.None;
        public bool Autosize = false;

        public SMRichText(SMControl parent)
        {
            Font = parent.Font;
            Paragraph = parent.Paragraph;
            Columns = -1;
            ColumnSeparatorWidth = 20;
        }

        public SMRichText()
        {
        }

        /// <summary>
        /// Checks if recalculation is needed.
        /// </summary>
        /// <param name="plainText">Input text to be checked against last calculated string</param>
        /// <returns>Returns True, if text is different from last calculated string</returns>
        public bool CheckTextChanges(string plainText, int width)
        {
            return !p_prevText.Equals(plainText) || (p_prevWidth != width);
        }

        public Size MeasureString(MNPageContext context, string plainText, int width)
        {
            if (width < 0) width = 5000;
            if (!p_prevText.Equals(plainText) || (p_prevWidth != width) || drawWords == null || richLayout == null)
            {
                p_prevText = plainText;
                drawWords = WordListFromString(plainText);
                Rectangle textBounds = new Rectangle(0, 0, width, 100);
                richLayout = RecalculateWordsLayout(context, textBounds);
                drawLines = richLayout.Lines;
            }

            return new Size(richLayout.rightX, richLayout.bottomY);
        }

        public void DrawString(MNPageContext context, SMStatusLayout layout, Rectangle textBounds)
        {
            DrawString(context, layout, p_prevText, textBounds, 0);
        }

        public void DrawString(MNPageContext context, SMStatusLayout layout, string plainText, Rectangle textBounds)
        {
            DrawString(context, layout, plainText, textBounds, 0);
        }

        public void DrawString(MNPageContext context, SMStatusLayout layout, string plainText, Rectangle textBounds, int nPage)
        {
            if (!p_prevText.Equals(plainText) || (p_prevWidth != textBounds.Width) || drawWords == null)
            {
                p_prevText = plainText;
                drawWords = WordListFromString(plainText);
                richLayout = RecalculateWordsLayout(context, textBounds);
            }

            foreach (SMWordBase wt in drawWords)
            {
                if (wt.PageNo == nPage)
                    wt.Paint(context, layout, textBounds.X, textBounds.Y);
            }
        }

        public void ForceRecalc()
        {
            p_prevText = "";
            p_prevWidth = -1;
            drawWords = null;
        }

        /// <summary>
        /// Text is normal text and contains words, spaces and tags
        /// tags are like HTML tags
        /// examples are given bellow 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        // examples of tags:
        // <tag>
        // <tag attr1=val1>
        // <tag attr2="val1" attr3='val3' attr4=val5>
        public List<SMWordBase> WordListFromString(string text)
        {
            List<SMWordBase> list = new List<SMWordBase>();
            TextParseMode mode = TextParseMode.General;
            StringBuilder word = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            RunningFormat fmt = new RunningFormat();

            TextTag tt = new TextTag();
            string argumentName = "";
            fmt.SetFontStyle(Font.Style);
            fmt.fontSize = Font.Size;
            fmt.fontName = Font.Name;
            fmt.defaultFontSize = fmt.fontSize;

            foreach (char readedChar in text)
            {
                if (mode == TextParseMode.General)
                {
                    if (readedChar == '&')
                    {
                        mode = TextParseMode.SpecChar;
                        sb.Clear();
                        continue;
                    }
                    else if (readedChar == '<')
                    {
                        mode = TextParseMode.WaitForTagNameStart;
                        sb.Clear();
                        continue;
                    }
                    else if (Char.IsWhiteSpace(readedChar))
                    {
                        ClearWordBuffer(list, word, fmt);
                        if (readedChar == '\n')
                            list.Add(new SMWordSpecial(Font) { Type = SMWordSpecialType.Newline });
                        else if (readedChar == '\r')
                        {
                        }
                        else
                            AppendWord(list, " ", fmt);
                    }
                    else
                    {
                        word.Append(readedChar);
                    }
                }
                else if (mode == TextParseMode.SpecChar)
                {
                    if (readedChar == ';') { word.Append(GetCharFromCode(sb.ToString())); mode = TextParseMode.General; }
                    else sb.Append(readedChar);
                }
                else if (mode == TextParseMode.WaitForTagNameStart)
                {
                    if (readedChar == '>') mode = TextParseMode.General;
                    else if (!Char.IsWhiteSpace(readedChar)) { sb.Append(readedChar); mode = TextParseMode.ReadTagName; }
                }
                else if (mode == TextParseMode.ReadTagName)
                {
                    if (Char.IsWhiteSpace(readedChar))
                    {
                        tt.tag = sb.ToString();
                        sb.Clear();
                        mode = TextParseMode.WaitForArgOrEnd;
                    }
                    else if (readedChar == '>')
                    {
                        tt.tag = sb.ToString();
                        mode = TextParseMode.General;
                        AppendTag(list, word, tt, fmt);
                        tt.Clear();
                    }
                    else sb.Append(readedChar);
                }
                else if (mode == TextParseMode.WaitForArgOrEnd)
                {
                    if (readedChar == '>')
                    {
                        mode = TextParseMode.General;
                        AppendTag(list, word, tt, fmt);
                        tt.Clear();
                    }
                    else if (Char.IsWhiteSpace(readedChar))
                    {
                    }
                    else
                    {
                        sb.Clear();
                        sb.Append(readedChar);
                        mode = TextParseMode.ReadArgName;
                    }
                }
                else if (mode == TextParseMode.ReadArgName)
                {
                    if (readedChar == '>')
                    {
                        mode = TextParseMode.General;
                        if (sb.Length > 0)
                        {
                            tt.attrs.Add(sb.ToString(), string.Empty);
                            AppendTag(list, word, tt, fmt);
                            tt.Clear();
                        }
                    }
                    else if (readedChar == '=')
                    {
                        argumentName = sb.ToString();
                        sb.Clear();
                        mode = TextParseMode.WaitForArgValue;
                    }
                    else if (Char.IsWhiteSpace(readedChar))
                    {
                        argumentName = sb.ToString();
                        sb.Clear();
                        mode = TextParseMode.WaitForAssignOrEnd;
                    }
                    else
                    {
                        sb.Append(readedChar);
                    }
                }
                else if (mode == TextParseMode.WaitForAssignOrEnd)
                {
                    if (readedChar == '=')
                    {
                        mode = TextParseMode.WaitForArgValue;
                    }
                    else if (readedChar == '>')
                    {
                        mode = TextParseMode.General;
                        if (argumentName.Length > 0)
                        {
                            tt.attrs.Add(argumentName, string.Empty);
                            AppendTag(list, word, tt, fmt);
                            tt.Clear();
                        }
                    }
                    else if (!Char.IsWhiteSpace(readedChar))
                    {
                        mode = TextParseMode.ReadArgName;
                        if (argumentName.Length > 0)
                        {
                            tt.attrs.Add(argumentName, string.Empty);
                        }
                        sb.Clear();
                        sb.Append(readedChar);
                    }
                }
                else if (mode == TextParseMode.WaitForArgValue)
                {
                    if (readedChar == '>')
                    {
                        mode = TextParseMode.General;
                        if (argumentName.Length > 0)
                        {
                            tt.attrs.Add(argumentName, string.Empty);
                            AppendTag(list, word, tt, fmt);
                            tt.Clear();
                        }
                    }
                    else if (readedChar == '\"')
                    {
                        sb.Clear();
                        mode = TextParseMode.ReadArgValueString;
                    }
                    else if (readedChar == '\'')
                    {
                        sb.Clear();
                        mode = TextParseMode.ReadArgValueQuote;
                    }
                    else if (!Char.IsWhiteSpace(readedChar))
                    {
                        sb.Clear();
                        sb.Append(readedChar);
                        mode = TextParseMode.ReadArgValue;
                    }
                }
                else if (mode == TextParseMode.ReadArgValue)
                {
                    if (readedChar == '>')
                    {
                        mode = TextParseMode.General;
                        if (argumentName.Length > 0)
                        {
                            tt.attrs.Add(argumentName, sb.ToString());
                            AppendTag(list, word, tt, fmt);
                            tt.Clear();
                        }
                    }
                    else if (readedChar == '&')
                    {
                        sb2.Clear();
                        mode = TextParseMode.ReadArgValueSpecChar;
                    }
                    else if (Char.IsWhiteSpace(readedChar))
                    {
                        mode = TextParseMode.WaitForArgOrEnd;
                        if (argumentName.Length > 0)
                        {
                            tt.attrs.Add(argumentName, sb.ToString());
                            sb.Clear();
                            argumentName = "";
                        }
                    }
                    else
                    {
                        sb.Append(readedChar);
                    }
                }
                else if (mode == TextParseMode.ReadArgValueSpecChar)
                {
                    if (readedChar == ';') { sb.Append(GetCharFromCode(sb2.ToString())); mode = TextParseMode.ReadArgValue; }
                    else sb2.Append(readedChar);
                }
                else if (mode == TextParseMode.ReadArgValueString)
                {
                    if (readedChar == '&')
                    {
                        sb2.Clear();
                        mode = TextParseMode.ReadArgValueStringSpecChar;
                    }
                    else if (readedChar == '\"')
                    {
                        mode = TextParseMode.WaitForArgOrEnd;
                        if (argumentName.Length > 0)
                        {
                            tt.attrs.Add(argumentName, sb.ToString());
                            sb.Clear();
                            argumentName = "";
                        }
                    }
                    else
                    {
                        sb.Append(readedChar);
                    }
                }
                else if (mode == TextParseMode.ReadArgValueStringSpecChar)
                {
                    if (readedChar == ';') { sb.Append(GetCharFromCode(sb2.ToString())); mode = TextParseMode.ReadArgValueString; }
                    else sb2.Append(readedChar);
                }
                else if (mode == TextParseMode.ReadArgValueQuote)
                {
                    if (readedChar == '&')
                    {
                        sb2.Clear();
                        mode = TextParseMode.ReadArgValueQuoteSpecChar;
                    }
                    else if (readedChar == '\"')
                    {
                        mode = TextParseMode.WaitForArgOrEnd;
                        if (argumentName.Length > 0)
                        {
                            tt.attrs.Add(argumentName, sb.ToString());
                            sb.Clear();
                            argumentName = "";
                        }
                    }
                    else
                    {
                        sb.Append(readedChar);
                    }
                }
                else if (mode == TextParseMode.ReadArgValueQuoteSpecChar)
                {
                    if (readedChar == ';') { sb.Append(GetCharFromCode(sb2.ToString())); mode = TextParseMode.ReadArgValueQuote; }
                    else sb2.Append(readedChar);
                }
            }

            // finalization
            if (word.Length > 0)
            {
                AppendWord(list, word.ToString(), fmt);
                word.Clear();
            }

            // set first editable as focused
            foreach (SMWordBase wb in list)
            {
                if (wb is SMWordToken)
                {
                    SMWordToken wt = (SMWordToken)wb;
                    if (wt.Editable)
                    {
                        wt.Focused = true;
                        break;
                    }
                }
            }

            return list;
        }

        public static char GetCharFromCode(string charCode)
        {
            switch (charCode)
            {
                case "amp": return '&';
                case "lt": return '<';
                case "gt": return '>';
                case "quote": return '\'';
                case "dquote": return '\"';
                case "nl": return '\n';
                case "nbsp": return Convert.ToChar(0xA0);
                default:
                    {
                        if (charCode.StartsWith("#x"))
                        {
                            int a = Convert.ToInt32(charCode.Substring(2), 16);
                            return Convert.ToChar(a);
                        }
                        else if (charCode.StartsWith("#"))
                        {
                            int a = Convert.ToInt32(charCode.Substring(1), 10);
                            return Convert.ToChar(a);
                        }
                        else
                        {
                            return ' ';
                        }
                    }
            }
        }

        private class RunningFormat
        {
            public SMDragResponse dragResponse = SMDragResponse.None;
            public bool Bold = false;
            public bool Italic = false;
            public bool Underline = false;
            public bool Strikeout = false;
            public FontStyle fontStyle
            {
                get
                {
                    FontStyle fs = FontStyle.Regular;
                    if (Bold) fs |= FontStyle.Bold;
                    if (Italic) fs |= FontStyle.Italic;
                    if (Underline) fs |= FontStyle.Underline;
                    if (Strikeout) fs |= FontStyle.Strikeout;
                    return fs;
                }
            }
            public float defaultFontSize = 20;
            public float fontSize = 20;
            public MNFontName fontName = MNFontName.LucidaSans;
            public float lineOffset = 0;

            public SMFont GetFont()
            {
                return SMGraphics.GetVirtFontVariation(fontName, fontSize, Bold, Italic, Underline, Strikeout);
            }

            internal void SetFontStyle(FontStyle fontStyle)
            {
                Bold = ((fontStyle & FontStyle.Bold) != 0);
                Italic = ((fontStyle & FontStyle.Italic) != 0);
                Underline = ((fontStyle & FontStyle.Underline) != 0);
                Strikeout = ((fontStyle & FontStyle.Strikeout) != 0);
            }
        }

        private void AppendTag(List<SMWordBase> list, StringBuilder word, TextTag tt, RunningFormat fmt)
        {
            // TODO
            switch (tt.tag)
            {
                case "draggable":
                    fmt.dragResponse = SMDragResponse.Drag;
                    break;
                case "/draggable":
                    ClearWordBuffer(list, word, fmt);
                    fmt.dragResponse = SMDragResponse.None;
                    break;
                case "drop":
                    {
                        SMWordToken wt = new SMWordToken(Font);
                        wt.text = tt.attrs.ContainsKey("text") ? tt.attrs["text"] : "_____";
                        wt.tag = tt.attrs.ContainsKey("tag") ? tt.attrs["tag"] : "";
                        wt.Draggable = SMDragResponse.None;
                        wt.Cardinality = SMConnectionCardinality.One;
                        wt.Evaluation = EvaluationType;
                        list.Add(wt);
                    }
                    break;
                case "edit":
                    {
                        SMWordToken wt = new SMWordToken(Font);
                        wt.text = tt.attrs.ContainsKey("text") ? tt.attrs["text"] : "_____";
                        wt.tag = tt.attrs.ContainsKey("tag") ? tt.attrs["tag"] : "";
                        wt.Draggable = SMDragResponse.None;
                        wt.Editable = true;
                        wt.Cardinality = SMConnectionCardinality.One;
                        wt.Evaluation = EvaluationType;
                        list.Add(wt);
                    }
                    break;
                case "page":
                    ClearWordBuffer(list, word, fmt);
                    list.Add(new SMWordSpecial(Font) { Type = SMWordSpecialType.NewPage });
                    break;
                case "hr":
                    ClearWordBuffer(list, word, fmt);
                    list.Add(new SMWordSpecial(Font) { Type = SMWordSpecialType.HorizontalLine });
                    break;
                case "br":
                    ClearWordBuffer(list, word, fmt);
                    list.Add(new SMWordSpecial(Font) { Type = SMWordSpecialType.Newline });
                    AppendWord(list, "\n", fmt);
                    break;
                case "col":
                    ClearWordBuffer(list, word, fmt);
                    list.Add(new SMWordSpecial(Font) { Type = SMWordSpecialType.NewColumn });
                    //AppendWord(list, "\n", control, fmt);
                    break;
                case "r":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Bold = false;
                    fmt.Italic = false;
                    fmt.Strikeout = false;
                    fmt.Underline = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/r":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Bold = Font.Bold;
                    fmt.Italic = Font.Italic;
                    fmt.Underline = Font.Underline;
                    fmt.Strikeout = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "b":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Bold = true;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/b":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Bold = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "i":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Italic = true;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/i":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Italic = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "u":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Underline = true;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/u":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Underline = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "so":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Strikeout = true;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/so":
                    ClearWordBuffer(list, word, fmt);
                    fmt.Strikeout = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                default:
                    if (tt.tag.StartsWith("fs"))
                    {
                        int arg = 0;
                        if (int.TryParse(tt.tag.Substring(2), out arg))
                        {
                            fmt.fontSize = (float)arg / 100 * fmt.defaultFontSize;
                            fmt.lineOffset = (float)arg/100 - 1f;
                        }
                    }
                    break;
            }
        }

        private void ClearWordBuffer(List<SMWordBase> list, StringBuilder word, RunningFormat fmt)
        {
            if (word.Length > 0)
            {
                AppendWord(list, word.ToString(), fmt);
                word.Clear();
            }
        }

        private void AppendWord(List<SMWordBase> list, string text, RunningFormat fmt)
        {
            // TODO

            SMWordText wt = new SMWordText(Font);
            wt.text = text;
            wt.tag = text.Trim();
            wt.Draggable = fmt.dragResponse;
            wt.Evaluation = MNEvaluationType.None;
            wt.Font = fmt.GetFont();
            wt.lineOffset = Convert.ToInt32(-55 * fmt.lineOffset);
            list.Add(wt);
        }

        private class TextTag
        {
            public string tag = "";
            public Dictionary<string, string> attrs = new Dictionary<string, string>();
            public void Clear()
            {
                tag = string.Empty;
                attrs.Clear();
            }
        }



        private enum TextParseMode
        {
            General,
            SpecChar,
            WaitForTagNameStart,
            ReadTagName,
            WaitForArgOrEnd,
            ReadArgName,
            WaitForAssignOrEnd,
            WaitForArgValue,
            ReadArgValue,
            ReadArgValueString,
            ReadArgValueQuote,
            ReadArgValueSpecChar,
            ReadArgValueStringSpecChar,
            ReadArgValueQuoteSpecChar
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
        public SMRichLayout RecalculateWordsLayout(MNPageContext context, Rectangle textBounds)
        {
            textBounds.X = 0;
            textBounds.Y = 0;
            float lineY = textBounds.Y;
            float lineX = textBounds.X + ColumnSeparatorWidth / 2;
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
                lineWidth = textBounds.Width / Columns - ColumnSeparatorWidth;
                columnWidth = textBounds.Width / Columns;
                lineX = textBounds.X + columnNo * columnWidth + ColumnSeparatorWidth / 2;
                lineEnd = lineX + lineWidth;
            }
            else
            {
                lineWidth = textBounds.Width;
                columnWidth = textBounds.Width;
                lineX = textBounds.X;
                lineEnd = lineX + lineWidth;
            }

            float bottom = textBounds.Bottom;
            bool isSpaceText = false;
            bool isNewPage = false;
            int startsWithParentheses = 0;

            // first placement of word tokens
            foreach (SMWordBase wt in drawWords)
            {
                isSpaceText = false;
                writeLineNo = true;
                isNewLine = false;
                isNewPage = false;
                isNewColumn = false;
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
                        writeLineNo = false;
                        isNewPage = true;
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

                if (writeLineNo && !Autosize)
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
                        lineY += lineHeight * Paragraph.LineSpacing / 2;
                    else
                        lineY += lineHeight * Paragraph.LineSpacing;

                    currLine = new SMWordLine();
                    richLayout.Lines.Add(currLine);

                    lineHeight = context.g.MeasureString("M", Font.Font).Height /2;
                    lineNo++;

                    if (Columns != -1 && !Autosize)
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
                    if (Columns > 1)
                        lineX = textBounds.X + columnNo * columnWidth + ColumnSeparatorWidth / 2;
                    else
                        lineX = textBounds.X;
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

            lineY += lineHeight * Paragraph.LineSpacing;

            // vertical alignment
            AdjustVerticaly(textBounds, richLayout.Lines, Paragraph.VertAlign);

            // horizontal aligment
            AdjustLinesHorizontaly((int)(lineWidth - ColumnSeparatorWidth), Paragraph.Align, richLayout.Lines);

            richLayout.Pages = pageNo + 1;
            richLayout.bottomY = (int)lineY + 1;
            richLayout.rightX = rightX + 1;

            return richLayout;
        }

        private void AdjustVerticaly(Rectangle textBounds, List<SMWordLine> drawWords, SMVerticalAlign valign)
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

        private void AdjustLinesVerticaly(Rectangle textBounds, List<SMWordLine> drawWords, SMVerticalAlign valign)
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
                    diff = (textBounds.Bottom + textBounds.Top) / 2 - (lineY + lineTop) / 2;
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

        private void AdjustLinesHorizontaly(int areaWidth, SMHorizontalAlign align, List<SMWordLine> drawWords)
        {
            int i = 0;
            int max = drawWords.Count;
            for (i = 0; i < max; i++)
            {
                AdjustLineWords(drawWords[i], areaWidth, align, i < max - 1);
            }

        }

        private void AdjustLineWords(List<SMWordBase> drawWords, float areaWidth,
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


        public Rectangle[] CalcRectangles(MNPageContext context, string plainText, Rectangle textBounds)
        {
            p_prevText = plainText;
            drawWords = WordListFromString(plainText);
            richLayout = RecalculateWordsLayout(context, textBounds);
            drawLines = richLayout.Lines;

            List<Rectangle> ra = new List<Rectangle>();
            foreach (SMWordBase wb in drawWords)
            {
                if (wb is SMWordText)
                {
                    ra.Add(new Rectangle((int)wb.rect.X, (int)wb.rect.Y, (int)wb.rect.Width, (int)wb.rect.Height));
                }
            }

            return ra.ToArray<Rectangle>();
        }

    }
}
