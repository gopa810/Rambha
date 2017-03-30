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
    public partial class DialogDuplicating : Form
    {
        public DialogDuplicating()
        {
            InitializeComponent();
        }

        public int Rows
        {
            get { return Convert.ToInt32(numericUpDown1.Value); }
            set { numericUpDown1.Value = value; }
        }
        public int Columns
        {
            get { return Convert.ToInt32(numericUpDown2.Value); }
            set { numericUpDown2.Value = value; }
        }
        public int Spacing
        {
            get { return Convert.ToInt32(numericUpDown3.Value); }
            set { numericUpDown3.Value = value; }
        }
        public bool HorizontalLines
        {
            get { return checkBox1.Checked; }
            set { checkBox1.Checked = value; }
        }
        public bool VerticalLines
        {
            get { return checkBox2.Checked; }
            set { checkBox2.Checked = value; }
        }
    }
}
