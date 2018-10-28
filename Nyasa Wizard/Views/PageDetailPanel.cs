using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using Rambha.Document;
using Rambha.Script;

namespace SlideMaker.Views
{
    public partial class PageDetailPanel : UserControl, INotificationTarget
    {
        public event NormalEventHandler BackToParentView;

        public event NormalEventHandler NewPageRequested;


        private SMControl CurrentSelectedControl = null;
        private SMRectangleArea CurrentSelectedArea = null;



        public PageDetailPanel()
        {
            InitializeComponent();
            MNNotificationCenter.AddReceiver(this, null);
            MNNotificationCenter.AddReceiver(treeObjectView1, null);

            containerA.EditView = pageScrollArea1.GetPageEditView();

            actionCenter.pageScroll = pageScrollArea1;
            actionCenter.editView = pageScrollArea1.GetPageEditView();
            actionCenter.tabControl = tabControlDetails;
            actionCenter.listView = listViewActions;
            actionCenter.actionTabPage = tabActions;

            pageScrollArea1.GetPageEditView().pageActions = actionCenter;

            pageScrollArea1.NextPageRequested += new PageChangedEventHandler(pageScrollArea1_NextPageRequested);
            pageScrollArea1.PrevPageRequested += new PageChangedEventHandler(pageScrollArea1_PrevPageRequested);
        }

