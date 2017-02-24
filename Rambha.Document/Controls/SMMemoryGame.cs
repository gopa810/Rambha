using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using Rambha.Document;
using Rambha.Script;
using Rambha.Serializer;

namespace Rambha.Document
{
    public class SMMemoryGame: SMControl
    {
        private class SMMemoryGameCard
        {
            public string Tag = string.Empty;
            public MNLazyImage Image = null;

            public SMMemoryGameCard(MNDocument doc)
            {
                Image = new MNLazyImage(doc);
            }

            /// <summary>
            /// True if card is visible (VisibleImage is displayed)
            /// False if card is hidden (HiddenImage is displayed)
            /// </summary>
            public SMMemoryCardState State = SMMemoryCardState.Hidden;

            /// <summary>
            /// True if changing of state is enabled
            /// False if changing of state is disabled
            /// </summary>
            public bool CanChangeState = true;

            public void NextState()
            {
                if (State == SMMemoryCardState.Hidden)
                    State = SMMemoryCardState.HiddenToVisible;
                else if (State == SMMemoryCardState.HiddenToVisible)
                    State = SMMemoryCardState.Visible;
                else if (State == SMMemoryCardState.Visible)
                    State = SMMemoryCardState.VisibleToHidden;
                else if (State == SMMemoryCardState.VisibleToHidden)
                    State = SMMemoryCardState.Hidden;
            }

            public void ReverseState()
            {
                if (State == SMMemoryCardState.Hidden)
                    State = SMMemoryCardState.Visible;
                else if (State == SMMemoryCardState.Visible)
                    State = SMMemoryCardState.Hidden;
            }
        }

        private enum SMMemoryCardState
        {
            Visible,
            VisibleToHidden,
            Hidden,
            HiddenToVisible
        }

        private enum SMMemoryGameEval
        {
            Matched,
            Incomplete,
            Mismatched
        }

        private int p_rows = 0;
        private int p_columns = 0;

        public MNLazyImage BackImage = null;

        private List<SMMemoryGameCard> cards = new List<SMMemoryGameCard>();

        private SMMemoryGameCard[,] matrix = null;

        private Size lastCellSize = Size.Empty;

        public SMMemoryGame(MNPage page): base(page)
        {
            Rows = 2;
            Columns = 2;
            BackImage = new MNLazyImage(page.Document);
        }

        public int Rows 
        {
            get { return p_rows; }
            set { p_rows = value; RecalcMatrix(); } 
        }

        public int Columns 
        {
            get { return p_columns; }
            set { p_columns = value; RecalcMatrix(); }
        }

        private void RecalcMatrix()
        {
            if (p_rows > 0 && p_columns > 0)
            {
                matrix = new SMMemoryGameCard[p_rows, p_columns];
                for (int i = 0; i < p_rows; i++)
                    for (int j = 0; j < p_columns; j++)
                        matrix[i, j] = null;
                MixCards();
            }

        }

        private void MixCards()
        {
            if (cards.Count == 0 || Rows < 1 || Columns < 1)
                return;

            Random rnd = new Random();
            List<SMMemoryGameCard> temp = new List<SMMemoryGameCard>();
            int max = (Rows * Columns);
            if (max % 2 > 0) max--;
            int start = rnd.Next(0, cards.Count);
            if (start % 2 > 0) start++;
            for (int k = 0; k < max; k++)
                temp.Add(cards[(k + start)%cards.Count]);


            for (int i = 0; i < p_rows; i++)
            {
                for (int j = 0; j < p_columns; j++)
                {
                    matrix[i, j] = GetRandomCard(rnd, temp);
                }
            }
        }

        public int GetCardCount()
        {
            return cards.Count;
        }

        public MNReferencedImage GetReferencedImageAt(int i)
        {
            return cards[i].Image.Image;
        }

        public string GetTagAt(int i)
        {
            return cards[i].Tag;
        }

        public void AddCard(string tg, MNReferencedImage mri)
        {
            SMMemoryGameCard nc = new SMMemoryGameCard(Document);
            nc.Tag = tg;
            nc.Image.Image = mri;
            cards.Add(nc);

            if (matrix != null)
            {
                for (int r = 0; r < matrix.GetLength(0); r++)
                {
                    for (int c = 0; c < matrix.GetLength(1); c++)
                    {
                        if (matrix[r, c] == null)
                        {
                            matrix[r, c] = nc;
                            // to stop loop
                            r = matrix.GetLength(0);
                            break;
                        }
                    }
                }
            }
        }

        private SMMemoryGameCard GetRandomCard(Random r, List<SMMemoryGameCard> list)
        {
            if (list.Count == 0)
                return null;
            int a = r.Next(0, list.Count);
            SMMemoryGameCard card = list[a];
            list.RemoveAt(a);
            return card;
        }

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            if (token.Equals("addCard"))
            {
                string tag = args.getSafe(0).getStringValue();
                string img1 = args.getSafe(1).getStringValue();

                MNReferencedImage i1 = Document.FindImage(img1);

                if (i1 != null)
                {
                    SMMemoryGameCard card = new SMMemoryGameCard(Document);
                    card.Tag = tag;
                    card.Image.Image = i1;

                    cards.Add(card);
                }
            }
            else if (token.Equals("mixCards"))
            {
                MixCards();
            }
            else if (token.Equals("clearChanged"))
            {
                ClearChanged();
            }
            return base.ExecuteMessage(token, args);
        }

