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
    public partial class ObjectDumpFrame : Form
    {
        MNDocument lastDocument = null;

        public ObjectDumpFrame()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
            richTextBox1.Copy();
        }

        public void WriteLS(StringBuilder sb, int spaces)
        {
            sb.Append("".PadLeft(spaces));
        }

        public void SetDocument(MNDocument doc)
        {
            lastDocument = doc;

            if (lastDocument != null)
                RefreshContent(lastDocument);
        }

        private void RefreshContent(MNDocument doc)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("PAGES");
            foreach (MNPage p in doc.Data.Pages)
            {
                WritePage(sb, p, 1);
            }
            sb.AppendLine("END PAGES");

            sb.AppendLine("TEMPLATES");
            foreach (MNPage p in doc.Data.Templates)
            {
                WritePage(sb, p, 1);
            }
            sb.AppendLine("END TEMPLATES");

            richTextBox1.Text = sb.ToString();
        }

        public void WritePage(StringBuilder sb, MNPage p, int level)
        {
            WriteLS(sb, level);
            sb.AppendFormat("PAGE: {0}\n", p.Id);

            foreach (SMControl ctrl in p.Objects)
            {
                WriteControl(sb, ctrl, level + 1);
            }
        }

        public void WriteControl(StringBuilder S, SMControl C, int L)
        {
            WriteLS(S, L);
            S.AppendFormat("OBJECT: {0}\n", C.Id);
        }

        public void WriteArea(StringBuilder S, SMRectangleArea A, int L)
        {
        }

        public void WriteRuler(StringBuilder S, int val, int L, string label)
        {
            WriteLS(S, L);
            S.Append(label);
            S.AppendLine(":");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lastDocument != null)
                RefreshContent(lastDocument);
        }

    }
}