        public void InitializeControlList(MNPage currentPage)
        {
            listToolbox.BeginUpdate();
            listToolbox.Items.Clear();
            if (currentPage != null)
            {
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "TextView (1 column)", Data = currentPage.ObjectTypeToTag(typeof(SMTextView)), Args = "columns=1" });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "TextEdit Short", Data = currentPage.ObjectTypeToTag(typeof(SMLetterInput)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "TextEdit", Data = currentPage.ObjectTypeToTag(typeof(SMTextEdit)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Label", Data = currentPage.ObjectTypeToTag(typeof(SMLabel)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Picture", Data = currentPage.ObjectTypeToTag(typeof(SMImage)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Selection", Data = currentPage.ObjectTypeToTag(typeof(SMSelection)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Image Button", Data = currentPage.ObjectTypeToTag(typeof(SMImageButton)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "CheckBox", Data = currentPage.ObjectTypeToTag(typeof(SMCheckBox)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Drawable", Data = currentPage.ObjectTypeToTag(typeof(SMDrawable)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Free Drawing", Data = currentPage.ObjectTypeToTag(typeof(SMFreeDrawing)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Text Container", Data = currentPage.ObjectTypeToTag(typeof(SMTextContainer)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Text Puzzle", Data = currentPage.ObjectTypeToTag(typeof(SMTextPuzzle)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Keyboard", Data = currentPage.ObjectTypeToTag(typeof(SMKeyboard)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Memory Game", Data = currentPage.ObjectTypeToTag(typeof(SMMemoryGame)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "OrderedList", Data = currentPage.ObjectTypeToTag(typeof(SMOrderedList)) });

                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Label (ChildrenText)", Data = currentPage.ObjectTypeToTag(typeof(SMLabel)), Args = "style=ChildrenText" });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Button: Next Page", Data = currentPage.ObjectTypeToTag(typeof(SMLabel)), Args = "text=Next >;onClick=(view showpage #next);style=NavigationButton;clickable=true", DefaultSize = new Size(128, 48) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Button: Previous Page", Data = currentPage.ObjectTypeToTag(typeof(SMLabel)), Args = "text=< Back;onClick=(view showpage #back);style=NavigationButton;clickable=true", DefaultSize = new Size(128, 48) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Button: Goto Page <text>", Data = currentPage.ObjectTypeToTag(typeof(SMLabel)), Args = "text=<text>;onClick=(view showpage \"PageName\");style=NavigationButton;clickable=true", DefaultSize = new Size(128, 48) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Horizontal Line", Data = currentPage.ObjectTypeToTag(typeof(SMDrawable)), Args = "drawings=line 0 50 100 50" });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Vertical Line", Data = currentPage.ObjectTypeToTag(typeof(SMDrawable)), Args = "drawings=line 50 0 50 100" });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "TextView (1 column + navig)", Data = currentPage.ObjectTypeToTag(typeof(SMTextView)), Args = "columns=1;navigbuttons=true" });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "TextView (2 columns + navig)", Data = currentPage.ObjectTypeToTag(typeof(SMTextView)), Args = "columns=2;navigbuttons=true" });
                /*listToolbox.Items.Add(new PageEditDraggableItem()
                {
                    Text = "GROUP: List of Labels",
                    Data = currentPage.ObjectTypeToTag(typeof(SMControlGroup))
                });
                listToolbox.Items.Add(new PageEditDraggableItem()
                {
                    Text = "GROUP: List of Images",
                    Data = currentPage.ObjectTypeToTag(typeof(SMControlGroup))

                });
                listToolbox.Items.Add(new PageEditDraggableItem()
                {
                    Text = "GROUP: List of Checkboxes",
                    Data = currentPage.ObjectTypeToTag(typeof(SMControlGroup))
                });*/
            }
            listToolbox.EndUpdate();
        }


        private void pageScrollArea1_NewPageRequested(object sender, PageEditViewArguments e)
        {
            if (NewPageRequested != null)
                NewPageRequested(sender, e);
        }

        private void pageScrollArea1_BackToParentView(object sender, EventArgs e)
        {
            if (BackToParentView != null)
                BackToParentView(sender, e);
        }

        public PageEditView GetEditView()
        {
            return pageScrollArea1.GetPageEditView();
        }

        public void RefreshView()
        {
            pageScrollArea1.InvalidateClient();
        }

        private void listToolbox_MouseDown(object sender, MouseEventArgs e)
        {
            int index = listToolbox.IndexFromPoint(e.X, e.Y);
            if (index >= 0)
            {
                DragDropEffects de = listToolbox.DoDragDrop(listToolbox.Items[index], DragDropEffects.Copy);
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            ImagesDialog dlg = new ImagesDialog();
            dlg.SetDocument(MNNotificationCenter.CurrentDocument);
            dlg.ShowDialog();
        }

        private void onButtonClick_StyleAdd(object sender, EventArgs e)
        {
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            string styleName = "";
            for (int i = 0; i < 99; i++)
            {
                styleName = string.Format("String{0}", i);
                if (doc.FindStyle(styleName) == null)
                    break;
            }

            if (styleName.Length > 0)
            {
                EditStyleName dlg = new EditStyleName();
                dlg.Document = doc;
                dlg.StyleName = styleName;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MNReferencedStyle sm = doc.GetDefaultStyle().CreateCopy();
                    sm.Name = dlg.StyleName;
                    doc.DefaultLanguage.Styles.Add(sm);
                    doc.DefaultLanguage.Modified = true;
                    listBoxStyles.Items.Add(sm);
                }
            }
        }

        private void buttonAddPage_Click(object sender, EventArgs e)
        {
        }

        private void addTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            MNPage nt = new MNPage(doc);
            nt.Id = doc.Data.GetNextId();
            nt.IsTemplate = true;
            nt.Title = "Template " + doc.Data.Templates.Count.ToString();
            nt.Description = "template for pages";
            doc.Data.Templates.Add(nt);
            RefreshListboxes(MNNotificationCenter.CurrentDocument);
        }

        private void listBoxImages_DrawItem(object sender, DrawItemEventArgs e)
        {
        }

        private void listBoxImages_MeasureItem(object sender, MeasureItemEventArgs e)
        {
        }

        private void listBoxPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            if (listBoxPages.SelectedIndex >= 0 &&
                listBoxPages.SelectedIndex < listBoxPages.Items.Count)
            {
                MNDocument document = MNNotificationCenter.CurrentDocument;
                int pageNo = listBoxPages.SelectedIndex;
                if (pageNo >= 0 && pageNo < document.Pages.Count)
                {
                    MNPage currentPage = document.Pages[pageNo];
                    MNNotificationCenter.CurrentPage = currentPage;

                    //pageScrollArea1.Dock = DockStyle.Fill;
                    pageScrollArea1.Visible = true;
                    pageScrollArea1.SetPage(currentPage);
                    pageScrollArea1.Invalidate();

                    MNNotificationCenter.BroadcastMessage(pageScrollArea1, "ObjectSelected", currentPage);
                }
                else if ((pageNo - document.Pages.Count) >= 0 && (pageNo - document.Pages.Count) < document.Templates.Count)
                {
                    MNPage template = document.Templates[pageNo - document.Pages.Count];
                    MNNotificationCenter.CurrentPage = template;

                    //pageScrollArea1.Dock = DockStyle.Fill;
                    pageScrollArea1.Visible = true;
                    pageScrollArea1.SetPage(template);
                    pageScrollArea1.Invalidate();

                    MNNotificationCenter.BroadcastMessage(pageScrollArea1, "ObjectSelected", template);
                }
            }*/
        }

        private void listBoxPages_MouseDown(object sender, MouseEventArgs e)
        {
            /*
            int index = listBoxImages.IndexFromPoint(e.X, e.Y);
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            if (index >= doc.Pages.Count && index < (doc.Pages.Count + doc.Templates.Count))
            {
                DragDropEffects de = listBoxImages.DoDragDrop(listBoxPages.Items[index], DragDropEffects.Copy);
            }*/
        }

        private void listBoxPages_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            /*
            MNPage image = listBoxPages.Items[e.Index] as MNPage;
            SizeF sizeOfText = e.Graphics.MeasureString("M", SystemFonts.MenuFont);
            e.ItemHeight = Convert.ToInt32(sizeOfText.Height * 2 + 12);
            image.ItemHeight = e.ItemHeight;
            image.ItemTextHeight = Convert.ToInt32(sizeOfText.Height);
            */
        }

        private void listBoxPages_DrawItem(object sender, DrawItemEventArgs e)
        {
            /*
            if (e.Index < 0)
                return;

            if ((e.State & DrawItemState.Selected) > 0)
            {
                e.Graphics.FillRectangle(Brushes.LightGreen, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
            }

            MNPage image = listBoxPages.Items[e.Index] as MNPage;
            SizeF sizeOfText = e.Graphics.MeasureString("M", SystemFonts.MenuFont);

            if (image.IsTemplate)
            {
                e.Graphics.DrawString("T" + (e.Index + 1 - image.Document.Pages.Count).ToString(), SMGraphics.GetFontVariation(SystemFonts.MenuFont, SystemFonts.MenuFont.Size * 2),
                    Brushes.DarkCyan, e.Bounds.X + 8, e.Bounds.Y + 8);
            }
            else
            {
                e.Graphics.DrawString((e.Index + 1).ToString(), SMGraphics.GetFontVariation(SystemFonts.MenuFont, SystemFonts.MenuFont.Size * 2),
                    Brushes.DarkCyan, e.Bounds.X + 8, e.Bounds.Y + 8);
            }
            e.Graphics.DrawString(image.Title, SystemFonts.MenuFont, Brushes.Black, e.Bounds.X + image.ItemHeight + 16, e.Bounds.Y + 4);
            e.Graphics.DrawString(image.Description, SystemFonts.MenuFont, Brushes.Gray, e.Bounds.X + image.ItemHeight + 8, e.Bounds.Y + 8 + sizeOfText.Height);
            */
        }

        private void listBoxImages_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private bool bStyle_OmitSetting = false;

        private void listBoxStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            MNReferencedStyle ss = listBoxStyles.SelectedItem as MNReferencedStyle;
            if (!bStyle_OmitSetting)
            {
                if (ss != null)
                {
                    MNPage p = MNNotificationCenter.CurrentPage;
                    foreach (SMControl c in p.Objects)
                    {
                        if (c.Area.Selected)
                            c.ApplyStyle(ss);
                    }
                    pageScrollArea1.InvalidateClient();
                }
            }
        }

        public void RefreshListboxes(MNDocument document)
        {
            propertyGrid2.SelectedObject = document.Book;
            listBoxPages.Items.Clear();
            int selid = -1;
            int i = 0;
            foreach (MNPage page in document.Data.Pages)
            {
                page.Index = i;
                if (page == MNNotificationCenter.CurrentPage)
                    selid = i;
                listBoxPages.Items.Add(page);
                i++;
            }
            if (selid >= 0)
            {
                b_omit_listbox_pages = true;
                listBoxPages.SelectedIndex = selid;
                b_omit_listbox_pages = false;
            }


            listBoxStyles.Items.Clear();
            foreach (MNReferencedStyle style in document.DefaultLanguage.Styles)
            {
                listBoxStyles.Items.Add(style);
            }
        }

        void INotificationTarget.OnNotificationReceived(object sender, string msg, params object[] args)
        {
            if (sender == this) return;

            switch (msg)
            {
                case "FilesAdded":
                    break;
                case "ObjectSelected":
                    if (args != null && args.Length > 0)
                    {
                        objectWasSelected(sender, args[0]);
                        //treeObjectView1.SelectItemWithData(args[0]);
                    }
                    break;
                case "DocumentChanged":
                    if (args != null && args.Length > 0 && args[0] is MNDocument)
                    {
                        MNDocument doc = args[0] as MNDocument;

                        RefreshListboxes(doc);
                        /*if (listBoxPages.Items.Count > 0)
                            listBoxPages.SelectedIndex = 0;*/
                        treeObjectView1.SetObject(doc);

                        if (doc.Data.Pages.Count > 0)
                            pageScrollArea1.SetPage(doc.Data.Pages[0]);

                        numericUpDown1.Value = doc.Book.DefaultFontSize;
                    }
                    break;
                case "PagesChanged":
                    RefreshListboxes(MNNotificationCenter.CurrentDocument);
                    break;
                case "NewPageInserted":
                    TVItem item = treeObjectView1.Tree;
                    if (item != null) item = item.FindChild("Pages");
                    if (item != null) item.RefreshChildren();
                    treeObjectView1.SelectItemWithData(args[0]);
                    treeObjectView1.Invalidate();
                    break;
                case "StyleListChanged":
                    RefreshListboxes(MNNotificationCenter.CurrentDocument);
                    break;
                case "keyAction":
                    if (args.Length > 0 && args[0] is Keys)
                    {
                        actionCenter.KeyActionMode((Keys)args[0]);
                    }
                    break;
                case "startKeyActionMode":
                    actionCenter.StartKeyActionMode();
                    break;
                case "stopKeyActionMode":
                    actionCenter.StopKeyActionMode();
                    break;
            }
        }

        private void objectWasSelected(object sender, object obj)
        {
            propertyGrid1.SelectedObject = obj;

            containerA.ClearPanels();

            // initialization of event scripts
            string currStyle = null;
            SMRectangleArea area = null;
            if (obj is SMRectangleArea)
            {
                ShowTab(0);
                area = (SMRectangleArea)obj;
                CurrentSelectedControl = null;
            }
            else if (obj is SMControl)
            {
                ShowTab(0);
                CurrentSelectedControl = obj as SMControl;
                currStyle = CurrentSelectedControl.StyleName;
                MNPage page = MNNotificationCenter.CurrentPage;
                if (page != null)
                {
                    area = CurrentSelectedControl.Area;
                }

                EVStorage.EvControlName.Instance.SetObject(obj as SMControl);
                containerA.AddPanel("General", EVStorage.EvControlName.Instance);

                if (obj is SMLabel)
                {
                    EVStorage.EvLabel.Instance.SetObject(obj as SMLabel);
                    containerA.AddPanel("Label", EVStorage.EvLabel.Instance);
                }

                if (obj is SMImage)
                {
                    EVStorage.EvReferencedImage.Instance.SetControl(obj as SMImage);
                    EVStorage.EvReferencedImage.Instance.HeaderText = "Image";
                    containerA.AddPanel("RefImage", EVStorage.EvReferencedImage.Instance);
                }

                if (obj is SMImageButton)
                {
                    SMImageButton smib = (SMImageButton)obj;
                    EVReferencedImage eri = EVStorage.EvReferencedImage.Instance;
                    eri.SetDocument(smib.Document);
                    eri.SetImage(smib.ImgA);
                    eri.HeaderText = "Normal State Image";
                    containerA.AddPanel("", eri);
                }

                if (obj is SMMemoryGame)
                {
                    EVStorage.EvMemoryGame.Instance.SetControl(obj as SMMemoryGame);
                    containerA.AddPanel("Memory Game", EVStorage.EvMemoryGame.Instance);
                }

                if (obj is SMOrderedList)
                {
                    EVStorage.EvOrderedList.Instance.SetObject(obj as SMOrderedList);
                    containerA.AddPanel("Ordered List", EVStorage.EvOrderedList.Instance);
                }

                EVStorage.EvControlStyle.Instance.SetObject(obj as SMControl);
                containerA.AddPanel("Style", EVStorage.EvControlStyle.Instance);

                EVStorage.EvControlScripts.Instance.SetControl(obj as SMControl);
                containerA.AddPanel("Scripts", EVStorage.EvControlScripts.Instance);

                richTextFreeProperties.Text = ((SMControl)obj).PropertiesText;
            }
            else if (obj is MNPage)
            {
                CurrentSelectedControl = null;

                MNPage page = obj as MNPage;
                ShowTab(0);
                InitializeControlList(page);
                area = page.Area;
                page.ClearSelection();
                pageScrollArea1.GetPageEditView().ClearSelection();

                EVStorage.EvPageName.Instance.SetObject(page);
                containerA.AddPanel("Page", EVStorage.EvPageName.Instance);

                pageNotesChangeNotify = false;
                richTextBoxPageNotes.Text = page.MessageText;
                pageNotesChangeNotify = true;
            }
            else if (obj is MNDocument)
            {
                propertyGrid1.SelectedObject = (obj as MNDocument).Book;
                ShowTab(0);
                CurrentSelectedControl = null;
            }

            if (area != null)
            {
                CurrentSelectedArea = area;
            }

            if (currStyle != null)
            {
                for (int i = 0; i < listBoxStyles.Items.Count; i++)
                {
                    if ((listBoxStyles.Items[i] as MNReferencedStyle).Name == currStyle)
                    {
                        bStyle_OmitSetting = true;
                        listBoxStyles.SelectedIndex = i;
                        bStyle_OmitSetting = false;
                        break;
                    }
                }
            }
        }

        private TVItem p_action_item = null;

        public void ShowTab(int i)
        {
            tabControl2.SelectedIndex = i;
        }

        private void treeObjectView1_OnInitializeActionMenu(object sender, TreeObjectViewEventArgs e)
        {
            p_action_item = e.Item;

            if (p_action_item == null) return;

            NABase[] pa = p_action_item.GetActions();

            if (pa != null && pa.Length > 0)
            {
                contextMenuStrip1.Items.Clear();
                foreach (NABase s in pa)
                {
                    ToolStripItem tsi = contextMenuStrip1.Items.Add(s.Title);
                    tsi.Tag = s;
                    tsi.Click += new EventHandler(tsi_Click);
                }

                contextMenuStrip1.Show(e.ScreenPoint);
            }
        }

        void tsi_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                ToolStripItem tsi = sender as ToolStripItem;
                if (tsi.Tag != null && tsi.Tag is NABase)
                {
                    (tsi.Tag as NABase).Execute();
                }
            }
        }

