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
    public partial class EVPageName : UserControl
    {
        public MNPage Page = null;

        public EVPageName()
        {
            InitializeComponent();
        }

        public void SetObject(MNPage page)
        {
            Page = page;
            textBox1.Text = page.Title;
            textBox2.Text = page.MessageText;
            textBox3.Text = page.TextB;
            textBox4.Text = page.TextC;
            textBox5.Text = page.MessageTitle;

            checkBox1.Checked = page.ShowBackNavigation;
            checkBox2.Checked = page.ShowTitle;
            checkBox3.Checked = page.ShowHome;
            checkBox4.Checked = page.ShowHelp;
            checkBox5.Checked = page.ShowForwardNavigation;
            checkBox6.Checked = page.ShowMessageAlways;
            checkBox7.Checked = page.ShowAudio;

            comboBox1.Items.Clear();
            int i = 1;
            int sel = -1;
            comboBox1.Items.Add("<no template>");
            foreach (MNPage tmp in page.Document.Data.Templates)
            {
                if (tmp == page.Template)
                    sel = i;
                comboBox1.Items.Add(tmp);
                i++;
            }

            if (sel >= 0)
                comboBox1.SelectedIndex = sel;
            else if (page.Template == null)
                comboBox1.SelectedIndex = 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Page != null)
            {
                Page.Title = textBox1.Text;
                EVContainer.Shared.OnValueUpdated();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (Page != null)
            {
                Page.MessageText = textBox2.Text;
                //Page.ShowHelp = (textBox2.Text.Length > 0);
                EVContainer.Shared.OnValueUpdated();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = comboBox1;

            if (cb.SelectedIndex >= 0 && cb.SelectedIndex < cb.Items.Count)
            {
                object so = cb.Items[cb.SelectedIndex];
                if (so is string)
                {
                    Page.Template = null;
                }
                else if (so is MNPage)
                {
                    MNPage p = (MNPage)so;
                    if (p != null)
                    {
                        Page.Template = p;
                    }
                }
                EVContainer.Shared.OnValueUpdated();
            }
        }

        private void textBox3_TextB_Changed(object sender, EventArgs e)
        {
            if (Page != null)
            {
                Page.TextB = textBox3.Text;
            }
        }

        private void textBox4_TextC_Changed(object sender, EventArgs e)
        {
            if (Page != null)
            {
                Page.TextC = textBox4.Text;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Page.ShowBackNavigation = checkBox1.Checked;
            EVContainer.Shared.OnValueUpdatedImmediate();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Page.ShowTitle = checkBox2.Checked;
            EVContainer.Shared.OnValueUpdatedImmediate();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Page.ShowHome = checkBox3.Checked;
            EVContainer.Shared.OnValueUpdatedImmediate();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Page.ShowHelp = checkBox4.Checked;
            EVContainer.Shared.OnValueUpdatedImmediate();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Page.ShowForwardNavigation = checkBox5.Checked;
            EVContainer.Shared.OnValueUpdatedImmediate();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            Page.MessageTitle = textBox5.Text;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            Page.ShowMessageAlways = checkBox6.Checked;
            EVContainer.Shared.OnValueUpdatedImmediate();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            Page.ShowAudio = checkBox7.Checked;
            EVContainer.Shared.OnValueUpdatedImmediate();
        }

    }
}
