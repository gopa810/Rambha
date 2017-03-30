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
    public partial class EVControlStyle : UserControl
    {
        public SMControl Object = null;

        public EVControlStyle()
        {
            InitializeComponent();
        }

        public void SetObject(SMControl obj)
        {
            Object = obj;

            textBox1.Name = Object.StyleName;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Object.StyleName = textBox1.Name;
        }
    }
}
