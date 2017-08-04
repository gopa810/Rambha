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
using Rambha.Script;
using Rambha.Serializer;

namespace SlideMaker.Views
{
    public delegate void NormalEventHandler(object sender, EventArgs e);

    public delegate void PageChangedEventHandler(object sender, PageEditViewArguments e);

    public partial class PageEditView : UserControl, INotificationTarget
    {
        public event PageChangedEventHandler NewPageRequested;

        public IPageScrollArea ScrollAreaController = null;

        public MNDocument Document { get { return PageData != null ? PageData.Document : null; } }

        public Point LastUserPoint { get; set; }

        public SMScreen DisplaySize { get; set; }

        // this is normalized to page size 1000 x 1000 points
        public Point LastRelativePoint { get; set; }

        private MNPage PageData { get; set; }

        public MNPageContext Context = new MNPageContext();

        public SMRectangleArea SelectionArea = new SMRectangleArea();

        public KeyPageActions pageActions = null;

        private float zoom_ratio = 1f;

        private Size view_size = new Size(1024, 768);

        private bool b_key_control = false;

        public int AverageValueSelection = 0;
        private Pen markRectPen;

        private Font p_bigFont = null;

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
                if (PageData != null)
                    PageData.ClearSelection();
                SelectionArea.Clear();
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
            Context.navigIconBack = Properties.Resources.navigIconBack;
            Context.navigIconFwd = Properties.Resources.navigIconFwd;
            Context.navigIconHelp = Properties.Resources.navigIconHelp;
            Context.navigIconMenu = Properties.Resources.navigIconMenu;
            Context.navigArrowBack = Properties.Resources.navigArrowLeft;
            Context.navigArrowFwd = Properties.Resources.navigArrowRight;
            Context.navigSpeakerOn = Properties.Resources.SpeakerOn;
            Context.navigSpeakerOff = Properties.Resources.SpeakerOff;

            MNNotificationCenter.AddReceiver(this, "ObjectSelected");

            this.MouseWheel += PageEditView_MouseWheel;

            markRectPen = SMGraphics.GetPen(Color.Gray, 2);

            p_bigFont = new Font(FontFamily.GenericSansSerif, 40);
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

            CheckLabelBackgrounds();

            PageData.PaintBackground(Context);
            PageData.Paint(Context, false);
            PageData.Paint(Context, true);

            if (HasSelectedObjects)
            {
                SelectionArea.PaintSelectionMarks(Context);
            }

            if (Context.isMarkingArea)
            {
                e.Graphics.DrawRectangle(markRectPen, Context.GetMarkingRect());
            }
            else if (Context.isMovingTag)
            {
                SizeF sz = e.Graphics.MeasureString(p_moved_tag, p_bigFont);
                Rectangle r = new Rectangle(Context.lastClientPoint.X - (int)(sz.Width / 2), Context.lastClientPoint.Y - (int)(sz.Height / 2),
                    (int)sz.Width + 2, (int)sz.Height + 2);
                e.Graphics.FillRectangle(Brushes.Pink, r);
                e.Graphics.DrawString(p_moved_tag, p_bigFont, Brushes.DarkViolet, r);
            }

            if (PageData.ShowMessageAlways)
            {
                Context.PaintMessageBox(false);
            }
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
            if (pageActions.IsKeyActionMode) pageActions.StopKeyActionMode();

            if (e.Button == System.Windows.Forms.MouseButtons.Right && PageData != null)
            {
                /*Point clientPoint = new Point(e.X, e.Y);
                LastUserPoint = PointToScreen(clientPoint);
                LastRelativePoint = Context.PhysicalToRelative(clientPoint);
                contextMenuStrip1.Show(LastUserPoint);*/
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
            if (pageActions.IsKeyActionMode) pageActions.StopKeyActionMode();

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
            }
            else
            {
                if (Context.isMarkingArea)
                {
                    RefreshOriginalAreas();
                }

                Context.isMarkingArea = false;
                Context.trackingType = SMControlSelection.None;
                Point offset = Context.TrackedDrawOffset;
                Context.TrackedDrawOffset = Point.Empty;

                if (Context.isMovingTag)
                {
                    Point logPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
                    SMControl c = PageData.FindObjectContainingPoint(Context, logPoint);
                    if (c != null)
                    {
                        c.Tag = p_moved_tag;
                    }
                }
                else
                {
                }

            }

            Context.isMovingTag = false;


            Invalidate();
        }

        private void RefreshOriginalAreas()
        {
            origTotalSelectionRect = SelectionArea.RelativeArea;
            origAreas.Clear();
            foreach (SMControl c in PageData.SelectedObjects)
            {
                origAreas[c.Id] = c.Area.RelativeArea;
            }
        }

        private Rectangle origTotalSelectionRect = Rectangle.Empty;
        private Dictionary<long, Rectangle> origAreas = new Dictionary<long, Rectangle>();

