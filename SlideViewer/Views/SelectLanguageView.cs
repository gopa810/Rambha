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
        }

        public MNBookHeader CurrentBook = null;

        public IMainFrameDelegate ParentFrame = null;

        public bool SetBook(MNBookHeader bh)
        {
            CurrentBook = bh;
            listBox1.Items.Clear();
            if (bh.Languages != null && bh.Languages.Count > 0)
            {
                foreach (MNBookLanguage lang in bh.Languages)
                {
                    listBox1.Items.Add(lang.LanguageName);
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
                MNBookLanguage lang = null;
                if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < CurrentBook.Languages.Count)
                    lang = CurrentBook.Languages[listBox1.SelectedIndex];
                ParentFrame.dialogDidSelectLanguage(lang);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ParentFrame != null)
            {
                ParentFrame.dialogDidSelectLanguage(null);
            }
        }
    }

    public interface IMainFrameDelegate
    {
        void showSelectLanguageDialog(MNBookHeader book);
        void dialogDidSelectLanguage(MNBookLanguage lang);
        void SetShowPanel(string panel);
    }
}
