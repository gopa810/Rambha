using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlideMaker
{
    public partial class ControlSeparator : UserControl
    {
        public EVContainer ParentFrame { get; set; }
        public UserControl AssociatedControl { get; set; }

        public ControlSeparator(EVContainer parent, UserControl uc)
        {
            InitializeComponent();
            ParentFrame = parent;
            b_omit_checked = true;
            checkBox1.Checked = true;
            b_omit_checked = false;
            AssociatedControl = uc;
        }


        public string Title
        {
            get { return checkBox1.Text; }
            set { checkBox1.Text = value; }
        }

        public bool Checked
        {
            get { return checkBox1.Checked; }
        }

        private bool b_omit_checked = false;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (b_omit_checked) return;
            AssociatedControl.Visible = checkBox1.Checked;
            ParentFrame.OnPanelVisibilityChange(this);

        }


    }
}
