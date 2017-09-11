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
    public partial class DialogCloneCounter : Form
    {
        public DialogCloneCounter()
        {
            InitializeComponent();
        }

        public string DialogValue_Prefix
        {
            get { return textBox1.Text;  }
        }

        public int DialogValue_StartInt
        {
            get { return Convert.ToInt32(numericUpDown1.Value); }
        }
        public int DialogValue_EndInt
        {
            get { return Convert.ToInt32(numericUpDown2.Value); }
        }

    }
}