        public override void Paint(MNPageContext context)
        {
            base.Paint(context);

            SMRectangleArea area = Page.GetArea(Id);
            Rectangle bounds = area.GetBounds(context);
            Size cell = new Size(bounds.Width / Columns, bounds.Height / Rows);
            lastCellSize = cell;

            if (context.drawSelectionMarks)
            {
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Columns; c++)
                    {
                        Rectangle rect = GetCellRect(context, r, c, cell);
                        rect.Offset(bounds.Location);
                        context.g.DrawRectangle(Pens.LightGray, rect);
                    }
                }
            }

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    Rectangle rect = GetCellRect(context, r, c, cell);
                    rect.Offset(bounds.Location);
                    SMMemoryGameCard card = matrix[r, c];
                    if (card != null)
                    {
                        if (card.State == SMMemoryCardState.Hidden)
                        {
                            if (BackImage.Image != null)
                                DrawCard(context, rect, BackImage.Image);
                        }
                        else if (card.State == SMMemoryCardState.HiddenToVisible)
                        {
                            if (BackImage.Image != null)
                                DrawCard(context, rect, BackImage.Image);
                            if (card.Image != null)
                                DrawCard(context, rect, card.Image.Image);
                        }
                        else if (card.State == SMMemoryCardState.Visible)
                        {
                            if (card.Image != null)
                                DrawCard(context, rect, card.Image.Image);
                        }
                        else if (card.State == SMMemoryCardState.VisibleToHidden)
                        {
                            if (card.Image != null)
                                DrawCard(context, rect, card.Image.Image);
                            if (BackImage.Image != null)
                                DrawCard(context, rect, BackImage.Image);
                        }
                    }
                }
            }
        }

        private void DrawCard(MNPageContext context, Rectangle rect, MNReferencedImage image)
        {
            Size imgSize = SMGraphics.GetMaximumSize(rect, image.ImageData.Size);
            context.g.DrawImage(image.ImageData, rect.X + rect.Width / 2 - imgSize.Width / 2,
                rect.Y + rect.Height / 2 - imgSize.Height / 2, imgSize.Width, imgSize.Height);
        }

        public Rectangle GetCellRect(MNPageContext ctx, int row, int column, Size cellSize)
        {
            Rectangle r = new Rectangle(column * cellSize.Width, row * cellSize.Height, cellSize.Width, cellSize.Height);
            return Style.ApplyPadding(r);
        }

        private int p_last_row = 0;
        private int p_last_col = 0;

        public override void OnTapBegin(PVDragContext dc)
        {
            int r, c;

            if (GetTapPoint(dc, out r, out c))
            {
                p_last_col = c;
                p_last_row = r;

                Debugger.Log(0, "", string.Format("START coordinates: {0},{1}\n\n", r, c));

                if (matrix[r, c] != null && matrix[r,c].CanChangeState)
                {
                    matrix[r, c].NextState();
                }
            }

            base.OnTapBegin(dc);
        }

        private bool GetTapPoint(PVDragContext dc, out int r, out int c)
        {
            SMRectangleArea area = Page.GetArea(Id);
            Rectangle bounds = area.GetBounds(dc.context);
            c = (dc.lastPoint.X - bounds.X) / lastCellSize.Width;
            r = (dc.lastPoint.Y - bounds.Y) / lastCellSize.Height;
            return (r >= 0 && r < Rows && c >= 0 && c < Columns);
        }

        public override void OnTapEnd(PVDragContext dc)
        {
            int r, c;

            if (GetTapPoint(dc, out r, out c))
            {
                Debugger.Log(0,"", string.Format("END coordinates: {0},{1}\nPrevious: {2},{3}\n\n", r, c, p_last_row, p_last_col));
                if (p_last_col == c && p_last_row == r)
                {
                    if (matrix[r, c] != null && matrix[r, c].CanChangeState)
                    {
                        matrix[r, c].NextState();
                        SMMemoryGameEval ge = CheckStatus();
                        if (ge == SMMemoryGameEval.Matched)
                        {
                            MakeUnchangeable();
                        }
                        else if (ge == SMMemoryGameEval.Mismatched)
                        {
                            if (dc.context.ViewController != null)
                            {
                                GSCoreCollection args = new GSCoreCollection();
                                args.Add(new GSInt32(1500));
                                args.Add(this);
                                args.Add(new GSString("clearChanged"));
                                dc.context.ViewController.ExecuteMessage("scheduleCall", args);
                            }
                        }
                        else if (ge == SMMemoryGameEval.Incomplete)
                        {
                        }
                    }
                }
                else
                {
                    matrix[p_last_row, p_last_col].ReverseState();
                }
            }

        }

        private void ClearChanged()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (matrix[r, c] != null && matrix[r, c].CanChangeState
                        && matrix[r, c].State == SMMemoryCardState.Visible)
                    {
                        matrix[r, c].State = SMMemoryCardState.Hidden;
                    }
                }
            }
        }

        private void MakeUnchangeable()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (matrix[r, c] != null && matrix[r, c].CanChangeState
                        && matrix[r, c].State == SMMemoryCardState.Visible)
                    {
                        matrix[r, c].CanChangeState = false;
                    }
                }
            }
        }

        private SMMemoryGameEval CheckStatus()
        {
            List<SMMemoryGameCard> list = new List<SMMemoryGameCard>();
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (matrix[r, c] != null && matrix[r, c].CanChangeState
                        && matrix[r, c].State == SMMemoryCardState.Visible)
                    {
                        list.Add(matrix[r, c]);
                    }
                }
            }

            if (list.Count > 2)
                return SMMemoryGameEval.Mismatched;
            if (list.Count < 2)
                return SMMemoryGameEval.Incomplete;
            if (list[0].Tag.Equals(list[1].Tag))
                return SMMemoryGameEval.Matched;
            else
                return SMMemoryGameEval.Mismatched;
        }

        public override void OnTapCancel(PVDragContext dc)
        {
            base.OnTapCancel(dc);
        }

        public override void LoadStatus(RSFileReader br)
        {
            base.LoadStatus(br);

            byte b;
            int r, c;
            while ((b = br.ReadByte()) != 0)
            {
                if (b == 10)
                {
                    r = br.ReadInt32();
                    c = br.ReadInt32();
                    matrix = new SMMemoryGameCard[r, c];
                }
                else if (b == 20)
                {
                    r = br.ReadInt32();
                    c = br.ReadInt32();
                    matrix[r, c] = new SMMemoryGameCard(Document);
                    matrix[r, c].Tag = br.ReadString();
                    matrix[r, c].State = (SMMemoryCardState)br.ReadInt32();
                    matrix[r, c].CanChangeState = br.ReadBool();
                }
            }

            for (r = 0; r < Rows; r++)
            {
                for (c = 0; c < Columns; c++)
                {
                    if (matrix[r, c] != null)
                    {
                        SMMemoryGameCard card = FindCard(matrix[r,c].Tag);
                        if (card != null)
                        {
                            matrix[r,c].Image = card.Image;
                        }
                    }
                }
            }
        }

        private SMMemoryGameCard FindCard(string tag)
        {
            foreach (SMMemoryGameCard card in cards)
            {
                if (card.Tag.Equals(tag))
                    return card;
            }
            return null;
        }

        public override void SaveStatus(RSFileWriter bw)
        {
            base.SaveStatus(bw);

            if (Rows > 0 && Rows < 100 && Columns > 0 && Columns < 100)
            {
                bw.WriteByte(10);
                bw.WriteInt32(Rows);
                bw.WriteInt32(Columns);


                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Columns; c++)
                    {
                        if (matrix[r, c] != null)
                        {
                            bw.WriteByte(20);
                            bw.WriteInt32(r);
                            bw.WriteInt32(c);
                            bw.WriteString(matrix[r, c].Tag);
                            bw.WriteInt32((int)matrix[r,c].State);
                            bw.WriteBool(matrix[r, c].CanChangeState);
                        }
                    }
                }
            }

            bw.WriteByte(0);
        }

        public override bool Load(RSFileReader br)
        {
            if (base.Load(br))
            {
                byte b;
                cards.Clear();
                while ((b = br.ReadByte()) != 0)
                {
                    switch (b)
                    {
                        case 10:
                            Rows = br.ReadInt32();
                            break;
                        case 11:
                            Columns = br.ReadInt32();
                            break;
                        case 12:
                            SMMemoryGameCard card = new SMMemoryGameCard(Document);
                            while ((b = br.ReadByte()) != 0)
                            {
                                switch (b)
                                {
                                    case 100:
                                        card.Tag = br.ReadString();
                                        break;
                                    case 110:
                                        card.Image.ImageId = br.ReadInt64();
                                        break;
                                }
                            }
                            cards.Add(card);
                            break;
                        case 13:
                            BackImage.ImageId = br.ReadInt64();
                            break;
                    }
                }

                MixCards();
            }

            return true;
        }

        public override void Save(RSFileWriter bw)
        {
            base.Save(bw);

            bw.WriteByte(10);
            bw.WriteInt32(Rows);

            bw.WriteByte(11);
            bw.WriteInt32(Columns);

            foreach (SMMemoryGameCard card in cards)
            {
                bw.WriteByte(12);

                // writing card
                bw.WriteByte(100);
                bw.WriteString(card.Tag);

                bw.WriteByte(110);
                bw.WriteInt64(card.Image.ImageId);

                bw.WriteByte(0);
                // end of writing card
            }

            bw.WriteByte(13);
            bw.WriteInt64(BackImage.ImageId);

            bw.WriteByte(0);
        }
    }
}
