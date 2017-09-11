using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlideMaker
{
    public partial class DialogImportEditText : Form
    {
        public DialogImportEditText()
        {
            InitializeComponent();
        }

        public string DialogValue_Text
        {
            get { return richTextBox1.Text; }
            set { richTextBox1.Text = value; }
        }

        public string[] DialogValue_Lines
        {
            get { return richTextBox1.Lines; }
            set { richTextBox1.Lines = value; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogCloneCounter dcc = new DialogCloneCounter();
            if (dcc.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                for(int i= dcc.DialogValue_StartInt; i <= dcc.DialogValue_EndInt; i++)
                {
                    sb.AppendLine();
                    sb.AppendLine(string.Format("{0}{1}", dcc.DialogValue_Prefix, i));
                }
                sb.AppendLine();

                richTextBox1.Text = sb.ToString();
            }
        }
    }
}