        private void PageEditView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pageActions.IsKeyActionMode) pageActions.StopKeyActionMode();

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

        private string p_moved_tag = "";

        private void PageEditView_MouseDown(object sender, MouseEventArgs e)
        {
            if (pageActions.IsKeyActionMode) pageActions.StopKeyActionMode();

            Context.startClientPoint.X = e.X;
            Context.startClientPoint.Y = e.Y;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                LastUserPoint = Context.startClientPoint;
                pasteToolStripMenuItem.Enabled = (PageData.Objects.Count > 0);
                contextMenuStrip2.Show(PointToScreen(LastUserPoint));
            }
            else
            {
                //Log("--- MOUSE DOWN EVENT ---\n");
                if (PageData == null)
                {
                    Debugger.Log(0, "", "Page Data is null\n");
                    return;
                }

                Point logPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
                Context.startClientPoint = logPoint;

                if (b_key_movetag)
                {
                    SMControl obj = PageData.FindObjectContainingPoint(Context, logPoint);
                    if (obj != null)
                    {
                        p_moved_tag = obj.SafeTag;
                        Context.isMovingTag = true;
                    }
                }
                else
                {
                    //
                    // test if user clicks into object that is already selected
                    // if yes, then copy all selected objects into tracked objects
                    // function returns true if we hit some boundary line, so in that case
                    // we just show properties for that boundary line (constraint)
                    // function returns false if we did not hit any constraint
                    //
                    if (!PreserveSelectionForMove(logPoint))
                    {
                        if (!b_key_control)
                            PageData.ClearSelection();
                        SelectObjectsContainingPoint(Context, logPoint);
                    }

                    if (Context.isTracking && PageData.SelectedObjects.Count > 0)
                    {
                        MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", PageData.SelectedObjects[0]);
                        Context.TrackedStartLogical = logPoint;
                        Context.TrackedDrawOffset = Point.Empty;
                        RefreshOriginalAreas();
                        Invalidate();
                    }
                    else
                    {
                        UserClickOnEmptySpace(logPoint);
                        origAreas.Clear();
                    }
                }
            }
        }

        public void SelectObjectsContainingPoint(MNPageContext Context, Point logPoint)
        {
            SelectionArea.RelativeArea = Rectangle.Empty;
            SelectionArea.Dock = SMControlSelection.None;
            for (int i = PageData.Objects.Count - 1; i >= 0; i--)
            {
                SMControl po = PageData.Objects[i];
                if (po.Area.TestHitLogical(Context, logPoint))
                {
                    po.Area.Selected = !po.Area.Selected;

                    if (po.Area.Selected)
                    {
                        MNPage.MergeRectangles(ref SelectionArea.RelativeArea, po.Area.RelativeArea);
                        if (SelectionArea.Dock == SMControlSelection.None)
                        {
                            SelectionArea.Dock = po.Area.Dock;
                        }
                        else if (SelectionArea.Dock != po.Area.Dock)
                        {
                            SelectionArea.Dock = SMControlSelection.All;
                        }
                        Context.trackingType = SMControlSelection.All;
                        break;
                    }
                }
            }
        }

        public void SelectObjectsIntersectingRect(MNPageContext Context, Rectangle r)
        {
            SelectionArea.RelativeArea = Rectangle.Empty;
            SelectionArea.Dock = SMControlSelection.None;

            foreach (SMControl po in PageData.Objects)
            {
                SMRectangleArea area = po.Area;
                SMControlSelection testResult = area.TestHitLogical(Context, r);
                if (testResult != SMControlSelection.None)
                {
                    area.Selected = true;
                    MNPage.MergeRectangles(ref SelectionArea.RelativeArea, area.RelativeArea);
                    if (SelectionArea.Dock == SMControlSelection.None)
                    {
                        SelectionArea.Dock = po.Area.Dock;
                    }
                    else if (SelectionArea.Dock != po.Area.Dock)
                    {
                        SelectionArea.Dock = SMControlSelection.All;
                    }
                    //Context.trackingType = SMControlSelection.All;
                }
            }

            //Debugger.Log(0, "", "S.Area.Dock = " + SelectionArea.Dock.ToString() + "\n");
            //Debugger.Log(0, "", "S.Area.Rect = " + SelectionArea.RelativeArea.ToString() + "\n\n");
        }

        private bool HasSelectedObjects
        {
            get
            {
                return !SelectionArea.RelativeArea.IsEmpty;
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
                                MNNotificationCenter.CurrentPage = Page;
                            }
                            else if (args[0] is SMControl)
                            {
                                SMControl selectedControl = args[0] as SMControl;
                                MNPage selectedPage = selectedControl.Page;
                                SMRectangleArea area = selectedControl.Area;
                                if (Page != selectedPage)
                                    Page = selectedPage;
                                selectedPage.ClearSelection();
                                if (area != null) area.Selected = true;
                                MNNotificationCenter.CurrentPage = Page;
                                Invalidate();
                            }
                        }
                        break;
                    case "NewPageInserted":
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


        private void UserClickOnEmptySpace(Point logPoint)
        {
            if (logPoint.X > 0 && logPoint.X < Context.PageWidth &&
                logPoint.Y > 0 && logPoint.Y < Context.PageHeight)
            {
                MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", Page);
                Context.isMarkingArea = true;
            }
            else
            {
                MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", Document);
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
            if (!SelectionArea.RelativeArea.IsEmpty)
            {
                Context.trackingType = AreaTestHitLogical(Context, SelectionArea, logPoint);
            }
            else
            {
                Context.trackingType = SMControlSelection.None;
            }

            //Debugger.Log(0, "", "Preserve Selection result: " + Context.trackingType + "\n");

            return Context.isTracking;
        }



        private void PageEditView_MouseMove(object sender, MouseEventArgs e)
        {
            //if (b_is_key_action_mode) StopKeyActionMode();

            Context.lastClientPoint.X = e.X;
            Context.lastClientPoint.Y = e.Y;

            Point logPoint = Context.PhysicalToLogical(Context.lastClientPoint);
            Context.lastClientPoint = logPoint;

            if (PageData == null) return;

            if (Context.isTracking)
            {
                // these data we need for tracking
                int relX = logPoint.X - Context.TrackedStartLogical.X;
                int relY = logPoint.Y - Context.TrackedStartLogical.Y;

                switch (Context.trackingType)
                {
                    case SMControlSelection.All:
                        SelectionArea.RelativeArea.X = origTotalSelectionRect.X + relX;
                        SelectionArea.RelativeArea.Y = origTotalSelectionRect.Y + relY;
                        SelectionArea.RelativeArea.Width = origTotalSelectionRect.Width;
                        SelectionArea.RelativeArea.Height = origTotalSelectionRect.Height;
                        break;
                    case SMControlSelection.Bottom | SMControlSelection.Left:
                        SelectionArea.RelativeArea.X = origTotalSelectionRect.X + relX;
                        SelectionArea.RelativeArea.Y = origTotalSelectionRect.Y;
                        SelectionArea.RelativeArea.Width = origTotalSelectionRect.Width - relX;
                        SelectionArea.RelativeArea.Height = origTotalSelectionRect.Height + relY;
                        break;
                    case SMControlSelection.Bottom | SMControlSelection.Right:
                        SelectionArea.RelativeArea.X = origTotalSelectionRect.X;
                        SelectionArea.RelativeArea.Y = origTotalSelectionRect.Y;
                        SelectionArea.RelativeArea.Width = origTotalSelectionRect.Width + relX;
                        SelectionArea.RelativeArea.Height = origTotalSelectionRect.Height + relY;
                        break;
                    case SMControlSelection.Top | SMControlSelection.Left:
                        SelectionArea.RelativeArea.X = origTotalSelectionRect.X + relX;
                        SelectionArea.RelativeArea.Y = origTotalSelectionRect.Y + relY;
                        SelectionArea.RelativeArea.Width = origTotalSelectionRect.Width - relX;
                        SelectionArea.RelativeArea.Height = origTotalSelectionRect.Height - relY;
                        break;
                    case SMControlSelection.Top | SMControlSelection.Right:
                        SelectionArea.RelativeArea.X = origTotalSelectionRect.X;
                        SelectionArea.RelativeArea.Y = origTotalSelectionRect.Y + relY;
                        SelectionArea.RelativeArea.Height = origTotalSelectionRect.Height - relY;
                        SelectionArea.RelativeArea.Width = origTotalSelectionRect.Width + relX;
                        break;
                    case SMControlSelection.Right:
                        SelectionArea.RelativeArea.X = origTotalSelectionRect.X;
                        SelectionArea.RelativeArea.Y = origTotalSelectionRect.Y;
                        SelectionArea.RelativeArea.Width = origTotalSelectionRect.Width + relX;
                        SelectionArea.RelativeArea.Height = origTotalSelectionRect.Height;
                        break;
                    case SMControlSelection.Left:
                        SelectionArea.RelativeArea.X = origTotalSelectionRect.X + relX;
                        SelectionArea.RelativeArea.Y = origTotalSelectionRect.Y;
                        SelectionArea.RelativeArea.Width = origTotalSelectionRect.Width - relX;
                        SelectionArea.RelativeArea.Height = origTotalSelectionRect.Height;
                        break;
                    case SMControlSelection.Bottom:
                        SelectionArea.RelativeArea.X = origTotalSelectionRect.X;
                        SelectionArea.RelativeArea.Y = origTotalSelectionRect.Y;
                        SelectionArea.RelativeArea.Width = origTotalSelectionRect.Width;
                        SelectionArea.RelativeArea.Height = origTotalSelectionRect.Height + relY;
                        break;
                    case SMControlSelection.Top:
                        SelectionArea.RelativeArea.X = origTotalSelectionRect.X;
                        SelectionArea.RelativeArea.Y = origTotalSelectionRect.Y + relY;
                        SelectionArea.RelativeArea.Width = origTotalSelectionRect.Width;
                        SelectionArea.RelativeArea.Height = origTotalSelectionRect.Height - relY;
                        break;
                }
                RecalculateSelectedControls();

                //Context.TrackedDrawOffset.X = logPoint.X - Context.TrackedStartLogical.X;
                //Context.TrackedDrawOffset.Y = logPoint.Y - Context.TrackedStartLogical.Y;
                Invalidate();
                ChangeCursorAccordingSelection(logPoint);
            }
            else if (Context.isMarkingArea)
            {
                ClearSelection();
                SelectObjectsIntersectingRect(Context, Context.GetMarkingRect());
                Invalidate();
            }
            else if (Context.isMovingTag)
            {
                Invalidate();
            }
            else
            {
                ChangeCursorAccordingSelection(logPoint);
            }

        }

        private void RecalculateSelectedControls()
        {
            Rectangle abst = Rectangle.Empty;
            foreach (SMControl c in PageData.SelectedObjects)
            {
                // if resizing, then recalculate internal layout
                if (Context.trackingType != SMControlSelection.All)
                    c.TextDidChange();

                SMRectangleArea area = c.Area;
                if (origAreas.ContainsKey(c.Id))
                {
                    Rectangle relativeRect = origAreas[c.Id];
                    ConvertRelativeToAbstract(ref origTotalSelectionRect, ref relativeRect, ref abst);
                    ConvertAbstractToRelative(ref SelectionArea.RelativeArea, ref abst, ref relativeRect);
                    area.SetRawRectangle(PageEditDisplaySize.LandscapeBig, relativeRect);
                    if (area.Dock != SMControlSelection.None)
                        area.DockModified = true;
                }
            }
        }

        private void ConvertRelativeToAbstract(ref Rectangle total, ref Rectangle relativeArea, ref Rectangle abstractArea)
        {
            abstractArea.X = (relativeArea.X - total.X) * 1000 / total.Width;
            abstractArea.Y = (relativeArea.Y - total.Y) * 1000 / total.Height;
            abstractArea.Width = relativeArea.Width * 1000 / total.Width;
            abstractArea.Height = relativeArea.Height * 1000 / total.Height;
        }

        private void ConvertAbstractToRelative(ref Rectangle total, ref Rectangle abstractArea, ref Rectangle relativeArea)
        {
            relativeArea.X = abstractArea.X * total.Width / 1000 + total.X;
            relativeArea.Y = abstractArea.Y * total.Height / 1000 + total.Y;
            relativeArea.Width = abstractArea.Width * total.Width / 1000;
            relativeArea.Height = abstractArea.Height * total.Height / 1000;
        }

        public bool HitDist(int a, int b)
        {
            return Math.Abs(a - b) <= 3;
        }

        public virtual SMControlSelection AreaTestHitLogical(MNPageContext context, SMRectangleArea area, Point logicalPoint)
        {
            if (HitDist(area.RelativeArea.Left, logicalPoint.X))
            {
                if (HitDist(area.RelativeArea.Top, logicalPoint.Y))
                {
                    return SMControlSelection.Top | SMControlSelection.Left;
                }
                else if (HitDist(area.RelativeArea.Bottom, logicalPoint.Y))
                {
                    return SMControlSelection.Bottom | SMControlSelection.Left;
                }
                else if (HitDist(area.CenterY, logicalPoint.Y))
                {
                    return SMControlSelection.Left;
                }
            }
            else if (HitDist(area.RelativeArea.Right, logicalPoint.X))
            {
                if (HitDist(area.RelativeArea.Top, logicalPoint.Y))
                {
                    return SMControlSelection.Top | SMControlSelection.Right;
                }
                else if (HitDist(area.RelativeArea.Bottom, logicalPoint.Y))
                {
                    return SMControlSelection.Bottom | SMControlSelection.Right;
                }
                else if (HitDist(area.CenterY, logicalPoint.Y))
                {
                    return SMControlSelection.Right;
                }
            }
            else if (HitDist(area.CenterX, logicalPoint.X))
            {
                if (HitDist(area.RelativeArea.Top, logicalPoint.Y))
                {
                    return SMControlSelection.Top;
                }
                else if (HitDist(area.RelativeArea.Bottom, logicalPoint.Y))
                {
                    return SMControlSelection.Bottom;
                }
            }

            return area.RelativeArea.Contains(logicalPoint) ? SMControlSelection.All : SMControlSelection.None;
        }

        private void ChangeCursorAccordingSelection(Point logPoint)
        {
            bool cursorSet = false;
            if (!HasSelectedObjects)
                return;
            SMRectangleArea area = SelectionArea;
            area.Selected = true;
            SMControlSelection testResult = AreaTestHitLogical(Context, SelectionArea, logPoint);
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
        }

        public void OnNotificationReceived(object sender, string msg, params object[] args)
        {
            switch (msg)
            {
                case "DocumentChanged": break;
            }
        }

        public void PageEditView_DragDrop(object sender, DragEventArgs e)
        {
            if (Page.CurrentScreenDimension != SMScreen.Screen_1024_768__4_3)
                return;

            if (e.Data.GetDataPresent(typeof(PageEditDraggableItem)))
            {
                PageEditDraggableItem ctrl = e.Data.GetData(typeof(PageEditDraggableItem)) as PageEditDraggableItem;
                if (ctrl != null && ctrl.Data != null)
                {
                    object obj = PageData.TagToObject(ctrl.Data);

                    if (obj is SMControl)
                    {
                        SMControl pm = (SMControl)obj;
                        pm.Id = Document.Data.GetNextId();

                        // creating coordinates
                        PlaceObjectIntoPage(this.PointToClient(new Point(e.X, e.Y)), pm, ctrl.DefaultSize);

                        // aplying special commands
                        if (ctrl.Args.Length > 0)
                        {
                            string[] args = ctrl.Args.Split(';');
                            foreach (string a in args)
                            {
                                string[] pa = a.Split('=');
                                if (pa.Length == 2)
                                {
                                    pm.ExecuteMessage("set", new GSString(pa[0]), new GSString(pa[1]));
                                }
                            }
                        }

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
                        SMControl fc = Page.FindObjectContainingPoint(Context, Context.PhysicalToLogical(PointToClient(new Point(e.X, e.Y))));
                        if (fc != null)
                        {
                            if (fc is SMImage)
                            {
                                (fc as SMImage).Img.Image = ri;
                            }
                            else if (fc is SMOrderedList)
                            {
                                (fc as SMOrderedList).AddImage(ri);
                            }
                            Invalidate();
                        }
                        else
                        {
                            SMImage pm = new SMImage(Page);
                            pm.Img.Image = ri;
                            pm.Id = Document.Data.GetNextId();

                            PlaceObjectIntoPage(this.PointToClient(new Point(e.X, e.Y)), pm, Size.Empty);
                        }
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(MNReferencedImage)))
            {
                MNReferencedImage ri = (MNReferencedImage)e.Data.GetData(typeof(MNReferencedImage));

                SMControl fc = Page.FindObjectContainingPoint(Context, Context.PhysicalToLogical(new Point(e.X, e.Y)));
                if (fc != null)
                {
                    if (fc is SMImage)
                    {
                        (fc as SMImage).Img.Image = ri;
                    }
                    else if (fc is SMOrderedList)
                    {
                        (fc as SMOrderedList).AddImage(ri);
                    }
                    Invalidate();
                }
                else
                {
                    SMImage pm = new SMImage(Page);
                    pm.Img.Image = ri;
                    pm.Id = Document.Data.GetNextId();

                    PlaceObjectIntoPage(this.PointToClient(new Point(e.X, e.Y)), pm, Size.Empty);
                }
            }
            else if (e.Data.GetDataPresent(typeof(MNPage)))
            {
                MNPage template = e.Data.GetData(typeof(MNPage)) as MNPage;
                if (template.IsTemplate)
                    Page.Template = template;
                Invalidate();
            }
            else if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {

            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                string strType = "Label";
                string text = (string)e.Data.GetData(DataFormats.UnicodeText);
                text = text.Trim();
                if (text.StartsWith("(@"))
                {
                    int i = text.IndexOf(")");
                    if (i > 0)
                    {
                        strType = text.Substring(2, i - 2);
                        text = text.Substring(i + 1).Trim();
                    }
                }

                SMControl fc = Page.FindObjectContainingPoint(Context, Context.PhysicalToLogical(new Point(e.X, e.Y)));
                if (fc != null)
                {
                    if (fc is SMOrderedList)
                    {
                        (fc as SMOrderedList).AddText(text);
                    }
                    else
                    {
                        fc.Text = text;
                    }
                    Invalidate();
                }
                else
                {
                    object obj = PageData.TagToObject(strType) ?? PageData.TagToObject("Label");
                    if (obj is SMControl)
                    {
                        SMControl pm = (SMControl)obj;
                        pm.Id = Document.Data.GetNextId();
                        pm.Text = text;

                        // creating coordinates
                        PlaceObjectIntoPage(this.PointToClient(new Point(e.X, e.Y)), pm, Size.Empty);
                    }
                }
            }
        }

        private void PlaceObjectIntoPage(Point clientPoint, SMControl pm, Size defSizeInput)
        {
            Size defSize = defSizeInput.IsEmpty ? pm.GetDefaultSize() : defSizeInput;
            Point center = clientPoint;
            pm.Area.SetCenterSize(Context.PhysicalToLogical(center), defSize);
            pm.Area.Selected = true;

            PageData.ClearSelection();
            PageData.Objects.Add(pm);
            MNNotificationCenter.BroadcastMessage(this, "ControlAdded", pm);

            SelectionArea.RelativeArea = pm.Area.RelativeArea;
            SelectionArea.Dock = pm.Area.Dock;
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
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
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

        private bool b_key_movetag = false;

        private void PageEditView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                b_key_control = true;
            }

            if (pageActions.IsKeyActionMode)
            {
                if (pageActions != null)
                {
                    pageActions.KeyActionMode(e.KeyCode);
                }
            }
            else
            {
                if (e.KeyCode == Keys.T)
                {
                    b_key_movetag = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (PageData != null && PageData.HasSelectedObjects())
                    {
                        if (MessageBox.Show("Delete selected objects?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            PageData.DeleteSelectedObjects();
                            SelectionArea.Clear();
                            Invalidate();
                        }
                    }
                }
                else if (e.Control && e.KeyCode == Keys.C)
                {
                    CopySelectedObjectsToClipboard();
                }
                else if (e.Control && e.KeyCode == Keys.V)
                {
                    PasteSelectedObjectsFromClipboard();
                    Invalidate();
                }
                else if (e.KeyCode == Keys.Space)
                {
                    pageActions.StartKeyActionMode();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    pageActions.StopKeyActionMode();
                }
            }
        }


        public void StopKeyActionMode()
        {
            MNNotificationCenter.BroadcastMessage(this, "stopKeyActionMode");
        }

        private void PasteSelectedObjectsFromClipboard()
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                if (text.StartsWith("<PAGEPIECE>") && text.EndsWith("</PAGEPIECE>"))
                {
                    string textData = text.Substring(11, text.Length - 23);
                    byte[] data = Convert.FromBase64String(textData);

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            RSFileReader reader = new RSFileReader(br);
                            PageData.PasteSelection(reader);
                        }
                    }
                }
            }
        }

        private void CopySelectedObjectsToClipboard()
        {
            if (PageData.GetSelectedCount() > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(ms))
                    {
                        RSFileWriter bw = new RSFileWriter(writer);

                        PageData.SaveSelection(bw);

                        string text64 = Convert.ToBase64String(ms.GetBuffer());
                        try
                        {
                            Clipboard.Clear();
                            Clipboard.SetText(string.Format("<PAGEPIECE>{0}</PAGEPIECE>", text64));
                        }
                        catch (Exception ex)
                        {
                            Debugger.Log(0, "", ex.ToString() + "\n");
                            Debugger.Log(0, "", ex.StackTrace + "\n");
                        }
                    }
                }

            }
        }

        private void PageEditView_KeyUp(object sender, KeyEventArgs e)
        {
            b_key_control = false;
            b_key_movetag = false;
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Page.CurrentScreenDimension != SMScreen.Screen_1024_768__4_3)
                return;

            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                string strType = "Label";
                string text =  (string)Clipboard.GetData(DataFormats.UnicodeText);
                text = text.Trim();
                if (text.StartsWith("(@"))
                {
                    int i = text.IndexOf(")");
                    if (i > 0)
                    {
                        strType = text.Substring(2, i - 2);
                        text = text.Substring(i + 1).Trim();
                    }
                }


                PasteTextAtPage(strType, text);
            }
            else if (Clipboard.ContainsImage())
            {
                MNReferencedImage ri = Document.CreateNewImage();
                ri.ImageData = Clipboard.GetImage();
                ri.Id = Document.Data.GetNextId();
                ri.Name = "Image Pasted " + DateTime.Now.ToLongTimeString();

                SMImage pm;
                SMControl fc = Page.FindObjectContainingPoint(Context, Context.PhysicalToLogical(LastUserPoint));
                if (fc != null)
                {
                    if (fc is SMImage)
                    {
                        pm = (SMImage)fc;
                        pm.Img.Image = ri;
                    }
                    else if (fc is SMOrderedList)
                    {
                        SMOrderedList smm = (SMOrderedList)fc;
                        smm.AddImage(ri);
                    }
                    this.Invalidate();
                }
                else
                {
                    pm = new SMImage(Page);
                    pm.Id = Document.Data.GetNextId();
                    pm.Img.Image = ri;
                    pm.ContentScaling = SMContentScaling.Fit;
                    // creating coordinates
                    PlaceObjectIntoPage(LastUserPoint, pm, new Size(256, 256));
                }

            }
        }

        public void AlignHorizontal()
        {
            int count = 0;
            int value = 0;
            int itemValueA = 0;
            int itemValueB = 0;
            foreach (SMControl ra in PageData.SelectedObjects)
            {
                itemValueA = ra.Area.Top;
                itemValueB = ra.Area.Bottom;
                count++;
            }

            if (count == 0)
                return;

            value = value / count;
            foreach (SMControl ra in PageData.SelectedObjects)
            {
                ra.Area.Top = itemValueA / count;
                ra.Area.Bottom = itemValueA / count;
            }

            this.Invalidate();
        }

        public void AlignVertical()
        {
            int count = 0;
            double value = 0;
            double itemValue = 0;
            foreach (SMControl ra in PageData.SelectedObjects)
            {
                itemValue = ra.Area.CenterX;
                if (AverageValueSelection == 0)
                {
                    value += itemValue;
                    count++;
                }
                else if (AverageValueSelection == 1)
                {
                    value = Math.Max(value, itemValue);
                    count = 1;
                }
                else if (AverageValueSelection == 2)
                {
                    value = Math.Max(value, itemValue);
                    count = 1;
                }
                else if (AverageValueSelection == 3)
                {
                    value = itemValue;
                    count = 1;
                    break;
                }
            }

            if (count == 0)
                return;

            value = value / count;
            foreach (SMControl c in PageData.SelectedObjects)
            {
                c.Area.CenterX = Convert.ToInt32(value);
            }

            this.Invalidate();
        }

        public void AlignHeight()
        {
            int count = 0;
            int value = 0;
            int itemValue = 0;
            foreach (SMControl ra in PageData.Objects)
            {
                if (ra.Area.Selected)
                {
                    itemValue = ra.Area.Height;
                    if (AverageValueSelection == 0)
                    {
                        value += itemValue;
                        count++;
                    }
                    else if (AverageValueSelection == 1)
                    {
                        value = Math.Max(value, itemValue);
                        count = 1;
                    }
                    else if (AverageValueSelection == 2)
                    {
                        value = Math.Max(value, itemValue);
                        count = 1;
                    }
                    else if (AverageValueSelection == 3)
                    {
                        value = itemValue;
                        count = 1;
                        break;
                    }
                }
            }

            if (count == 0)
                return;

            value = value / count;
            foreach (SMControl ra in PageData.Objects)
            {
                if (ra.Area.Selected)
                {
                    ra.Area.Height = value;
                }
            }

            this.Invalidate();
        }

        public void AlignWidth()
        {
            int count = 0;
            double value = 0;
            double itemValue = 0;
            foreach (SMControl c in PageData.SelectedObjects)
            {
                itemValue = c.Area.Width;
                if (AverageValueSelection == 0)
                {
                    value += itemValue;
                    count++;
                }
                else if (AverageValueSelection == 1)
                {
                    value = Math.Max(value, itemValue);
                    count = 1;
                }
                else if (AverageValueSelection == 2)
                {
                    value = Math.Max(value, itemValue);
                    count = 1;
                }
                else if (AverageValueSelection == 3)
                {
                    value = itemValue;
                    count = 1;
                    break;
                }
            }

            if (count == 0)
                return;

            value = value / count;
            foreach (SMControl c in PageData.SelectedObjects)
            {
                c.Area.Width = Convert.ToInt32(value);
            }

            this.Invalidate();
        }



        public void BringSelectionToFront()
        {
            List<SMControl> unselected = new List<SMControl>();
            List<SMControl> selected = new List<SMControl>();

            foreach (SMControl ct in PageData.Objects)
            {
                if (ct.Area.Selected)
                    selected.Add(ct);
                else
                    unselected.Add(ct);
            }

            PageData.Objects.Clear();
            PageData.Objects.AddRange(unselected);
            PageData.Objects.AddRange(selected);

            this.Invalidate();
        }

        public void SendSelectionToBack()
        {
            List<SMControl> unselected = new List<SMControl>();
            List<SMControl> selected = new List<SMControl>();

            foreach (SMControl ct in PageData.Objects)
            {
                if (ct.Area.Selected)
                    selected.Add(ct);
                else
                    unselected.Add(ct);
            }

            PageData.Objects.Clear();
            PageData.Objects.AddRange(selected);
            PageData.Objects.AddRange(unselected);

            this.Invalidate();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedObjectsToClipboard();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SMControl ct in PageData.Objects)
            {
                if (ct.Area != null)
                {
                    ct.Area.Selected = true;
                }
            }

            SelectionArea.RelativeArea = PageData.GetTotalSelectionRect();
            SelectionArea.Dock = PageData.GetTotalSelectionDock();
            this.Invalidate();
        }

        public void DuplicateItems()
        {
            DialogDuplicating dlg = new DialogDuplicating();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                PageData.DuplicateSelectedObjects(dlg.Rows, dlg.Columns, dlg.HorizontalLines, dlg.VerticalLines, dlg.Spacing);
                Invalidate();
            }
        }

        public void SetSelectionProperty(string p)
        {
            List<SMControl> list = PageData.SelectedObjects;

            foreach (SMControl c in list)
            {
                if (p == "clickable")
                    c.Clickable = true;
                else if (p == "draggable")
                    c.Draggable = SMDragResponse.Line;
                else if (p == "dropable")
                    c.Cardinality = SMConnectionCardinality.One;
                else if (p == "eval:inherited")
                    c.Evaluation = MNEvaluationType.Inherited;
                else if (p == "eval:none")
                    c.Evaluation = MNEvaluationType.Inherited;
            }

        }

        public void ClearSelection()
        {
            SelectionArea.Clear();
            PageData.ClearSelection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">0 - toogleCheckZero, 1 - toogleCheckOne, 2 - toogleCheckMany</param>
        public void MakeGroup(int type, string grp_name)
        {
            if (grp_name == null)
            {
                DialogNewName dlg = new DialogNewName();
                dlg.NamePrompt = "New Group Name:";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    grp_name = dlg.ObjectName;
                }
            }

            if (grp_name != null)
            {
                foreach (SMControl ct in PageData.Objects)
                {
                    if (ct.Area.Selected)
                    {
                        ct.Clickable = true;
                        ct.GroupName = grp_name;
                        switch(type)
                        {
                            case 0:
                                ct.ScriptOnClick = "(control toogleCheckZero)";
                                break;
                            case 1:
                                ct.ScriptOnClick = "(control toogleCheckOne)";
                                break;
                            case 2:
                                ct.ScriptOnClick = "(control toogleCheckMany)";
                                break;
                        }
                    }
                }

            }
        }

        public void SetExpectedChecked(int p)
        {
            foreach (SMControl ct in PageData.Objects)
            {
                if (ct.Area.Selected)
                {
                    if (p == 0)
                        ct.ExpectedChecked = Bool3.False;
                    else if (p == 1)
                        ct.ExpectedChecked = Bool3.True;
                    else if (p == 2)
                        ct.ExpectedChecked = Bool3.Undef;
                    else if (p == 3)
                        ct.ExpectedChecked = Bool3.Both;
                }
            }
        }

        public void SetSelectionProperty(string p, int p_2)
        {
            List<SMControl> list = Page.SelectedObjects;
            if (p == "drag")
            {
                foreach (SMControl c in list)
                {
                    switch (p_2)
                    {
                        case 0: c.Draggable = SMDragResponse.None; break;
                        case 1: c.Draggable = SMDragResponse.Line; break;
                        case 2: c.Draggable = SMDragResponse.Drag; break;
                        case 3: c.Draggable = SMDragResponse.Undef; break;
                    }
                }
            }
            else if (p == "dragline")
            {
                foreach (SMControl c in list)
                {
                    switch (p_2)
                    {
                        case 0: c.DragLineAlign = SMDragLineAlign.Undef; break;
                        case 1: c.DragLineAlign = SMDragLineAlign.TopBottom; break;
                        case 2: c.DragLineAlign = SMDragLineAlign.LeftRight; break;
                    }
                }
            }
            else if (p == "cardinality")
            {
                foreach (SMControl c in list)
                {
                    switch (p_2)
                    {
                        case 0: c.Cardinality = SMConnectionCardinality.Undef; break;
                        case 1: c.Cardinality = SMConnectionCardinality.None; break;
                        case 2: c.Cardinality = SMConnectionCardinality.One; break;
                        case 3: c.Cardinality = SMConnectionCardinality.Many; break;
                    }
                }
            }
            else if (p == "names")
            {
                DialogSetNamesControls d = new DialogSetNamesControls();
                d.SetControls(list);
                d.ShowDialog();
                Invalidate();
            }
            else if (p == "tags")
            {
                DialogSetNamesControls d = new DialogSetNamesControls();
                d.SetTags = true;
                d.SetControls(list);
                d.ShowDialog();
                Invalidate();
            }
        }

        /// <summary>
        /// This copies all controls from template to current page, assigns new IDs and sets
        /// template under it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void electTemplateToPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PageData.Template != null)
            {
                PageData.Objects.Clear();
                PageData.Connections.Clear();

                MNPage t = PageData.Template;
                byte[] buffer;
                foreach (SMControl ctrl in t.Objects)
                {
                    long new_id = PageData.Document.Data.GetNextId();

                    buffer = ctrl.GetBytes();
                    SMControl new_control = SMControl.FromBytes(PageData, buffer);
                    new_control.Id = new_id;
                    PageData.Objects.Add(new_control);
                }

                PageData.MessageText = t.MessageText;
                PageData.MessageTitle = t.MessageTitle;
                PageData.TextB = t.TextB;
                PageData.TextC = t.TextC;
                PageData.Template = t.Template;

                Invalidate();
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            electTemplateToPageToolStripMenuItem.Enabled = (PageData != null && PageData.Template != null);
        }

        public void InsertPageLayout()
        {
            ToolWindowInsertPageLayout form = new ToolWindowInsertPageLayout();

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (form.SelectedTemplate != null)
                {
                    MNPage t = form.SelectedTemplate;
                    MNPage.CopyControlsFrom(t, PageData);

                    Invalidate();
                    Focus();
                }
            }
        }

        private void pasteOnelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                string strType = "Label";
                string text = (string)Clipboard.GetData(DataFormats.UnicodeText);
                text = text.Trim().Replace("\r\n", " ").Replace("\n", " ");

                PasteTextAtPage(strType, text);
            }
        }

        private void PasteTextAtPage(string strType, string text)
        {
            SMControl fc = Page.FindObjectContainingPoint(Context, Context.PhysicalToLogical(LastUserPoint));
            if (fc != null)
            {
                if (fc is SMLabel || fc is SMCheckBox || fc is SMTextContainer)
                {
                    fc.Text = text;
                }
                else if (fc is SMOrderedList)
                {
                    SMOrderedList sm = (SMOrderedList)fc;
                    sm.AddText(text);
                }
                else if (fc is SMImage)
                {
                    SMImage si = (SMImage)fc;
                    si.Text = text;
                    if (si.ContentArangement == SMContentArangement.ImageOnly)
                        si.ContentArangement = SMContentArangement.ImageAbove;
                }
                else if (fc is SMSelection)
                {
                    SMSelection ss = (SMSelection)fc;
                    ss.Text = text.Replace(" ", "|");
                }
                Invalidate();
            }
            else
            {
                object obj = PageData.TagToObject(strType) ?? PageData.TagToObject("Label");
                if (obj is SMControl)
                {
                    SMControl pm = (SMControl)obj;
                    pm.Id = Document.Data.GetNextId();
                    pm.Text = text;
                    pm.Autosize = true;

                    // creating coordinates
                    PlaceObjectIntoPage(LastUserPoint, pm, Size.Empty);
                }
            }
        }


        internal void CheckLabelBackgrounds()
        {
            if (PageData != null)
            {
                foreach (SMControl sc in PageData.Objects)
                {
                    CheckBackgroundShadow(sc);
                }
            }
        }

        private void CheckBackgroundShadow(SMControl sc)
        {
            if (sc is SMLabel)
            {
                SMLabel lab = sc as SMLabel;
                if (lab.Area.BackType == SMBackgroundType.Shadow)
                {
                    if (lab.Area.BackgroundImage == null)
                    {
                        CreateBackgroundImageForLabel(lab);
                    }
                }
                else
                {
                    lab.Area.BackgroundImage = null;
                }
            }
        }

        private void CreateBackgroundImageForLabel(SMLabel lab)
        {
            RectangleStatistic rs = new RectangleStatistic();

                        SMRectangleArea area = lab.Area;
            Rectangle bounds = area.GetBounds(Context);
            Rectangle textBounds = lab.ContentPadding.ApplyPadding(bounds);


            string plainText = lab.Text;
            MNReferencedAudioText runningText = null;

            if (lab.Content != null)
            {
                plainText = null;
                if (lab.Content is MNReferencedText)
                    plainText = ((MNReferencedText)lab.Content).Text;
                else if (lab.Content is MNReferencedAudioText)
                    runningText = lab.Content as MNReferencedAudioText;
                else if (lab.Content is MNReferencedSound)
                    plainText = lab.Text;
            }

            if (plainText.StartsWith("$"))
            {
                plainText = Document.ResolveProperty(plainText.Substring(1));
            }


            Rectangle[] rc = lab.richText.CalcRectangles(Context, plainText, textBounds);

            foreach (Rectangle ri in rc)
            {
                rs.Rects.Add(ri);
            }

            ShadowPainter sp = new ShadowPainter(rs.TotalRectangle, Color.White, 16, 16);
            rs.DrawInto(sp);

            lab.Area.BackgroundImage = sp.GetPNGImage();
            lab.Area.BackgroundImageOffset = sp.OffsetBitmap;
        }

        public void ClearAllConnections()
        {
            if (PageData != null)
            {
                PageData.Connections.Clear();
                foreach (SMControl c in PageData.Objects)
                {
                    c.UIStateHover = false;
                    if (c is SMTextContainer)
                    {
                        (c as SMTextContainer).drawWords.Clear();
                        (c as SMTextContainer).drawWordsModified = true;
                    }
                    else if (c is SMImage)
                    {
                        (c as SMImage).DroppedTag = "";
                        (c as SMImage).DroppedText = "";
                        (c as SMImage).DroppedImage = null;
                    }
                }
                Invalidate();
            }
        }
    }


    public class PageEditDraggableItem
    {
        public string Text { get; set; }

        public string Data { get; set; }

        public string Args { get; set; }

        public Size DefaultSize { get; set; }

        public PageEditDraggableItem()
        {
            Text = "";
            Data = "";
            Args = "";
            DefaultSize = Size.Empty;
        }

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
