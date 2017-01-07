using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Rambha.Document;
using Rambha.Script;

namespace SlideViewer.Views
{
    public partial class PageView : UserControl
    {
        private MNDocument Document { get; set; }

        private MNPage p_current_page = null;

        public PageViewController ViewController { get; set; }

        public MNDocumentExecutor DocExec = null;

        public MNPageContext Context { get; set; }

        public PageView()
        {
            InitializeComponent();
            ViewController = new PageViewController();
            ViewController.View = this;
            DocExec = new MNDocumentExecutor(ViewController);
            Context = new MNPageContext();
        }

        public void Start()
        {
            CurrentPage = Document.GetPage(Document.StartPage);
            Invalidate();
        }

        public MNDocument CurrentDocument
        {
            get { return Document; }
            set
            {
                Document = value;
                Document.Viewer = DocExec;
                DocExec.SetDocument(Document);
            }
        }

        public MNPage CurrentPage 
        {
            get
            {
                return p_current_page;
            }
            set
            {
                MNPage oldPage = p_current_page;
                if (p_current_page != null)
                    p_current_page.OnPageWillDisappear();
                if (value != null)
                    value.OnPageWillAppear();
                p_current_page = value;
                DocExec.CurrentPage = value;
                Invalidate();
                if (value != null)
                    value.OnPageDidAppear();
                if (oldPage != null)
                    value.OnPageDidDisappear();

            }
        }

        public void RecalculateMatrix()
        {
            // calculate new size of this control
            Size viewSize = this.Size;
            Size pageSize = Context.PageSize;

            float zoom_ratio = (float)viewSize.Height / pageSize.Height;

            // calculate transformation matrix
            Matrix a = new Matrix();
            a.Scale(zoom_ratio, zoom_ratio);
            Context.LastMatrix = a;
            a = new Matrix();
            a.Scale(zoom_ratio, zoom_ratio);
            a.Invert();
            Context.LastInvertMatrix = a;
            Context.PageWidth = pageSize.Width;
            Context.PageHeight = pageSize.Height;

        }

        private void PageView_Paint(object sender, PaintEventArgs e)
        {
            if (CurrentPage == null)
                return;

            e.Graphics.Transform = Context.LastMatrix;

            Context.g = e.Graphics;
            Context.CurrentPage = CurrentPage;
            Context.drawSelectionMarks = false;

            CurrentPage.Paint(Context);

            // painting dragged item
            switch (MouseContext.DragType)
            {
                case SMDragResponse.Drag:
                    PaintDraggedItem(Context, MouseContext);
                    break;
                case SMDragResponse.Line:
                    e.Graphics.DrawLine(Pens.Black, MouseContext.startPoint, MouseContext.lastPoint);
                    break;
                default:
                    break;
            }
        }

        private void PaintDraggedItem(MNPageContext context, PVDragContext mouseContext)
        {
            if (mouseContext.draggedItem != null)
            {
                SMTokenItem ti = mouseContext.draggedItem;
                if (ti.Text != null && ti.Text.Length > 0)
                {
                    if (ti.ContentSize.Width == 0)
                    {
                        SizeF sf = context.g.MeasureString(ti.Text, ti.TextFont);
                        ti.ContentSize = new Size(Convert.ToInt32(sf.Width),Convert.ToInt32(sf.Height));
                    }
                    Rectangle rc = new Rectangle(mouseContext.lastPoint.X - ti.ContentSize.Width/2, mouseContext.lastPoint.Y - ti.ContentSize.Height,
                        ti.ContentSize.Width + 2, ti.ContentSize.Height + 2);
                    context.g.DrawString(ti.Text, ti.TextFont, ti.TextBrush, rc);
                }
                else if (ti.Image != null)
                {
                    if (ti.ContentSize.Width == 0)
                    {
                        Size sf = ti.Image.Size;
                        double d = 64.0 / (Math.Max(sf.Width,sf.Height) + 1);
                        ti.ContentSize = new Size(Convert.ToInt32(d*sf.Width), Convert.ToInt32(d*sf.Height));
                    }
                    Rectangle rc = new Rectangle(mouseContext.lastPoint.X - ti.ContentSize.Width / 2, 
                        mouseContext.lastPoint.Y - ti.ContentSize.Height / 2,
                        ti.ContentSize.Width + 2, ti.ContentSize.Height + 2);
                    context.g.DrawImage(ti.Image, rc);
                }
            }
        }

        private SMControl FindObjectContainingPoint(Point logPoint)
        {
            foreach (SMControl po in CurrentPage.SortedObjects)
            {
                SMRectangleArea area = CurrentPage.GetArea(po.Id);
                SMControlSelection testResult = area.TestHitLogical(Context, logPoint);
                if (testResult != SMControlSelection.None)
                {
                    return po;
                }
            }

            return null;
        }

        private PVDragContext MouseContext = new PVDragContext();

        /// <summary>
        /// Pointer started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageView_MouseDown(object sender, MouseEventArgs e)
        {
            if (CurrentPage == null)
                return;

            MouseContext.lastPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
            MouseContext.startPoint = MouseContext.lastPoint;
            MouseContext.startControl = FindObjectContainingPoint(MouseContext.lastPoint);
            MouseContext.endControl = null;
            MouseContext.State = PVDragContext.Status.ClickDown;
            MouseContext.DragType = SMDragResponse.None;
            MouseContext.context = Context;

            if (MouseContext.startControl != null)
                MouseContext.startControl.OnTapBegin(MouseContext);
            MouseContext.StartClicked = true;

            timerLongClick.Start();
            Invalidate();
        }

