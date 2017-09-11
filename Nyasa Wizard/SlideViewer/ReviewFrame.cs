using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class ReviewFrame : Form, INotificationTarget
    {
        private static ReviewFrame p_Shared = null;

        private static ReviewBook p_Book = new ReviewBook();
        private static ReviewPage p_Page = new ReviewPage();
        private static ReviewItem p_Item = new ReviewItem();

        private static MNPage p_PageOrig = null;
        private static SMControl p_ItemOrig = null;

        public static bool IsVisible()
        {
            return (p_Shared != null && p_Shared.Visible);
        }

        public static ReviewFrame Shared
        {
            get
            {
                if (p_Shared == null)
                {
                    p_Shared = new ReviewFrame();
                    MNNotificationCenter.AddReceiver(p_Shared, null);
                    if (MNNotificationCenter.CurrentDocument != null)
                    {
                        MNDocument doc = MNNotificationCenter.CurrentDocument;
                        p_Shared.StartDocumentReview(doc, doc.Book.FilePath);
                    }
                    p_Shared.Show();
                }
                return p_Shared;
            }
        }

        public static void DisplayWindow()
        {
            Shared.Visible = true;
        }

        public void SetPage(MNPage p)
        {
            if (p != null)
                InitWithPage(p);
            Page = p;
        }

        private void InitWithPage(MNPage p)
        {
            tabControl1.SelectedTab = tabPage;

            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head></head><body style='font-family:Helvetica'>");


            if (p_Book.Pages.ContainsKey(p.Id))
            {
                p_Page = p_Book.Pages[p.Id];
            }
            else
            {
                webBrowser1.DocumentText = "";
                return;
            }

            if (!p.TextB.Equals(p_Page.PageTitle))
            {
                sb.AppendFormat("<p><b>Page Title</b><br>{0}</p>", p_Page.PageTitle);
            }
            if (!p.MessageText.Equals(p_Page.PageHelp))
            {
                sb.AppendFormat("<p><b>Page Help</b><br>{0}</p>", p_Page.PageHelp);
            }
            if (!string.IsNullOrWhiteSpace(p_Page.PageNotes))
            {
                sb.AppendFormat("<p><b>Page Help</b><br>{0}</p>", p_Page.PageNotes);
            }

            bool header = false;
            foreach(KeyValuePair<long,ReviewItem> revi in p_Page.Items)
            {
                if (!header)
                {
                    sb.AppendFormat("<h3 style='background:#efa0ef'>Items</h3>");
                    header = true;
                }

                sb.AppendLine("<table style='border:1px solid black;'>");
                sb.AppendFormat("<tr><td>ID</td><td>{0}</td></tr>", revi.Key);
                SMControl itor = p_PageOrig.FindObject(revi.Key);
                if (itor != null && !itor.Text.Equals(p_Item.ItemText))
                {
                    sb.AppendFormat("<tr><td>New Text</td><td>{0}</td></tr>", revi.Value.ItemText);
                    sb.AppendFormat("<tr><td>Orig Text</td><td>{0}</td></tr>", itor.Text);
                }
                else if (itor != null)
                {
                    sb.AppendFormat("<tr><td>Text</td><td>{0}</td></tr>", revi.Value.ItemText);
                }

                if (!string.IsNullOrWhiteSpace(revi.Value.ItemNotes))
                    sb.AppendFormat("<tr><td>Notes</td><td>{0}</td></tr>", revi.Value.ItemNotes);
                sb.AppendLine("</table>");

            }

            sb.Append("</body></html>");

            p_PageOrig = p;
            p_ItemOrig = null;

            webBrowser1.DocumentText = sb.ToString();
        }

        public MNDocument Document { get; set; }

        public MNPage Page { get; set; }

        public ReviewFrame()
        {
            InitializeComponent();
        }

        private void ReviewFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePosition();

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }

        }

        public void SavePosition()
        {
            Rectangle r = this.DesktopBounds;
            Properties.Settings.Default.ReviewFrameLocation = r.Location;
            Properties.Settings.Default.ReviewFrameSize = r.Size;
            Properties.Settings.Default.Save();
        }

        public void OnNotificationReceived(object sender, string message, params object[] args)
        {
            if (message.Equals("DocumentChanged"))
            {
                MNDocument doc = args[0] as MNDocument;
                StopDocumentReview();
                StartDocumentReview(doc, doc.Book.FilePath);
            }
            else if (message.Equals("StartDocumentReview"))
            {
                StopDocumentReview();
                StartDocumentReview(args[0] as MNDocument, args[1] as string);
            }
            else if (message.Equals("AppWillClose"))
            {
                StopDocumentReview();
            }
            else if (message.Equals("StopDocumentReview"))
            {
                StopDocumentReview();
            }
            else if (message.Equals("PageDidAppear"))
            {
                SetPage(args[0] as MNPage);
            }
            else if (message.Equals("ObjectForReview"))
            {
            }
        }

        public void SetPageTab()
        {
            if (tabControl1.SelectedTab != tabPage)
            {
                tabControl1.SelectedTab = tabPage;
            }
        }

        private void StartDocumentReview(MNDocument doc, string filePath)
        {
            Document = doc;
            ClearUI();

            labelBookTitle.Text = Document.Book.BookTitle;
            tabControl1.SelectedTab = tabBook;

            LoadData(filePath.Replace(".smb", ".smr"));

            p_PageOrig = null;
            p_ItemOrig = null;
            p_Page = new ReviewPage();
            p_Item = new ReviewItem();
        }

        private void LoadData(string p)
        {
            p_Book = new ReviewBook();
            if (File.Exists(p))
            {
                Debugger.Log(0, "", "Review File " + p + " has been read.");
                XmlDocument doc = new XmlDocument();
                doc.Load(p);
                p_Book.LoadFromXml(doc);
            }
            else
            {
                Debugger.Log(0, "", "File " + p + " does not exist.\n");
            }

            textBookNotes.Text = p_Book.BookNotes;
        }

        private void ClearUI()
        {
            labelBookTitle.Text = "";
            textBookNotes.Text = "";
            webBrowser1.DocumentText = "";
        }

        private void StopDocumentReview()
        {
            if (Document != null)
            {
                SaveData(Document.Book.FilePath.Replace(".smb", ".smr"));
            }
        }

        private void SaveData(string p)
        {
            if (p.Length == 0)
                return;
            XmlDocument doc = new XmlDocument();
            p_Book.SaveToXml(doc);
            doc.Save(p);
        }

        private void ReviewFrame_Load(object sender, EventArgs e)
        {
            if (!SlideMaker.Properties.Settings.Default.ReviewFrameSize.IsEmpty)
            {
                this.DesktopBounds =
                    new Rectangle(SlideMaker.Properties.Settings.Default.ReviewFrameLocation,
                        SlideMaker.Properties.Settings.Default.ReviewFrameSize);
            }
        }

        private void ReviewFrame_FormClosed(object sender, FormClosedEventArgs e)
        {
            SavePosition();
        }

        private void textBookNotes_TextChanged(object sender, EventArgs e)
        {
            p_Book.BookNotes = textBookNotes.Text;
        }

    }
}
