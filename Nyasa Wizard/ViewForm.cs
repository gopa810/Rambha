using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


using Rambha.Document;

using SlideMaker.Views;

namespace SlideMaker
{
    public partial class ViewForm : Form, IMainFrameDelegate
    {
        private static ViewForm p_shared = null;
        public static ViewForm Shared
        {
            get
            {
                if (p_shared == null) p_shared = new ViewForm();
                if (MNNotificationCenter.CurrentDocument != null)
                {
                    p_shared.SetDocumentAndPage(MNNotificationCenter.CurrentDocument, 
                        MNNotificationCenter.CurrentPage);
                }
                return p_shared;
            }
        }

        public void SetDocumentAndPage(MNDocument doc, MNPage page)
        {
            pageView1.SetDocument(doc);
            pageView1.CurrentPage = page;
        }

        public ViewForm()
        {
            InitializeComponent();

            pageView1.mainFrameDelegate = this;
            pageView1.InitBitmaps();
            MNNotificationCenter.AddReceiver(pageView1, "ObjectSelected");


            //ReviewFrame.DisplayWindow();

        }

        private void ViewForm_Load(object sender, EventArgs e)
        {
            if (!SlideMaker.Properties.Settings.Default.ViewFrameSize.IsEmpty)
            {
                this.DesktopBounds =
                    new Rectangle(Properties.Settings.Default.ViewFrameLocation,
                        Properties.Settings.Default.ViewFrameSize);
                //AdjustLayoutPageView();
            }
        }

        private void ViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePosition();

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        public static bool IsVisible()
        {
            return (p_shared != null && p_shared.Visible);
        }

        public void SavePosition()
        {
            Rectangle r = this.DesktopBounds;
            Properties.Settings.Default.ViewFrameLocation = r.Location;
            Properties.Settings.Default.ViewFrameSize = r.Size;
            Properties.Settings.Default.Save();
        }

        public void AdjustLayoutPageView()
        {
            int part;
            Size size;
            Size asp = GetAspects();

            part = Math.Min(this.ClientRectangle.Width / asp.Width, this.ClientRectangle.Height / asp.Height);
            size = new Size(part * asp.Width, part * asp.Height);

            pageView1.Size = size;
            pageView1.RecalculateMatrix();
            pageView1.Invalidate();
        }
        public Size GetAspects()
        {
            return (pageView1.Context.DisplaySize == PageEditDisplaySize.PortaitBig
                || pageView1.Context.DisplaySize == PageEditDisplaySize.PortaitSmall) ?
                new Size(3, 4) : new Size(4, 3);
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

        public void showSelectLanguageDialog(MNDocument book)
        {
            MessageBox.Show("Selecting another language");
        }

        public void SetShowPanel(string panel)
        {
        }

        public void RefreshList()
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

        public void dialogDidSelectLanguage(MNLocalisation loc)
        {
        }

        private void pageView1_SizeChanged(object sender, EventArgs e)
        {
            AdjustLayoutPageView();
        }


    }
}