        private NABase[] GetActionsForObject(GSCore cr)
        {
            if (cr is MNPage)
            {
                return new NABase[]
                {
                };
            }
            else if (cr is MNDocument)
            {
            }

            return null;
        }

        private void ProcessObjectMessage(GSCore core, string msg)
        {
            if (core is MNPage)
            {
                switch (msg)
                {
                    default: break;
                }
            }
            else if (core is MNDocument)
            {
                switch (msg)
                {
                    default: break;
                }
            }
        }

        private void tabControlDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Debugger.Log(0, "", "Selected Tab: " + tabControlDetails.SelectedIndex + "\n");
            if (tabControlDetails.SelectedTab == tabActions)
            {
                actionCenter.IsKeyActionMode = true;
            }
            else
            {
                actionCenter.IsKeyActionMode = false;
            }
        }

        public KeyPageActions actionCenter = new KeyPageActions();

        private void toolStripButtonInsertPages_Click(object sender, EventArgs e)
        {
            MNPage Page = null;
            if (listBoxPages.SelectedIndex >= 0 &&
                listBoxPages.SelectedIndex < listBoxPages.Items.Count)
            {
                MNDocument document = MNNotificationCenter.CurrentDocument;
                int pageNo = listBoxPages.SelectedIndex;
                if (pageNo >= 0 && pageNo < document.Data.Pages.Count)
                {
                    Page = document.Data.Pages[pageNo];
                }
            }

            if (Page != null)
            {
                DialogEnterPageNames dlg = new DialogEnterPageNames();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string[] names = dlg.Names;
                    if (names != null && names.Length > 0)
                    {
                        List<MNPage> pages = Page.Document.Data.Pages;
                        int idx = pages.IndexOf(Page);
                        if (idx >= 0 && idx < pages.Count)
                        {
                            foreach (string pageName in names)
                            {
                                idx++;
                                MNPage p = new MNPage(Page.Document);
                                p.Title = pageName;
                                p.Id = Page.Document.Data.GetNextId();
                                pages.Insert(idx, p);
                            }
                        }

                        int seli = listBoxPages.SelectedIndex;
                        RefreshListboxes(Page.Document);
                        listBoxPages.SelectedIndex = seli;

                        MNNotificationCenter.BroadcastMessage(Page, "PageInserted");
                    }
                }
            }

        }

        private bool b_omit_listbox_pages = false;

        private void listBoxPages_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (b_omit_listbox_pages)
                return;

            if (listBoxPages.SelectedIndex >= 0 &&
                listBoxPages.SelectedIndex < listBoxPages.Items.Count)
            {
                MNDocument document = MNNotificationCenter.CurrentDocument;
                int pageNo = listBoxPages.SelectedIndex;
                if (pageNo >= 0 && pageNo < document.Data.Pages.Count)
                {
                    MNPage currentPage = document.Data.Pages[pageNo];
                    MNNotificationCenter.CurrentPage = currentPage;

                    //pageScrollArea1.Dock = DockStyle.Fill;
                    pageScrollArea1.Visible = true;
                    pageScrollArea1.SetPage(currentPage);
                    pageScrollArea1.Invalidate();

                    MNNotificationCenter.BroadcastMessage(pageScrollArea1, "ObjectSelected", currentPage);
                }
            }
        }

        private void tsbClonePage_Click(object sender, EventArgs e)
        {
            if (listBoxPages.SelectedIndex >= 0 &&
                listBoxPages.SelectedIndex < listBoxPages.Items.Count)
            {
                MNDocument document = MNNotificationCenter.CurrentDocument;
                int pageNo = listBoxPages.SelectedIndex;
                if (pageNo >= 0 && pageNo < document.Data.Pages.Count)
                {
                    MNPage currentPage = document.Data.Pages[pageNo];
                    MNPage newPage = null;

                    DialogCloneCounter dcc = new DialogCloneCounter();
                    if (dcc.ShowDialog() == DialogResult.OK)
                    {
                        for (int i = dcc.DialogValue_StartInt; i <= dcc.DialogValue_EndInt; i++)
                        {
                            newPage = new MNPage(document);
                            MNPage.CopyControlsFrom(currentPage, newPage);
                            newPage.Id = currentPage.Document.Data.GetNextId();
                            newPage.Title = string.Format("{0}{1}", dcc.DialogValue_Prefix, i);
                            newPage.Description = currentPage.Description;
                            newPage.MessageText = currentPage.MessageText;
                            newPage.MessageTitle = currentPage.MessageTitle;
                            newPage.TextB = currentPage.TextB;
                            newPage.TextC = currentPage.TextC;
                            newPage.ShowHelp = currentPage.ShowHelp;
                            newPage.ShowHome = currentPage.ShowHome;
                            newPage.ShowBackNavigation = currentPage.ShowBackNavigation;
                            newPage.ShowTitle = currentPage.ShowTitle;
                            newPage.ShowForwardNavigation = currentPage.ShowForwardNavigation;
                            // increment for inserting into listbox and also for next iteration
                            pageNo++;
                            document.Data.Pages.Insert(pageNo, newPage);
                            listBoxPages.Items.Insert(pageNo, newPage);
                            listBoxPages.SelectedIndex = pageNo;
                        }

                        if (newPage != null)
                        {
                            MNNotificationCenter.CurrentPage = newPage;

                            //pageScrollArea1.Dock = DockStyle.Fill;
                            pageScrollArea1.Visible = true;
                            pageScrollArea1.SetPage(newPage);
                            pageScrollArea1.Invalidate();

                            MNNotificationCenter.BroadcastMessage(pageScrollArea1, "ObjectSelected", newPage);
                        }
                    }
                }
            }
        }

        private void tsbAddTemplate(object sender, EventArgs e)
        {
            MNPage p = new MNPage(MNSharedObjects.internalDocument);
            MNSharedObjects.internalDocument.Data.Templates.Add(p);

            DialogNewPageName d = new DialogNewPageName();
            if (d.ShowDialog() == DialogResult.OK)
            {
                p.Title = d.PageName;
            }

            listBoxTemplates.Items.Add(p);
        }


        public void SetSharedDocument()
        {
            listBoxTemplates.BeginUpdate();
            listBoxTemplates.Items.Clear();
            foreach (MNPage p in MNSharedObjects.internalDocument.Data.Templates)
            {
                listBoxTemplates.Items.Add(p);
            }
            listBoxTemplates.EndUpdate();
        }

        public void RefreshTemplateList()
        {
            listBoxTemplates.BeginUpdate();
            listBoxTemplates.Items.Clear();
            foreach (MNPage p in MNSharedObjects.internalDocument.Data.Templates)
            {
                listBoxTemplates.Items.Add(p);
            }
            listBoxTemplates.EndUpdate();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            MNSharedObjects.Save();
        }

        private void listBoxTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplates.SelectedIndex >= 0 &&
                listBoxTemplates.SelectedIndex < listBoxTemplates.Items.Count)
            {
                MNPage currentPage = listBoxTemplates.Items[listBoxTemplates.SelectedIndex] as MNPage;
                if (currentPage != null)
                {
                    MNNotificationCenter.CurrentPage = currentPage;

                    //pageScrollArea1.Dock = DockStyle.Fill;
                    pageScrollArea1.Visible = true;
                    pageScrollArea1.SetPage(currentPage);
                    pageScrollArea1.Invalidate();

                    MNNotificationCenter.BroadcastMessage(pageScrollArea1, "ObjectSelected", currentPage);
                }
            }
        }

        /// <summary>
        /// REFRESH PAGE LIST
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            RefreshListboxes(MNNotificationCenter.CurrentDocument);
        }

        /// <summary>
        /// DELETE PAGE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (listBoxPages.SelectedIndex >= 0 && listBoxPages.SelectedIndex < listBoxPages.Items.Count)
            {
                MNPage page = (MNPage)listBoxPages.Items[listBoxPages.SelectedIndex];
                if (MessageBox.Show("Delete selected page?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    MNDocument doc = MNNotificationCenter.CurrentDocument;
                    doc.Data.Pages.Remove(page);
                    RefreshListboxes(MNNotificationCenter.CurrentDocument);
                }
            }
        }

        /// <summary>
        /// Child font set 32 pt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label9_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Font.Size = Convert.ToInt32(numericUpDown1.Value);
                        c.Font.Name = MNNotificationCenter.CurrentDocument.Book.DefaultFontName;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        private void tabPageStyles_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        ///  align top left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Paragraph.VertAlign = SMVerticalAlign.Top;
                        c.Paragraph.Align = SMHorizontalAlign.Left;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// align top center
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Paragraph.VertAlign = SMVerticalAlign.Top;
                        c.Paragraph.Align = SMHorizontalAlign.Center;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// align top right
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Paragraph.VertAlign = SMVerticalAlign.Top;
                        c.Paragraph.Align = SMHorizontalAlign.Right;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// align left center
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Paragraph.VertAlign = SMVerticalAlign.Center;
                        c.Paragraph.Align = SMHorizontalAlign.Left;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// align center
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Paragraph.VertAlign = SMVerticalAlign.Center;
                        c.Paragraph.Align = SMHorizontalAlign.Center;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// align right center
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Paragraph.VertAlign = SMVerticalAlign.Center;
                        c.Paragraph.Align = SMHorizontalAlign.Right;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// align bottom left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Paragraph.VertAlign = SMVerticalAlign.Bottom;
                        c.Paragraph.Align = SMHorizontalAlign.Left;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// align bottom center
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Paragraph.VertAlign = SMVerticalAlign.Bottom;
                        c.Paragraph.Align = SMHorizontalAlign.Center;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// align bottom right
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Paragraph.VertAlign = SMVerticalAlign.Bottom;
                        c.Paragraph.Align = SMHorizontalAlign.Right;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// set 11 pt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label7_Click(object sender, EventArgs e)
        {
            SetFontSizeForSelection(11);
        }

        /// <summary>
        /// set 16 pt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label8_Click(object sender, EventArgs e)
        {
            SetFontSizeForSelection(16);
        }

        private void SetFontSizeForSelection(int fontSize)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Font.Size = fontSize;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        /// <summary>
        /// set 50 pt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label10_Click(object sender, EventArgs e)
        {
            SetFontSizeForSelection(40);
        }

        /// <summary>
        /// set times
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Font.Name = MNFontName.Times;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        /// <summary>
        /// set vag rounded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Font.Name = MNFontName.VagRounded;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        /// <summary>
        /// set berlin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Font.Name = MNFontName.BerlinSansFB;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        /// <summary>
        /// set gill sans
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Font.Name = MNFontName.GilSansMurari;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        /// <summary>
        /// set garamond
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Font.Name = MNFontName.AdobeGaramondPro;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (MNNotificationCenter.CurrentDocument != null)
            {
                MNNotificationCenter.CurrentDocument.Book.DefaultFontSize = Convert.ToInt32(numericUpDown1.Value);
            }
        }

        void pageScrollArea1_PrevPageRequested(object sender, PageEditViewArguments e)
        {
            ListBox lb = listBoxPages;
            int si = lb.SelectedIndex;
            if (si >= 0 && si < lb.Items.Count)
            {
                si--;
                if (si >= 0 && si < lb.Items.Count)
                {
                    lb.SelectedIndex = si;
                }
            }
        }

        void pageScrollArea1_NextPageRequested(object sender, PageEditViewArguments e)
        {
            ListBox lb = listBoxPages;
            int si = lb.SelectedIndex;
            if (si >= 0 && si < lb.Items.Count)
            {
                si++;
                if (si >= 0 && si < lb.Items.Count)
                {
                    lb.SelectedIndex = si;
                }
            }
        }

        /// <summary>
        /// Delete template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ListBox lb = listBoxTemplates;
            if (lb.SelectedIndex >= 0 && lb.SelectedIndex < lb.Items.Count)
            {
                MNPage p = lb.Items[lb.SelectedIndex] as MNPage;
                if (MessageBox.Show("Delete template " + p.Title + "?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MNSharedObjects.internalDocument.Data.Templates.Remove(p);
                    RefreshTemplateList();
                }
            }
        }

        /// <summary>
        ///  refresh list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            RefreshTemplateList();
        }

        private void label13_Click(object sender, EventArgs e)
        {
            SetFontSizeForSelection(20);
        }

        private void label12_Click(object sender, EventArgs e)
        {
            SetFontSizeForSelection(12);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.Font.Bold = true;
                        c.NormalState.ForeColor = Color.White;
                        c.NormalState.BackColor = Color.Black;
                        c.NormalState.CornerRadius = 30;
                        c.NormalState.BorderColor = Color.Black;
                        c.NormalState.BorderStyle = SMBorderStyle.RoundRectangle;
                        c.HighlightState.ForeColor = Color.White;
                        c.HighlightState.BackColor = Color.DarkBlue;
                        c.HighlightState.CornerRadius = 30;
                        c.HighlightState.BorderStyle = SMBorderStyle.RoundRectangle;
                        c.HighlightState.BorderColor = Color.DarkBlue;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.NormalState.ForeColor = Color.Black;
                        c.NormalState.BackColor = Color.AntiqueWhite;
                        c.NormalState.BorderStyle = SMBorderStyle.None;
                        c.NormalState.CornerRadius = 0;
                        c.HighlightState.ForeColor = Color.Black;
                        c.HighlightState.BackColor = Color.NavajoWhite;
                        c.HighlightState.CornerRadius = 0;
                        c.HighlightState.BorderStyle = SMBorderStyle.None;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            SetSelectionBorderWidth(1);
        }

        private void SetSelectionBorderWidth(int w)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.NormalState.BorderWidth = w;
                        c.HighlightState.BorderWidth = w;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        private void SetSelectionBorderType(SMBorderStyle bs)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.NormalState.BorderStyle = bs;
                        c.HighlightState.BorderStyle = bs;
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            SetSelectionBorderWidth(2);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            SetSelectionBorderWidth(4);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            SetSelectionBorderType(SMBorderStyle.None);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            SetSelectionBorderType(SMBorderStyle.Rectangle);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            SetSelectionBorderType(SMBorderStyle.RoundRectangle);
        }

        private void SetSelectionPadding(SMControlSelection cs, int value)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        switch (cs)
                        {
                            case SMControlSelection.Left:
                                c.ContentPadding.Left = value;
                                break;
                            case SMControlSelection.Right:
                                c.ContentPadding.Right = value;
                                break;
                            case SMControlSelection.Top:
                                c.ContentPadding.Top = value;
                                break;
                            case SMControlSelection.Bottom:
                                c.ContentPadding.Bottom = value;
                                break;
                        }
                        c.StyleDidChange();
                    }
                }
                pageScrollArea1.InvalidateClient();
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Left, 4);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Left, 8);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Left, 16);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Top, 4);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Top, 8);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Top, 16);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Right, 4);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Right, 8);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Right, 16);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Bottom, 4);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Bottom, 8);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            SetSelectionPadding(SMControlSelection.Bottom, 16);
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            // go down
            MNDocument doc = MNNotificationCenter.CurrentDocument;

            int i = listBoxPages.SelectedIndex;
            if (i >= 0 && i < doc.Data.Pages.Count - 1)
            {
                MNPage p = doc.Data.Pages[i];
                doc.Data.Pages.RemoveAt(i);
                doc.Data.Pages.Insert(i + 1, p);

                listBoxPages.Items.RemoveAt(i);
                listBoxPages.Items.Insert(i + 1, p);
                listBoxPages.SelectedIndex = i + 1;
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            // go up
            // go down
            MNDocument doc = MNNotificationCenter.CurrentDocument;

            int i = listBoxPages.SelectedIndex;
            if (i > 0 && i < doc.Data.Pages.Count)
            {
                MNPage p = doc.Data.Pages[i];
                doc.Data.Pages.RemoveAt(i);
                doc.Data.Pages.Insert(i - 1, p);

                listBoxPages.Items.RemoveAt(i);
                listBoxPages.Items.Insert(i - 1, p);
                listBoxPages.SelectedIndex = i - 1;
            }
        }


        public void SetSelectionExpChecked(Bool3 expStatus)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.ExpectedChecked = expStatus;
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        public void SetSelectionImgScalling(SMContentScaling expStatus)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected && c is SMImage)
                    {
                        SMImage si = (SMImage)c;
                        si.ContentScaling = expStatus;
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        public void SetContentToTags()
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        if (c is SMTextContainer)
                        {
                            (c as SMTextContainer).ContentToTags();
                        }
                        else if (c is SMTextView)
                        {
                            (c as SMTextView).ContentToTags();
                        }
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        private void button35_Click(object sender, EventArgs e)
        {
            SetSelectionExpChecked(Bool3.False);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            SetSelectionExpChecked(Bool3.True);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            SetSelectionImgScalling(SMContentScaling.Fit);
        }

        private void button38_Click(object sender, EventArgs e)
        {
            SetSelectionImgScalling(SMContentScaling.Fill);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            if (!bStyle_OmitSetting)
            {
                MNPage p = MNNotificationCenter.CurrentPage;
                foreach (SMControl c in p.Objects)
                {
                    if (c.Area.Selected)
                    {
                        c.HighlightState.BackColor = Color.DarkTurquoise;
                    }
                }
                pageScrollArea1.InvalidateClient();
            }

        }

        private void button40_Click(object sender, EventArgs e)
        {
            SetSelectionExpChecked(Bool3.Both);
        }

        private void button41_Click(object sender, EventArgs e)
        {
            MNPage p = MNNotificationCenter.CurrentPage;

            for (int i = 1; i < 10; i++)
            {
                string groupName = string.Format("g{0}", i);
                if (!p.HasGroup(groupName))
                {
                    PageEditView pev = pageScrollArea1.GetPageEditView();
                    pev.MakeGroup(1, groupName);
                    break;
                }
            }
        }

        private void button42_Click(object sender, EventArgs e)
        {
            SetContentToTags();
        }

        private void button43_Click(object sender, EventArgs e)
        {
            MNPage p = MNNotificationCenter.CurrentPage;
            foreach (SMControl c in p.Objects)
            {
                if (c.Area.Selected)
                {
                    c.ScriptOnClick = "(control toogleCheck)";
                    c.Clickable = true;
                }
            }
            pageScrollArea1.InvalidateClient();
        }

        private void button44_Click(object sender, EventArgs e)
        {
            MNPage p = MNNotificationCenter.CurrentPage;
            foreach (SMControl c in p.Objects)
            {
                if (c.Area.Selected)
                {
                    c.ScriptOnClick = "";
                    c.Clickable = true;
                }
            }
            pageScrollArea1.InvalidateClient();
        }

        private void button45_Click(object sender, EventArgs e)
        {
            MNPage p = MNNotificationCenter.CurrentPage;
            foreach (SMControl c in p.Objects)
            {
                if (c.Area.Selected)
                {
                    c.StyleName = "_brief";
                    c.StyleDidChange();
                }
            }
            pageScrollArea1.InvalidateClient();
        }

        private bool pageNotesChangeNotify = true;
        private void richTextBoxPageNotes_TextChanged(object sender, EventArgs e)
        {
            if (pageNotesChangeNotify)
            {
                PageEditView pev = pageScrollArea1.GetPageEditView();
                if (pev != null && pev.Page != null)
                {
                    pev.Page.MessageText = richTextBoxPageNotes.Text;
                }
            }
        }

        private void richTextFreeProperties_TextChanged(object sender, EventArgs e)
        {
            if (CurrentSelectedControl != null)
            {
                CurrentSelectedControl.PropertiesText = richTextFreeProperties.Text;
                CurrentSelectedControl.ProcessProperties(richTextFreeProperties.Text);
            }
        }
    }
}
