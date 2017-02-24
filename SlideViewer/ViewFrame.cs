using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Rambha.Document;

namespace SlideViewer
{
    public partial class ViewFrame : Form, IMainFrameDelegate
    {
        public SVBookLibrary Library = new SVBookLibrary();
        public MNBookHeader CurrentBook = null;

        public ViewFrame()
        {
            InitializeComponent();
            AdjustLayoutPageView();

            panelBook.Dock = DockStyle.Fill;
            panelFiles.Dock = DockStyle.Fill;
            panelUpdater.Dock = DockStyle.Fill;
            panelSelectLanguage.Dock = DockStyle.Fill;

            Library.FetchRemote(null);
        }

        public MNDocument Document
        {
            get { return pageView1.CurrentDocument; }
            set { pageView1.CurrentDocument = value; RefreshContentFromDocument(); }
        }


        public void SetShowPanel(string panel)
        {
            if (panel == "files")
            {
                panelBook.Visible = false;
                panelFiles.Visible = true;
                panelUpdater.Visible = false;
                panelSelectLanguage.Visible = false;
            }
            else if (panel == "book")
            {
                panelBook.Visible = true;
                panelFiles.Visible = false;
                panelUpdater.Visible = false;
                panelSelectLanguage.Visible = false;
            }
            else if (panel == "lang")
            {
                panelSelectLanguage.Visible = true;
                panelBook.Visible = false;
                panelFiles.Visible = false;
                panelUpdater.Visible = false;
            }
            else if (panel == "updater")
            {
                panelSelectLanguage.Visible = false;
                panelBook.Visible = false;
                panelFiles.Visible = false;
                panelUpdater.Visible = true;
                panelUpdater.ParentFrame = this;
            }
        }

        /// <summary>
        /// Refreshes UI from Document data
        /// </summary>
        private void RefreshContentFromDocument()
        {/*
            AdjustLayoutPageView();

            if (Document != null && Document.Pages != null)
            {
                toolStripComboBox2.Items.Clear();
                for (int i = 0; i < Document.Pages.Count; i++)
                {
                    toolStripComboBox2.Items.Add(string.Format("[{0}] {1}", i + 1, Document.Pages[i].Title));
                }
            }

            toolStripComboBox2.SelectedIndex = toolStripComboBox2.Items.Count - 1;*/
        }

        /// <summary>
        /// Display Mode selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            if (toolStripComboBox1.SelectedIndex >= 0)
            {
                // set new layout to pageView1
                switch (toolStripComboBox1.SelectedIndex)
                {
                    case 0:
                        pageView1.Context.DisplaySize = PageEditDisplaySize.LandscapeBig;
                        break;
                    case 1:
                        pageView1.Context.DisplaySize = PageEditDisplaySize.LandscapeSmall;
                        break;
                    case 2:
                        pageView1.Context.DisplaySize = PageEditDisplaySize.PortaitBig;
                        break;
                    case 3:
                        pageView1.Context.DisplaySize = PageEditDisplaySize.PortaitSmall;
                        break;
                    default:
                        break;
                }

                // adjusting layout
                AdjustLayoutPageView();
            }*/
        }

