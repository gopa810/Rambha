using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Rambha.GraphView
{
    public partial class DialogSelectObject : Form
    {
        private object p_selected = null;

        public DialogSelectObject(IEnumerable<object> list)
        {
            InitializeComponent();

            p_actions = new List<object>();
            p_actions.AddRange(list);
            //

            UpdateActionsList(p_lastfilter);

            button1.Enabled = false;
        }

        private List<object> p_actions = null;
        private string p_lastfilter = "";

        public string TitleLabel
        {
            set
            {
                label1.Text = value;
            }
        }

        private void UpdateActionsList(string filter)
        {
            p_lastfilter = filter;
            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            if (filter == null || filter.Length == 0)
            {
                foreach (GVDeclarationProcedure sma in p_actions)
                {
                    listBox1.Items.Add(sma);
                }
            }
            else
            {
                foreach (GVDeclarationProcedure sma in p_actions)
                {
                    string s = sma.ToString();
                    if (s.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        listBox1.Items.Add(sma);
                }
            }
            listBox1.EndUpdate();
            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
                p_selected = listBox1.Items[0];
                button1.Enabled = true;
            }
        }

        public object SelectedObject
        {
            get
            {
                return p_selected;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            p_selected = listBox1.SelectedItem;
            if (p_selected != null)
                button1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateActionsList(textBox1.Text);
        }
    }
}
