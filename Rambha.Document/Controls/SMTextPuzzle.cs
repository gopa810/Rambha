using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;

using Rambha.Serializer;


namespace Rambha.Document
{
    public class SMTextPuzzle: SMControl
    {
        [Browsable(true), Category("Layout")]
        public int Spacing { get; set; }

        [Browsable(true), Category("Text Layout")]
        public Color HighlightColor { get; set; }

        [Browsable(true), Category("Text Layout")]
        public int Rows { get; set; }

        [Browsable(true), Category("Text Layout")]
        public int Columns { get; set; }



        public SMTextPuzzle(MNPage p)
            : base(p)
        {
            Text = "Puzzle";
            Rows = 6;
            Columns = 6;
            Spacing = 8;
            HighlightColor = Color.Green;
            Evaluation = MNEvaluationType.Inherited;
        }

        public override System.Drawing.Size GetDefaultSize()
        {
            return new Size(256,196);
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteInt32(Spacing);
            bw.WriteInt32(Rows);
            bw.WriteInt32(Columns);
            bw.WriteColor(HighlightColor);

            bw.WriteByte(0);
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
                            Spacing = br.ReadInt32();
                            Rows = br.ReadInt32();
                            Columns = br.ReadInt32();
                            HighlightColor = br.ReadColor();
                            break;
                        default:
                            return false;
                    }
                }

