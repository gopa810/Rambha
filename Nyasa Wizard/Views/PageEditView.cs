using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Rambha.Document;

namespace SlideMaker.Views
{
    public delegate void NormalEventHandler(object sender, EventArgs e);

    public delegate void PageChangedEventHandler(object sender, PageEditViewArguments e);

    public partial class PageEditView : UserControl, INotificationTarget
    {
        public event PageChangedEventHandler NewPageRequested;

        public IPageScrollArea ScrollAreaController = null;

        public MNDocument Document { get; set; }

        public Point LastUserPoint { get; set; }

        public PageEditDisplaySize DisplaySize { get; set; }

        // this is normalized to page size 1000 x 1000 points
        public Point LastRelativePoint { get; set; }

        private MNPage PageData { get; set; }

        public MNPageContext Context = new MNPageContext();

        private float zoom_ratio = 1f;

        private Size view_size = new Size(1024, 768);

        public float ZoomRatio
        {
            set
            {
                zoom_ratio = value;
                RecalculateZoomSizeMatrix();
            }
            get { return zoom_ratio; }
        }

        private void RecalculateZoomSizeMatrix()
        {
            if (Page != null)
            {
                // determine stable location point
                // with current size
                //Point stableLocation = this.Location;
                zoom_ratio = Math.Min(Size.Width/view_size.Width, Size.Height/view_size.Height);

                // calculate new size of this control
                /*Size ns = new Size(Convert.ToInt32(view_size.Width * zoom_ratio),
                    Convert.ToInt32(view_size.Height * zoom_ratio));*/

                // calculate transformation matrix
                RectangleF logicalBounds = new RectangleF(0,0,view_size.Width,view_size.Height);
                PointF[] targetParalelogram = new PointF[] {
                    new PointF(0,0),
                    new PointF(Size.Width, 0),
                    new PointF(0,Size.Height)
                };
                Matrix a = new Matrix(logicalBounds, targetParalelogram);
                Context.LastMatrix = a;

                a = new Matrix(logicalBounds, targetParalelogram);
                a.Invert();
                Context.LastInvertMatrix = a;

                // set new values
                //this.Location = stableLocation;
                //this.Size = ns;
            }
        }

        public Size ViewSize
        {
            set
            {
                view_size = value;
                RecalculateZoomSizeMatrix();
            }
            get
            {
                return view_size;
            }
        }

        public MNPage Page
        {
            get
            {
                return PageData;
            }
            set
            {
                PageData = value;
                /*if (PageData != null && PageData.Document != null)
                    ViewSize = PageData.Document.PageSize;*/
                RecalculateZoomSizeMatrix();
                Invalidate();
            }
        }

        public PageEditView()
        {
            InitializeComponent();

            Context = new MNPageContext();
            Context.cursorBoundaryReady = new Cursor(new MemoryStream(Properties.Resources.cursor_b_ready));
            Context.cursorBoundaryMoving = new Cursor(new MemoryStream(Properties.Resources.cursor_b_move));
            Context.cursorBoundaryAnchor = new Cursor(new MemoryStream(Properties.Resources.cursor_b_anchor));

            MNNotificationCenter.AddReceiver(this, "ObjectSelected");

            this.MouseWheel += PageEditView_MouseWheel;
        }

        private void PageEditView_Paint(object sender, PaintEventArgs e)
        {
            if (PageData == null)
                return;

            e.Graphics.Transform = Context.LastMatrix;

            Context.g = e.Graphics;
            Context.CurrentPage = Page;
            Context.DisplaySize = DisplaySize;
            Context.zoom = ZoomRatio;
            Context.PageWidth = view_size.Width;
            Context.PageHeight = view_size.Height;
            Context.drawSelectionMarks = true;

            PageData.Paint(Context);

        }

