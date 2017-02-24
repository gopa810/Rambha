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
    public partial class DialogNewPageName : Form
    {
        public DialogNewPageName()
        {
            InitializeComponent();
        }

        public string PageName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public bool InsertAfter
        {
            get { return radioButton1.Checked; }
        }

        public bool InsertBefore
        {
            get { return radioButton2.Checked; } 
        }
    }
}
