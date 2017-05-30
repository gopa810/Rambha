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
            updaterDownloader1.Dock = DockStyle.Fill;
            updaterSelectFiles1.Visible = false;
            updaterGetStatus1.Visible = false;
            updaterDownloader1.Visible = false;

        }

        public void SelectTab(int index)
        {
            switch (index)
            {
                case 0:
                    updaterSelectFiles1.Visible = false;
                    updaterGetStatus1.Visible = true;
                    updaterDownloader1.Visible = false;
                    break;
                case 1:
                    updaterSelectFiles1.Visible = true;
                    updaterGetStatus1.Visible = false;
                    updaterDownloader1.Visible = false;
                    break;
                case 2:
                    updaterSelectFiles1.Visible = false;
                    updaterGetStatus1.Visible = false;
                    updaterDownloader1.Visible = true;
                    break;
            }
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
                SelectTab(0);
            }
            else if (lib.Status == SVBookLibrary.DBStatus.Fetching) 
            {
                if (Library.Callback == null)
                {
                    Library.Callback = new SVBookLibrary.OnStringCompletedDelegate(OnStepCompletedAsync);
                }
                SelectTab(0);
            }
            else if (lib.Status == SVBookLibrary.DBStatus.Updated)
            {
                SelectTab(1);
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

            //Debugger.Log(0, "", "File downloaded\n");

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
            else if (s.Equals("StartDown"))
            {
            }
            else if (s.Equals("DeleteFiles"))
            {
            }
        }

        private void updaterSelectFiles1_OnDiscardChanges(object sender, EventArgs e)
        {
            ParentFrame.SetShowPanel("files");
        }

        private void updaterSelectFiles1_OnApplyChanges(object sender, EventArgs e)
        {
            // create list of files to remove
            List<string> files = updaterSelectFiles1.FilesToDelete;
            if (files != null && files.Count > 0)
            {
                foreach (string s in files)
                {
                    string fullPath = Path.Combine(Library.LastDirectory,s);
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }
            }
            Library.GetCurrentBookDatabase(null);

            if (updaterSelectFiles1.FilesToDownload != null && updaterSelectFiles1.FilesToDownload.Count > 0)
            {
                // create list of files to download
                updaterDownloader1.FilesToDownload = updaterSelectFiles1.FilesToDownload;
                SelectTab(2);
                updaterDownloader1.Start(Library);
            }
            else
            {
                ParentFrame.SetShowPanel("files");
                ParentFrame.RefreshList();
            }
        }

        private void updaterGetStatus1_OnRetryFetchRemote(object sender, EventArgs e)
        {
            Start(Library);
        }

        private void updaterDownloader1_OnDownloadComplete(object sender, EventArgs e)
        {
            Library.GetCurrentBookDatabase(null);
            ParentFrame.RefreshList();
            ParentFrame.SetShowPanel("files");
        }

    }
}
