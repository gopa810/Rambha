using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace SlideViewer
{
    public partial class UpdaterDownloader : UserControl
    {
        public event GeneralArgsEvent OnExit;

        public event GeneralArgsEvent OnDownloadComplete;

        public List<string> FilesToDownload = new List<string>();

        public List<string> FilesError = new List<string>();
        public List<string> FilesSuccess = new List<string>();

        public WebClient webClient = new WebClient();

        public UpdaterDownloader()
        {
            InitializeComponent();
            button1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FilesToDownload.AddRange(FilesError);
            FilesError.Clear();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = FilesToDownload.Count;
            progressBar1.Value = 0;
            DownloadNextFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Library.CancelRemoteFetch();
            if (OnExit != null)
                OnExit(this, e);
        }

        SVBookLibrary Library = null;
        // starts
        public void Start(SVBookLibrary lib)
        {
            Library = lib;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = FilesToDownload.Count;
            progressBar1.Value = 0;
            DownloadNextFile();
        }

        private void DownloadNextFile()
        {
            if (FilesToDownload.Count > 0)
            {
                string nextFile = FilesToDownload[0];
                FilesToDownload.RemoveAt(0);
                Library.FetchRemoteFile(nextFile, new SVBookLibrary.OnStringCompletedDelegate(FileCompleted));
            }
            else
            {
                if (FilesError.Count > 0)
                {
                    button1.Visible = true;
                    label2.Text = string.Format("Some files were not downloaded successfully. Click Retry button to download {0} files", FilesError.Count);
                }
                else
                {
                    if (OnDownloadComplete != null)
                        OnDownloadComplete(this, EventArgs.Empty);
                }
            }
        }

        private void FileCompleted(Dictionary<string, object> args)
        {
            string msg = args["message"].ToString();
            string result = args["result"].ToString();
            if (msg.Equals("FileDownloaded"))
            {
                if (result.Equals("OK"))
                {
                    string fileTarget = args["file"].ToString();
                    if (File.Exists(fileTarget))
                        File.Delete(fileTarget);
                    File.Move(args["temp"].ToString(), fileTarget);
                    FilesSuccess.Add(Path.GetFileName(fileTarget));
                }
                else if (result.Equals("Error") || result.Equals("Cancel"))
                {
                    FilesError.Add(Path.GetFileName(args["file"].ToString()));
                }

                progressBar1.Value = (FilesSuccess.Count + FilesError.Count);
            }

            DownloadNextFile();
        }
    }
}
