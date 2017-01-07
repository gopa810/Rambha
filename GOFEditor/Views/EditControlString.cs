using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.GOF;

namespace GOFEditor.Views
{
    public partial class EditControlString : UserControl
    {
        public MainForm ParentFrame { get; set; }
        public GOFString Value { get; set; }

        public EditControlString()
        {
            InitializeComponent();
        }

        public void SetValue(string key, GOFString value)
        {
            labelKey.Text = key;
            Value = value;
            textValueBox.Text = value.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Value.Text = textValueBox.Text;
            ParentFrame.CloseCurrentPresentation();
        }

        // convert to RunningText
        private void button2_Click(object sender, EventArgs e)
        {
            string key = labelKey.Text;
            GOFRunningText rt = new GOFRunningText();
            rt.Text = textValueBox.Text;
            ParentFrame.ReplaceType(key, Value, rt);
        }
    }
}
