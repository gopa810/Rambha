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
    public partial class EVOrderedList : UserControl
    {
        public SMOrderedList Object = null;

        public EVOrderedList()
        {
            InitializeComponent();
        }

        public void SetObject(SMControl obj)
        {
            Object = (SMOrderedList)obj;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int from, to;
            if (int.TryParse(textBox1.Text, out from) && int.TryParse(textBox2.Text, out to))
            {
                if (Object != null && from >= 0 && from < Object.Objects.Count &&
                    to >= 0 && to < Object.Objects.Count)
                {
                    SMOrderedList.StringItem obj = Object.Objects[from];
                    Object.Objects.RemoveAt(from);
                    Object.Objects.Insert(to, obj);
                    textBox1.Text = "";
                    textBox2.Text = "";
                    EVContainer.Shared.OnValueUpdatedImmediate();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogEditOrderedListTexts d = new DialogEditOrderedListTexts();

            d.SetObject(Object);

            if (d.ShowDialog() == DialogResult.OK)
            {
                EVContainer.Shared.OnValueUpdatedImmediate();
            }
        }
    }
}
