using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker
{
    public partial class EVControlName : UserControl
    {
        public SMControl Object = null;

        public EVControlName()
        {
            InitializeComponent();
        }

        public void SetObject(SMControl obj)
        {
            Object = obj;
            textBox1.Text = obj.Text;
            textBox2.Text = obj.Tag;
            checkBox1.Checked = obj.Selectable;

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Object != null)
            {
                Object.Text = textBox1.Text;
                EVContainer.Shared.OnValueUpdated();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (Object != null)
            {
                Object.Tag = textBox2.Text;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Object != null)
            {
                Object.Selectable = checkBox1.Checked;
            }
        }
    }
}
