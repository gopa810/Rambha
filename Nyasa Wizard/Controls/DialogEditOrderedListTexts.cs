using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker
{
    public partial class DialogEditOrderedListTexts : Form
    {
        public SMOrderedList Object = null;

        public DialogEditOrderedListTexts()
        {
            InitializeComponent();

            button1.Click += new EventHandler(button1_Click);
        }

        void button1_Click(object sender, EventArgs e)
        {
            string[] lines = richTextBox1.Lines;
            Object.Objects.Clear();
            Object.DrawnObjects.Clear();
            foreach (string s in lines)
            {
                Object.Objects.Add(s);
            }
        }

        public void SetObject(SMOrderedList obj)
        {
            Object = obj;
            richTextBox1.Text = "";
            foreach (object o in obj.Objects)
            {
                if (o is String)
                {
                    richTextBox1.AppendText((o as String) + "\n");
                }
            }
        }
    }
}
