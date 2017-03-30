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
    public partial class DialogSetNamesControls : Form
    {
        List<SMControl> ctrls = null;
        StringBuilder sbb = new StringBuilder();

        public bool SetTags = false;

        public DialogSetNamesControls()
        {
            InitializeComponent();
        }

        public void SetControls(List<SMControl> c)
        {
            sbb.Clear();
            ctrls = new List<SMControl>();
            foreach (SMControl cc in c)
            {
                if (cc is SMLabel || cc is SMImage || cc is SMTextView || cc is SMCheckBox)
                {
                    ctrls.Add(cc);
                    sbb.AppendLine(cc.GetType().Name);
                }
            }
            string str = ListToText(ctrls);
            textBox1.Text = str;
            textBox1.SelectAll();
            textBox2.Text = sbb.ToString();
        }

        private string ListToText(List<SMControl> ctrls)
        {
            StringBuilder sb = new StringBuilder();
            foreach (SMControl c in ctrls)
            {
                string name = SetTags ? c.Tag : c.Text;
                if (name.IndexOf('\r') >= 0 || name.IndexOf('\n') >= 0)
                {
                    string[] p = name.Split('\r', '\n');
                    for (int i = 0; i < p.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(' ');
                        sb.AppendLine(p[i].TrimStart());
                    }
                }
                else
                {
                    sb.AppendLine(name.TrimStart());
                }
            }
            return sb.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TextToList(textBox1.Text, ctrls);
        }

        private void TextToList(string p, List<SMControl> ctrls)
        {
            List<string> sp = new List<string>(p.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries));

            for (int i = 0; i < sp.Count; i++)
            {
                while ((i + 1 < sp.Count) && sp[i + 1].StartsWith(" "))
                {
                    sp[i] = sp[i] + '\n' + sp[i + 1].Substring(1);
                    sp.RemoveAt(i + 1);
                }
            }

            for (int j = 0; j < sp.Count && j < ctrls.Count; j++ )
            {
                if (SetTags)
                    ctrls[j].Tag = sp[j];
                else
                    ctrls[j].Text = sp[j];
            }
        }

    }
}
