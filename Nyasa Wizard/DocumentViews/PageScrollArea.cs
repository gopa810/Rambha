using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SlideMaker.Document;

namespace SlideMaker.DocumentViews
{
    public partial class PageScrollArea : UserControl, IDocumentDelegate, IPageScrollArea
    {
        private MNDocument Document = null;

        public event PageChangedEventHandler PageObjectSelected;

        public event PageChangedEventHandler NewPageRequested;


        public PageScrollArea()
        {
            InitializeComponent();
            pageEditView1.ScrollAreaController = this;
        }

        public PageEditView EditView
        {
            get
            {
                return pageEditView1;
            }
        }

        public void documentHasChanged(MNDocument doc)
        {
            Document = doc;
            pageEditView1.Size = doc.PageSize;
            checkPagePlacement();
        }

        private void checkPagePlacement()
        {
            int newX = pageEditView1.Location.X;
            int newY = pageEditView1.Location.Y;

            int innerWidth = this.Width - (this.VScroll ? SystemInformation.VerticalScrollBarWidth : 0);
            int innerHeight = this.Height - (this.HScroll ? SystemInformation.HorizontalScrollBarHeight : 0);

            if (pageEditView1.Size.Width < innerWidth)
            {
                newX = (innerWidth - pageEditView1.Size.Width) / 2;
            }
            if (pageEditView1.Size.Height < innerHeight)
            {
                newY = (innerHeight - pageEditView1.Size.Height) / 2;
            }

            pageEditView1.Location = new Point(newX, newY);
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
                e.Effect = DragDropEffects.Copy;
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
                e.Effect = DragDropEffects.Copy;
            }
        }

        public void ZoomIn()
        {
            float zoom = pageEditView1.ZoomRatio;
            zoom *= 1.5f;
            if (zoom > 10f)
                zoom = 10;
            if (pageEditView1.ZoomRatio != zoom)
            {
                // change location
                Point loc = pageEditView1.Location;
                float change = zoom / pageEditView1.ZoomRatio;
                pageEditView1.Location = new Point((int)(change * loc.X), (int)(change * loc.Y));

                // set new zoom
                pageEditView1.ZoomRatio = zoom;

                // check position
                checkPagePlacement();
            }
        }

        public void ZoomOut()
        {
            float zoom = pageEditView1.ZoomRatio;
            zoom /= 1.5f;
            if (zoom < 0.3f)
                zoom = 0.3f;
            if (pageEditView1.ZoomRatio != zoom)
            {
                // change location
                Point loc = pageEditView1.Location;
                float change = zoom / pageEditView1.ZoomRatio;
                pageEditView1.Location = new Point((int)(change * loc.X), (int)(change * loc.Y));

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
    }

    public interface IPageScrollArea
    {
        void ZoomIn();
        void ZoomOut();
    }
}
