using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMWordToken: SMWordTextBase
    {
        private SMConnectionCardinality droppable = SMConnectionCardinality.Undef;
        public SMTokenItem droppedItem = null;
        public MNEvaluationResult UIStateError = MNEvaluationResult.NotEvaluated;
        public string text = string.Empty;
        public bool Editable = false;
        public bool Focused = false;
        public string editedText = "";


        public SMConnectionCardinality Cardinality
        {
            get { return droppable; }
            set { droppable = value; }
        }

        public SMWordToken(SMFont f)
            : base(f)
        {
        }

        public string GetCurrentText()
        {
            if (Editable)
            {
                return editedText + (editedText.Length < text.Length ? text.Substring(editedText.Length) : "") + (Focused ? " " : "");
            }
            else
            {
                if (droppedItem != null)
                {
                    return droppedItem.Text;
                }
                else
                {
                    return this.text;
                }
            }
        }

        public override Brush GetCurrentTextBrush(SMStatusLayout layout)
        {
            if (Editable)
            {
                if (tag != null && editedText != null)
                {
                    int a = tag.Length;
                    int b = editedText.Length;
                    int c = Math.Min(a, b);
                    if (c > 0)
                    {
                        if (tag.Substring(0, c).Equals(editedText.Substring(0, c), StringComparison.CurrentCultureIgnoreCase))
                        {
                            return SMGraphics.GetBrush(Color.DarkGreen);
                        }
                        else
                        {
                            return SMGraphics.GetBrush(Color.Red);
                        }
                    }


                }

                return SMGraphics.GetBrush(Color.DarkGreen);
            }
            else
            {
                if (droppedItem != null)
                {
                    return SMGraphics.GetBrush(Color.MediumBlue);
                }
                else
                {
                    return base.GetCurrentTextBrush(layout);
                }
            }
        }

        public override void Paint(MNPageContext context, SMStatusLayout layout, int X, int Y)
        {
            string s = GetCurrentText();

            //Brush b = SMGraphics.GetBrush(layout.BackColor);

            if (UIStateHover)
            {
                context.g.DrawRectangle(SMGraphics.GetPen(SMGraphics.dropableLayoutH.BorderColor, 4), rect.X + X, rect.Y + Y, rect.Width, rect.Height);
            }
            else
            {
                context.g.DrawRectangle(SMGraphics.GetPen(SMGraphics.dropableLayoutN.BorderColor, 2), rect.X + X, rect.Y + Y, rect.Width, rect.Height);
            }

            if (Editable && Focused)
            {
                SizeF sz = context.g.MeasureString(editedText, this.Font.Font);
                context.g.FillRectangle(Brushes.LightBlue, rect.X + X + (int)sz.Width, rect.Y + Y + 1, 10, rect.Height - 2);
            }

            context.g.DrawString(s, this.Font.Font, GetCurrentTextBrush(layout), rect.Location.X + X, rect.Location.Y + Y);
        }

        /// <summary>
        /// This is called when dragging operation is located over control
        /// </summary>
        /// <param name="p"></param>
        public override void HoverPoint(Point p)
        {
            if (Cardinality == SMConnectionCardinality.One || Cardinality == SMConnectionCardinality.Many)
            {
                UIStateHover = rect.Contains(p);
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
        public static List<SMWordBase> WordListFromString(string text, SMControl control)
        {
            List<SMWordBase> list = new List<SMWordBase>();
            TextParseMode mode = TextParseMode.General;
            StringBuilder word = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            TextTag tt = new TextTag();
            RunningFormat fmt = new RunningFormat();
            string argumentName = "";
            fmt.SetFontStyle(control.Font.Style);
            fmt.fontSize = control.Font.Size;
            fmt.fontName = control.Font.Name;
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
                        ClearWordBuffer(list, word, control, fmt);
                        if (readedChar == '\n')
                            list.Add(new SMWordSpecial(control.Font) { Type = SMWordSpecialType.Newline });
                        else
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
                        if (argumentName.Length > 0 && !tt.attrs.ContainsKey(argumentName))
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
                            tt.attrs[argumentName] = sb.ToString();
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

            public string selectForReplacementTarget = null;
        }

        private static void AppendTag(List<SMWordBase> list, StringBuilder word, TextTag tt, SMControl control, RunningFormat fmt)
        {
            // TODO
            ClearWordBuffer(list, word, control, fmt);
            switch (tt.tag)
            {
                case "draggable":
                    fmt.dragResponse = SMDragResponse.Drag;
                    break;
                case "/draggable":
                    fmt.dragResponse = SMDragResponse.None;
                    break;
                case "drop":
                    {
                        SMWordToken wt = new SMWordToken(control.Font);
                        wt.text = tt.attrs.ContainsKey("text") ? tt.attrs["text"] : "_____";
                        wt.tag = tt.attrs.ContainsKey("tag") ? tt.attrs["tag"] : "";
                        wt.Draggable = SMDragResponse.None;
                        wt.Cardinality = SMConnectionCardinality.One;
                        wt.Evaluation = control.HasLazyEvaluation ? MNEvaluationType.Lazy : MNEvaluationType.Immediate;
                        list.Add(wt);
                    }
                    break;
                case "edit":
                    {
                        SMWordToken wt = new SMWordToken(control.Font);
                        wt.text = tt.attrs.ContainsKey("text") ? tt.attrs["text"] : "_____";
                        wt.tag = tt.attrs.ContainsKey("tag") ? tt.attrs["tag"] : "";
                        wt.Draggable = SMDragResponse.None;
                        wt.Editable = true;
                        wt.Cardinality = SMConnectionCardinality.One;
                        wt.Evaluation = control.HasLazyEvaluation ? MNEvaluationType.Lazy : MNEvaluationType.Immediate;
                        list.Add(wt);
                    }
                    break;
                case "seltorepl":
                    fmt.selectForReplacementTarget = tt.attrs.ContainsKey("target") ? tt.attrs["target"] : null;
                    break;
                case "/seltorepl":
                    fmt.selectForReplacementTarget = null;
                    break;
                case "page":
                    list.Add(new SMWordSpecial(control.Font) { Type = SMWordSpecialType.NewPage });
                    break;
                case "hr":
                    list.Add(new SMWordSpecial(control.Font) { Type = SMWordSpecialType.HorizontalLine });
                    break;
                case "br":
                    list.Add(new SMWordSpecial(control.Font) { Type = SMWordSpecialType.Newline });
                    AppendWord(list, "\n", control, fmt);
                    break;
                case "col":
                    list.Add(new SMWordSpecial(control.Font) { Type = SMWordSpecialType.NewColumn });
                    //AppendWord(list, "\n", control, fmt);
                    break;
                case "r":
                    fmt.Bold = false;
                    fmt.Italic = false;
                    fmt.Strikeout = false;
                    fmt.Underline = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/r":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.Bold = control.Font.Bold;
                    fmt.Italic = control.Font.Italic;
                    fmt.Underline = control.Font.Underline;
                    fmt.Strikeout = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "b":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.Bold = true;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/b":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.Bold = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "i":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.Italic = true; 
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/i":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.Italic = false; 
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "u":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.Underline = true;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/u":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.Underline = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "so":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.Strikeout = true;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/so":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.Strikeout = false;
                    //fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                default:
                    ClearWordBuffer(list, word, control, fmt);
                    if (tt.tag.StartsWith("fs"))
                    {
                        int arg = 0;
                        if (int.TryParse(tt.tag.Substring(2), out arg))
                        {
                            fmt.fontSize = (float)arg / 100 * fmt.defaultFontSize;
                        }
                    }
                    break;
            }
        }

        private static void ClearWordBuffer(List<SMWordBase> list, StringBuilder word, SMControl control, RunningFormat fmt)
        {
            if (word.Length > 0)
            {
                AppendWord(list, word.ToString(), control, fmt);
                word.Clear();
            }
        }

        private static void AppendWord(List<SMWordBase> list, string text, SMControl control, RunningFormat fmt)
        {
            // TODO

            SMWordText wt = new SMWordText(control.Font);
            wt.text = text;
            wt.tag = text.Trim();
            wt.Draggable = fmt.dragResponse;
            wt.Evaluation = MNEvaluationType.None;
            wt.Font = fmt.GetFont();
            wt.replacementTarget = fmt.selectForReplacementTarget;
            list.Add(wt);
        }

        public void AcceptBack()
        {
            if (Editable)
            {
                if (editedText.Length > 0)
                    editedText = editedText.Substring(0, editedText.Length - 1);
            }
        }

        public void AcceptString(string p)
        {
            if (Editable && p != null)
            {
                editedText += p;
            }
        }
    }



}
