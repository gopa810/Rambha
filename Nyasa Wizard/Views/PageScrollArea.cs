using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SlideMaker;
using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class PageScrollArea : UserControl, IPageScrollArea, INotificationTarget
    {
        public event PageChangedEventHandler PageObjectSelected;

        public event PageChangedEventHandler NewPageRequested;

        public event NormalEventHandler BackToParentView;

        public PageScrollArea()
        {
            InitializeComponent();
            pageEditView1.ScrollAreaController = this;
            toolStripComboBox1.SelectedIndex = 0;
            checkPagePlacement();

            MNNotificationCenter.AddReceiver(this, "ObjectSelected");
        }

        public void SetPage(MNPage page)
        {
            pageEditView1.Document = page.Document;
            pageEditView1.Page = page;
            MNNotificationCenter.CurrentPage = page;
        }

        void INotificationTarget.OnNotificationReceived(object sender, string msg, params object[] args)
        {
            pageEditView1.OnNotificationReceived(sender, msg, args);
        }

        private void checkPagePlacement()
        {
            Rectangle rect = panel1.Bounds;
            rect.X = 10;
            rect.Y = 10;
            rect.Height -= 20;
            rect.Width -= 20;

            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height/2);

            Size pageSize = pageEditView1.Context.PageSize;

            double A = Math.Min(Convert.ToDouble(rect.Width) / pageSize.Width, 
                Convert.ToDouble(rect.Height) / pageSize.Height);

            Size controlSize = new Size(Convert.ToInt32(A * pageSize.Width), Convert.ToInt32(A * pageSize.Height));
            Rectangle controlBounds = new Rectangle(center.X - controlSize.Width/2, center.Y - controlSize.Height/2, controlSize.Width, controlSize.Height);

            pageEditView1.Bounds = controlBounds;
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    DisplaySize = PageEditDisplaySize.LandscapeBig;
                    break;
                case 1:
                    DisplaySize = PageEditDisplaySize.LandscapeSmall;
                    break;
                case 2:
                    DisplaySize = PageEditDisplaySize.PortaitBig;
                    break;
                case 3:
                    DisplaySize = PageEditDisplaySize.PortaitSmall;
                    break;
                default:
                    break;
            }
        }
        private void PageScrollArea_SizeChanged(object sender, EventArgs e)
        {
            checkPagePlacement();
        }

        private void PageScrollArea_DragEnter(object sender, DragEventArgs e)
        {
            Point pt = this.PointToClient(new Point(e.X, e.Y));
            if (pageEditView1.Bounds.Contains(pt))
            {
                pageEditView1.PageEditView_DragEnter(sender, e);
            }
            else
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else if (e.Data.GetDataPresent(typeof(PageEditDraggableItem)))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else if (e.Data.GetDataPresent(typeof(MNReferencedImage)))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else if (e.Data.GetDataPresent(typeof(MNPage)))
                {
                    MNPage page = e.Data.GetData(typeof(MNPage)) as MNPage;
                    e.Effect = (page.IsTemplate ? DragDropEffects.Copy : DragDropEffects.None);
                }
            }
        }

        private void PageScrollArea_DragDrop(object sender, DragEventArgs e)
        {
            Point pt = this.PointToClient(new Point(e.X, e.Y));
            if (pageEditView1.Bounds.Contains(pt))
            {
                pageEditView1.PageEditView_DragDrop(sender, e);
            }
            else
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    int accepted = 0;
                    foreach (string file in files)
                    {
                        if (pageEditView1.Document.AcceptFile(file) != null)
                            accepted++;
                    }
                    e.Effect = DragDropEffects.Copy;
                    if (accepted > 0)
                    {
                        MNNotificationCenter.BroadcastMessage(this, "FilesAdded");
                    }
                }
                else if (e.Data.GetDataPresent(typeof(PageEditDraggableItem)))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else if (e.Data.GetDataPresent(typeof(MNPage)))
                {
                    MNPage tmp = e.Data.GetData(typeof(MNPage)) as MNPage;
                    if (tmp.IsTemplate && pageEditView1.Page != null)
                    {
                        pageEditView1.Page.Template = tmp;
                        pageEditView1.Invalidate();
                    }
                    e.Effect = (tmp.IsTemplate ? DragDropEffects.Copy : DragDropEffects.None);
                }
            }
        }

        public void ZoomIn()
        {
            float zoom = pageEditView1.ZoomRatio;
            zoom *= 1.5f;
            if (zoom > 10f)
                zoom = 10;
            SetZoom(zoom);
        }

        public void ZoomOut()
        {
            float zoom = pageEditView1.ZoomRatio;
            zoom /= 1.5f;
            if (zoom < 0.3f)
                zoom = 0.3f;
            SetZoom(zoom);
        }

        public void InvalidatePageEditors()
        {
            pageEditView1.Invalidate();
        }

        private void SetZoom(float zoom)
        {
            if (pageEditView1.ZoomRatio != zoom)
            {
                // set new zoom
                pageEditView1.ZoomRatio = zoom;

                // check position
                checkPagePlacement();
            }
        }

        private void pageEditView1_NewPageRequested(object sender, PageEditViewArguments e)
        {
            if (NewPageRequested != null)
                NewPageRequested(sender, e);
        }

        private void pageEditView1_PageObjectSelected(object sender, PageEditViewArguments e)
        {
            if (PageObjectSelected != null)
                PageObjectSelected(sender, e);
        }

        public PageEditDisplaySize DisplaySize 
        {
            get 
            {
                return pageEditView1.DisplaySize; 
            }
            set
            { 
                pageEditView1.DisplaySize = value;
                switch (value)
                {
                    case PageEditDisplaySize.LandscapeBig:
                        pageEditView1.ViewSize = new Size(1024, 768);
                        break;
                    case PageEditDisplaySize.LandscapeSmall:
                        pageEditView1.ViewSize = new Size(800, 600);
                        break;
                    case PageEditDisplaySize.PortaitBig:
                        pageEditView1.ViewSize = new Size(768, 1024);
                        break;
                    case PageEditDisplaySize.PortaitSmall:
                        pageEditView1.ViewSize = new Size(600, 800);
                        break;

                }
                if (pageEditView1.Page != null)
                {
                    pageEditView1.Context.DisplaySize = value;
                    foreach (SMControl sm in pageEditView1.Page.Objects)
                    {
                        SMRectangleArea area = pageEditView1.Page.GetArea(sm.Id);
                        if (area.Selected) area.GetBoundsRecalc(pageEditView1.Context, true);
                    }
                }
                pageEditView1.Invalidate();
            } 
        }

        public void InvalidateClient()
        {
            pageEditView1.Invalidate();
        }

        private void PageScrollArea_MouseClick(object sender, MouseEventArgs e)
        {
            MNNotificationCenter.BroadcastMessage(this, "SelectedObject", pageEditView1.Document);
        }

        // go back
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.BackToParentView != null)
                this.BackToParentView(this, e);
        }
    }

    public interface IPageScrollArea
    {
        void ZoomIn();
        void ZoomOut();
    }
}
