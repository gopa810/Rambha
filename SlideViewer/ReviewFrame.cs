using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using Rambha.Document;

namespace SlideViewer
{
    public partial class ReviewFrame : Form, INotificationTarget
    {
        private static ReviewFrame p_Shared = null;

        private static ReviewBook p_Book = new ReviewBook();
        private static ReviewPage p_Page = new ReviewPage();
        private static ReviewItem p_Item = new ReviewItem();

        private static MNPage p_PageOrig = null;
        private static SMControl p_ItemOrig = null;

        public static ReviewFrame Shared
        {
            get
            {
                if (p_Shared == null)
                {
                    p_Shared = new ReviewFrame();
                    MNNotificationCenter.AddReceiver(p_Shared, null);
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
            label9.Text = p.Id.ToString();
        }

        private void InitWithPage(MNPage p)
        {
            tabControl1.SelectedTab = tabPage;

            if (p_Book.Pages.ContainsKey(p.Id))
            {
                p_Page = p_Book.Pages[p.Id];
            }
            else
            {
                p_Page = new ReviewPage();
                p_Book.Pages[p.Id] = p_Page;
                p_Page.PageTitle = p.TextB;
                p_Page.PageHelp = p.MessageText;
            }

            richTextBox1.Text = p_Page.PageTitle;
            richTextBox2.Text = p_Page.PageHelp;
            richTextBox3.Text = p_Page.PageNotes;
            textItemText.Text = "";
            textItemNotes.Text = "";

            p_PageOrig = p;
            p_ItemOrig = null;

            UpdatePageHelpColor();
            UpdatePageTitleColor();

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
                //this.Visible = false;
            }

        }

        public void SavePosition()
        {
            Rectangle r = this.DesktopBounds;
            Properties.Settings.Default.ReviewFramePosition = r.Location;
            Properties.Settings.Default.ReviewFrameSize = r.Size;
            Properties.Settings.Default.Save();
        }

        public void OnNotificationReceived(object sender, string message, params object[] args)
        {
            if (message.Equals("StartDocumentReview"))
            {
                StopDocumentReview();
                StartDocumentReview(args[0] as MNDocument, args[1] as string);
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
                SetItem(args[0] as SMControl);
            }
        }

        private void SetItem(SMControl sControl)
        {
            if (p_Page.Items.ContainsKey(sControl.Id))
            {
                p_Item = p_Page.Items[sControl.Id];
            }
            else
            {
                p_Item = new ReviewItem();
                p_Page.Items[sControl.Id] = p_Item;
                p_Item.ItemText = sControl.Text;
                p_Item.ItemNotes = "";
            }

            tabControl1.SelectedTab = tabItem;

            labelControlId.Text = sControl.Id.ToString();
            textItemText.Text = p_Item.ItemText;
            textItemNotes.Text = p_Item.ItemNotes;

            p_ItemOrig = sControl;
            UpdateItemTextColor();
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

            LoadData(Document.Book.FilePath.Replace(".smb", ".smr"));

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
                XmlDocument doc = new XmlDocument();
                doc.Load(p);
                p_Book.LoadFromXml(doc);
            }

            textBookNotes.Text = p_Book.BookNotes;
        }

        private void ClearUI()
        {
            labelBookTitle.Text = "";
            textBookNotes.Text = "";
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            textItemText.Text = "";
            textItemNotes.Text = "";
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
            XmlDocument doc = new XmlDocument();
            p_Book.SaveToXml(doc);
            doc.Save(p);
        }

        private void ReviewFrame_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.ReviewFrameSize.IsEmpty)
            {
                this.DesktopBounds =
                    new Rectangle(Properties.Settings.Default.ReviewFramePosition,
                        Properties.Settings.Default.ReviewFrameSize);
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

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            p_Page.PageTitle = richTextBox1.Text;
            UpdatePageTitleColor();
        }

        private void UpdatePageTitleColor()
        {
            if (p_PageOrig != null)
            {
                richTextBox1.ForeColor = p_PageOrig.TextB.Equals(p_Page.PageTitle) ? Color.Gray : Color.Black;
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            p_Page.PageHelp = richTextBox2.Text;
            UpdatePageHelpColor();
        }

        private void UpdatePageHelpColor()
        {
            if (p_PageOrig != null)
            {
                richTextBox2.ForeColor = p_PageOrig.MessageText.Equals(p_Page.PageHelp) ? Color.Gray : Color.Black;
            }
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            p_Page.PageNotes = richTextBox3.Text;
        }

        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {
            p_Item.ItemText = textItemText.Text;
            UpdateItemTextColor();
        }

        private void UpdateItemTextColor()
        {
            if (p_ItemOrig != null)
            {
                textItemText.ForeColor = p_ItemOrig.Text.Equals(p_Item.ItemText) ? Color.Gray : Color.Black;
            }
        }

        private void richTextBox5_TextChanged(object sender, EventArgs e)
        {
            p_Item.ItemNotes = textItemNotes.Text;
        }
    }
}
