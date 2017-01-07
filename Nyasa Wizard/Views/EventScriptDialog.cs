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
    public partial class EventScriptDialog : Form
    {
        public SMRectangleArea Area { get; set; }
        public String EventName { get { return textBox1.Text; } }
        public String Script { get { return richTextBox1.Text; } }

        public EventScriptDialog(SMRectangleArea area, string name, string script)
        {
            InitializeComponent();

            Area = area;

            string[] items = new string[] {
                "onPageAppearance",
                "onPageDisappearance",
                "onDrop",
                "onClick",
                "onDoubleClick",
                "onLongClick"
            };

            if (name.Length > 0)
            {
                textBox1.Text = name;
                textBox1.Enabled = false;
                listBox1.Enabled = false;
            }
        }



        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                textBox1.Text = listBox1.SelectedItem.ToString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = textBox1.Text.Length > 0;
        }
    }
}
