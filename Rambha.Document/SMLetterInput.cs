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
    public class SMLetterInput: SMControl
    {
        [Browsable(true), Category("Layout")]
        public SMTextDirection TextDirection { get; set; }

        [Browsable(true), Category("Layout")]
        public string ContentCells { get; set; }

        protected int p_focusX = 0;
        protected int p_focusY = 0;
        protected int p_lastMove = 0;

        public SMLetterInput(MNPage page)
            : base(page)
        {
            Text = "";
            TextDirection = SMTextDirection.Horizontal;
            ContentCells = "0 0 H AB_ ABECEDA; 7 7 V X_ XYZ";
            Evaluation = MNEvaluationType.Inherited;
        }

        public override System.Drawing.Size GetDefaultSize()
        {
            return new Size(128,128);
        }

        private string prevContent = "";
        private List<CellInfo> cells = new List<CellInfo>();
        private int p_xmax;
        private int p_ymax;
        private int p_cellx;
        private string[,,] p_array = null;
        private StringFormat p_format = null;

        private class CellInfo
        {
            public int x1 = 0;
            public int y1 = 0;
            public int x2 = 3;
            public int y2 = 0;
            public string VisibleLetters = "";
            public string Letters = "";
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
                            ContentCells = br.ReadString();
                            TextDirection = (SMTextDirection)br.ReadInt32();
                            break;
                        default: 
                            return false;
                    }

                }
                return true;
            }

            return false;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteString(ContentCells);
            bw.WriteInt32((Int32)TextDirection);

            bw.WriteByte(0);
        }

        public override void Paint(MNPageContext context)
        {
            SMRectangleArea area = context.CurrentPage.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);

            PrepareBrushesAndPens();

            if (p_format == null)
            {
                p_format = new StringFormat();
                p_format.Alignment = StringAlignment.Center;
                p_format.LineAlignment = StringAlignment.Center;
            }

            if (!prevContent.Equals(ContentCells))
            {
                AnalyzeContents();
                prevContent = ContentCells;
                if (p_xmax < 1 || p_ymax < 1)
                    return;
            }

            Rectangle textBounds = bounds;
            textBounds.X += Style.ContentPadding.Left;
            textBounds.Y += Style.ContentPadding.Top;
            textBounds.Width -= (Style.ContentPadding.Left + Style.ContentPadding.Right);
            textBounds.Height -= (Style.ContentPadding.Top + Style.ContentPadding.Bottom);

            // size of one cell
            p_cellx = Math.Min(textBounds.Height / p_ymax, textBounds.Width / p_xmax);
            Font usedFont = SMGraphics.GetFontVariation(Style.Font, p_cellx * 0.7f);


            // prepare rectangle for letter
            Rectangle rect = new Rectangle();
            rect.Width = p_cellx;
            rect.Height = p_cellx;
            Brush bb;

            int lastIndex = p_ymax - 1;
            for (int x = 0; x < p_xmax; x++)
            {
                rect.X = textBounds.X + x * p_cellx;
                for (int y = 0; y < p_ymax; y++)
                {
                    rect.Y = textBounds.Y + y * p_cellx;
                    if (p_array[x, y, 1].Length > 0)
                    {
                        if (x == p_focusX && y == p_focusY && UIStateError == MNEvaluationResult.Focused)
                            context.g.FillRectangle(Brushes.LightBlue, rect);
                        else
                            context.g.FillRectangle(tempBackBrush, rect);
                        context.g.DrawLine(tempBorderPen, rect.X, rect.Y, rect.X + p_cellx, rect.Y);
                        context.g.DrawLine(tempBorderPen, rect.X, rect.Y, rect.X, rect.Y + p_cellx);
                        if (y >= p_ymax - 1 || p_array[x,y+1,1].Length == 0)
                            context.g.DrawLine(tempBorderPen, rect.X, rect.Y + p_cellx, rect.X + p_cellx, rect.Y + p_cellx);
                        if (x >= p_xmax - 1 || p_array[x+1,y,1].Length == 0)
                            context.g.DrawLine(tempBorderPen, rect.X + p_cellx, rect.Y, rect.X + p_cellx, rect.Y + p_cellx);
                        bb = ((UIStateError == MNEvaluationResult.Incorrect
                            && (p_array[x,y,0] != p_array[x,y,1])) ? Brushes.Red : tempForeBrush);
                        context.g.DrawString(p_array[x, y, 0], usedFont, bb, rect, p_format);
                    }
                }
            }


            // draw selection marks
            base.Paint(context);
        }

        public override void OnClick(PVDragContext dc)
        {
            SMRectangleArea area = Page.GetArea(Id);
            Rectangle bounds = area.GetBounds(dc.context);

            int x = dc.lastPoint.X - Style.ContentPadding.Left - bounds.X;
            int y = dc.lastPoint.Y - Style.ContentPadding.Top - bounds.Y;

            if (x < 0) x = 0;
            if (y < 0) y = 0;

            p_focusX = x / p_cellx;
            p_focusY = y / p_cellx;

            if (!IsExpectedCell(p_focusX, p_focusY))
            {
                FindNearestCell();
            }

            p_lastMove = 0;

            base.OnClick(dc);
        }

        private void AnalyzeContents()
        {
            cells.Clear();
            string[] p = ContentCells.Split(';');
            char[] splitter = { ' ' };
            foreach (string pp in p)
            {
                string[] n = pp.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (n.Length == 5 && (n[2] == "H" || n[2] == "V") && n[4].Length > 0)
                {
                    int x, y;
                    if (int.TryParse(n[0], out x) && int.TryParse(n[1], out y))
                    {
                        CellInfo ci = new CellInfo();
                        ci.x1 = x;
                        ci.y1 = y;
                        if (n[2] == "H")
                        {
                            ci.x2 = x + n[4].Length - 1;
                            ci.y2 = y;
                        }
                        else
                        {
                            ci.x2 = x;
                            ci.y2 = y + n[4].Length - 1;
                        }

                        ci.VisibleLetters = n[3];
                        ci.Letters = n[4];

                        if (ci.VisibleLetters.Length < ci.Letters.Length)
                        {
                            ci.VisibleLetters = n[3].PadRight(n[4].Length, '_');
                        }

                        cells.Add(ci);
                    }
                }
            }

            int xmin = -1, xmax = -1, ymin = -1, ymax = -1;
            foreach (CellInfo ci in cells)
            {
                if (xmin < 0) xmin = ci.x1; else xmin = Math.Min(xmin, ci.x1);
                if (ymin < 0) ymin = ci.y1; else ymin = Math.Min(ymin, ci.y1);
                if (xmax < 0) xmax = ci.x2; else xmax = Math.Max(xmax, ci.x2);
                if (ymax < 0) ymax = ci.y2; else ymax = Math.Max(ymax, ci.y2);
            }

            foreach (CellInfo ci in cells)
            {
                ci.x1 -= xmin;
                ci.x2 -= xmin;
                ci.y1 -= ymin;
                ci.y2 -= ymin;
            }

            ymax -= ymin;
            xmax -= xmin;
            ymin = 0;
            xmin = 0;

            p_array = new string[xmax + 1, ymax + 1, 2];
            p_xmax = xmax + 1;
            p_ymax = ymax + 1;

            for (int i = 0; i < p_xmax; i++)
            {
                for (int j = 0; j < p_ymax; j++)
                {
                    p_array[i, j, 0] = string.Empty;
                    p_array[i, j, 1] = string.Empty;
                }
            }

            foreach (CellInfo ci in cells)
            {
                int len = ci.VisibleLetters.Length;
                int xd = (ci.x1 == ci.x2 ? 0 : 1);
                int yd = (ci.y1 == ci.y2 ? 0 : 1);
                int sx = ci.x1;
                int sy = ci.y1;
                for (int i = 0; i < len; i++)
                {
                    p_array[sx, sy, 0] = ci.VisibleLetters[i] == '_' ? string.Empty : ci.VisibleLetters[i].ToString();
                    p_array[sx, sy, 1] = ci.Letters[i].ToString();
                    sx += xd;
                    sy += yd;
                }
            }
        }

        public override MNEvaluationResult Evaluate()
        {
            if (HasImmediateEvaluation || HasLazyEvaluation)
            {
                for (int i = 0; i < p_xmax; i++)
                {
                    for (int j = 0; j < p_ymax; i++)
                    {
                        if (p_array[i, j, 0].ToLower().Equals(p_array[i, j, 1].ToLower()))
                        {
                            UIStateError = MNEvaluationResult.Incorrect;
                            return UIStateError;
                        }
                    }
                }
            }
            return base.Evaluate();
        }

        public bool IsExpectedCell(int x, int y)
        {
            if (x < p_xmax && y < p_ymax)
                return (p_array[x, y, 1].Length > 0);
            else
                return false;
        }

        public bool IsInteractedCell(int x, int y)
        {
            if (x < p_xmax && y < p_ymax)
                return (p_array[x, y, 0].Length > 0);
            else
                return false;
        }

        public bool FindFirstEmptyCell()
        {
            for (int i = 0; i < p_xmax; i++)
            {
                for (int j = 0; j < p_ymax; i++)
                {
                    if (IsExpectedCell(i, j) && !IsInteractedCell(i,j))
                    {
                        p_focusX = i;
                        p_focusY = j;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool GotoNextCell()
        {
            bool b1, b2;

            b1 = IsExpectedCell(p_focusX + 1, p_focusY);
            b2 = IsExpectedCell(p_focusX, p_focusY + 1);

            if (b1 && b2)
            {
                if (p_lastMove == 1)
                {
                    p_focusX++;
                    return true;
                }
                else if (p_lastMove == 2)
                {
                    p_focusY++;
                    return true;
                }
                else
                {
                    b1 = IsInteractedCell(p_focusX + 1, p_focusY);
                    b2 = IsInteractedCell(p_focusX, p_focusY + 1);
                }
            }
            
            if (b2 && b1 || b1)
            {
                p_focusX++;
                p_lastMove = 1;
                return true;
            }
            else if (b2)
            {
                p_focusY++;
                p_lastMove = 2;
                return true;
            }

            return false;
        }

        public bool FindNearestCell()
        {
            for (int i = 1; i < 20; i++)
            {
                //p[0]
                p_focusX++;
                p_focusY++;
                if (IsExpectedCell(p_focusX, p_focusY))
                    return true;

                for (int j = 0; j < i + 1; j++)
                {
                    p_focusY--;
                    if (IsExpectedCell(p_focusX, p_focusY))
                        return true;
                }
                for (int j = 0; j < i + 1; j++)
                {
                    p_focusX--;
                    if (IsExpectedCell(p_focusX, p_focusY))
                        return true;
                }
                for (int j = 0; j < i + 1; j++)
                {
                    p_focusY++;
                    if (IsExpectedCell(p_focusX, p_focusY))
                        return true;
                }
                for (int j = 0; j < i + 1; j++)
                {
                    p_focusX++;
                    if (IsExpectedCell(p_focusX, p_focusY))
                        return true;
                }
            }

            return false;
        }

        public void AcceptString(string s)
        {
            foreach (char c in s)
            {
                AcceptChar(c);
            }
        }

        public void AcceptChar(char c)
        {
            if (IsExpectedCell(p_focusX, p_focusY))
            {
                p_array[p_focusX, p_focusY, 0] = c.ToString();
                GotoNextCell();
            }
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
