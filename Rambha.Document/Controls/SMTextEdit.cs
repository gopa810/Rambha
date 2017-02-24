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

        public override void Paint(MNPageContext context)
        {
            SMRectangleArea area = context.CurrentPage.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);

            if (tempBackBrush == null || tempBackBrush.Color != Style.BackColor)
            {
                tempBackBrush = new SolidBrush(Style.BackColor);
            }
            if (tempForeBrush == null || tempForeBrush.Color != Style.ForeColor)
            {
                tempForeBrush = new SolidBrush(Style.ForeColor);
            }
            if (tempForePen == null || tempForePen.Color != Style.ForeColor)
            {
                tempForePen = new Pen(Style.ForeColor);
            }

            Rectangle textBounds = bounds;
            textBounds.X += Style.ContentPadding.Left;
            textBounds.Y += Style.ContentPadding.Top;
            textBounds.Width -= (Style.ContentPadding.Left + Style.ContentPadding.Right);
            textBounds.Height -= (Style.ContentPadding.Top + Style.ContentPadding.Bottom);

            // size of one cell
            SizeF sizeChar = context.g.MeasureString("M", Style.Font);
            int cellSize = (int)textBounds.Height / LinesCount;
            Font drawFont = SMGraphics.GetFontVariation(Style.Font, cellSize);

            // prepare formating
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            if (stringReadyToAdd.Length != 0)
            {
                foreach (char c in stringReadyToAdd)
                {
                    if (ProcessChar(context, textBounds.Width, c))
                        break;
                }
                stringReadyToAdd = "";
            }

            Brush textB = (UIStateError == MNEvaluationResult.Incorrect ? Brushes.Red : tempForeBrush);
            Brush cursB = (UIStateError == MNEvaluationResult.Incorrect ? Brushes.Pink : Brushes.LightBlue);

            // drawing all positions
            for (int i = 0; i < LinesCount; i++)
            {
                int y = cellSize * (i + 1) + textBounds.Y;
                context.g.DrawLine(tempForePen, textBounds.Left, y, textBounds.Right, y);
                // draws cursor only if control is focused
                if ((i == Lines.Count - 1) && (UIStateError == MNEvaluationResult.Focused))
                {
                    SizeF ll = context.g.MeasureString(Lines[i], Style.Font);
                    context.g.DrawString(Lines[i], Style.Font, textB, 0, cellSize * i);
                    context.g.FillRectangle(cursB, ll.Width + 2, cellSize * i, cellSize * 2 / 3, cellSize);
                }
                else if (i < Lines.Count)
                {
                    context.g.DrawString(Lines[i], Style.Font, textB, 0, cellSize * i);
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

        public void RecalculateWords(MNPageContext context, float width)
        {
            string[] p = TextBuilder.ToString().Split(' ');
            Font font = Style.Font;
            Graphics g = context.g;
            StringBuilder currLine = new StringBuilder();
            float currWidth = 0;
            int counter = 0;
            SizeF ts;

            Lines.Clear();

            foreach (string s in p)
            {
                if (counter > 0)
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
                }

                ts = g.MeasureString(s, font);
                if (ts.Width + currWidth > width)
                {
                    Lines.Add(currLine.ToString());
                    currLine.Clear();
                    currWidth = 0;
                }

                currLine.Append(s);
                currWidth += ts.Width;

                counter++;
            }
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
            if (token == "AcceptChar")
            {
                if (args.Count > 0)
                {
                    AcceptString(args[0].getStringValue());
                    return args[0];
                }

                return GSVoid.Void;
            }
            else
            {
                return base.ExecuteMessage(token, args);
            }
        }
    }
}
