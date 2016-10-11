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

using SlideMaker.Document;
using SlideMaker.DocumentViews;

namespace SlideMaker
{
    public delegate void PageChangedEventHandler(object sender, PageEditViewArguments e);

    public partial class PageEditView : UserControl, IDocumentDelegate
    {
        public event PageChangedEventHandler PageObjectSelected;

        public event PageChangedEventHandler NewPageRequested;

        public IPageScrollArea ScrollAreaController = null;

        public MNDocument Document { get; set; }

        public Point LastUserPoint { get; set; }

        // this is normalized to page size 1000 x 1000 points
        public Point LastRelativePoint { get; set; }

        private MNPage PageData { get; set; }

        private MNPageContext Context = new MNPageContext();

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
            Matrix a = new Matrix();
            a.Scale(zoom_ratio, zoom_ratio);
            Context.LastMatrix = a;
            a = new Matrix();
            a.Scale(zoom_ratio, zoom_ratio);
            a.Invert();
            Context.LastInvertMatrix = a;
            Size viewSize = view_size;
            this.Size = new Size(Convert.ToInt32(viewSize.Width * zoom_ratio),
                Convert.ToInt32(viewSize.Height * zoom_ratio));
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
                if (PageData != null && PageData.Document != null)
                    ViewSize = PageData.Document.PageSize;
                Invalidate();
            }
        }

        public PageEditView()
        {
            InitializeComponent();
            this.MouseWheel += PageEditView_MouseWheel;
        }

        private void PageEditView_Paint(object sender, PaintEventArgs e)
        {
            if (PageData == null)
                return;

            RecalculateMatrix();
            e.Graphics.Transform = Context.LastMatrix;

            Context.g = e.Graphics;
            Context.zoom = ZoomRatio;
            Context.PageWidth = view_size.Width;
            Context.PageHeight = view_size.Height;
            Context.drawSelectionMarks = true;

            /*Debugger.Log(0, "", "Page Log Width: " + context.lwidth + ", Height: " + context.lheight + "\n");
            Debugger.Log(0, "", "Matrix " + LastMatrix.Elements[0] + "," + LastMatrix.Elements[1]
                 + "," + LastMatrix.Elements[2] + "," + LastMatrix.Elements[3]
                  + "," + LastMatrix.Elements[4] + "," + LastMatrix.Elements[5] + "\n");*/

            // now we should use logical coordinates only
            //e.Graphics.FillRectangle(Brushes.White, 0, 0, Context.PageWidth, Context.PageHeight);
            //e.Graphics.DrawRectangle(Pens.Black, 0, 0, Context.PageWidth, Context.PageHeight);


            PageData.Paint(Context);

        }

        /// <summary>
        /// Returns true if Transformation matrix (LastMatrix) was changed 
        /// and needs to be updated in Graphics context
        /// </summary>
        /// <returns></returns>
        private bool RecalculateMatrix()
        {

            /*
            int lwidth = PageData.Document.PageWidth;
            int lheight = PageData.Document.PageHeight;

            int pwidth = ClientSize.Width;
            int pheight = ClientSize.Height;

            Debugger.Log(0, "", "Client Width: " + pwidth + ", Client Height: " + pheight + "\n");

            if (lwidth == lastlwidth && lheight == lastlheight &&
                pwidth == lastpwidth && pheight == lastpheight)
                return false;

            lastlheight = lheight;
            lastlwidth = lwidth;
            lastpwidth = pwidth;
            lastpheight = pheight;

            Context.PageHeight = lheight;
            Context.PageWidth = lwidth;

            double ratio = (double)lwidth / (double)lheight;

            int tmp = Convert.ToInt32(ratio * pheight);
            if (tmp < pwidth)
            {
                // width calculated from ratio and physical height is lower than
                // actual physical width, what means page will be displayed completely
                // so we use this
                double ratioPhysToLog = (double)pheight / lheight;
                int totalLogWidth = Convert.ToInt32(pwidth / ratioPhysToLog);

                Rectangle presentedRect = new Rectangle(-(totalLogWidth - lwidth) / 2, 0, totalLogWidth, lheight);
                Point[] pls = new Point[3];
                pls[0] = new Point(0, 0);
                pls[1] = new Point(pwidth - 1, 0);
                pls[2] = new Point(0, pheight - 1);
                Context.LastMatrix = new Matrix(presentedRect, pls);
                Context.LastInvertMatrix = new Matrix(presentedRect, pls);
                Context.LastInvertMatrix.Invert();
            }
            else
            {
                double ratioPhysToLog = (double)pwidth / lwidth;
                int totalLogHeight = Convert.ToInt32(pheight / ratioPhysToLog);

                Rectangle presentedRect = new Rectangle(0, -(totalLogHeight - lheight) / 2, lwidth, totalLogHeight);
                Point[] pls = new Point[3];
                pls[0] = new Point(0, 0);
                pls[1] = new Point(pwidth - 1, 0);
                pls[2] = new Point(0, pheight - 1);
                Context.LastMatrix = new Matrix(presentedRect, pls);
                Context.LastInvertMatrix = new Matrix(presentedRect, pls);
                Context.LastInvertMatrix.Invert();
            }
            */
            return true;
        }

        private void setPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureSettings dlg = new PictureSettings();
            dlg.Document = Document;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.HasSelectedImage)
                {
                    MNPageImage mpi = new MNPageImage(PageData);
                    mpi.Image = dlg.Image;
                    //mpi.AnchorPoint = LastRelativePoint;
                    mpi.Size = mpi.Image.ImageData.Size;
                    mpi.Selected = SMControlSelection.All;

                    PageData.ClearSelection();
                    PageData.Objects.Add(mpi);

                    Invalidate();

                    PageEditViewArguments args = new PageEditViewArguments();
                    args.PageView = this;
                    args.Page = PageData;
                    args.Document = Document;
                    args.Object = mpi;

                    if (PageObjectSelected != null)
                    {
                        PageObjectSelected.Invoke(this, args);
                    }

                }
            }
        }

        private void insertMantraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Document.Mantras.Count == 0)
                return;

            InsertMantraDialog dlg = new InsertMantraDialog();
            dlg.SetMantras(Document.Mantras);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                MNReferencedMantra selected = dlg.SelectedItem;
                if (selected != null)
                {
                    MNPageMantra pm = new MNPageMantra(PageData);
                    pm.Mantra.Mantra = selected;
                    pm.Mantra.Text = selected.Number + ". " + selected.MantraText;
                    pm.Mantra.ImageCode = 0;
                    //pm.Mantra.AnchorPoint = LastRelativePoint;

                    pm.HotSpot.Radius = 30;
                    //pm.HotSpot.AnchorPoint = new Point(LastRelativePoint.X + 200, LastRelativePoint.Y + 100);

                    pm.Mantra.Selected = SMControlSelection.All;

                    PageData.ClearSelection();
                    PageData.Objects.Add(pm);
                    PageData.Objects.Add(pm.Mantra);
                    PageData.Objects.Add(pm.HotSpot);
                    //PageData.Objects.Add(pm);

                    Invalidate();

                    PageEditViewArguments args = new PageEditViewArguments();
                    args.PageView = this;
                    args.Page = PageData;
                    args.Document = Document;
                    args.Object = pm.Mantra;

                    if (PageObjectSelected != null)
                    {
                        PageObjectSelected.Invoke(this, args);
                    }

                }
            }
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
            PagePropertiesDialog dlg = new PagePropertiesDialog();

