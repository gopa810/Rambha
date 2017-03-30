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
    public partial class SelectLanguageView : UserControl
    {
        public SelectLanguageView()
        {
            InitializeComponent();
            ChooseFont = new Font(FontFamily.GenericSansSerif, 12);
        }

        public RemoteFileRef CurrentBook;

        public IMainFrameDelegate ParentFrame = null;

        public Font ChooseFont = null;

        public bool SetBook(RemoteFileRef mainFile)
        {
            CurrentBook = mainFile;
            listBox1.Items.Clear();
            if (mainFile == null)
                return false;
            if (mainFile.Subs != null && mainFile.Subs.Count > 0)
            {
                foreach (RemoteFileRef lang in mainFile.Subs)
                {
                    listBox1.Items.Add(lang);
                }
                listBox1.SelectedIndex = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SelectLanguageView_SizeChanged(object sender, EventArgs e)
        {
            Size sz = this.Size;
            panel1.Location = new Point(sz.Width / 4, sz.Height / 4);
            panel1.Size = new Size(sz.Width/2, sz.Height/2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ParentFrame != null)
            {
                RemoteFileRef lang = null;
                if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < listBox1.Items.Count)
                    lang = listBox1.Items[listBox1.SelectedIndex] as RemoteFileRef;
                ParentFrame.dialogDidSelectLanguage(CurrentBook, lang);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ParentFrame != null)
            {
                ParentFrame.dialogDidSelectLanguage(null, null);
            }
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)(e.Graphics.MeasureString("M", ChooseFont).Height) * 2;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Font font = ChooseFont;
            int off = (int)font.SizeInPoints / 2;

            if ((e.State & DrawItemState.Selected) != 0)
            {
                e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
            }

            if (e.Index >= 0 && e.Index < listBox1.Items.Count)
            {
                RemoteFileRef rf = listBox1.Items[e.Index] as RemoteFileRef;
                if (rf.Local == false)
                {
                    e.Graphics.DrawString(string.Format("{0} (Internet)", rf.Text), font, Brushes.Black, off, off + e.Bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(rf.Text, font, Brushes.Black, off, off + e.Bounds.Top);
                }
            }
        }
    }

    public interface IMainFrameDelegate
    {
        void showSelectLanguageDialog(MNBookHeader book);
        void dialogDidSelectLanguage(RemoteFileRef book, RemoteFileRef lang);
        void SetShowPanel(string panel);
        void RefreshList();
    }
}
