using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

namespace SlideViewer
{
    public partial class UpdaterView : UserControl
    {
        public IMainFrameDelegate ParentFrame = null;

        public List<RemoteFileRef> LocalDb = null;
        public SVBookLibrary Library = null;

        public UpdaterView()
        {
            InitializeComponent();

            updaterGetStatus1.Dock = DockStyle.Fill;
            updaterSelectFiles1.Dock = DockStyle.Fill;
            updaterSelectFiles1.Visible = false;
            updaterGetStatus1.Visible = false;

        }

        public void Start(SVBookLibrary lib)
        {
            Library = lib;
            LocalDb = lib.GetLocalFileDatabase();
            updaterSelectFiles1.Library = lib;

            if (lib.Status == SVBookLibrary.DBStatus.Idle
                || lib.Status == SVBookLibrary.DBStatus.Stopped)
            {
                lib.FetchRemote(new SVBookLibrary.OnStringCompletedDelegate(OnStepCompletedAsync));
                updaterSelectFiles1.Visible = false;
                updaterGetStatus1.Visible = true;
            }
            else if (lib.Status == SVBookLibrary.DBStatus.Fetching) 
            {
                if (Library.Callback == null)
                {
                    Library.Callback = new SVBookLibrary.OnStringCompletedDelegate(OnStepCompletedAsync);
                }
                updaterSelectFiles1.Visible = false;
                updaterGetStatus1.Visible = true;
            }
            else if (lib.Status == SVBookLibrary.DBStatus.Updated)
            {
                updaterSelectFiles1.Visible = true;
                updaterGetStatus1.Visible = false;
                updaterSelectFiles1.UpdateLists();
            }

            updaterGetStatus1.SetStatus(lib.Status, "Error");

        }



        public string RootFile = "";

        private void updaterGetStatus1_OnShowManagerButtonPressed(object sender, EventArgs e)
        {
            if (ParentFrame != null)
            {
                Library.CancelRemoteFetch();
                ParentFrame.SetShowPanel("files");
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var client = new WebClient())
            {
                RootFile = client.DownloadString(Library.RootFileLink);
            }

            Debugger.Log(0, "", "File downloaded\n");

        }

        private void OnStepCompletedAsync(Dictionary<string, object> contAsync)
        {
            SVBookLibrary.OnStringCompletedDelegate deleg = new SVBookLibrary.OnStringCompletedDelegate(OnStepCompleted);
            panel1.Invoke(deleg, contAsync);
        }

        private void OnStepCompleted(Dictionary<string, object> content)
        {
            if (!content.ContainsKey("message")) return;

            string s = content["message"].ToString();
            if (s.Equals("RootFileDownloaded"))
            {
                string resultText = content["result"].ToString();
                if (resultText.Equals("OK"))
                {
                    updaterSelectFiles1.Visible = true;
                    updaterSelectFiles1.UpdateLists();

                    updaterGetStatus1.Visible = false;
                    updaterGetStatus1.SetStatus(SVBookLibrary.DBStatus.Updated, "");
                }
                else if (resultText.Equals("Error"))
                {
                    updaterGetStatus1.SetStatus(SVBookLibrary.DBStatus.Stopped, content["error"].ToString());
                }
                else
                {
                    updaterGetStatus1.SetStatus(SVBookLibrary.DBStatus.Stopped, "Cancelled");
                }
            }
        }

        private void updaterSelectFiles1_OnDiscardChanges(object sender, EventArgs e)
        {
            ParentFrame.SetShowPanel("files");
        }

        private void updaterSelectFiles1_OnApplyChanges(object sender, EventArgs e)
        {
            // downloading and 
            // updating files

        }

        private void updaterGetStatus1_OnRetryFetchRemote(object sender, EventArgs e)
        {
            Start(Library);
        }

    }
}
