using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMWordToken
    {
        public int LineNo = 0;
        private Font font = null;
        public RectangleF rect = RectangleF.Empty;
        private Brush textBrush = Brushes.Black;
        public Image image = null;
        public string text = string.Empty;
        private SMDragResponse draggable = SMDragResponse.Undef;
        private SMDropResponse droppable = SMDropResponse.Undef;
        public string tag = string.Empty;
        public SMStyle style = null;
        public SMTokenItem droppedItem = null;
        public bool UIStateHover = false;
        public MNEvaluationResult UIStateError = MNEvaluationResult.NotEvaluated;
        public MNEvaluationType Evaluation = MNEvaluationType.Lazy;

        public Font Font
        {
            get
            {
                if (font != null) return font;
                if (style != null) return style.Font;
                return null;
            }
            set
            {
                font = value;
            }
        }
        public Brush TextBrush
        {
            get
            {
                if (textBrush == null && style != null)
                    textBrush = new SolidBrush(style.ForeColor);
                return textBrush;
            }
            set
            {
                textBrush = value;
            }
        }
        public SMDragResponse Draggable
        {
            get
            {
                if (draggable == SMDragResponse.Undef && style != null)
                    return style.Draggable;
                return draggable;
            }
            set
            {
                draggable = value;
            }
        }
        public SMDropResponse Droppable
        {
            get
            {
                if (droppable == SMDropResponse.Undef && style != null)
                    return style.Droppable;
                return droppable;
            }
            set
            {
                droppable = value;
            }
        }

        public bool IsDraggable
        {
            get
            {
                return Draggable == SMDragResponse.Drag || Draggable == SMDragResponse.Line;
            }
        }

        public SMWordToken()
        {
        }

        public SMWordToken(SMTokenItem item)
        {
            font = item.TextFont ?? font;
            textBrush = item.TextBrush ?? textBrush;
            text = item.Text ?? text;
            tag = item.Tag;
            image = item.Image;
        }

        public virtual SMTokenItem GetDraggableItem()
        {
            SMTokenItem item = new SMTokenItem();
            item.Tag = tag;
            item.Text = text;
            item.TextFont = font;
            item.TextBrush = textBrush;
            item.ContentSize = Size.Empty;
            return item;
        }

        public void Paint(MNPageContext context)
        {
            if (UIStateHover)
            {
                context.g.FillRectangle(SMGraphics.GetBrush(style.HighBackColor), this.rect);
            }
            if (droppedItem != null)
            {
                context.g.DrawString(droppedItem.Text, this.Font, this.TextBrush, this.rect.Location);
            }
            else
            {
                context.g.DrawString(this.text, this.Font, this.TextBrush, this.rect.Location);
            }
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
        public static List<SMWordToken> WordListFromString(string text, SMControl control)
        {
            List<SMWordToken> list = new List<SMWordToken>();
            TextParseMode mode = TextParseMode.General;
            StringBuilder word = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            TextTag tt = new TextTag();
            RunningFormat fmt = new RunningFormat();
            string argumentName = "";

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
                        if (word.Length > 0)
                        {
                            AppendWord(list, word.ToString(), control, fmt);
                            word.Clear();
                        }
                        AppendWord(list, " ", control, fmt);
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
                        AppendTag(list, word, tt, control, fmt);
                        tt.Clear();
                    }
                    else sb.Append(readedChar);
                }
                else if (mode == TextParseMode.WaitForArgOrEnd)
                {
                    if (readedChar == '>')
                    {
                        mode = TextParseMode.General;
                        AppendTag(list, word, tt, control, fmt);
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
                            AppendTag(list, word, tt, control, fmt);
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
                            AppendTag(list, word, tt, control, fmt);
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
                            AppendTag(list, word, tt, control, fmt);
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
                            AppendTag(list, word, tt, control, fmt);
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
                AppendWord(list, word.ToString(), control, fmt);
                word.Clear();
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
        }

        private static void AppendTag(List<SMWordToken> list, StringBuilder word, TextTag tt, SMControl control, RunningFormat fmt)
        {
            // TODO
            switch (tt.tag)
            {
                case "draggable":
                    fmt.dragResponse = SMDragResponse.Drag;
                    break;
                case "/draggable":
                    if (word.Length > 0)
                    {
                        AppendWord(list, word.ToString(), control, fmt);
                        word.Clear();
                    }
                    fmt.dragResponse = SMDragResponse.None;
                    break;
                case "drop":
                    {
                        SMWordToken wt = new SMWordToken();
                        wt.text = tt.attrs.ContainsKey("text") ? tt.attrs["text"] : "_____";
                        wt.tag = tt.attrs.ContainsKey("tag") ? tt.attrs["tag"] : "";
                        wt.style = control.Style;
                        wt.Droppable = SMDropResponse.One;
                        wt.Evaluation = control.HasLazyEvaluation ? MNEvaluationType.Lazy : MNEvaluationType.Immediate;
                        list.Add(wt);
                    }
                    break;
            }
        }

        private static void AppendWord(List<SMWordToken> list, string text, SMControl control, RunningFormat fmt)
        {
            // TODO
            SMWordToken wt = new SMWordToken();
            wt.text = text;
            wt.tag = text.ToLower();
            wt.textBrush = SMGraphics.GetBrush(control.Style.ForeColor);
            wt.style = control.Style;
            wt.Draggable = fmt.dragResponse;
            wt.Evaluation = MNEvaluationType.None;
            list.Add(wt);
        }
    }
}
