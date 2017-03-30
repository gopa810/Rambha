using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;
using Rambha.Script;

namespace SlideMaker
{
    public partial class EVControlScripts : UserControl
    {
        private SMControl control = null;

        public EVControlScripts()
        {
            InitializeComponent();
        }

        public void SetControl(SMControl ctrl)
        {
            control = ctrl;
            richTextBox1.Text = ctrl.ScriptOnClick;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (control != null)
                control.ScriptOnClick = richTextBox1.Text;
        }
    }
}
