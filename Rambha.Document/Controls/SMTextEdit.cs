using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMTextEdit: SMControl
    {
        [Browsable(true), Category("Layout")]
        public int LinesCount { get; set; }

        [Browsable(true), Category("Evaluation")]
        public string ExpectedValue { get; set; }

        protected StringBuilder TextBuilder = new StringBuilder();
        protected List<string> Lines = new List<string>();
        protected bool wordsModified = false;

        public SMTextEdit(MNPage page)
            : base(page)
        {
            Text = "";
            LinesCount = 2;
            Evaluation = MNEvaluationType.Inherited;
            ExpectedValue = "";
        }

        public override System.Drawing.Size GetDefaultSize()
        {
            return new Size(128,24);
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
                            LinesCount = br.ReadInt32();
                            ExpectedValue = br.ReadString();
                            break;
                        default:
                            return false;
                    }
                }

                TextBuilder.Clear();
                TextBuilder.Append(Text);

                return true;
            }

            return false;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteInt32((Int32)LinesCount);
            bw.WriteString(ExpectedValue);

            bw.WriteByte(0);
        }

        public override void ExportToHtml(MNExportContext ctx, int zorder, StringBuilder sbHtml, StringBuilder sbCss, StringBuilder sbJS)
        {
            sbHtml.Append("<div ");
            sbHtml.AppendFormat(" id=\"c{0}\" ", this.Id);
            sbHtml.AppendFormat(" style ='position:absolute;z-index:{0};", zorder);
            sbHtml.Append(Area.HtmlLTRB());
            sbHtml.Append("'>");
            sbHtml.AppendFormat("<textarea style='width:100%;height:100%;font-family:{0};font-size:100%;background:lightyellow;border:1px solid black;'></textarea>",
                Font.Name);
            //sbHtml.Append("<b>" + GetType().Name + "</b><br>" + this.Text);
            sbHtml.Append("</div>\n");
        }

        public override void Paint(MNPageContext context)
        {
            Rectangle bounds = Area.GetBounds(context);

            SMStatusLayout layout = PrepareBrushesAndPens();

            Rectangle textBounds = ContentPadding.ApplyPadding(bounds);

            // size of one cell
            SizeF sizeChar = context.g.MeasureString("M", Font.Font);
            int cellSize = (int)(sizeChar.Height * 12 / 10);
            Font drawFont = GetUsedFont();

            // prepare formating
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            if (wordsModified)
            {
                RecalculateWords(context, textBounds.Width);
                while (Lines.Count > LinesCount && LinesCount > 0)
                {
                    TextBuilder.Remove(TextBuilder.Length - 1, 1);
                    RecalculateWords(context, textBounds.Width);
                }
                wordsModified = false;
            }

            Brush textB = (UIStateError == MNEvaluationResult.Incorrect ? Brushes.Red : tempForeBrush);
            Brush cursB = (UIStateError == MNEvaluationResult.Incorrect ? Brushes.Pink : Brushes.LightBlue);

            Font usedFont = Font.Font;
            // drawing all positions
            for (int i = 0; i < LinesCount; i++)
            {
                int y = cellSize * (i + 1) + textBounds.Y;
                context.g.DrawLine(Pens.Gray, textBounds.Left, y, textBounds.Right, y);
                // draws cursor only if control is focused
                if (i < Lines.Count)
                {
                    SizeF ll = context.g.MeasureString(Lines[i], usedFont);
                    int y2 = y - (int)ll.Height - 3;
                    context.g.DrawString(Lines[i], usedFont, textB, textBounds.X, y2);

                    if (i == Lines.Count - 1)
                    {
                        context.g.FillRectangle(cursB, textBounds.X + ll.Width + 2, y2, cellSize * 2 / 3, cellSize);
                    }
                }
            }

            // draw selection marks
            base.Paint(context);
        }

        private string stringReadyToAdd = "";

        public void AcceptString(string c)
        {
            stringReadyToAdd += c;
        }

        public bool ProcessChar(MNPageContext context, float width, char c)
        {
            bool refused = false;

            if (c == Convert.ToChar(9))
            {
                if (TextBuilder.Length > 0)
                {
                    TextBuilder.Remove(TextBuilder.Length - 1, 1);
                    RecalculateWords(context, width);
                }
            }
            else
            {
                TextBuilder.Append(c);
                RecalculateWords(context, width);

                // if adding character will cause to exceed the maximum number of lines
                // then character is not appended (in this case it is removed from the end)
                if (Lines.Count > LinesCount)
                {
                    TextBuilder.Remove(TextBuilder.Length - 1, 1);
                    RecalculateWords(context, width);
                    refused = true;
                }
            }

            if (HasImmediateEvaluation)
            {
                Evaluate();
            }

            return refused;
        }

        public int RecalculateWords(MNPageContext context, float width)
        {
            string[] p = TextBuilder.ToString().Split(' ');
            Font font = Font.Font;
            Graphics g = context.g;
            StringBuilder currLine = new StringBuilder();
            float currWidth = 0;
            int counter = 0;
            SizeF ts;

            Lines.Clear();

            foreach (string s in p)
            {
                /*if (counter > 0)
                {
                    ts = g.MeasureString(" ", font);
                    if (ts.Width + currWidth > width)
                    {
                        Lines.Add(currLine.ToString());
                        currLine.Clear();
                        currWidth = 0;
                    }
                    else
                    {
                        currLine.Append(" ");
                        currWidth += ts.Width;
                    }
                }*/

                ts = g.MeasureString(s, font);
                if (ts.Width + currWidth > width)
                {
                    Lines.Add(currLine.ToString());
                    currLine.Clear();
                    currWidth = 0;
                }

                if (currLine.Length > 0)
                    currLine.Append(" ");
                currLine.Append(s);
                currWidth += ts.Width;

                counter++;
            }

            if (currLine.Length > 0)
            {
                Lines.Add(currLine.ToString());
            }

            if (Lines.Count == 0)
            {
                Lines.Add("");
            }
            return -1;
        }

        public override MNEvaluationResult Evaluate()
        {
            if (HasImmediateEvaluation)
            {
                if (TextBuilder.Length > 0 && Text.StartsWith(TextBuilder.ToString()))
                {
                    UIStateError = MNEvaluationResult.Correct;
                }
                else
                {
                    UIStateError = MNEvaluationResult.Incorrect;
                }
            }
            else if (HasLazyEvaluation)
            {
                if (TextBuilder.ToString().ToLower().Equals(Text.ToLower()))
                {
                    UIStateError = MNEvaluationResult.Correct;
                }
                else
                {
                    UIStateError = MNEvaluationResult.Incorrect;
                }
            }

            return base.Evaluate();
        }

        public override void DisplayAnswers()
        {
            TextBuilder.Clear();
            TextBuilder.Append(Text);
        }

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            if (token.Equals("acceptString"))
            {
                TextBuilder.Append(args.getSafe(0).getStringValue());
                wordsModified = true;
                return this;
            }
            else if (token.Equals("acceptBack"))
            {
                if (TextBuilder.Length > 0)
                {
                    TextBuilder.Remove(TextBuilder.Length - 1, 1);
                    wordsModified = true;
                }
                return this;
            }

            return base.ExecuteMessage(token, args);
        }

        public override void ResetStatus()
        {
            Text = "";
            Lines.Clear();
            base.ResetStatus();
        }
    }
}
