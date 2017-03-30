// Very primitive RTF 2 HTML reader
// Converts tiny subset of RTF (from VS IDE) into html.
// Author: Mike Stall (http://blogs.msdn.com/jmstall)
// Gets input RTF from clipboard.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace FilesGenerator
{
    class Convertor
    {
        public string Main(string rtf)
        {

            stack.Add(new FMT());

            // We assume the colortable and fontable are a standard preset used by VS.
            // Avoids hassle of parsing them.
            // Skip past {\colortbl.*;} and to the start of the real data
            // @todo – regular expression would be good here.

            StringBuilder sb = new StringBuilder();

            Format(sb, rtf);

            return sb.ToString().Replace("Th e", "The");

        }

        public class FMTChange: FMT
        {
            public bool boldChanged = false;
            public bool italicChanged = false;
            public bool fontSizeChanged = false;
            public FMTChange()
            {
            }
            public FMTChange(FMT oldLevel, FMT newLevel)
            {
                if (oldLevel.bold != newLevel.bold)
                {
                    boldChanged = true;
                    bold = newLevel.bold;
                }
                if (oldLevel.italic != newLevel.italic)
                {
                    italicChanged = true;
                    italic = newLevel.italic;
                }
                if (oldLevel.fontSize != newLevel.fontSize)
                {
                    fontSizeChanged = true;
                    fontSize = newLevel.fontSize;
                }
            }
        }

        public class FMT
        {
            public bool bold = false;
            public bool italic = false;
            public int fontSize = 24;
            public bool block = false;
            public FMT()
            {
            }
            public FMT(FMT a)
            {
                bold = a.bold;
                italic = a.italic;
                fontSize = a.fontSize;
            }
        }

        public List<FMT> stack = new List<FMT>();

        public FMT LastLevel
        {
            get
            {
                return stack[stack.Count - 1];
            }
        }

        public void AddLevel()
        {
            if (LastLevel.block)
                return;
            stack.Add(new FMT(LastLevel));
        }

        public FMTChange RemoveLevel()
        {
            if (stack.Count > 1)
            {
                FMT last = LastLevel;
                stack.RemoveAt(stack.Count - 1);
                return new FMTChange(last, LastLevel);
            }
            else
            {
                LastLevel.bold = false;
                LastLevel.italic = false;
                LastLevel.fontSize = 24;
            }

            return new FMTChange();
        }

        public class Tracker
        {
            public string text = null;
            public int index = 0;
            public char outText = ' ';
            public StringBuilder outCmd = new StringBuilder();
            public StringBuilder outArg = new StringBuilder();

            public ItemType type = ItemType.Text;

            public string Cmd
            {
                get { return outCmd.ToString(); }
            }
            public string Arg
            {
                get { return outArg.ToString(); }
            }

            public bool GetNext()
            {
                if (index < text.Length)
                {
                    switch (text[index])
                    {
                        case '{':
                            type = ItemType.OpenBracket;
                            index++;
                            return true;
                        case '}':
                            type = ItemType.CloseBracket;
                            index++;
                            return true;
                        case '\\':
                            index++;
                            if (index >= text.Length)
                                return false;
                            if (text[index] == '\\')
                            {
                                outText = '\\';
                                type = ItemType.Text;
                                return true;
                            }
                            else
                            {
                                type = ItemType.Command;
                                outCmd.Clear();
                                outArg.Clear();
                                bool readArg = false;
                                while (index < text.Length)
                                {
                                    if (text[index] == '\'')
                                    {
                                        outCmd.Append("\'");
                                        readArg = true;
                                        index++;
                                        break;
                                    }
                                    else if (Char.IsLetter(text[index]))
                                    {
                                        outCmd.Append(text[index]);
                                        index++;
                                    }
                                    else if (Char.IsWhiteSpace(text[index]))
                                    {
                                        index++;
                                        return true;
                                    }
                                    else if (text[index] == '\\' || text[index] == '}' || text[index] == '{')
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        readArg = true;
                                        break;
                                    }
                                }

                                while (readArg && index < text.Length)
                                {
                                    if (Char.IsWhiteSpace(text[index]))
                                    {
                                        index++;
                                        return true;
                                    }
                                    else if (text[index] == '\\' || text[index] == '}' || text[index] == '{')
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        outArg.Append(text[index]);
                                        index++;
                                    }
                                }

                                return true;
                            }
                        default:
                            type = ItemType.Text;
                            outText = text[index];
                            index++;
                            return true;
                    }
                }
                else
                {
                    type = ItemType.End;
                    return false;
                }
            }
        }

        public enum ItemType
        {
            Text,
            Command,
            OpenBracket,
            CloseBracket,
            End
        }

        // Convert the RTF data into an HTML stream.
        // This rtf snippet is past the font + color tables, so we're just transfering control words now.
        // Write out HTML to the text writer.
        void Format(StringBuilder tw, string rtf)
        {
            // Example: \fs20 \cf2 using\cf0  System;
            // root –> ('text' '\' ('control word' | 'escaped char'))+
            // 'control word'  –> (alpha)+ (numeric*) space?
            // 'escaped char' = 'x'. Some characters \, {, } are escaped: '\x' –> 'x'
            // @todo – handle embedded groups (begin with '{')

            int idx = 0;
            Tracker tracker = new Tracker();
            tracker.text = rtf;
            while (tracker.GetNext())
            {
                switch (tracker.type)
                {
                    case ItemType.OpenBracket:
                        AddLevel();
                        idx++;
                        break;
                    case ItemType.CloseBracket:
                        FMTChange c = RemoveLevel();
                        if (c.boldChanged)
                        {
                            tw.AppendFormat("<{0}b>", c.bold ? "" : "/");
                        }
                        if (c.italicChanged)
                        {
                            tw.AppendFormat("<{0}i>", c.italic ? "" : "/");
                        }
                        idx++;
                        break;
                    case ItemType.Text:
                        if (LastLevel.block)
                            break;
                        tw.Append(tracker.outText);
                        break;
                    case ItemType.Command:
                        switch (tracker.outCmd.ToString())
                        {
                            case "\'":
                                tw.Append(Convert.ToChar(Convert.ToInt32(tracker.outArg.ToString(), 16)));
                                break;
                            case "b":
                                if (tracker.Arg == "0")
                                {
                                    LastLevel.bold = false;
                                    tw.Append("</b>");
                                }
                                else if (tracker.Arg == "1")
                                {
                                    tw.Append("<b>");
                                    LastLevel.bold = true;
                                }
                                else
                                {
                                    LastLevel.bold = true;
                                    tw.Append("<b>");
                                }
                                break;
                            case "i":
                                if (tracker.Arg == "0")
                                {
                                    LastLevel.italic = false;
                                    tw.Append("</i>");
                                }
                                else if (tracker.Arg == "1")
                                {
                                    LastLevel.italic = true;
                                    tw.Append("<i>");
                                }
                                else
                                {
                                    LastLevel.bold = true;
                                    tw.Append("<i>");
                                }
                                break;
                            case "fonttbl":
                                LastLevel.block = true;
                                break;
                            case "par":
                                //tw.Append("\n");
                                break;
                            case "ldblquote":
                                tw.Append("\u201c");
                                break;
                            case "rdblquote":
                                tw.Append("\u201D");
                                break;
                            case "lquote":
                                tw.Append("\u2018");
                                break;
                            case "rquote":
                                tw.Append("\u2019");
                                break;
                            case "pard":
                                stack.Clear();
                                stack.Add(new FMT());
                                break;
                            case "fs":
                                int fs = int.Parse(tracker.Arg);
                                if (fs != LastLevel.fontSize)
                                {
                                    switch (fs)
                                    {
                                        case 26:
                                            tw.Append("<fs120>");
                                            break;
                                        default:
                                            tw.Append("<fs100>");
                                            break;
                                    }
                                    LastLevel.fontSize = fs;
                                }
                                break;
                            case "f": break;
                            case "endash":
                                tw.Append("\u2013");
                                break;
                            default:
                                Console.Write("Unrecognized control word '" + tracker.Cmd + tracker.Arg + "'\n");
                                break;

                        }
                        break;
                }
            }

        } // end Format()

    }

}