        private void setPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void insertMantraToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void insertNewPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NewPageRequested != null)
            {
                PageEditViewArguments args = new PageEditViewArguments();

                args.Document = Document;
                args.Page = PageData;

                NewPageRequested.Invoke(this, args);
            }
        }

        private void PageEditView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && PageData != null)
            {
                Point clientPoint = new Point(e.X, e.Y);
                LastUserPoint = PointToScreen(clientPoint);
                LastRelativePoint = Context.PhysicalToRelative(clientPoint);
                contextMenuStrip1.Show(LastUserPoint);
            }
        }

        private void pagePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void PageEditView_SizeChanged(object sender, EventArgs e)
        {
            RecalculateZoomSizeMatrix();
            Invalidate();
        }

        private void PageEditView_MouseUp(object sender, MouseEventArgs e)
        {
            Context.isTracking = false;
            Point offset = Context.TrackedDrawOffset;
            Context.TrackedDrawOffset = Point.Empty;

            if (Context.TrackedObjects.Count > 0)
            {
                foreach (SMControl obj in Context.TrackedObjects)
                {
                    SMRectangleArea po = Page.GetArea(obj.Id);
                    if (po.TrackedSelection != SMControlSelection.None)
                    {
                        po.Move(Context, offset);
                        if (obj.Autosize)
                            obj.RecalculateSize(Context);
                    }
                }
            }

            Context.TrackedObjects.Clear();

            Invalidate();
        }

        private void PageEditView_MouseWheel(object sender, MouseEventArgs e)
        {
            //you can do anything here
            if (e.Delta < 0)
            {
                if (ScrollAreaController != null)
                    ScrollAreaController.ZoomIn();
            }
            else if (e.Delta > 0)
            {
                if (ScrollAreaController != null)
                    ScrollAreaController.ZoomOut();
            }

        }


        private void PageEditView_MouseDown(object sender, MouseEventArgs e)
        {
            //Log("--- MOUSE DOWN EVENT ---\n");
            if (PageData == null)
            {
                Debugger.Log(0,"","Page Data is null\n");
                return;
            }

            Point logPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));

            Context.TrackedObjects.Clear();
            foreach (SMControl sc in PageData.Objects)
            {
                SMRectangleArea area = Page.GetArea(sc.Id);
                area.TrackedSelection = SMControlSelection.None;
            }

            //
            // test if user clicks into object that is already selected
            // if yes, then copy all selected objects into tracked objects
            // function returns true if we hit some boundary line, so in that case
            // we just show properties for that boundary line (constraint)
            // function returns false if we did not hit any constraint
            //
            if (!PreserveSelectionForMove(logPoint))
            {
                if (Context.TrackedObjects.Count == 0)
                {
                    SelectObjectsContainingPoint(logPoint);
                }

                Context.isTracking = (Context.TrackedObjects.Count > 0);
                if (Context.isTracking)
                {
                    MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", Context.TrackedObjects[0]);
                    Context.TrackedStartLogical = logPoint;
                    Context.TrackedDrawOffset = Point.Empty;
                    Invalidate();
                }
                else
                {
                    UserClickOnEmptySpace(logPoint);
                }
            }

        }

        void INotificationTarget.OnNotificationReceived(object sender, string msg, params object[] args)
        {
            if (sender != this)
            {
                switch (msg)
                {
                    case "ObjectSelected":
                        if (args != null && args.Length > 0)
                        {
                            if (args[0] is MNPage)
                            {
                                Page = args[0] as MNPage;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Finds ruler that was hit by current logical point
        /// </summary>
        /// <param name="logPoint">Input point in logical coordinate system</param>
        /// <returns>Ruler that was hit</returns>
        private SMRuler FindBoundaryRuler(Point logPoint)
        {
            foreach (SMControl obj in PageData.Objects)
            {
                SMRectangleArea po = Page.GetArea(obj.Id);
                if (po.Selected)
                {
                    SMControlSelection testResult = po.TestHitLogical(Context, logPoint);
                    switch (testResult & SMControlSelection.All)
                    {
                        case SMControlSelection.None:
                            return null;
                        case SMControlSelection.Left:
                            return po.LeftRuler;
                        case SMControlSelection.Top:
                            return po.TopRuler;
                        case SMControlSelection.Right:
                            return po.RightRuler;
                        case SMControlSelection.Bottom:
                            return po.BottomRuler;
                        default:
                            return null;
                    }
                }
            }

            return null;
        }

        private void UserClickOnEmptySpace(Point logPoint)
        {
            if (logPoint.X > 0 && logPoint.X < Context.PageWidth &&
                logPoint.Y > 0 && logPoint.Y < Context.PageHeight)
            {
                MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", Page);
            }
            else
            {
                MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", Document);
            }
        }

        private void SelectObjectsContainingPoint(Point logPoint)
        {
            PageData.ClearSelection();
            foreach (SMControl po in PageData.SortedObjects)
            {
                SMRectangleArea area = Page.GetArea(po.Id);
                SMControlSelection testResult = area.TestHitLogical(Context, logPoint);
                if (testResult != SMControlSelection.None)
                {
                    area.Selected = true;
                    area.TrackedSelection = SMControlSelection.All;
                    Context.TrackedObjects.Add(po);
                    break;
                }
            }
        }

        /// <summary>
        /// test if user clicks into object that is already selected
        /// if yes, then copy all selected objects into tracked objects
        /// </summary>
        /// <param name="logPoint"></param>
        /// <returns></returns>
        private bool PreserveSelectionForMove(Point logPoint)
        {
            foreach (SMControl po in PageData.SortedObjects)
            {
                SMRectangleArea area = Page.GetArea(po.Id);
                if (area.Selected)
                {
                    SMControlSelection testResult = area.TestHitLogical(Context, logPoint);
                    if (testResult != SMControlSelection.None)
                    {
                        if ((testResult & SMControlSelection.AnyBoundary) != SMControlSelection.None)
                        {
                            MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", area.GetBoundaryRuler(testResult));
                            return true;
                        }

                        foreach (SMControl po2 in PageData.Objects)
                        {
                            SMRectangleArea area2 = Page.GetArea(po2.Id);
                            if (area2.Selected)
                            {
                                // if we hit some boundary tracking anchor
                                // then we do not track other selected items
                                // only that one, which is directly hit
                                area2.TrackedSelection = (testResult == SMControlSelection.All ? SMControlSelection.All : SMControlSelection.None);
                                Context.TrackedObjects.Add(po2);
                            }
                        }
                        // control that is directly hit is tracked according its hitpoint
                        area.TrackedSelection = testResult;
                        break;
                    }
                }
                else
                {
                    SMControlSelection testResult = area.TestHitLogical(Context, logPoint);
                    if (testResult != SMControlSelection.None)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private void PageEditView_MouseMove(object sender, MouseEventArgs e)
        {
            Point logPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
            if (PageData == null) return;

            if (Context.isTracking)
            {
                // these data we need for tracking
                Context.TrackedDrawOffset.X = logPoint.X - Context.TrackedStartLogical.X;
                Context.TrackedDrawOffset.Y = logPoint.Y - Context.TrackedStartLogical.Y;
                Invalidate();
            }

            ChangeCursorAccordingSelection(logPoint);

        }


        private void ChangeCursorAccordingSelection(Point logPoint)
        {
            bool cursorSet = false;
            foreach (SMControl po in Page.Objects)
            {
                SMRectangleArea area = Page.GetArea(po.Id);
                if (area.Selected)
                {
                    SMControlSelection testResult = area.TestHitLogical(Context, logPoint);
                    switch (testResult)
                    {
                        case SMControlSelection.None:
                            break;
                        case SMControlSelection.All:
                            Cursor = Cursors.SizeAll;
                            cursorSet = true;
                            break;
                        case SMControlSelection.Top | SMControlSelection.Left:
                        case SMControlSelection.Bottom | SMControlSelection.Right:
                            Cursor = Cursors.SizeNWSE;
                            cursorSet = true;
                            break;
                        case SMControlSelection.Top:
                        case SMControlSelection.Bottom:
                            Cursor = Cursors.SizeNS;
                            cursorSet = true;
                            break;
                        case SMControlSelection.Top | SMControlSelection.Right:
                        case SMControlSelection.Bottom | SMControlSelection.Left:
                            Cursor = Cursors.SizeNESW;
                            cursorSet = true;
                            break;
                        case SMControlSelection.Left:
                        case SMControlSelection.Right:
                            Cursor = Cursors.SizeWE;
                            cursorSet = true;
                            break;
                        case SMControlSelection.LeftBoundary:
                        case SMControlSelection.TopBoundary:
                        case SMControlSelection.RightBoundary:
                        case SMControlSelection.BottomBoundary:
                            Cursor = Context.cursorBoundaryReady;
                            cursorSet = true;
                            break;
                    }
                }
            }
            if (!cursorSet)
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void insertTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SMLabel pm = new SMLabel(PageData);
            pm.Text = "Text A";
            //pm.AnchorPoint = LastRelativePoint;


            PageData.ClearSelection();
            PageData.Objects.Add(pm);

            Invalidate();

            MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", pm);
        }

        private void insertLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MNLine line = new MNLine(PageData);
            //line.StartPoint.AnchorPoint = LastRelativePoint;
            //line.EndPoint.AnchorPoint = new Point(LastRelativePoint.X + 200, LastRelativePoint.Y + 100);

            PageData.ClearSelection();
            PageData.Objects.Add(line.StartPoint);
            PageData.Objects.Add(line.EndPoint);
            PageData.Objects.Add(line);

            Invalidate();

            MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", line);

        }

        public void OnNotificationReceived(object sender, string msg, params object[] args)
        {
            switch (msg)
            {
                case "DocumentChanged":
                    if (args != null && args.Length > 0 && args[0] is MNDocument)
                    {
                        Document = (MNDocument)args[0];
                    }
                    break;
            }
        }

        public void PageEditView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PageEditDraggableItem)))
            {
                PageEditDraggableItem ctrl = e.Data.GetData(typeof(PageEditDraggableItem)) as PageEditDraggableItem;
                if (ctrl != null && ctrl.Data != null)
                {
                    object obj = PageData.TagToObject(ctrl.Data);
                    if (obj is SMControl)
                    {
                        SMControl pm = (SMControl)obj;
                        pm.Id = Document.GetNextId();

                        // creating coordinates
                        PlaceObjectIntoPage(e, pm);
                    }
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string fileName = files[0];
                    object ao = Document.AcceptFile(fileName);
                    if (ao is MNReferencedImage)
                    {
                        MNReferencedImage ri = ao as MNReferencedImage;
                        SMImage pm = new SMImage(Page);
                        pm.Image = ri;
                        pm.Id = Document.GetNextId();

                        PlaceObjectIntoPage(e, pm);
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(MNReferencedImage)))
            {
                MNReferencedImage ri = (MNReferencedImage)e.Data.GetData(typeof(MNReferencedImage));
                SMImage pm = new SMImage(Page);
                pm.Image = ri;
                pm.Id = Document.GetNextId();

                PlaceObjectIntoPage(e, pm);
            }
            else if (e.Data.GetDataPresent(typeof(MNPage)))
            {
                MNPage template = e.Data.GetData(typeof(MNPage)) as MNPage;
                if (template.IsTemplate)
                    Page.Template = template;
                Invalidate();
            }
        }

        private void PlaceObjectIntoPage(DragEventArgs e, SMControl pm)
        {
            Size defSize = pm.GetDefaultSize();
            Point center = this.PointToClient(new Point(e.X, e.Y));
            SMRectangleArea area = Page.CreateNewArea(pm.Id);
            area.SetCenterSize(Context.PhysicalToLogical(center), defSize, DisplaySize);

            PageData.ClearSelection();
            PageData.Objects.Add(pm);
            MNNotificationCenter.BroadcastMessage(this, "ControlAdded", pm);
            pm.Style = Document.GetDefaultStyle();

            area.Selected = true;
            area.TrackedSelection = SMControlSelection.All;
            Invalidate();

            MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", pm);
        }


        public void PageEditView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PageEditDraggableItem)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(MNReferencedImage)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(MNPage)))
            {
                MNPage tmp = e.Data.GetData(typeof(MNPage)) as MNPage;
                e.Effect = (tmp.IsTemplate ? DragDropEffects.Copy : DragDropEffects.None);
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void PageEditView_DragLeave(object sender, EventArgs e)
        {

        }

        private void PageEditView_DragOver(object sender, DragEventArgs e)
        {

        }


        private void PageEditView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (PageData != null && PageData.HasSelectedObjects())
                {
                    if (MessageBox.Show("Delete selected objects?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PageData.DeleteSelectedObjects();
                        Invalidate();
                    }
                }
            }
        }

        private void PageEditView_KeyUp(object sender, KeyEventArgs e)
        {
        }

    }


    public class PageEditDraggableItem
    {
        public string Text { get; set; }

        public string Data { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public enum PageEditMouseMode
    {
        Normal,
        BoundaryEdit
    }

    public enum PageEditCorner
    {
        TopLeft, BottomLeft,
        TopRight, BottomRight
    }

}
