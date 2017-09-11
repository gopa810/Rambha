using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;
using Rambha.Script;

namespace SlideMaker
{
    public partial class DialogEditPageTexts : Form
    {
        public class PageProxy
        {
            public MNPage page;
            public override string ToString()
            {
                return page.Title;
            }

        }
        public DialogEditPageTexts()
        {
            InitializeComponent();

            listBox1.Items.Clear();
            foreach(MNPage p in MNNotificationCenter.CurrentDocument.Data.Pages)
            {
                listBox1.Items.Add(new PageProxy() { page = p });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.ClearSelected();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, true);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            long pageId = 0;
            string propertyName = string.Empty;
            bool validLocation = false;

            // TODO: save texts
            foreach(string s in richTextBox2.Lines)
            {
                if (s.StartsWith("@"))
                {
                    if (validLocation)
                    {
                        MNPage page = MNNotificationCenter.CurrentDocument.FindPageId(pageId);
                        if (page != null)
                        {
                            page.SetPropertyValue(propertyName, sb.ToString().TrimEnd());
                        }
                    }
                    sb.Clear();
                    string[] p = s.Substring(1).Split(',');
                    if (p.Length == 2 && long.TryParse(p[0], out pageId))
                    {
                        propertyName = p[1];
                        validLocation = true;
                    }
                }
                else
                {
                    sb.AppendLine(s);
                }
            }

            if (validLocation)
            {
                MNPage page = MNNotificationCenter.CurrentDocument.FindPageId(pageId);
                if (page != null)
                {
                    page.SetPropertyValue(propertyName, sb.ToString().TrimEnd());
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RescanPages();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            RescanPages();
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }


        private void RescanPages()
        {
            string[] propertiesToEdit = richTextBox1.Lines;
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.GetSelected(i))
                {
                    PageProxy pp = (PageProxy)listBox1.Items[i];
                    foreach(string s in propertiesToEdit)
                    {
                        GSCore prop = pp.page.GetPropertyValue(s);
                        if (!(prop is GSVoid))
                        {
                            sb.AppendFormat("@{0},{1}\n{2}\n", pp.page.Id, s, prop.getStringValue());
                        }
                    }
                }
            }

            if (sb.ToString().Equals(richTextBox2.Text))
                return;

            richTextBox2.Text = sb.ToString();
        }

        public string GetTextWithoutPropertyTitle()
        {
            string[] propertiesToEdit = richTextBox1.Lines;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                if (listBox1.GetSelected(i))
                {
                    PageProxy pp = (PageProxy)listBox1.Items[i];
                    foreach (string s in propertiesToEdit)
                    {
                        GSCore prop = pp.page.GetPropertyValue(s);
                        if (!(prop is GSVoid))
                        {
                            sb.AppendLine();
                            sb.AppendLine(prop.getStringValue());
                        }
                    }
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogImportEditText dlg = new DialogImportEditText();

            dlg.DialogValue_Text = GetTextWithoutPropertyTitle();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] lines = dlg.DialogValue_Lines;

                // extract tags from original text
                List<string> tags = new List<string>();
                foreach(string rs in richTextBox2.Lines)
                {
                    if (rs.StartsWith("@"))
                        tags.Add(rs);
                }

                StringBuilder sb = new StringBuilder();
                int currTag = 0;
                // merge: replace empty lines with tags
                foreach(string ns in lines)
                {
                    if (string.IsNullOrEmpty(ns))
                    {
                        if (currTag >= tags.Count)
                            break;
                        sb.AppendLine(tags[currTag]);
                        currTag++;
                    }
                    else
                    {
                        sb.AppendLine(ns);
                    }
                }

                richTextBox2.Text = sb.ToString();
            }
        }
    }
}
