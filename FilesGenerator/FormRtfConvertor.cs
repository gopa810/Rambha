using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FilesGenerator
{
    public partial class FormRtfConvertor : Form
    {
        public FormRtfConvertor()
        {
            InitializeComponent();
            richTextBox3.Text = Properties.Settings.Default.Pairs;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Rtf = richTextBox1.Rtf;
            Convertor conv = new Convertor();
            richTextBox2.Text = UsePairs(conv.Main(Rtf));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string Rtf = richTextBox1.Rtf;
            richTextBox2.Text = Rtf;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox2.SelectedText = richTextBox2.SelectedText.Replace('\n', ' ') + "\n";
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Pairs = richTextBox3.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = UsePairs(richTextBox2.Text);
        }

        private string UsePairs(string p)
        {
            StringBuilder sb = new StringBuilder(p);
            foreach (string line in richTextBox3.Lines)
            {
                sb.Replace(line, line.Replace(" ", ""));
            }
            return sb.ToString();
        }

        private void form1GeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
        }
    }
}
