using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FilesGenerator
{
    public partial class Form1 : Form
    {
        public string WorkDir = @"E:\Dropbox\Books for Software";

        public Form1()
        {
            InitializeComponent();
            //Form3 f = new Form3();
            //f.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CmdFileGenerator.Generate(WorkDir);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LangFileChecker.Check(WorkDir);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AllFileGenerator.Generate(WorkDir);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            AllFileGenerator.ListFiles(WorkDir, listBox1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < listBox1.Items.Count)
            {
                label1.Text = "";
                string file = listBox1.Items[listBox1.SelectedIndex].ToString();
                AllFileGenerator.ProcessFileComplete(file);
                label1.Text = "File Processed: " + file;
            }
        }
    }
}
