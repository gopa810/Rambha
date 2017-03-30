using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker
{
    public partial class ToolWindowInsertPageLayout : Form
    {
        public const int ITEM_HEIGHT = 70;
        public const int PADDING = 2;
        public const int ICON_HEIGHT = 66;
        public const int ICON_WIDTH = 88;

        public Font NameFont = new Font(FontFamily.GenericSansSerif, 20);

        public ToolWindowInsertPageLayout()
        {
            InitializeComponent();
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = ITEM_HEIGHT;
        }

        public MNPage SelectedTemplate
        {
            get
            {
                int Index = listBox1.SelectedIndex;
                if (Index >= 0 && Index < listBox1.Items.Count)
                {
                    return (MNPage)listBox1.Items[Index];
                }
                return null;
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e.Graphics.FillRectangle(Brushes.Azure, e.Bounds);
            else
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            Rectangle drawRect = new Rectangle(PADDING, PADDING, ICON_WIDTH, ICON_HEIGHT);

            if (e.Index >= 0 && e.Index < listBox1.Items.Count)
            {
                MNPage p = listBox1.Items[e.Index] as MNPage;
                drawRect.Y = e.Bounds.Top + PADDING;
                DrawPageInto(e.Graphics, p, drawRect);

                e.Graphics.DrawString(p.Title, NameFont, Brushes.Black, ICON_WIDTH + 3 * PADDING, e.Bounds.Top + ICON_HEIGHT / 2 - PADDING);
            }

        }

        private void DrawPageInto(Graphics graphics, MNPage p, Rectangle drawRect)
        {
            graphics.DrawRectangle(Pens.Black, drawRect);
            foreach (SMControl sc in p.Objects)
            {
                SMRectangleArea area = sc.Area;
                Rectangle rect = area.GetBounds(PageEditDisplaySize.LandscapeBig);
                rect.X = Convert.ToInt32(Convert.ToDouble(rect.X) * drawRect.Width / 1024);
                rect.Y = Convert.ToInt32(Convert.ToDouble(rect.Y) * drawRect.Height / 768);
                rect.Width = Convert.ToInt32(Convert.ToDouble(rect.Width) * drawRect.Width / 1024);
                rect.Height = Convert.ToInt32(Convert.ToDouble(rect.Height) * drawRect.Height / 768);
                rect.Y += drawRect.Top;
                rect.X += drawRect.Left;

                graphics.DrawRectangle(Pens.Gray, rect);
                if (sc is SMImage)
                {
                    graphics.DrawLine(Pens.Gray, rect.Left, rect.Bottom, rect.Right, rect.Top);
                    graphics.DrawLine(Pens.Gray, rect.Right, rect.Bottom, rect.Left, rect.Top);
                }
            }
        }

        private char[] spaceCharacterSet = { ' ' };

        private class PageStats
        {
            public int texts = -1;
            public int pictures = -1;
            public int selections = -1;
            public int checkboxes = -1;
            public int textview = -1;

            public bool Contains(PageStats ps)
            {
                if (texts >= 0 && ps.texts != texts) return false;
                if (pictures >= 0 && ps.pictures != pictures) return false;
                if (selections >= 0 && ps.selections != selections) return false;
                if (checkboxes >= 0 && ps.checkboxes != checkboxes) return false;
                if (textview >= 0 && ps.textview != textview) return false;
                return true;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (texts >= 0) sb.AppendFormat("{0}t ", texts);
                if (pictures >= 0) sb.AppendFormat("{0}p ", pictures);
                if (selections >= 0) sb.AppendFormat("{0}s ", selections);
                if (checkboxes >= 0) sb.AppendFormat("{0}c ", checkboxes);
                if (textview >= 0) sb.AppendFormat("{0}tv ", textview);
                return sb.ToString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string[] tags = textBox1.Text.Split(spaceCharacterSet, StringSplitOptions.RemoveEmptyEntries);
            PageStats ps = new PageStats();
            PageStats pageFindStats = new PageStats();
            string text = "";
            foreach (string sto in tags)
            {
                if (Char.IsLetter(sto[0]))
                    text = sto;
                else if (sto.EndsWith("t"))
                    int.TryParse(sto.Substring(0, sto.Length - 1), out ps.texts);
                else if (sto.EndsWith("c"))
                    int.TryParse(sto.Substring(0, sto.Length - 1), out ps.checkboxes);
                else if (sto.EndsWith("p"))
                    int.TryParse(sto.Substring(0, sto.Length - 1), out ps.pictures);
                else if (sto.EndsWith("s"))
                    int.TryParse(sto.Substring(0, sto.Length - 1), out ps.selections);
                else if (sto.EndsWith("tv"))
                    int.TryParse(sto.Substring(0, sto.Length - 2), out ps.textview);
            }

            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            foreach (MNPage page in MNSharedObjects.internalDocument.Data.Templates)
            {
                if (text.Length > 0)
                {
                    if (page.Title.IndexOf(text, 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        listBox1.Items.Add(page);
                    }
                }
                else
                {
                    FillPageStats(pageFindStats, page);
                    //Debugger.Log(0, "", "Page " + page.Title + " = " + pageFindStats.ToString() + "\n");
                    if (ps.Contains(pageFindStats))
                        listBox1.Items.Add(page);
                }
            }
            listBox1.EndUpdate();

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
        }

        private void FillPageStats(PageStats ps, MNPage p)
        {
            ps.checkboxes = 0;
            ps.selections = 0;
            ps.texts = 0;
            ps.pictures = 0;
            ps.textview = 0;
            foreach (SMControl c in p.Objects)
            {
                if (c is SMImage)
                    ps.pictures++;
                else if (c is SMLabel)
                    ps.texts++;
                else if (c is SMCheckBox)
                    ps.checkboxes++;
                else if (c is SMSelection)
                    ps.selections++;
                else if (c is SMTextView)
                    ps.textview++;
            }
        }

        private bool ContainsTags(string[] tags, string p)
        {
            string[] tp = p.Split(spaceCharacterSet, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in tags)
            {
                if (p.IndexOf(s) < 0)
                    return false;
            }

            return true;
        }
    }
}