        /// <summary>
        /// page selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*try
            {
                pageView1.CurrentPage = pageView1.Document.Pages[toolStripComboBox2.SelectedIndex];
                pageView1.Invalidate();
            }
            catch
            {
            }*/
        }

        /// <summary>
        /// Refresh button (refresh page view, page list , etc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            RefreshContentFromDocument();
        }

        /// <summary>
        /// Start Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Start();
        }

        public void Start()
        {
            pageView1.Start();
        }

        public Size GetAspects()
        {
            return (pageView1.Context.DisplaySize == PageEditDisplaySize.PortaitBig
                || pageView1.Context.DisplaySize == PageEditDisplaySize.PortaitSmall) ?
                new Size(3,4) : new Size(4,3);
        }

        public void AdjustLayoutPageView()
        {
            int part;
            Size size;
            Size asp = GetAspects();

            part = Math.Min(panelBook.Width/asp.Width, panelBook.Height / asp.Height);
            size = new Size(part * asp.Width, part * asp.Height);

            pageView1.Size = size;
            pageView1.RecalculateMatrix();
            pageView1.Invalidate();
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            AdjustLayoutPageView();
        }

        public Size AdjustSizeForm(int mode)
        {
            Size s = ClientSize;
            float part;
            Size asp = GetAspects();

            switch (mode)
            {
                case 1: // changed is left or right
                    part = s.Width / asp.Width;
                    break;
                case 2:
                    part = s.Height / asp.Height;
                    break;
                default:
                    part = Math.Min(s.Width / asp.Width, s.Height / asp.Height);
                    break;
            }
            s = new System.Drawing.Size(Convert.ToInt32(part * asp.Width), Convert.ToInt32(part * asp.Height));

            return SizeFromClientSize(s);
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
        }


        // From Windows SDK
        private const int WM_SIZING = 0x214;

        private const int WMSZ_LEFT = 1;
        private const int WMSZ_RIGHT = 2;
        private const int WMSZ_TOP = 3;
        private const int WMSZ_BOTTOM = 6;

        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SIZING)
            {
                RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));

                Size newSize = AdjustSizeForm(2);

                switch (m.WParam.ToInt32()) // Resize handle
                {
                    case WMSZ_LEFT:
                    case WMSZ_RIGHT:
                        // Left or right handles, adjust height                        
                        rc.Bottom = rc.Top + AdjustSizeForm(1).Height;
                        break;

                    case WMSZ_TOP:
                    case WMSZ_BOTTOM:
                        // Top or bottom handles, adjust width
                        rc.Right = rc.Left + AdjustSizeForm(2).Width;
                        break;

                    case WMSZ_LEFT + WMSZ_TOP:
                    case WMSZ_LEFT + WMSZ_BOTTOM:
                        // Top-left or bottom-left handles, adjust width
                        rc.Left = rc.Right - AdjustSizeForm(2).Width;
                        break;

                    case WMSZ_RIGHT + WMSZ_TOP:
                        // Top-right handle, adjust height
                        //rc.Top = rc.Bottom - newSize.Height;
                        rc.Right = rc.Left + AdjustSizeForm(2).Width;
                        break;

                    case WMSZ_RIGHT + WMSZ_BOTTOM:
                        // Bottom-right handle, adjust height
//                        rc.Bottom = rc.Top + newSize.Height;
                        rc.Right = rc.Left + AdjustSizeForm(2).Width;
                        break;
                }

                Marshal.StructureToPtr(rc, m.LParam, true);
            }
            base.WndProc(ref m);
        }

        private void ViewFrame_Shown(object sender, EventArgs e)
        {
            SetShowPanel("files");
            ShowFiles(Properties.Settings.Default.FilesDirectory);
        }

        public void ShowFiles(string directory)
        {
            if (directory.Length == 0)
            {
                FolderBrowserDialog d = new FolderBrowserDialog();
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    directory = d.SelectedPath;
                    Properties.Settings.Default.FilesDirectory = d.SelectedPath;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    return;
                }
            }

            Library.GetCurrentBookDatabase(directory);

            listBox1.Items.Clear();

            foreach (MNBookHeader bh in Library.Books)
            {
                listBox1.Items.Add(bh);
            }
        }


        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                MNBookHeader bh = (MNBookHeader)(listBox1.Items[listBox1.SelectedIndex]);
                if (bh != null)
                {
                    CurrentBook = bh;
                    pageView1.CurrentBook = bh;
                    pageView1.SetDocument(bh.LoadFull());
                    // this is default loading of language file
                    if (bh.Languages != null && bh.Languages.Count > 0)
                    {
                        MNLocalisation file = new MNLocalisation();
                        file.Load(bh.Languages[0].FilePath, true);
                        pageView1.CurrentDocument.CurrentLanguage = file;
                    }
                    // this is presenting book to the user
                    SetShowPanel("book");
                    pageView1.Start();
                }
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            ShowFiles("");
        }

        public void showSelectLanguageDialog(MNBookHeader book)
        {
            if (book != null && book.Languages != null && book.Languages.Count > 0)
            {
                CurrentBook = book;
                SetShowPanel("lang");
                panelSelectLanguage.SetBook(book);
                panelSelectLanguage.ParentFrame = this;
            }
        }

        public void dialogDidSelectLanguage(MNBookLanguage lang)
        {
            if (CurrentBook != null)
            {
                if (lang != null)
                {
                    MNLocalisation file = new MNLocalisation();
                    file.Load(lang.FilePath, true);
                    pageView1.CurrentDocument.CurrentLanguage = file;
                    pageView1.ReloadPage();
                }
                SetShowPanel("book");
            }
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            MNBookHeader bh = GetBookAtIndex(e.Index);
            if (bh == null)
                return;

            e.ItemHeight = 32;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) != 0)
            {
                e.Graphics.FillRectangle(SMGraphics.GetBrush(Color.LightBlue), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(SMGraphics.GetBrush(listBox1.BackColor), e.Bounds);
            }

            MNBookHeader bh = GetBookAtIndex(e.Index);
            if (bh == null)
                return;

            Rectangle r = new Rectangle(e.Bounds.Left + 4, e.Bounds.Top + 4, e.Bounds.Height - 8, e.Bounds.Height - 8);

            e.Graphics.FillRectangle(SMGraphics.GetBrush(bh.BookColor), r);
            e.Graphics.DrawRectangle(Pens.Black, r);

            r = new Rectangle(e.Bounds.Left + e.Bounds.Height + 16, e.Bounds.Top, e.Bounds.Width - e.Bounds.Height - 16, e.Bounds.Height);
            e.Graphics.DrawString(bh.BookTitle, SMGraphics.GetFontVariation(MNFontName.LucidaSans, 12f),
                SMGraphics.GetBrush(listBox1.ForeColor), r, SMGraphics.StrFormatLeftCenter);
        }

        public MNBookHeader GetBookAtIndex(int index)
        {
            if (index < 0 || index >= listBox1.Items.Count)
                return null;

            return listBox1.Items[index] as MNBookHeader;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetShowPanel("updater");
            panelUpdater.Start(Library);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.FileName = "root.txt";
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string str = SVBookLibrary.DBToString(Library.GetLocalFileDatabase());
                File.WriteAllText(d.FileName, str);
            }
        }
    }
}
