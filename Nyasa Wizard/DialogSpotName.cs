﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlideMaker
{
    public partial class DialogSpotName : Form
    {
        public DialogSpotName()
        {
            InitializeComponent();
        }

        public string SpotText
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
    }
}
