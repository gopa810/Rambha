using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SlideViewer
{
    public partial class UpdaterGetStatus : UserControl
    {
        public event GeneralArgsEvent OnShowManagerButtonPressed;
        public event GeneralArgsEvent OnRetryFetchRemote;

        public UpdaterGetStatus()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OnShowManagerButtonPressed != null)
                OnShowManagerButtonPressed(this, e);
        }

        internal void SetStatus(SVBookLibrary.DBStatus p, string message)
        {
            button2.Visible = false;
            button1.Enabled = true;
            switch (p)
            {
                case SVBookLibrary.DBStatus.Fetching:
                    label2.Text = "Connecting...";
                    break;
                case SVBookLibrary.DBStatus.Stopped:
                    label2.Text = message;
                    button2.Visible = true;
                    break;
                case SVBookLibrary.DBStatus.Idle:
                    label2.Text = "Waiting...";
                    break;
                case SVBookLibrary.DBStatus.Updated:
                    label2.Text = "Updated";
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (OnRetryFetchRemote != null)
                OnRetryFetchRemote(this, e);
        }

    }

    public delegate void GeneralArgsEvent(object sender, EventArgs e);
}
