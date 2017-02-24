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

        public MNPageContext Context { get; set; }

        public PageView()
        {
            InitializeComponent();
            ViewController = new PageViewController();
            ViewController.View = this;
            p_docexec = new MNDocumentExecutor(ViewController);
            Context = new MNPageContext();
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
                ReloadPage();
                Invalidate();
                if (value != null)
                    value.OnPageEvent("OnPageDidAppear");
                if (oldPage != null)
                    value.OnPageEvent("OnPageDidDisappear");

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
            Context.ViewController = ViewController;

            CurrentPage.Paint(Context);

            if (p_displayedMenu != null)
            {
                p_displayedMenu.Paint(Context);
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
                        SizeF sf = context.g.MeasureString(ti.Text, context.MenuTitleFont);
                        ti.ContentSize = new Size(Convert.ToInt32(sf.Width),Convert.ToInt32(sf.Height));
                    }
                    Rectangle rc = new Rectangle(mouseContext.lastPoint.X - ti.ContentSize.Width/2, mouseContext.lastPoint.Y - ti.ContentSize.Height,
                        ti.ContentSize.Width + 2, ti.ContentSize.Height + 2);
                    context.g.DrawString(ti.Text, context.MenuTitleFont, Brushes.Black, rc);
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

            if (p_displayedMenu == null)
            {
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
            }
            else
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

            if (p_displayedMenu != null)
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

            MouseContext.context = Context;
            MouseContext.lastPoint = Context.PhysicalToLogical(new Point(e.X, e.Y));
            MouseContext.endControl = FindObjectContainingPoint(MouseContext.lastPoint);

            /*if (MouseContext.startControl != null)
                MouseContext.startControl.OnTapEnd(MouseContext);*/

            if (MouseContext.State == PVDragContext.Status.Dragging)
            {
                if (MouseContext.trackedControl != null)
                {
                    MouseContext.trackedControl.OnDropFinished(MouseContext);
                    MouseContext.trackedControl.OnDragHotTrackEnded(MouseContext.draggedItem, MouseContext);
                    Debugger.Log(0,"", "Dropping into control\n");
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

            // if menu is displayed, we dont need movements of mouse
            if (p_displayedMenu != null)
                return;

            if (MouseContext.endControl != null)
                MouseContext.endControl.OnDropMove(MouseContext);
            if (MouseContext.State == PVDragContext.Status.Dragging)
            {
                if (MouseContext.startControl != null)
                    MouseContext.startControl.OnTapMove(MouseContext);

                if (MouseContext.trackedControl == null && MouseContext.endControl != null && MouseContext.endControl != MouseContext.startControl
                    && MouseContext.endControl.Droppable != SMDropResponse.None)
                {
                    MouseContext.endControl.OnDragHotTrackStarted(MouseContext.draggedItem, MouseContext);
                }
                else if (MouseContext.trackedControl != null && MouseContext.endControl == null
                    && MouseContext.trackedControl.Droppable != SMDropResponse.None)
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
                    if (MouseContext.startControl != null && MouseContext.startControl.Draggable != SMDragResponse.None)
                    {
                        MouseContext.State = PVDragContext.Status.Dragging;
                        MouseContext.DragType = MouseContext.startControl.Draggable;
                        if (MouseContext.DragType == SMDragResponse.Drag || MouseContext.DragType == SMDragResponse.Line)
                            MouseContext.draggedItem = MouseContext.startControl.GetDraggableItem(MouseContext.lastPoint);
                        MouseContext.startControl.OnDragStarted(MouseContext);
                        MouseContext.startControl.OnDragMove(MouseContext);
                    }
                    Invalidate();
                }
            }

            //Debugger.Log(0, "", "MouseState: " + MouseContext.State.ToString() + "\n");
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
        public void ReloadPage()
        {
            if (CurrentDocument != null)
            {
                // load content for all controls
                foreach (SMControl control in p_current_page.Objects)
                {
                    if (control.ContentId != null && control.ContentId.Length > 0)
                    {
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
                        else if (value != null && value is MNReferencedText)
                        {
                            PlaySound(value as MNReferencedSound);
                        }
                    }
                }
            }
        }

        public void PlaySound(MNReferencedSound sound)
        {
            Player.SetSound(sound);
            Player.Play();
        }

        public MNReferencedCore FindContentObject(SMContentType type, string contentId)
        {
            MNReferencedCore value = null;

            if (CurrentDocument.CurrentLanguage != null)
            {
                value = CurrentDocument.CurrentLanguage.FindObject(contentId);
            }
            
            if (value == null && type == SMContentType.Text)
            {
                MNReferencedText rt = CurrentDocument.FindText(contentId);
                if (rt != null)
                {
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
            t.Tick += new EventHandler(t_Tick);
        }

        private void t_Tick(object sender, EventArgs e)
        {
            if (scheduled != null)
            {
                scheduled.Target.ExecuteMessage(scheduled.Message, scheduled.Args);
                scheduled = null;
            }
        }

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            string arg1 = string.Empty;
            MNPage p = null;
            switch(token)
            {
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
                        // displaying next page (page with index + 1)
                        // store current page index to history
                        if (View.CurrentPage != null)
                            p = View.CurrentDocument.FindPageWithIndex(View.CurrentPage.Index + 1);
                        if (p != null)
                        {
                            if (View.CurrentPage != null)
                                PageHistory.Add(View.CurrentPage.Index);
                            View.CurrentPage = p;
                        }
                    }
                    else if (arg1.Equals("#back"))
                    {
                        // showing page that was displayed previously
                        // take index of that page from page history
                        if (PageHistory.Count > 0)
                        {
                            p = View.CurrentDocument.FindPageWithIndex(PageHistory[PageHistory.Count - 1]);
                            PageHistory.RemoveAt(PageHistory.Count - 1);
                        }
                        if (p != null)
                            View.CurrentPage = p;
                    }
                    else
                    {
                        // displaying specific page
                        // store current page index to history
                        p = View.CurrentDocument.FindPage(arg1);
                        if (p != null)
                        {
                            if (View.CurrentPage != null)
                                PageHistory.Add(View.CurrentPage.Index);
                            View.CurrentPage = p;
                        }
                    }
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
                    break;
                case "scheduleCall":
                    if (t.Enabled) t.Stop();
                    if (scheduled != null)
                    {
                        scheduled.Target.ExecuteMessage(scheduled.Message, scheduled.Args);
                        scheduled = null;
                    }

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
