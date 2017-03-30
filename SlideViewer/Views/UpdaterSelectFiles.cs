using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

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

        public List<string> FilesToDelete = new List<string>();
        public List<string> FilesToDownload = new List<string>();

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
                    if (!rl.Text.Equals("Default"))
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
                    textBounds.X += PaddingLeft;
                    textBounds.Y += PaddingTop;
                    textBounds.Width -= PaddingLeft + PaddingRight;
                    textBounds.Height -= PaddingTop + PaddingBottom;

                    e.Graphics.DrawString((item as SMVItem).Text, HeaderFont, Brushes.White, e.Bounds.X + PaddingLeft, e.Bounds.Y + PaddingTop);
                }
                else
                {
                    // draw background
                    e.Graphics.FillRectangle(0 != (e.State & DrawItemState.Selected) ? Brushes.LightGreen : Brushes.White, e.Bounds);

                    Rectangle textBounds = e.Bounds;
                    textBounds.X += PaddingLeft + e.Bounds.Height;
                    textBounds.Y += PaddingTop;
                    textBounds.Width -= PaddingLeft + PaddingRight + e.Bounds.Height;
                    textBounds.Height -= PaddingTop + PaddingBottom;

                    int dim = textBounds.Height;

                    e.Graphics.DrawRectangle(Pens.Black, PaddingLeft + e.Bounds.X, PaddingTop + e.Bounds.Y, dim, dim);
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

                e.Graphics.DrawRectangle(Pens.Black, PaddingLeft + e.Bounds.X, PaddingTop + e.Bounds.Y, dim, dim);
                if ((item as RemoteFileRef).Selected)
                {
                    e.Graphics.DrawLine(Pens.Black, PaddingLeft + e.Bounds.X, PaddingTop + e.Bounds.Y, PaddingLeft + dim + e.Bounds.X, PaddingTop + dim + e.Bounds.Y);
                    e.Graphics.DrawLine(Pens.Black, PaddingLeft + e.Bounds.X, PaddingTop + e.Bounds.Y + dim, PaddingLeft + dim + e.Bounds.X, PaddingTop + e.Bounds.Y);
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
            FilesToDelete.Clear();
            FilesToDownload.Clear();

            foreach (object obj in listBox1.Items)
            {
                if (obj is SMVItem)
                {
                    SMVItem item = obj as SMVItem;
                    if (item.Selected)
                    {
                        RemoteFileRef rf = item.File;
                        if (rf.NewVersionAvailable)
                        {
                            FilesToDownload.Add(rf.FileName);
                            FilesToDownload.Add(Path.GetFileNameWithoutExtension(rf.FileName) + ".smd");
                            FilesToDownload.Add(Path.GetFileNameWithoutExtension(rf.FileName) + ".sme");
                        }
                        foreach (RemoteFileRef rfa in rf.Subs)
                        {
                            if (rf.NewVersionAvailable)
                                FilesToDownload.Add(rf.FileName);
                        }
                    }
                }
                else if (obj is RemoteFileRef)
                {
                    RemoteFileRef rf = obj as RemoteFileRef;
                    if (rf.Selected == false && rf.Local == true)
                        FilesToDelete.Add(rf.FileName);
                    else if (rf.Local == false && rf.Selected == true)
                    {
                        FilesToDownload.Add(rf.FileName);
                        FilesToDownload.Add(Path.GetFileNameWithoutExtension(rf.FileName) + ".smd");
                        FilesToDownload.Add(Path.GetFileNameWithoutExtension(rf.FileName) + ".sme");
                        foreach (RemoteFileRef subFile in rf.Subs)
                        {
                            if (subFile.Selected && !subFile.Local)
                                FilesToDownload.Add(subFile.FileName);
                            else if (subFile.Local && !subFile.Selected)
                                FilesToDelete.Add(subFile.FileName);
                        }
                    }
                }
            }

            if (OnApplyChanges != null)
                OnApplyChanges(this, e);
        }

        public void UpdateLists()
        {
            List<SMVItem> items = new List<SMVItem>();
            
            List<RemoteFileRef> news = Library.GetNewFiles();
            List<object> upds = Library.GetUpdatedFiles();

            // copy files into listbox
            listBox1.Items.Clear();

            // new items
            if (news.Count > 0)
            {
                listBox1.Items.Add(SMVItem.MakeHeader("New Books"));
                foreach (RemoteFileRef rf in news)
                {
                    listBox1.Items.Add(rf);
                }
            }

            if (upds.Count > 0)
            {
                listBox1.Items.Add(SMVItem.MakeHeader("Updates"));
                foreach (RemoteFileRef rf in upds)
                {
                    listBox1.Items.Add(rf);
                }
            }
            
            // all items
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
