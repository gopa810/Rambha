using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker
{
    public partial class DialogSelectLanguage : Form
    {
        public LocalizationMainForm mainForm = null;
        public DialogSelectLanguage()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ListBox lb = listBox1;
            if (lb.SelectedIndex >= 0 && lb.SelectedIndex < lb.Items.Count)
            {
                object obj = lb.Items[lb.SelectedIndex];
                if (obj is MNBookLanguage)
                {
                    mainForm.OnSave();

                    MNLocalisation data = new MNLocalisation();
                    data.Load((obj as MNBookLanguage).FilePath, true);

                    mainForm.SetLocalisationData(data);
                    mainForm.SetFileName((obj as MNBookLanguage).FilePath);
                }
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim().Length > 0)
            {
                MNDocument doc = MNNotificationCenter.CurrentDocument;
                MNLocalisation data = new MNLocalisation();
                data.SetProperty("BookCode", doc.Book.BookCode);
                data.SetProperty("LanguageName", textBox2.Text.Trim());


                string path = Path.GetDirectoryName(MNNotificationCenter.CurrentFileName);
                string fileName = Path.Combine(path, string.Format("{0}_{1}.sme", data.GetProperty("BookCode"), data.GetProperty("LanguageCode")));
                data.Save(fileName);

                MNBookLanguage bookLang = new MNBookLanguage();
                bookLang.FilePath = fileName;
                bookLang.BookCode = data.GetProperty("BookCode");
                bookLang.LanguageName = data.GetProperty("LanguageName");
                doc.Book.Languages.Add(bookLang);

                mainForm.SetLocalisationData(data);
                mainForm.SetFileName(fileName);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }

    }
}
