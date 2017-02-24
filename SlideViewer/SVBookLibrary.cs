using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Diagnostics;

using Rambha.Document;

namespace SlideViewer
{
    public class SVBookLibrary
    {
        public enum DBStatus
        {
            Idle,
            Fetching,
            Updated,
            Stopped
        }

        public List<MNBookHeader> Books = new List<MNBookHeader>();
        public string LastDirectory = null;

        public List<RemoteFileRef> RemoteDatabase = null;
        public List<RemoteFileRef> DatabaseStatus = null;

        public DBStatus Status = DBStatus.Idle;
        public string StatusMessage = "";

        public string RootFileLink = "https://dl.dropboxusercontent.com/s/ahm2h2g9dl5vtvq/root.txt?dl=0";

        private WebClient webClient = new WebClient();

        public SVBookLibrary()
        {
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
        }

        public OnStringCompletedDelegate Callback = null;

        public void FetchRemote(OnStringCompletedDelegate clbk)
        {
            if (!webClient.IsBusy)
            {
                Status = DBStatus.Fetching;
                Callback = clbk;
                Uri link = new Uri(RootFileLink);
                webClient.DownloadStringAsync(link);
                StatusMessage = "";
            }
        }


        public void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Log("Download String Completed");

            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    Log("- error");
                    StatusMessage = e.Error.Message;
                    Dictionary<string, object> dm = new Dictionary<string, object>();
                    dm.Add("message", "RootFileDownloaded");
                    dm.Add("result", "Error");
                    dm.Add("error", StatusMessage);
                    Callback(dm);
                }
                else
                {
                    Log("- not cancelled");
                    SetRemoteDatabase(e.Result);
                    CalculateDatabaseStatus();
                    Status = DBStatus.Updated;

                    if (Callback != null)
                    {
                        Dictionary<string, object> dm = new Dictionary<string, object>();
                        dm.Add("message", "RootFileDownloaded");
                        dm.Add("result", "OK");
                        Callback(dm);
                    }
                }
            }
            else
            {
                Log("- cancelled");
                Status = DBStatus.Stopped;
                if (Callback != null)
                {
                    Dictionary<string, object> dm = new Dictionary<string, object>();
                    dm.Add("message", "RootFileDownloaded");
                    dm.Add("result", "Cancel");
                    dm.Add("error", "Connection was cancelled.");
                    Callback(dm);
                }
            }
        }

        private static void Log(string fmt, params object[] args)
        {
            Debugger.Log(0, "", string.Format(fmt, args) + "\n");
        }

        public delegate void OnStringCompletedDelegate(Dictionary<string, object> content);

        public void GetCurrentBookDatabase(string directory)
        {
            LastDirectory = directory;

            List<string> bookFileNames = new List<string>();
            List<string> langFileNames = new List<string>();

            foreach (string s in Directory.EnumerateFiles(directory))
            {
                if (s.EndsWith(".smb"))
                {
                    bookFileNames.Add(s);
                }
                else if (s.EndsWith(".sme"))
                {
                    langFileNames.Add(s);
                }
            }

            Books.Clear();
            foreach (string file in bookFileNames)
            {
                MNBookHeader bh = new MNBookHeader();
                if (bh.LoadHeader(file))
                {
                    Books.Add(bh);
                }
            }

            foreach (string file in langFileNames)
            {
                MNBookLanguage bl = new MNBookLanguage();
                PreviewLanguage(bl, file);
                MNBookHeader bh = GetBookByCode(bl.BookCode);
                if (bh != null)
                    bh.Languages.Add(bl);
            }
        }


        public void PreviewLanguage(MNBookLanguage bl, string fileName)
        {
            MNLocalisation file = new MNLocalisation();
            bl.FilePath = fileName;
            file.Load(bl.FilePath, false);
            bl.BookCode = file.GetProperty("BookCode");
            bl.LastTimeSave = file.GetProperty("LastTime");
            bl.LanguageName = file.GetProperty("LanguageName");
        }

        public MNLocalisation LoadLanguage(MNBookLanguage bl)
        {
            MNLocalisation file = new MNLocalisation();
            file.Load(bl.FilePath, true);
            return file;
        }

        public MNBookHeader GetBookByCode(string bookCode)
        {
            foreach (MNBookHeader bh in Books)
            {
                if (bh.BookCode.Equals(bookCode))
                    return bh;
            }
            return null;
        }


        public List<RemoteFileRef> GetLocalFileDatabase()
        {
            List<RemoteFileRef> list = new List<RemoteFileRef>();

            foreach (MNBookHeader book in Books)
            {
                RemoteFileRef bookFile = new RemoteFileRef();
                bookFile.Text = book.BookTitle;
                bookFile.FileName = Path.GetFileName(book.FilePath);
                bookFile.LastTime = book.LastTimeSave;
                bookFile.Selected = true;
                bookFile.Local = true;

                foreach (MNBookLanguage lang in book.Languages)
                {
                    RemoteFileRef langw = new RemoteFileRef();
                    langw.Local = true;
                    langw.Selected = true;
                    langw.Text = lang.LanguageName;
                    langw.LastTime = lang.LastTimeSave;
                    langw.FileName = Path.GetFileName(lang.FilePath);
                    bookFile.Subs.Add(langw);
                }

                list.Add(bookFile);
            }

            return list;
        }

        public static string DBToString(List<RemoteFileRef> list)
        {
            StringBuilder sb = new StringBuilder();

            foreach (RemoteFileRef file in list)
            {
                sb.AppendLine("BOOK:" + file.Text);
                sb.AppendLine("BOOKFILE:" + file.FileName);
                sb.AppendLine("BOOKTIME:" + file.LastTime);
                if (file.Subs != null)
                {
                    foreach (RemoteFileRef lang in file.Subs)
                    {
                        sb.AppendLine("LANG:" + lang.Text);
                        sb.AppendLine("LANGFILE:" + lang.FileName);
                        sb.AppendLine("LANGTIME:" + lang.LastTime);
                    }
                }
            }

            return sb.ToString();
        }

        public static List<RemoteFileRef> StringToDB(string str)
        {
            List<RemoteFileRef> list = new List<RemoteFileRef>();

            RemoteFileRef currBook = null;
            RemoteFileRef currLang = null;

            foreach (string s in str.Split('\n', '\r'))
            {
                if (s.StartsWith("BOOK:"))
                {
                    currBook = new RemoteFileRef();
                    list.Add(currBook);
                    currBook.Text = s.Substring(5);
                }
                else if (s.StartsWith("LANG:"))
                {
                    currLang = new RemoteFileRef();
                    if (currBook.Subs == null)
                        currBook.Subs = new List<RemoteFileRef>();
                    currBook.Subs.Add(currLang);
                    currLang.Text = s.Substring(5);
                }
                else if (s.StartsWith("BOOKFILE:"))
                {
                    if (currBook != null)
                        currBook.FileName = s.Substring(9);
                }
                else if (s.StartsWith("LANGFILE:"))
                {
                    if (currLang != null)
                        currLang.FileName = s.Substring(9);
                }
                else if (s.StartsWith("BOOKTIME:"))
                {
                    if (currBook != null)
                        currBook.LastTime = s.Substring(9);
                }
                else if (s.StartsWith("LANGTIME:"))
                {
                    if (currLang != null)
                        currLang.LastTime = s.Substring(9);
                }
            }

            return list;
        }

        public void SetRemoteDatabase(string content)
        {
            RemoteDatabase = StringToDB(content);
        }

        public void UpdateLocalDatabase()
        {
            if (LastDirectory != null)
                GetCurrentBookDatabase(LastDirectory);
        }

        public RemoteFileRef FindRef(List<RemoteFileRef> list, string text)
        {
            foreach (RemoteFileRef fr in list)
            {
                if (fr.Text.Equals(text))
                    return fr;
            }

            return null;
        }

        /// <summary>
        /// Assumptions: GetCurrentBookDatabase and SetRemoteDatabase 
        /// were called prior to this method
        /// </summary>
        /// <returns></returns>
        public bool CalculateDatabaseStatus()
        {
            if (RemoteDatabase == null)
                return false;


            List<RemoteFileRef> remoteDB = RemoteDatabase;
            List<RemoteFileRef> localDB = new List<RemoteFileRef>();
            if (Books.Count == 0 && LastDirectory != null)
                GetCurrentBookDatabase(LastDirectory);
            localDB = GetLocalFileDatabase();

            foreach (RemoteFileRef rf in remoteDB)
            {
                rf.Selected = false;
                rf.Local = false;

                RemoteFileRef local = FindRef(localDB, rf.Text);
                if (local != null)
                {
                    if (!local.LastTime.Equals(rf.LastTime))
                    {
                        local.NewVersionAvailable = true;
                    }

                    foreach (RemoteFileRef rlang in rf.Subs)
                    {
                        RemoteFileRef localLang = FindRef(local.Subs, rlang.Text);
                        if (localLang != null)
                        {
                            if (!localLang.LastTime.Equals(rlang.LastTime))
                            {
                                localLang.NewVersionAvailable = true;
                            }
                            rlang.Text = null;
                        }
                        else
                        {
                            rlang.Selected = false;
                            rlang.Local = false;
                            local.Subs.Add(rlang);
                        }
                    }

                    rf.Text = null;
                }
                else
                {
                    localDB.Add(rf);
                    foreach (RemoteFileRef rlang in rf.Subs)
                    {
                        rlang.Selected = false;
                        rlang.Local = false;
                    }
                }
            }

            DatabaseStatus = localDB;
            return true;
        }

        public void CancelRemoteFetch()
        {
            if (webClient.IsBusy)
                webClient.CancelAsync();
        }
    }
}
