using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Rambha.Document
{
    public class SMWordBase
    {
        protected SMControl Parent = null;
        public int LineNo = 0;
        public int ColumnNo = 0;
        public int PageNo = 0;
        public RectangleF rect = RectangleF.Empty;
        public string tag = string.Empty;
        public MNEvaluationType Evaluation = MNEvaluationType.Lazy;
        private SMDragResponse draggable = SMDragResponse.Undef;

        public SMWordBase(SMControl ctrl)
        {
            Parent = ctrl;
        }

        public virtual SMTokenItem GetDraggableItem()
        {
            return null;
        }

        public virtual bool IsSpace()
        {
            return false;
        }

        public virtual void Paint(MNPageContext context)
        {
        }
    
        public SMDragResponse Draggable
        {
            get { return draggable; }
            set { draggable = value; }
        }
    
        public bool IsDraggable
        {
            get
            {
                return Draggable == SMDragResponse.Drag || Draggable == SMDragResponse.Line;
            }
        }

        public virtual void HoverPoint(Point p)
        {
        }
    }

    public enum SMWordSpecialType
    {
        Newline,
        HorizontalLine
    }

    public class SMWordSpecial : SMWordBase
    {
        public SMWordSpecial(SMControl ct)
            : base(ct)
        {
        }

        public SMWordSpecialType Type {get;set;}
    }

    public class SMWordText: SMWordTextBase
    {
        public string text = string.Empty;
        public SMWordText(SMControl ct): base(ct)
        {
        }

        public SMWordText(SMControl ct, SMTokenItem item): base(ct)
        {
            text = item.Text ?? text;
            tag = item.Tag;
        }

        public override void Paint(MNPageContext context)
        {
            //context.g.FillRectangle(Brushes.LightGreen, rect);
            context.g.DrawString(this.text, this.Font, this.TextBrush, this.rect.Location);
        }

        public override bool IsSpace()
        {
            return text.Equals(" ");
        }

        public override SMTokenItem GetDraggableItem()
        {
            SMTokenItem item = new SMTokenItem();
            item.Tag = tag;
            item.Text = text;
            item.ContentSize = Size.Empty;
            return item;
        }

    }

    public class SMWordImage : SMWordBase
    {
        public Image image = null;
        public Size imageSize = Size.Empty;

        public override SMTokenItem GetDraggableItem()
        {
            SMTokenItem item = new SMTokenItem();
            item.Tag = tag;
            item.Image = image;
            item.ContentSize = Size.Empty;
            return item;
        }

        public SMWordImage(SMControl ct): base(ct)
        {
        }

        public SMWordImage(SMControl ct, SMTokenItem item): base (ct)
        {
            tag = item.Tag;
            image = item.Image;
        }

        public override void Paint(MNPageContext context)
        {
            context.g.DrawImage(image, rect);
        }
    }

    public class SMWordTextBase: SMWordBase
    {
        private Brush textBrush = Brushes.Black;
        private Font font = null;
        public bool fontStyleValid = false;
        public FontStyle fontStyle = FontStyle.Regular;


        public Font Font
        {
            get
            {
                Font f = font;
                if (f == null) f = Parent.GetUsedFont();
                if (f == null) f = SystemFonts.DefaultFont;
                if (fontStyleValid)
                {
                    if (fontStyle != f.Style)
                    {
                        f = SMGraphics.GetFontVariation(f, fontStyle);
                    }
                }
                return f;
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
                if (textBrush == null)
                    textBrush = new SolidBrush(Parent.Style.ForeColor);
                return textBrush;
            }
            set
            {
                textBrush = value;
            }
        }

        public SMWordTextBase(SMControl ct)
            : base(ct)
        {
        }
    }

    public class SMWordToken: SMWordTextBase
    {
        private SMDropResponse droppable = SMDropResponse.Undef;
        public SMTokenItem droppedItem = null;
        public bool UIStateHover = false;
        public MNEvaluationResult UIStateError = MNEvaluationResult.NotEvaluated;
        public string text = string.Empty;


        public SMDropResponse Droppable
        {
            get { return droppable; }
            set { droppable = value; }
        }

        public SMWordToken(SMControl ct): base(ct)
        {
        }

        public override void Paint(MNPageContext context)
        {
            if (UIStateHover)
            {
                context.g.FillRectangle(SMGraphics.GetBrush(Parent.Style.HighBackColor), this.rect);
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

        /// <summary>
        /// This is called when dragging operation is located over control
        /// </summary>
        /// <param name="p"></param>
        public override void HoverPoint(Point p)
        {
            if (Droppable == SMDropResponse.One || Droppable == SMDropResponse.Many)
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
            if (control.Style == null)
                return new List<SMWordBase>();
            List<SMWordBase> list = new List<SMWordBase>();
            TextParseMode mode = TextParseMode.General;
            StringBuilder word = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            TextTag tt = new TextTag();
            RunningFormat fmt = new RunningFormat();
            string argumentName = "";
            fmt.fontStyle = control.Style.Font.Style;

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
                            list.Add(new SMWordSpecial(control) { Type = SMWordSpecialType.Newline });
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
            public FontStyle fontStyle = FontStyle.Regular;
            public bool fontStyleValid = false;
        }

        private static void AppendTag(List<SMWordBase> list, StringBuilder word, TextTag tt, SMControl control, RunningFormat fmt)
        {
            // TODO
            switch (tt.tag)
            {
                case "draggable":
                    fmt.dragResponse = SMDragResponse.Drag;
                    break;
                case "/draggable":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.dragResponse = SMDragResponse.None;
                    break;
                case "drop":
                    {
                        SMWordToken wt = new SMWordToken(control);
                        wt.text = tt.attrs.ContainsKey("text") ? tt.attrs["text"] : "_____";
                        wt.tag = tt.attrs.ContainsKey("tag") ? tt.attrs["tag"] : "";
                        wt.Draggable = control.Draggable;
                        wt.Droppable = control.Droppable;
                        if (fmt.fontStyleValid)
                        {
                            wt.fontStyle = fmt.fontStyle;
                            wt.fontStyleValid = true;
                        }
                        wt.Droppable = SMDropResponse.One;
                        wt.Evaluation = control.HasLazyEvaluation ? MNEvaluationType.Lazy : MNEvaluationType.Immediate;
                        list.Add(wt);
                    }
                    break;
                case "br":
                    ClearWordBuffer(list, word, control, fmt);
                    list.Add(new SMWordSpecial(control) { Type = SMWordSpecialType.Newline });
                    AppendWord(list, "\n", control, fmt);
                    break;
                case "r":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.fontStyle = FontStyle.Regular;
                    fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/r":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.fontStyle = control.Style.Font.Style;
                    fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "b":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.fontStyle |= FontStyle.Bold;
                    fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/b":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.fontStyle = fmt.fontStyle & ~FontStyle.Bold;
                    fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "i":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.fontStyle |= FontStyle.Italic; 
                    fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/i":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.fontStyle = fmt.fontStyle & ~FontStyle.Italic; 
                    fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "u":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.fontStyle |= FontStyle.Underline;
                    fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
                    break;
                case "/u":
                    ClearWordBuffer(list, word, control, fmt);
                    fmt.fontStyle = fmt.fontStyle & ~FontStyle.Underline;
                    fmt.fontStyleValid = (fmt.fontStyle != control.Style.Font.Style);
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

            SMWordText wt = new SMWordText(control);
            wt.text = text;
            wt.tag = text.ToLower();
            wt.TextBrush = SMGraphics.GetBrush(control.Style.ForeColor);
            if (fmt.fontStyleValid)
            {
                wt.fontStyle = fmt.fontStyle;
                wt.fontStyleValid = true;
            }
            wt.Draggable = fmt.dragResponse;
            wt.Evaluation = MNEvaluationType.None;
            list.Add(wt);
        }
    }

    public class SMWordLine: List<SMWordBase>
    {
    }
}
