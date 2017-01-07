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
using SlideMaker.Views;

namespace SlideMaker
{
    public partial class NavigationFrame : Form, INotificationTarget
    {
        public NavigationFrame()
        {
            InitializeComponent();
            MNNotificationCenter.AddReceiver(this, null);
            MNNotificationCenter.AddReceiver(treeObjectView1, null);
        }

        void INotificationTarget.OnNotificationReceived(object sender, string msg, params object[] args)
        {
            switch (msg)
            {
                case "DocumentChanged":
                    if (args != null && args.Length > 0 && args[0] is MNDocument)
                    {
                        treeObjectView1.SetObject(args[0] as MNDocument);
                    }
                    break;
            }
        }

        private void treeObjectView1_OnInitializeItem(object sender, Views.TreeObjectViewEventArgs e)
        {

        }

        private void NavigationFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        private TVItem p_action_item = null;

        private void treeObjectView1_OnInitializeActionMenu(object sender, Views.TreeObjectViewEventArgs e)
        {
            p_action_item = e.Item;

            if (p_action_item == null) return;

            NABase[] pa = p_action_item.GetActions();

            if (pa != null && pa.Length > 0)
            {
                contextMenuStrip1.Items.Clear();
                foreach (NABase s in pa)
                {
                    ToolStripItem tsi = contextMenuStrip1.Items.Add(s.Title);
                    tsi.Tag = s;
                    tsi.Click += new EventHandler(tsi_Click);
                }

                contextMenuStrip1.Show(e.ScreenPoint);
            }
        }

        void tsi_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                ToolStripItem tsi = sender as ToolStripItem;
                if (tsi.Tag != null && tsi.Tag is NABase)
                {
                    (tsi.Tag as NABase).Execute();
                }
            }
        }

        private NABase[] GetActionsForObject(GSCore cr)
        {
            if (cr is MNPage)
            {
                return new NABase[]
                {
                };
            }
            else if (cr is MNDocument)
            {
            }

            return null;
        }

        private void ProcessObjectMessage(GSCore core, string msg)
        {
            if (core is MNPage)
            {
                switch (msg)
                {
                    default: break;
                }
            }
            else if (core is MNDocument)
            {
                switch (msg)
                {
                    default: break;
                }
            }
        }
    }
}
