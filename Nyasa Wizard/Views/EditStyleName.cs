using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class EditStyleName : Form
    {
        public MNDocument Document { get; set; }

        public EditStyleName()
        {
            Document = null;
            InitializeComponent();
        }

        public string StyleName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; CheckName(value); }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CheckName(StyleName);
        }

        private void CheckName(string styleName)
        {
            if (Document != null)
            {
                foreach (SMStyle s in Document.Styles)
                {
                    if (s.Name.Equals(styleName))
                    {
                        buttonOK.Enabled = false;
                        return;
                    }
                }
            }

            buttonOK.Enabled = true;
        }
    }
}
