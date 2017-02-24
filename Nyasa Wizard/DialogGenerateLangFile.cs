using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SlideMaker
{
    public partial class DialogGenerateLangFile : Form
    {
        private string BookFileName = "";

        public DialogGenerateLangFile()
        {
            InitializeComponent();
            textBox2.Text = Properties.Settings.Default.LastDirLang;
        }


        public void SetBookCode(string txt)
        {
            textBox1.Text = txt;
        }

        public void SetBookFileName(string bookFileName)
        {
            BookFileName = bookFileName;
        }

        public string GetInputDirectory()
        {
            return textBox2.Text;
        }

        public string GetOutputFileName()
        {
            return textBox3.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.SelectedPath = Properties.Settings.Default.LastDirLang;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = dlg.SelectedPath;
                Properties.Settings.Default.LastDirLang = dlg.SelectedPath;
                GenerateOutputFileName();
            }
        }

        private string GenerateOutputFileName()
        {
            textBox3.Text = Path.Combine(Path.GetDirectoryName(BookFileName), textBox1.Text + "_" + Path.GetFileName(textBox2.Text) + ".sme");
            return textBox3.Text;
        }

    }
}
