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
    public partial class DialogNewName : Form
    {
        public DialogNewName()
        {
            InitializeComponent();
        }

        public string NamePrompt
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        public string ObjectName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