        /// <summary>
        /// Pointer released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageView_MouseUp(object sender, MouseEventArgs e)
        {
            if (CurrentPage == null)
                return;


            MouseContext.context = Context;
            MouseContext.lastPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
            MouseContext.endControl = FindObjectContainingPoint(MouseContext.lastPoint);

            if (MouseContext.startControl != null)
                MouseContext.startControl.OnTapEnd(MouseContext);

            if (MouseContext.State == PVDragContext.Status.Dragging)
            {
                if (MouseContext.trackedControl != null)
                {
                    MouseContext.trackedControl.OnDropFinished(MouseContext);
                    MouseContext.trackedControl.OnDragHotTrackEnded(MouseContext.draggedItem, MouseContext);
                }
                if (MouseContext.startControl != null)
                {
                    MouseContext.startControl.OnDragFinished(MouseContext);
                }
            }
            else if (MouseContext.State == PVDragContext.Status.ClickDown)
            {
                if (MouseContext.startControl != null)
                {
                    MouseContext.startControl.OnClick(MouseContext);
                    MouseContext.startControl.OnTapEnd(MouseContext);
                }
                MouseContext.StartClicked = false;
            }
            else
            {
                if (MouseContext.startControl != null)
                {
                    MouseContext.startControl.OnTapEnd(MouseContext);
                }
                MouseContext.StartClicked = false;
            }

            MouseContext.State = PVDragContext.Status.None;
            MouseContext.DragType = SMDragResponse.None;
            MouseContext.draggedItem = null;
            Invalidate();
        }

        /// <summary>
        /// Moving pointer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageView_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentPage == null)
                return;


            MouseContext.lastPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
            MouseContext.endControl = FindObjectContainingPoint(MouseContext.lastPoint);


            if (MouseContext.endControl != null)
                MouseContext.endControl.OnDropMove(MouseContext);
            if (MouseContext.State == PVDragContext.Status.Dragging)
            {
                if (MouseContext.startControl != null)
                    MouseContext.startControl.OnTapMove(MouseContext);

                if (MouseContext.trackedControl == null && MouseContext.endControl != null && MouseContext.endControl != MouseContext.startControl
                    && MouseContext.endControl.Style.Droppable != SMDropResponse.None)
                {
                    MouseContext.endControl.OnDragHotTrackStarted(MouseContext.draggedItem, MouseContext);
                }
                else if (MouseContext.trackedControl != null && MouseContext.endControl == null
                    && MouseContext.trackedControl.Style.Droppable != SMDropResponse.None)
                {
                    MouseContext.trackedControl.OnDragHotTrackEnded(MouseContext.draggedItem, MouseContext);
                }
                MouseContext.trackedControl = MouseContext.endControl;
                Invalidate();
            }
            else if (MouseContext.State == PVDragContext.Status.ClickDown)
            {
                if (MouseContext.startControl != null)
                    MouseContext.startControl.OnTapMove(MouseContext);

                if (PointDistance(MouseContext.lastPoint, MouseContext.startPoint) > 10)
                {
                    if (timerLongClick.Enabled)
                        timerLongClick.Stop();
                    MouseContext.startControl.OnTapCancel(MouseContext);
                    MouseContext.StartClicked = false;
                    if (MouseContext.startControl != null && MouseContext.startControl.Style.Draggable != SMDragResponse.None)
                    {
                        MouseContext.State = PVDragContext.Status.Dragging;
                        MouseContext.DragType = MouseContext.startControl.Style.Draggable;
                        if (MouseContext.DragType == SMDragResponse.Drag)
                            MouseContext.draggedItem = MouseContext.startControl.GetDraggableItem(MouseContext.lastPoint);
                        MouseContext.startControl.OnDragStarted(MouseContext);
                        MouseContext.startControl.OnDragMove(MouseContext);
                    }
                    Invalidate();
                }
            }
        }

        private double PointDistance(Point a, Point b)
        {
            return Math.Sqrt(Convert.ToDouble((a.X - b.X)*(a.X - b.X)) + Convert.ToDouble((b.Y - a.Y)*(b.Y - a.Y)));
        }

        private void timerLongClick_Tick(object sender, EventArgs e)
        {
            if (MouseContext.State == PVDragContext.Status.ClickDown)
            {
                MouseContext.State = PVDragContext.Status.LongClicked;
                if (Document.HasViewer)
                    Document.Viewer.OnLongClick(MouseContext.startControl, MouseContext);
                //ExecuteScriptForKey(MouseContext.startControl, "onLongClick");
            }
            timerLongClick.Stop();
        }

        private void PageView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Document.HasViewer)
                Document.Viewer.OnDoubleClick(MouseContext.startControl, MouseContext);
            //ExecuteScriptForKey(MouseContext.startControl, "onDoubleClick");

        }

    }


    /// <summary>
    /// Main Controller in the presentation
    /// </summary>
    public class PageViewController : GSCore
    {
        public PageView View { get; set; }

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            switch(token)
            {
                case "restart":
                    View.Start();
                    break;
                default:
                    base.ExecuteMessage(token, args);
                    break;
            }

            return GSVoid.Void;
        }

        public override GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "document": return View.CurrentDocument;
                case "currentPage": return View.CurrentPage;
                case "context": return View.Context;
                case "self": return this;
            }
            return base.GetPropertyValue(s);
        }
    }
}
