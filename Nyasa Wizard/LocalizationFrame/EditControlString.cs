using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class EditControlString : UserControl
    {
        public LocalizationMainForm ParentFrame { get; set; }
        public MNReferencedText Value { get; set; }

        public EditControlString()
        {
            InitializeComponent();
        }

        public void SetValue(MNReferencedText value)
        {
            labelKey.Text = value.Name;
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
            MNReferencedAudioText rt = new MNReferencedAudioText();
            rt.Text = textValueBox.Text;
            ParentFrame.ReplaceType(key, Value, rt);
        }
    }
}
