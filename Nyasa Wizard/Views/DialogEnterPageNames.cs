using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlideMaker.Views
{
    public partial class DialogEnterPageNames : Form
    {
        public DialogEnterPageNames()
        {
            InitializeComponent();
        }

        public string[] Names
        {
            get
            {
                return richTextBox1.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
