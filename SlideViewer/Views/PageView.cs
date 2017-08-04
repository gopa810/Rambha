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

        public PageViewController ViewController = null;

        private MNDocumentExecutor p_docexec = null;

        private MNMenu p_displayedMenu = null;

        public MNBookHeader CurrentBook = null;

        public IMainFrameDelegate mainFrameDelegate = null;

        private AudioPlayer Player = new AudioPlayer();

        public MNMenu DisplayedMenu
        {
            get { return p_displayedMenu; }
            set { p_displayedMenu = value; }
        }

        public void SetDocument(MNDocument doc)
        {
            Document = doc;
            doc.Viewer = p_docexec;
            p_docexec.Document = doc;
            p_docexec.OnEvent("OnLoad", doc);
        }

        public MNDocumentExecutor DocExec
        {
            get
            {
                return p_docexec;
            }
        }

        public MNPageContext Context = null;
        private PVDragContext MouseContext = new PVDragContext();


        public PageView()
        {
            InitializeComponent();
            ViewController = new PageViewController();
            ViewController.View = this;
            p_docexec = new MNDocumentExecutor(ViewController);
            Context = new MNPageContext();
            refusal_timer.Tick += new EventHandler(refusal_timer_Tick);
            MouseContext.context = Context;

            InitBitmaps();

        }

        public void OnNotificationReceived(object sender, string message, params object[] args)
        {
            if (sender == this)
                return;

            switch (message)
            {
                case "ObjectSelected":
                    if (args != null && args.Length > 0 && args[0] is MNPage)
                    {
                        MNPage p = (MNPage)args[0];
                        if (p != CurrentPage)
                            CurrentPage = p;
                    }
                    break;
            }
        }

        public void InitBitmaps()
        {
            Context.navigIconBack = Properties.Resources.navigIconBack;
            Context.navigIconFwd = Properties.Resources.navigIconFwd;
            Context.navigIconHelp = Properties.Resources.navigIconHelp;
            Context.navigIconMenu = Properties.Resources.navigIconHome;
            Context.navigArrowBack = Properties.Resources.navigArrowLeft;
            Context.navigArrowFwd = Properties.Resources.navigArrowRight;
            Context.navigSpeakerOn = Properties.Resources.SpeakerOn;
            Context.navigSpeakerOff = Properties.Resources.SpeakerOff;
        }

        public void Start()
        {
            CurrentPage = Document.GetPage(Document.Book.StartPage);
            Invalidate();
        }

        public MNDocument CurrentDocument
        {
            get { return Document; }
            set
            {
                Document = value;
                if (value != null)
                {
                    Document.Viewer = DocExec;
                    DocExec.Document = Document;
                }
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
                    p_current_page.OnPageEvent("OnPageWillDisappear");
                if (value != null)
                    value.OnPageEvent("OnPageWillAppear");
                p_current_page = value;
                DocExec.CurrentPage = value;

                if (p_current_page != null && Document != p_current_page.Document)
                {
                    SetDocument(p_current_page.Document);
                }

                MNNotificationCenter.AudioOn = p_current_page != null ? p_current_page.DefaultAudioState : false;
                if (p_current_page != null)
                    p_current_page.ShowHints(false);
                ReloadPage(false);

                // repainting
                Invalidate();
                if (value != null)
                    value.OnPageEvent("OnPageDidAppear");
                if (oldPage != null)
                    value.OnPageEvent("OnPageDidDisappear");
                if (oldPage != value)
                    MNNotificationCenter.BroadcastMessage(this, "ObjectSelected", p_current_page);
                MNNotificationCenter.BroadcastMessage(this, "PageDidAppear", p_current_page);

            }
        }

        public void RecalculateMatrix()
        {
            // calculate new size of this control
            Size viewSize = this.Size;
            Size pageSize = Context.PageSize;

           if (viewSize.Height == 0 || pageSize.Height == 0)
                return;

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
            Context.ViewController = ViewController;

            CurrentPage.PaintBackground(Context);
            CurrentPage.Paint(Context, false);
            CurrentPage.Paint(Context, true);

            if (CurrentPage.ShowMessageAlways)
            {
                Context.PaintMessageBox(false);
            }

            if (p_displayedMenu != null)
            {
                p_displayedMenu.Paint(Context);
            }
            else if (Context.messageBox.Visible && !CurrentPage.ShowMessageAlways)
            {
                Context.PaintMessageBox(true);
            }
            else
            {
                // painting dragged item
                switch (MouseContext.DragType)
                {
                    case SMDragResponse.Drag:
                        PaintDraggedItem(Context, MouseContext);
                        break;
                    case SMDragResponse.Line:
                        e.Graphics.DrawLine(Pens.Black, MouseContext.startPoint, MouseContext.lastPoint);
                        PaintDraggedItem(Context, MouseContext);
                        break;
                    default:
                        break;
                }
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
                        SizeF sf = context.g.MeasureString(ti.Text, context.DragItemFont);
                        ti.ContentSize = new Size(Convert.ToInt32(sf.Width), Convert.ToInt32(sf.Height));
                    }
                    Rectangle rc = new Rectangle(mouseContext.lastPoint.X - ti.ContentSize.Width / 2, 
                        mouseContext.lastPoint.Y - ti.ContentSize.Height/2,
                        ti.ContentSize.Width + 2, ti.ContentSize.Height + 2);
                    //context.g.FillRectangle(Brushes.LightPink, rc);
                    context.g.DrawString(ti.Text, context.DragItemFont, Brushes.Red, rc);
                }
                else if (ti.Image != null)
                {
                    if (ti.ContentSize.Width == 0)
                    {
                        Size sf = ti.Image.Size;
                        double d = 64.0 / (Math.Max(sf.Width, sf.Height) + 1);
                        ti.ContentSize = new Size(Convert.ToInt32(d * sf.Width), Convert.ToInt32(d * sf.Height));
                    }
                    Rectangle rc = new Rectangle(mouseContext.lastPoint.X - ti.ContentSize.Width / 2, 
                        mouseContext.lastPoint.Y - ti.ContentSize.Height / 2,
                        ti.ContentSize.Width + 2, ti.ContentSize.Height + 2);
                    context.g.DrawImage(ti.Image, rc);
                }
            }
        }

        public int TestHitHeaderButton(Point p)
        {
            int x = p.X;
            int y = p.Y;
            if (y < MNPage.HEADER_HEIGHT)
            {
                if (x < MNPage.HEADER_HEIGHT)
                {
                    if (CurrentPage.ShowBackNavigation)
                        return 1;
                    else if (CurrentPage.ShowHome)
                        return 2;
                }
                else if (x < MNPage.HEADER_HEIGHT * 2)
                {
                    if (CurrentPage.ShowBackNavigation && CurrentPage.ShowHome)
                        return 2;
                }
                else if (x > Context.PageWidth - MNPage.HEADER_HEIGHT)
                {
                    if (CurrentPage.ShowForwardNavigation)
                        return 4;
                    else if (CurrentPage.ShowHelp)
                        return 3;
                    else
                        return 5;
                }
                else if (x > Context.PageWidth - 2 * MNPage.HEADER_HEIGHT)
                {
                    if (CurrentPage.ShowForwardNavigation && CurrentPage.ShowHelp)
                        return 3;
                    else if (CurrentPage.ShowForwardNavigation || CurrentPage.ShowHelp)
                        return 5;
                }
                else if (x > Context.PageWidth - 3 * MNPage.HEADER_HEIGHT)
                {
                    if (CurrentPage.ShowForwardNavigation && CurrentPage.ShowHelp)
                        return 5;
                }
            }
            else if (y > (Context.PageHeight / 2 - 100) && y < (Context.PageHeight / 2 + 100))
            {
                if (x < 80 && CurrentPage.ShowBackNavigation)
                    return 1;
                if (x > Context.PageWidth - 80 && CurrentPage.ShowForwardNavigation)
                    return 4;
            }

            return 0;
        }

        /// <summary>
        /// Pointer started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageView_MouseDown(object sender, MouseEventArgs e)
        {
            if (CurrentPage == null || MouseContext.State == PVDragContext.Status.RefusingDrop)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ReviewFrame.Shared.SetPageTab();

                if (!TemporaryMessageBoxVisible)
                    Context.hitHeaderButton = TestHitHeaderButton(Context.PhysicalToLogical(new Point(e.X, e.Y)));

                if (TemporaryMessageBoxVisible)
                {
                }
                else if (p_displayedMenu != null)
                {
                    MouseContext.lastPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
                    MouseContext.startPoint = MouseContext.lastPoint;
                    MouseContext.startControl = null;
                    MouseContext.endControl = null;
                    MouseContext.State = PVDragContext.Status.ClickDown;
                    MouseContext.DragType = SMDragResponse.None;
                    MouseContext.context = Context;
                    MouseContext.StartClicked = true;
                    Context.selectedMenuItem = p_displayedMenu.TestHit(MouseContext);
                }
                else if (Context.hitHeaderButton > 0)
                {
                }
                else if (p_displayedMenu == null)
                {
                    MouseContext.lastPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
                    MouseContext.startPoint = MouseContext.lastPoint;
                    MouseContext.startControl = CurrentPage.FindObjectContainingPoint(Context, MouseContext.lastPoint);
                    MouseContext.endControl = null;
                    MouseContext.State = PVDragContext.Status.ClickDown;
                    MouseContext.DragType = SMDragResponse.None;
                    MouseContext.context = Context;

                    if (MouseContext.startControl != null)
                        MouseContext.startControl.OnTapBegin(MouseContext);
                    MouseContext.StartClicked = true;

                    timerLongClick.Start();
                }

                Invalidate();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Point p = Context.PhysicalToLogical(new Point(e.X, e.Y));
                SMControl sControl = CurrentPage.FindObjectContainingPoint(Context, p);
                if (sControl != null)
                {
                    MNNotificationCenter.BroadcastMessage(this, "ObjectForReview", sControl);
                }
            }
        }

        /// <summary>
        /// Pointer released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageView_MouseUp(object sender, MouseEventArgs e)
        {
            if (CurrentPage == null || MouseContext.State == PVDragContext.Status.RefusingDrop)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (TemporaryMessageBoxVisible)
                {
                        Context.messageBox.Visible = false;
                }
                else if (p_displayedMenu != null)
                {
                    MouseContext.context = Context;
                    MouseContext.lastPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
                    MouseContext.endControl = null;

                    if (Context.selectedMenuItem == p_displayedMenu.TestHit(MouseContext)
                        && Context.selectedMenuItem >= 0 && Context.selectedMenuItem < p_displayedMenu.Items.Count)
                    {
                        if (p_docexec != null)
                        {
                            MNMenuItem mi = p_displayedMenu.Items[Context.selectedMenuItem];
                            p_docexec.OnMenuItem(mi, CurrentPage);
                        }
                    }

                    p_displayedMenu = null;
                    Context.selectedMenuItem = -1;
                }
                else if (Context.hitHeaderButton > 0)
                {
                    int test = TestHitHeaderButton(Context.PhysicalToLogical(new Point(e.X, e.Y)));
                    if (test == Context.hitHeaderButton)
                        ActivateHeaderButton(test);
                    Context.hitHeaderButton = 0;
                }
                else
                {
                    MouseContext.context = Context;
                    MouseContext.lastPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
                    MouseContext.endControl = CurrentPage.FindObjectContainingPoint(Context, MouseContext.lastPoint);

                    /*if (MouseContext.startControl != null)
                        MouseContext.startControl.OnTapEnd(MouseContext);*/
                    bool dropResult = true;

                    if (MouseContext.State == PVDragContext.Status.Dragging)
                    {
                        SenseTracking(true);

                        if (MouseContext.trackedControl != null)
                        {
                            dropResult = MouseContext.trackedControl.OnDropFinished(MouseContext);
                            //MouseContext.trackedControl.OnDragHotTrackEnded(MouseContext.draggedItem, MouseContext);
                            //Debugger.Log(0, "", "Dropping into control\n");
                        }
                        if (dropResult && MouseContext.startControl != null)
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
                            MouseContext.startControl.OnTapEnd(MouseContext);
                        MouseContext.StartClicked = false;
                    }

                    if (dropResult)
                    {
                        MouseContext.State = PVDragContext.Status.None;
                        MouseContext.DragType = SMDragResponse.None;
                        MouseContext.draggedItem = null;
                    }
                    else
                    {
                        MouseContext.State = PVDragContext.Status.RefusingDrop;
                        refusal_index = 0;
                        refusal_step = new Point((MouseContext.lastPoint.X - MouseContext.startPoint.X) / refusal_maximum,
                            (MouseContext.lastPoint.Y - MouseContext.startPoint.Y) / refusal_maximum);
                        refusal_timer.Interval = 30;
                        refusal_timer.Start();
                    }
                }

                Invalidate();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
            }
        }

        public bool TemporaryMessageBoxVisible
        {
            get
            {
                return Context.messageBox.Visible
                    && CurrentPage != null
                    && !CurrentPage.ShowMessageAlways;
            }
        }

        public void ActivateHeaderButton(int test)
        {
            switch (test)
            {
                case 1:
                    ViewController.GoBack();
                    break;
                case 2:
                    InitializePageMenu();
                    //ViewController.ShowPage(CurrentPage.Document.Book.HomePage);
                    break;
                case 3:
                    Context.ShowMessageBox();
                    Invalidate();
                    break;
                case 4:
                    ViewController.GoForward();
                    break;
                case 5:
                    MNNotificationCenter.AudioOn = !MNNotificationCenter.AudioOn;
                    if (MNNotificationCenter.AudioOn)
                        ReloadPage(false);
                    break;
            }
        }

        public void InitializePageMenu()
        {
            p_displayedMenu = new MNMenu();

            int itemsInSection = 0;

            if (!CurrentDocument.Book.HomePage.Equals(CurrentPage.Title))
            {
                p_displayedMenu.Items.Add(new MNMenuItem(CurrentDocument) { Text = "Goto home page", ActionScript = "(view homePage)", ImageName = "menuIconHome" });
                itemsInSection++;
            }
            if (!CurrentDocument.Book.HomePage.Equals(CurrentPage.Title))
            {
                p_displayedMenu.Items.Add(new MNMenuItem(CurrentDocument) { Text = "Restart/clear page", ActionScript = "(view reload)", ImageName = "menuItemRestart" });
                itemsInSection++;
            }
            if (itemsInSection > 0)
            {
                p_displayedMenu.Items.Add(new MNMenuItem(CurrentDocument) { Text = "" });
                itemsInSection = 0;
            }

            // new section

            if (CurrentPage.HasControlsWithHints)
            {
                p_displayedMenu.Items.Add(new MNMenuItem(CurrentDocument) { Text = "Show hints", ActionScript = "(page showHints)", ImageName = "menuIconShowHints" });
                itemsInSection++;
            }

            if (CurrentPage.HasControlsWithSpots)
            {
                p_displayedMenu.Items.Add(new MNMenuItem(CurrentDocument) { Text = "Show sound spots", ActionScript = "(page showSoundSpots)", ImageName = "menuItemShowSpots" });
                itemsInSection++;
            }

            if (itemsInSection > 0)
            {
                p_displayedMenu.Items.Add(new MNMenuItem(CurrentDocument) { Text = "" });
                itemsInSection = 0;
            }

            // new section

            if (CurrentDocument.Book.Languages.Count > 1)
            {
                p_displayedMenu.Items.Add(new MNMenuItem(CurrentDocument) { Text = "Select Language", ActionScript = "(view changeLanguage)", ImageName = "menuIconSelLang" });
                itemsInSection++;
            }

            p_displayedMenu.Items.Add(new MNMenuItem(CurrentDocument) { Text = "Select Book", ActionScript = "(view selectBook)", ImageName = "menuIconSelBook" });
            itemsInSection++;


        }


        private int refusal_maximum = 10;
        private int refusal_index = 0;
        private Point refusal_step = Point.Empty;
        private Timer refusal_timer = new Timer();

        void refusal_timer_Tick(object sender, EventArgs e)
        {
            RefusalPoint();
        }

        private void RefusalPoint()
        {
            if (MouseContext.State != PVDragContext.Status.RefusingDrop)
            {
                refusal_timer.Stop();
                return;
            }

            if (refusal_index >= refusal_maximum)
            {
                MouseContext.State = PVDragContext.Status.None;
                MouseContext.DragType = SMDragResponse.None;
                MouseContext.draggedItem = null;
                refusal_timer.Stop();
            }
            else
            {
                MouseContext.lastPoint.X -= refusal_step.X;
                MouseContext.lastPoint.Y -= refusal_step.Y;
            }

            refusal_index++;
            Invalidate();
        }

        /// <summary>
        /// Moving pointer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageView_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentPage == null || MouseContext.State == PVDragContext.Status.RefusingDrop)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseContext.context.lastClientPoint = new Point(e.X, e.Y);
                MouseContext.lastPoint = Context.PhysicalToLogical(MouseContext.context.lastClientPoint);
                MouseContext.endControl = CurrentPage.FindObjectContainingPoint(Context, MouseContext.lastPoint);

                // if menu is displayed, we dont need movements of mouse
                if (Context.hitHeaderButton > 0)
                    return;
                if (TemporaryMessageBoxVisible)
                    return;

                if (MouseContext.endControl != null)
                    MouseContext.endControl.OnDropMove(MouseContext);
                if (MouseContext.State == PVDragContext.Status.Dragging)
                {
                    if (MouseContext.startControl != null)
                    {
                        MouseContext.startControl.OnTapMove(MouseContext);
                    }

                    SenseTracking(false);

                    /*if (MouseContext.trackedControl == null && MouseContext.endControl != null 
                        && MouseContext.endControl != MouseContext.startControl
                        && MouseContext.endControl.Cardinality != SMConnectionCardinality.None)
                    {
                        MouseContext.endControl.OnDragHotTrackStarted(MouseContext.draggedItem, MouseContext);
                    }
                    else if (MouseContext.trackedControl != null && MouseContext.endControl != null
                        && MouseContext.trackedControl.Cardinality != SMConnectionCardinality.None)
                    {
                        MouseContext.trackedControl.OnDragHotTrackEnded(MouseContext.draggedItem, MouseContext);
                    }*/
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
                        if (MouseContext.startControl != null && MouseContext.startControl.Draggable != SMDragResponse.None)
                        {
                            MouseContext.startControl.OnTapCancel(MouseContext);
                            MouseContext.StartClicked = false;
                            if (MouseContext.startControl.Draggable == SMDragResponse.Drag
                                || MouseContext.startControl.Draggable == SMDragResponse.Line)
                                MouseContext.draggedItem = MouseContext.startControl.GetDraggableItem(MouseContext.lastPoint);
                            if (MouseContext.draggedItem != null)
                            {
                            MouseContext.startControl.OnDragStarted(MouseContext);
                            MouseContext.startControl.OnDragMove(MouseContext);
                                MouseContext.State = PVDragContext.Status.Dragging;
                                MouseContext.DragType = MouseContext.startControl.Draggable;
                            }
                        }
                        Invalidate();
                    }
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
            }
            //Debugger.Log(0, "", "MouseState: " + MouseContext.State.ToString() + "\n");
        }

        private void SenseTracking(bool endOfTracking)
        {
            SMControl prevTracked = null;
            foreach (SMControl sc in CurrentPage.Objects)
            {
                if (sc.UIStateHover)
                {
                    prevTracked = sc;
                    if (endOfTracking)
                        sc.UIStateHover = false;
                    break;
                }
            }

            if (MouseContext.endControl != prevTracked)
            {
                foreach (SMControl sc in CurrentPage.Objects)
                {
                    sc.UIStateHover = false;
                }

                if (!endOfTracking && MouseContext.endControl != null)
                    MouseContext.endControl.OnDragHotTrackStarted(MouseContext.draggedItem, MouseContext);
                if (prevTracked != null)
                    prevTracked.OnDragHotTrackEnded(MouseContext.draggedItem, MouseContext);
            }

        }

        private double PointDistance(Point a, Point b)
        {
            return Math.Sqrt(Convert.ToDouble((a.X - b.X) * (a.X - b.X)) + Convert.ToDouble((b.Y - a.Y) * (b.Y - a.Y)));
        }

        private void timerLongClick_Tick(object sender, EventArgs e)
        {
            if (MouseContext.State == PVDragContext.Status.ClickDown)
            {
                MouseContext.State = PVDragContext.Status.LongClicked;
                if (Document.HasViewer)
                {
                    if (MouseContext.startControl != null)
                        Document.Viewer.OnLongClick(MouseContext.startControl, MouseContext);
                    else
                        Document.Viewer.OnEvent("OnPageLongClick", CurrentPage);
                }
                //ExecuteScriptForKey(MouseContext.startControl, "onLongClick");
            }
            timerLongClick.Stop();
        }

        private void PageView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MouseContext.State == PVDragContext.Status.RefusingDrop)
                return;

            if (Document.HasViewer)
            {
                if (MouseContext.startControl != null)
                    Document.Viewer.OnDoubleClick(MouseContext.startControl, MouseContext);
                else
                    Document.Viewer.OnEvent("OnPageDoubleClick", CurrentPage);
            }
            //ExecuteScriptForKey(MouseContext.startControl, "onDoubleClick");

        }

        /// <summary>
        /// Reloading language localizations for all controls
        /// </summary>
        public void ReloadPage(bool resetStatus)
        {
            if (CurrentDocument != null && p_current_page != null)
            {
                Context.messageBox.Visible = p_current_page.ShowMessageAlways;

                if (resetStatus)
                {
                    p_current_page.ResetStatus();
                }

                    // reinitialize status
                    if (p_current_page.InitialStatus == null)
                {
                        p_current_page.StoreStatus();
                }

                // load content for all controls
                foreach (SMControl control in p_current_page.Objects)
                {
                    if (control.ContentId != null && control.ContentId.Length > 0)
                    {
                        //Debugger.Log(0,"", "NEED LOAD CONTENT: " + control.ContentId + "\n");
                        MNReferencedCore value = FindContentObject(control.ContentType, control.ContentId);
                        control.Content = value;
                        if (value != null && value is MNReferencedAudioText)
                        {
                            p_current_runtext = value as MNReferencedAudioText;
                            p_current_runtext.currentWord = 0;
                            timerRuntext.Interval = p_current_runtext.GetCurrentTimeInterval();
                            timerRuntext.Start();
                            PlaySound(p_current_runtext.Sound);
                            Invalidate();
                        }
                        else if (value != null && value is MNReferencedSound)
                        {
                            PlaySound(value as MNReferencedSound);
                        }
                    }
                }
            }
        }

        public void PlaySound(MNReferencedSound sound)
        {
            if (MNNotificationCenter.AudioOn)
            {
                Player.SetSound(sound);
                Player.Play();
            }
        }

        public MNReferencedCore FindContentObject(SMContentType type, string contentId)
        {
            MNReferencedCore value = null;

            //Debugger.Log(0, "", "--FindContentObject: A\n");
            if (CurrentDocument.CurrentLanguage != null)
            {
                //Debugger.Log(0, "", "--FindContentObject: B\n");
                value = CurrentDocument.CurrentLanguage.FindObject(contentId);
            }

            if (value == null)
            {
                if (CurrentDocument.DefaultLanguage != null)
                {
                    //Debugger.Log(0, "", "--FindContentObject: DEFAULT B\n");
                    value = CurrentDocument.DefaultLanguage.FindObject(contentId);
                }
            }
            
            if (value == null && type == SMContentType.Text)
            {
                //Debugger.Log(0, "", "--FindContentObject: C\n");
                MNReferencedText rt = CurrentDocument.FindText(contentId);
                if (rt != null)
                {
                    //Debugger.Log(0, "", "--FindContentObject: D\n");
                    MNReferencedText str = new MNReferencedText();
                    str.Text = rt.Text;
                    value = str;
                }

            }

            return value;
        }

        public MNReferencedAudioText p_current_runtext = null;

        private void timerRuntext_Tick(object sender, EventArgs e)
        {
            p_current_runtext.currentWord++;
            if (p_current_runtext.currentWord >= p_current_runtext.GetWordCount())
            {
                timerRuntext.Stop();
                Player.Stop();
                Player.DisposeAll();
            }
            else
            {
                timerRuntext.Interval = p_current_runtext.GetCurrentTimeInterval();
            }
            Invalidate();
        }
    }


    /// <summary>
    /// Main Controller in the presentation
    /// </summary>
    public class PageViewController : GSCore
    {
        public PageView View { get; set; }

        public Timer t = new Timer();

        private List<int> PageHistory = new List<int>();

        private class ScheduledTask
        {
            public GSCore Target = null;
            public string Message = string.Empty;
            public GSCoreCollection Args = null;
        }

        private ScheduledTask scheduled = null;

        public PageViewController()
        {
            Debugger.Log(0,"","Instance of controller\n");
            t.Tick += new EventHandler(t_Tick);
        }

        private void t_Tick(object sender, EventArgs e)
        {
            if (scheduled != null)
            {
                scheduled.Target.ExecuteMessage(scheduled.Message, scheduled.Args);
                scheduled = null;
                View.Invalidate();
            }
            t.Stop();
        }

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            string arg1 = string.Empty;
            switch(token)
            {
                case "invalidate":
                    View.Invalidate();
                    break;
                case "reload":
                    //View.CurrentPage.RestoreStatus();
                    View.ReloadPage(true);
                    break;
                case "restart":
                    View.Start();
                    break;
                case "selectBook":
                    if (View.CurrentDocument != null)
                        View.CurrentDocument.SaveBookStatus();
                    View.mainFrameDelegate.SetShowPanel("files");
                    break;
                case "showpage":
                    arg1 = args.getSafe(0).getStringValue();
                    if (arg1.Equals("#next"))
                    {
                        GoForward();
                    }
                    else if (arg1.Equals("#back"))
                    {
                        GoBack();
                    }
                    else
                    {
                        ShowPage(arg1);
                    }
                    break;
                case "showPageMenu":
                    View.InitializePageMenu();
                    View.Invalidate();
                    break;
                case "showmenu":
                    MNMenu m = View.CurrentDocument.FindMenu(args.getSafe(0).getStringValue());
                    if (m != null)
                        View.DisplayedMenu = m;
                    break;
                case "changeLanguage":
                    View.mainFrameDelegate.showSelectLanguageDialog(View.CurrentBook);
                    break;
                case "playSound":
                    GSCore aif = args.getSafe(0);
                    if (aif != null && aif is MNReferencedSound)
                    {
                        View.PlaySound(aif as MNReferencedSound);
                    }
                    else if (aif != null && aif is GSString)
                    {
                        MNReferencedSound sound = View.CurrentDocument.FindSound(aif.getStringValue());
                        if (sound != null)
                            View.PlaySound(sound);
                    }
                    break;
                case "scheduleClear":
                    if (t.Enabled) t.Stop();
                    scheduled = null;
                    break;
                case "scheduleCall":
                    if (t.Enabled) t.Stop();
                    if (scheduled != null)
                    {
                        scheduled.Target.ExecuteMessage(scheduled.Message, scheduled.Args);
                        scheduled = null;
                    }
                    if (args != null)
                    {
                        int interval = (int)args.getSafe(0).getIntegerValue();
                        if (interval > 0 && interval < 200000)
                        {
                            scheduled = new ScheduledTask();
                            scheduled.Target = args.getSafe(1);
                            scheduled.Message = args.getSafe(2).getStringValue();
                            scheduled.Args = args.getSublist(3);
                            t.Interval = interval;
                            t.Start();
                        }
                    }
                    break;
                case "homePage":
                    ShowPage(View.CurrentPage.Document.Book.HomePage);
                    break;
                default:
                    base.ExecuteMessage(token, args);
                    break;
            }

            return GSVoid.Void;
        }

        public void ShowPage(string arg1)
        {
            MNPage p = null;

            // displaying specific page
            // store current page index to history
            p = View.CurrentDocument.FindPage(arg1);
            if (p != null)
            {
                if (View.CurrentPage != null)
                {
                    PageHistory.Add(View.CurrentPage.Index);
                    Debugger.Log(0, "", "ADD HISTORY " + View.CurrentPage.Index.ToString() + "\n");
                    LogHistory();
                }
                View.CurrentPage = p;
            }
        }

        public void GoForward()
        {
            MNPage p = null;

            // displaying next page (page with index + 1)
            // store current page index to history
            if (View.CurrentPage != null)
            {
                if (View.CurrentPage.NextPage != null && View.CurrentPage.NextPage.Length > 0)
                    p = View.CurrentDocument.FindPage(View.CurrentPage.NextPage);
                else
                    p = View.CurrentDocument.FindPageWithIndex(View.CurrentPage.Index + 1);
            }
            if (p != null)
            {
                if (View.CurrentPage != null)
                {
                    PageHistory.Add(View.CurrentPage.Index);
                    Debugger.Log(0, "", "ADD HISTORY " + View.CurrentPage.Index.ToString() + "\n");
                    LogHistory();
                }
                View.CurrentPage = p;
            }
        }

        public void GoBack()
        {
            MNPage p = null;

            // showing page that was displayed previously
            // take index of that page from page history
            if (PageHistory.Count > 0)
            {
                int pageIndex = PageHistory[PageHistory.Count - 1];
                Debugger.Log(0, "", "RESTORE HISTORY " + pageIndex.ToString() +"\n");
                LogHistory();
                p = View.CurrentDocument.FindPageWithIndex(pageIndex);
                PageHistory.RemoveAt(PageHistory.Count - 1);
            }
            if (p != null)
                View.CurrentPage = p;
        }

        public void LogHistory()
        {
            Debugger.Log(0, "", "HISTORY: ");
            foreach (int i in PageHistory)
            {
                Debugger.Log(0, "", i.ToString() + ", ");
            }
            Debugger.Log(0, "", "\n");
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
