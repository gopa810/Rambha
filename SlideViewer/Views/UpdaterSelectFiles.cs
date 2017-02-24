using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideViewer
{
    public partial class UpdaterSelectFiles : UserControl
    {
        public int PaddingTop = 8;
        public int PaddingBottom = 8;
        public int PaddingLeft = 18;
        public int PaddingRight = 8;

        public Font HeaderFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
        public Font BookFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Regular);

        public StringFormat stringFormat = new StringFormat();


        public event GeneralArgsEvent OnApplyChanges;

        public event GeneralArgsEvent OnDiscardChanges;
        public SVBookLibrary Library;

        public UpdaterSelectFiles()
        {
            InitializeComponent();

            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Center;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = listBox1;
            object item = GetItem(lb, listBox1.SelectedIndex);
            if (item is RemoteFileRef)
            {
                RemoteFileRef rf = item as RemoteFileRef;
                listBox2.BeginUpdate();
                listBox2.Items.Clear();
                foreach (RemoteFileRef rl in rf.Subs)
                {
                    listBox2.Items.Add(rl);
                }
                listBox2.EndUpdate();
            }
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (!(sender is ListBox)) return;
            ListBox lb = sender as ListBox;
            object item = GetItem(lb, e.Index);
            if (item == null)
                return;

            if (item is SMVItem)
            {
                if ((item as SMVItem).IsHeader)
                {
                    e.ItemHeight = Convert.ToInt32(PaddingBottom + PaddingTop + HeaderFont.SizeInPoints);
                }
                else
                {
                    e.ItemHeight = Convert.ToInt32(PaddingBottom + PaddingTop + BookFont.SizeInPoints);
                }
            }
            else if (item is RemoteFileRef)
            {
                e.ItemHeight = Convert.ToInt32(PaddingBottom + PaddingTop + BookFont.SizeInPoints);
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!(sender is ListBox)) return;
            ListBox lb = sender as ListBox;
            // draw contents
            object item = GetItem(lb, e.Index);
            if (item == null)
                return;

            if (item is SMVItem)
            {
                if ((item as SMVItem).IsHeader)
                {
                    e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);

                    Rectangle textBounds = e.Bounds;
                    textBounds.Location.Offset(PaddingLeft, PaddingTop);
                    textBounds.Width = textBounds.Width - PaddingLeft - PaddingRight;
                    textBounds.Height = textBounds.Height - PaddingTop - PaddingBottom;

                    e.Graphics.DrawString((item as SMVItem).Text, HeaderFont, Brushes.White, textBounds, stringFormat);
                }
                else
                {
                    // draw background
                    e.Graphics.FillRectangle(0 != (e.State & DrawItemState.Selected) ? Brushes.LightGreen : Brushes.White, e.Bounds);

                    Rectangle textBounds = e.Bounds;
                    textBounds.Location.Offset(PaddingLeft + e.Bounds.Height, PaddingTop);
                    textBounds.Width = textBounds.Width - PaddingLeft - PaddingRight - e.Bounds.Height;
                    textBounds.Height = textBounds.Height - PaddingTop - PaddingBottom;

                    int dim = textBounds.Height;

                    e.Graphics.DrawRectangle(Pens.Black, PaddingLeft, PaddingTop, dim, dim);
                    if ((item as SMVItem).Selected)
                    {
                        e.Graphics.DrawLine(Pens.Black, PaddingLeft + e.Bounds.X, PaddingTop + e.Bounds.Y, PaddingLeft + dim + e.Bounds.X, PaddingTop + dim + e.Bounds.Y);
                        e.Graphics.DrawLine(Pens.Black, PaddingLeft + e.Bounds.X, PaddingTop + e.Bounds.Y + dim, PaddingLeft + dim + e.Bounds.X, PaddingTop + e.Bounds.Y);
                    }

                    e.Graphics.DrawString((item as SMVItem).Text, HeaderFont, Brushes.White, textBounds, stringFormat);

                }
            }
            else if (item is RemoteFileRef)
            {
                // draw background
                e.Graphics.FillRectangle(0 != (e.State & DrawItemState.Selected) ? Brushes.LightGreen : Brushes.White, e.Bounds);

                Rectangle textBounds = e.Bounds;
                textBounds.X += PaddingLeft + e.Bounds.Height;
                textBounds.Y += PaddingTop;
                textBounds.Width = textBounds.Width - PaddingLeft - PaddingRight - e.Bounds.Height;
                textBounds.Height -= PaddingTop + PaddingBottom;

                int dim = textBounds.Height;

                e.Graphics.DrawRectangle(Pens.Black, PaddingLeft, PaddingTop, dim, dim);
                if ((item as RemoteFileRef).Selected)
                {
                    e.Graphics.DrawLine(Pens.Black, PaddingLeft, PaddingTop, PaddingLeft + dim, PaddingTop + dim);
                    e.Graphics.DrawLine(Pens.Black, PaddingLeft, PaddingTop + dim, PaddingLeft + dim, PaddingTop);
                }

                e.Graphics.DrawString((item as RemoteFileRef).Text, HeaderFont, Brushes.Black, textBounds, stringFormat);
            }
        }

        private object GetItem(ListBox lb, int i)
        {
            if (0 <= i && i < lb.Items.Count)
            {
                return lb.Items[i];
            }

            return null;
        }

        public void AddItem(SMVItem item)
        {
            listBox1.Items.Add(item);
        }

        private void buttonDiscard_Click(object sender, EventArgs e)
        {
            if (OnDiscardChanges != null)
                OnDiscardChanges(this, e);
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (OnApplyChanges != null)
                OnApplyChanges(this, e);
        }

        public void UpdateLists()
        {
            List<SMVItem> items = new List<SMVItem>();
            /*
            List<RemoteFileRef> news = Library.GetNewFiles();

            // copy files into listbox
            listBox1.Items.Clear();

            // new items
            if (countOfNew > 0)
            {
                listBox1.Items.Add(SMVItem.MakeHeader("New Books"));
                foreach (RemoteFileRef rf in localDB)
                {
                    if (rf.Local == false)
                        listBox1.Items.Add(rf);
                }
            }

            if (countOfUpdates > 0)
            {
                listBox1.Items.Add(SMVItem.MakeHeader("Updates"));
                foreach (RemoteFileRef rf in localDB)
                {
                    if (rf.HasUpdate())
                    {
                        SMVItem item = SMVItem.MakeEntry(rf.Text, rf);
                        item.Selected = true;

                        listBox1.Items.Add(item);
                    }
                }
            }*/
            
            // all items
            listBox1.Items.Clear();
            listBox1.Items.Add(SMVItem.MakeHeader("Installed Books"));
            foreach (RemoteFileRef rf in Library.DatabaseStatus)
            {
                if (rf.Local)
                    listBox1.Items.Add(rf);
            }

        }


        private void HandleMouseClick(ListBox lb, Point pt)
        {
            int index = lb.IndexFromPoint(pt);

            Rectangle rect = lb.GetItemRectangle(index);
            if (!rect.Contains(pt))
                return;

            object item = GetItem(lb, index);
            if (item == null)
                return;

            if (item is SMVItem)
            {
                if ((item as SMVItem).IsHeader)
                    return;

                rect.Width = rect.Height + PaddingLeft;
                if (rect.Contains(pt))
                {
                    (item as SMVItem).Selected = !(item as SMVItem).Selected;
                    lb.Invalidate();
                }
            }
            else
            {
                rect.Width = rect.Height + PaddingLeft;
                if (rect.Contains(pt))
                {
                    (item as RemoteFileRef).Selected = !(item as RemoteFileRef).Selected;
                    lb.Invalidate();
                }
            }
        }

        private void listBox2_MouseClick(object sender, MouseEventArgs e)
        {
            HandleMouseClick(listBox2, new Point(e.X, e.Y));
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            HandleMouseClick(listBox1, new Point(e.X, e.Y));
        }
    }

    public class SMVItem
    {
        public string Text = "";
        public bool IsHeader = false;
        public RemoteFileRef File = null;

        public List<SMVItem> Items = new List<SMVItem>();

        public static SMVItem MakeHeader(string t)
        {
            SMVItem sm = new SMVItem();
            sm.Text = t;
            sm.IsHeader = true;
            return sm;
        }

        public static SMVItem MakeEntry(string t, RemoteFileRef b)
        {
            SMVItem sm = new SMVItem();
            sm.Text = t;
            sm.IsHeader = false;
            sm.File = b;
            return sm;
        }


        public bool Selected = false;
    }

    public class RemoteFileRef
    {
        public bool Local = false;
        public bool Selected = false;
        public string FileName = "";
        public string Text = "";
        public string LastTime = "";
        public bool NewVersionAvailable = false;

        public List<RemoteFileRef> Subs = new List<RemoteFileRef>();

        internal bool HasUpdate()
        {
            if (NewVersionAvailable) return true;
            foreach (RemoteFileRef rf in Subs)
            {
                if (rf.HasUpdate()) return true;
            }
            return false;
        }
    }

}