                return true;
            }

            return false;
        }

        private string p_prevText = "";
        private int p_prevRowCol = 0;
        private Brush p_prevHighBrush = Brushes.Green;
        private Color p_prevHighColor = Color.Transparent;

        private string p_alignedText = "";
        private int p_cellSize = 10;
        private bool[,] p_cellStatus = null;
        private bool[,] p_cellExpectedStatus = null;

        public override void Paint(MNPageContext context)
        {
            Rectangle bounds = Area.GetBounds(context);

            Rectangle textBounds = ContentPadding.ApplyPadding(bounds);

            if (p_prevHighColor != HighlightColor)
            {
                p_prevHighColor = HighlightColor;
                p_prevHighBrush = new SolidBrush(HighlightColor);
            }

            if (p_alignedText.Length == 0 || !p_prevText.Equals(Text) || p_prevRowCol != Rows*Columns)
            {
                if (Rows < 3) Rows = 3;
                if (Columns < 3) Columns = 3;
                p_prevRowCol = Rows * Columns;
                p_prevText = Text;
                p_cellStatus = new bool[Columns,Rows];
                p_cellExpectedStatus = new bool[Columns, Rows];
                p_alignedText = AlignText(Text);
                for (int i = 0; i < Columns; i++)
                {
                    for (int i2 = 0; i2 < Rows; i2++)
                    {
                        p_cellStatus[i, i2] = false;
                    }
                }
            }
            SizeF sizeChar = context.g.MeasureString("M", Font.Font);
            float cellSize = Math.Max(sizeChar.Height, sizeChar.Width);
            float targetCellSize = Math.Min((textBounds.Width - (Columns + 1)*Spacing) / Columns,
                (textBounds.Height - (Rows + 1)*Spacing) / Rows);

            // recalculate actuall size for puzzle
            textBounds.Height = (int)(targetCellSize * Rows + Spacing * Rows + Spacing);
            textBounds.Width = (int)(targetCellSize * Columns + Spacing * Columns + Spacing);

            // recalculate border
            bounds = ContentPadding.ApplyPadding(textBounds);

            context.g.DrawRectangle(Pens.Black, bounds);

            // recalculate size of font
            Font usedFont = Font.Font;
            if (targetCellSize > 0.1f)
            {
                usedFont = SMGraphics.GetFontVariation(Font.Name, Font.Size * targetCellSize / cellSize);
            }
            int index = 0;

            // prepare rectangle for letter
            Rectangle rect = new Rectangle();
            rect.Width = (int)targetCellSize;
            rect.Height = (int)targetCellSize;

            // prepare text formating
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            p_cellSize = (int)targetCellSize;

            // drawing letters
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    rect.X = Convert.ToInt32(c * targetCellSize + (c + 1) * Spacing) + textBounds.X;
                    rect.Y = Convert.ToInt32(r * targetCellSize + (r + 1) * Spacing) + textBounds.Y;
                    string s = p_alignedText.Substring(index, 1);
                    index++;
                    if (p_cellStatus[c, r])
                    {
                        context.g.FillRectangle(p_prevHighBrush, rect);
                    }
                    if (UIStateError == MNEvaluationResult.Incorrect)
                    {
                        if (p_cellStatus[c,r] != p_cellExpectedStatus[c,r])
                            context.g.DrawString(s, usedFont, Brushes.Gray, rect, format);
                        else
                            context.g.DrawString(s, usedFont, Brushes.Black, rect, format);
                    }
                    else
                    {
                        context.g.DrawString(s, usedFont, Brushes.Black, rect, format);
                    }
                }
            }

            // draw selection marks
            base.Paint(context);
        }


        public string AlignText(string text)
        {
            Random rnd = new Random();
            string[] lines = text.Split('\r', '\n');

            int r = 0, c = 0;
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                int i = 0;
                for (i = 0; i < line.Length; i++)
                {
                    if (line[i] == '_')
                    {
                        sb.Append(GenerateChar(rnd));
                        p_cellExpectedStatus[r, c] = false;
                    }
                    else
                    {
                        sb.Append(Char.ToUpper(line[i]));
                        p_cellExpectedStatus[r, c] = true;
                    }
                    c++;
                    if (c >= Columns)
                    {
                        r++;
                        c = 0;
                    }
                    if (r >= Rows)
                    {
                        return sb.ToString();
                    }
                }
                if (c != 0)
                {
                    while (c < Columns)
                    {
                        sb.Append(GenerateChar(rnd));
                        p_cellExpectedStatus[r, c] = false;
                        c++;
                    }
                    r++;
                    c = 0;
                    if (r >= Rows)
                    {
                        return sb.ToString();
                    }
                }
            }

            while (r < Rows)
            {
                c = 0;
                while (c < Columns)
                {
                    sb.Append(GenerateChar(rnd));
                    p_cellExpectedStatus[r, c] = false;
                    c++;
                }
                r++;
            }

            return sb.ToString();
        }

        public char GenerateChar(Random rnd)
        {
            return Convert.ToChar(Convert.ToInt32('A') + rnd.Next(26));
        }

        /// <summary>
        /// In case we have Immediate evaluation of this control
        /// we have to set status of cell back to flase if that cell should
        /// not be selected
        /// </summary>
        /// <param name="dc"></param>
        public override void OnClick(PVDragContext dc)
        {
            if (HasImmediateEvaluation)
            {
                Rectangle bounds = Area.GetBounds(dc.context);

                int x = dc.lastPoint.X - ContentPadding.Left - bounds.X;
                int y = dc.lastPoint.Y - ContentPadding.Top - bounds.Y;

                if (x < 0) x = 0;
                if (y < 0) y = 0;

                x = x / (p_cellSize + Spacing);
                y = y / (p_cellSize + Spacing);

                if (x < Columns && y < Rows)
                {
                    if (p_cellExpectedStatus[x, y] == false)
                        p_cellStatus[x, y] = false;
                }
            }
        }

        /// <summary>
        /// This starts highlighting for clicked cell
        /// </summary>
        /// <param name="dc"></param>
        public override void OnTapBegin(PVDragContext dc)
        {
            Rectangle bounds = Area.GetBounds(dc.context);

            int x = dc.lastPoint.X - ContentPadding.Left - bounds.X;
            int y = dc.lastPoint.Y - ContentPadding.Top - bounds.Y;

            if (x < 0) x = 0;
            if (y < 0) y = 0;

            x = x / (p_cellSize + Spacing);
            y = y / (p_cellSize + Spacing);

            if (x < Columns && y < Rows)
            {
                p_cellStatus[x, y] = !p_cellStatus[x, y];
            }
        }

        /// <summary>
        /// Lazy evaluation
        /// </summary>
        /// <returns></returns>
        public override MNEvaluationResult Evaluate()
        {
            if (HasLazyEvaluation)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; i < Columns; i++)
                    {
                        if (p_cellStatus[i, j] != p_cellExpectedStatus[i, j])
                        {
                            UIStateError = MNEvaluationResult.Incorrect;
                            return UIStateError;
                        }
                    }
                }

                UIStateError = MNEvaluationResult.Correct;
            }
            return UIStateError;
        }

        public override void DisplayAnswers()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; i < Columns; i++)
                {
                    p_cellStatus[i, j] = p_cellExpectedStatus[i, j];
                }
            }

            base.DisplayAnswers();
        }

        public override void LoadStatus(RSFileReader br)
        {
            base.LoadStatus(br);
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                switch (b)
                {
                    case 20:
                        Columns = br.ReadInt32();
                        Rows = br.ReadInt32();
                        p_cellStatus = new bool[Columns, Rows];
                        p_cellExpectedStatus = new bool[Columns, Rows];
                        for (int c = 0; c < Columns; c++)
                        {
                            for (int r = 0; r < Rows; r++)
                            {
                                p_cellStatus[c, r] = br.ReadBool();
                                p_cellExpectedStatus[c, r] = br.ReadBool();
                            }
                        }
                        break;
                }
            }
        }

        public override void SaveStatus(RSFileWriter bw)
        {
            base.SaveStatus(bw);

            if (p_cellStatus != null && p_cellExpectedStatus != null)
            {
                bw.WriteByte(20);
                bw.WriteInt32(Columns);
                bw.WriteInt32(Rows);
                for (int c = 0; c < Columns; c++)
                {
                    for (int r = 0; r < Rows; r++)
                    {
                        bw.WriteBool(p_cellStatus[c, r]);
                        bw.WriteBool(p_cellExpectedStatus[c, r]);
                    }
                }
            }


            bw.WriteByte(0);
        }
    }
}
