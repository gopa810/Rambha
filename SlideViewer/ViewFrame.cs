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
using Rambha.GOF;

namespace SlideViewer
{
    public partial class ViewFrame : Form
    {
        public List<MNBookHeader> Books = new List<MNBookHeader>();

        public ViewFrame()
        {
            InitializeComponent();
            AdjustLayoutPageView();

            panelBook.Dock = DockStyle.Fill;
            panelFiles.Dock = DockStyle.Fill;
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
            }
            else if (panel == "book")
            {
                panelBook.Visible = true;
                panelFiles.Visible = false;
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

            List<string> bookFileNames = new List<string>();
            List<string> langFileNames = new List<string>();

            foreach (string s in Directory.EnumerateFiles(directory))
            {
                if (s.EndsWith(".smd"))
                {
                    bookFileNames.Add(s);
                }
                else if (s.EndsWith(".gof"))
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

            listBox1.Items.Clear();

            foreach (MNBookHeader bh in Books)
            {
                listBox1.Items.Add(bh);
            }
        }

        public void PreviewLanguage(MNBookLanguage bl, string fileName)
        {
            GOFile file = new GOFile();
            bl.FilePath = fileName;
            file.Load(bl.FilePath, false);
            bl.BookCode = file.GetProperty("BookCode");
            bl.LanguageCode = file.GetProperty("LanguageCode");
            bl.LanguageName = file.GetProperty("LanguageName");
        }

        public GOFile LoadLanguage(MNBookLanguage bl)
        {
            GOFile file = new GOFile();
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

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                MNBookHeader bh = (MNBookHeader)(listBox1.Items[listBox1.SelectedIndex]);
                if (bh != null)
                {
                    pageView1.CurrentDocument = bh.LoadFull();
                    SetShowPanel("book");
                    pageView1.Start();
                }
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            ShowFiles("");
        }
    }
}