/*            dlg.PageOrientation = PageData.Document.PageOrientation;
            dlg.PageSize = PageData.Document.PageSize;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                PageData.Document.PageOrientation = dlg.PageOrientation;
                PageData.Document.PageSize = dlg.PageSize;

                Invalidate();
            }*/
        }

        private void PageEditView_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void PageEditView_MouseUp(object sender, MouseEventArgs e)
        {
            Context.isTracking = false;

            if (Context.TrackedObjects.Count > 0)
            {
                foreach (SMControl po in Context.TrackedObjects)
                {
                    if (po.TrackedSelection != SMControlSelection.None)
                        po.Move(Context, Context.TrackedDrawOffset);
                }
                Context.TrackedObjects.Clear();
            }

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
            if (PageData == null)
                return;

            Point logPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));

            Context.TrackedObjects.Clear();

            //
            // test if user clicks into object that is already selected
            // if yes, then copy all selected objects into tracked objects
            //
            foreach (SMControl po in PageData.Objects)
            {
                if (po.Selected != SMControlSelection.None)
                {
                    SMControlSelection testResult = po.TestHitLogical(Context, logPoint);
                    if (testResult != SMControlSelection.None)
                    {
                        foreach (SMControl po2 in PageData.Objects)
                        {
                            if (po.Selected != SMControlSelection.None)
                            {
                                // if we hit some boundary tracking anchor
                                // then we do not track other selected items
                                // only that one, which is directly hit
                                po2.TrackedSelection = (testResult == SMControlSelection.All ? SMControlSelection.All : SMControlSelection.None);
                                Context.TrackedObjects.Add(po2);
                            }
                        }
                        // control that is directly hit is tracked according its hitpoint
                        po.TrackedSelection = testResult;
                        break;
                    }
                }
                else
                {
                    // unselected items are not tracked at all
                    po.TrackedSelection = SMControlSelection.None;
                }
            }


            if (Context.TrackedObjects.Count == 0)
            {
                PageData.ClearSelection();
                foreach (SMControl po in PageData.Objects)
                {
                    SMControlSelection testResult = po.TestHitLogical(Context, logPoint);
                    if (testResult != SMControlSelection.None)
                    {
                        po.Selected = SMControlSelection.All;
                        po.TrackedSelection = SMControlSelection.All;
                        Context.TrackedObjects.Add(po);
                        break;
                    }
                }
            }

            if (Context.TrackedObjects.Count > 0)
            {
                Context.isTracking = true;
                if (PageObjectSelected != null)
                {
                    PageEditViewArguments args = new PageEditViewArguments();
                    args.Document = Document;
                    args.Page = PageData;
                    args.Object = Context.TrackedObjects[0];
                    args.PageView = this;
                    PageObjectSelected.Invoke(this, args);
                }
                Context.TrackedStartLogical = logPoint;
                Context.TrackedDrawOffset = Point.Empty;
                Invalidate();
            }
            else
            {
                Context.isTracking = false;
                if (logPoint.X > 0 && logPoint.X < Context.PageWidth &&
                    logPoint.Y > 0 && logPoint.Y < Context.PageHeight)
                {
                    if (PageObjectSelected != null)
                    {
                        PageEditViewArguments args = new PageEditViewArguments();
                        args.Document = Document;
                        args.Page = PageData;
                        args.PageView = this;
                        PageObjectSelected(this, args);
                    }
                }
                else
                {
                    if (PageObjectSelected != null)
                    {
                        PageEditViewArguments args = new PageEditViewArguments();
                        args.Document = Document;
                        args.PageView = this;
                        PageObjectSelected(this, args);
                    }
                }
            }
        }

        private void PageEditView_MouseMove(object sender, MouseEventArgs e)
        {
            Point logPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));

            if (Context.isTracking)
            {
                // these data we need for tracking
                Context.TrackedDrawOffset.X = logPoint.X - Context.TrackedStartLogical.X;
                Context.TrackedDrawOffset.Y = logPoint.Y - Context.TrackedStartLogical.Y;
            }

            if (PageData != null)
            {
                logPoint = ChangeCursorAccordingSelection(logPoint);
            }

            Invalidate();
        }

        private Point ChangeCursorAccordingSelection(Point logPoint)
        {
            bool cursorSet = false;
            foreach (SMControl po in PageData.Objects)
            {
                if (po.Selected == SMControlSelection.All)
                {
                    SMControlSelection testResult = po.TestHitLogical(Context, logPoint);
                    switch (testResult)
                    {
                        case SMControlSelection.None:
                            break;
                        case SMControlSelection.All:
                            Cursor = Cursors.SizeAll;
                            cursorSet = true;
                            break;
                        case SMControlSelection.TopLeft:
                        case SMControlSelection.BottomRight:
                            Cursor = Cursors.SizeNWSE;
                            cursorSet = true;
                            break;
                        case SMControlSelection.TopCenter:
                        case SMControlSelection.BottomCenter:
                            Cursor = Cursors.SizeNS;
                            cursorSet = true;
                            break;
                        case SMControlSelection.TopRight:
                        case SMControlSelection.BottomLeft:
                            Cursor = Cursors.SizeNESW;
                            cursorSet = true;
                            break;
                        case SMControlSelection.CenterLeft:
                        case SMControlSelection.CenterRight:
                            Cursor = Cursors.SizeWE;
                            cursorSet = true;
                            break;
                    }
                }
            }
            if (!cursorSet)
            {
                this.Cursor = Cursors.Default;
            }
            return logPoint;
        }

        private void insertTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SMLabel pm = new SMLabel(PageData);
            pm.Text = "Text A";
            //pm.AnchorPoint = LastRelativePoint;


            PageData.ClearSelection();
            PageData.Objects.Add(pm);

            Invalidate();

            PageEditViewArguments args = new PageEditViewArguments();
            args.PageView = this;
            args.Page = PageData;
            args.Document = Document;
            args.Object = pm;

            if (PageObjectSelected != null)
            {
                PageObjectSelected(this, args);
            }

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

            PageEditViewArguments args = new PageEditViewArguments();
            args.PageView = this;
            args.Page = PageData;
            args.Document = Document;
            args.Object = line;

            if (PageObjectSelected != null)
            {
                PageObjectSelected.Invoke(this, args);
            }

        }

        public void documentHasChanged(MNDocument doc)
        {
            this.Document = doc;
        }

        public void PageEditView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PageEditDraggableItem)))
            {
                PageEditDraggableItem ctrl = e.Data.GetData(typeof(PageEditDraggableItem)) as PageEditDraggableItem;
                if (ctrl != null && ctrl.Data != null && ctrl.Data is SMControl)
                {
                    SMControl pm = ((SMControl)ctrl.Data).CreateCopy();
                    pm.Page = this.PageData;
                    Size defSize = pm.GetDefaultSize();
                    Point center = this.PointToClient(new Point(e.X, e.Y));
                    pm.SetCenterSize(Context.PointToRelative(center), Context.SizeToRelative(defSize));

                    PageData.ClearSelection();
                    PageData.Objects.Add(pm);

                    Invalidate();

                    PageEditViewArguments args = new PageEditViewArguments();
                    args.PageView = this;
                    args.Page = PageData;
                    args.Document = Document;
                    args.Object = pm;

                    if (PageObjectSelected != null)
                    {
                        PageObjectSelected(this, args);
                    }
                }
            }
        }

        public void PageEditView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PageEditDraggableItem)))
            {
                e.Effect = DragDropEffects.Copy;
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

    }


    public class PageEditDraggableItem
    {
        public string Text { get; set; }

        public object Data { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
