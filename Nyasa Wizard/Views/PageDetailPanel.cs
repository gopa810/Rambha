using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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

            propertyPanelsContainer1.EditView = pageScrollArea1.GetPageEditView();
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
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "CheckBox", Data = currentPage.ObjectTypeToTag(typeof(SMCheckBox)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Drawable", Data = currentPage.ObjectTypeToTag(typeof(SMDrawable)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Free Drawing", Data = currentPage.ObjectTypeToTag(typeof(SMFreeDrawing)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Text Container", Data = currentPage.ObjectTypeToTag(typeof(SMTextContainer)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Text Puzzle", Data = currentPage.ObjectTypeToTag(typeof(SMTextPuzzle)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Keyboard", Data = currentPage.ObjectTypeToTag(typeof(SMKeyboard)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Memory Game", Data = currentPage.ObjectTypeToTag(typeof(SMMemoryGame)) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Button: Next Page", Data = currentPage.ObjectTypeToTag(typeof(SMLabel)), Args = "text=Next >;script=OnClick:(view showpage #next);style=NavigationButton;clickable=true", DefaultSize = new Size(128, 48) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Button: Previous Page", Data = currentPage.ObjectTypeToTag(typeof(SMLabel)), Args = "text=< Back;script=OnClick:(view showpage #back);style=NavigationButton;clickable=true", DefaultSize = new Size(128, 48) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Button: Goto Page <text>", Data = currentPage.ObjectTypeToTag(typeof(SMLabel)), Args = "text=<text>;script=OnClick:(view showpage PageName);style=NavigationButton;clickable=true", DefaultSize = new Size(128, 48) });
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Page Header", Data = currentPage.ObjectTypeToTag(typeof(SMLabel)), Args = "text=Page Header;style=PageHeader", DefaultSize = new Size(512, 48) });
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
                if (ss != null && CurrentSelectedControl != null && CurrentSelectedControl.Style != ss)
                {
                    CurrentSelectedControl.Style = ss;
                    pageScrollArea1.InvalidateClient();
                }
            }
        }

        public void RefreshListboxes(MNDocument document)
        {
            listBoxStyles.Items.Clear();
            foreach (MNReferencedStyle style in document.DefaultLanguage.Styles)
            {
                listBoxStyles.Items.Add(style);
            }
        }

        void INotificationTarget.OnNotificationReceived(object sender, string msg, params object[] args)
        {
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
            }
        }

        private void objectWasSelected(object sender, object obj)
        {
            propertyGrid1.SelectedObject = obj;

            propertyPanelsContainer1.ClearPanels();

            // initialization of event scripts
            SMRectangleArea area = null;
            MNReferencedStyle currStyle = null;
            if (obj is SMRectangleArea)
            {
                ShowTab(0);
                area = (SMRectangleArea)obj;
            }
            else if (obj is SMControl)
            {
                ShowTab(0);
                CurrentSelectedControl = obj as SMControl;
                currStyle = CurrentSelectedControl.Style;
                MNPage page = MNNotificationCenter.CurrentPage;
                if (page != null)
                {
                    area = page.GetArea(CurrentSelectedControl.Id);
                }

                EVControlName se = (EVControlName)EVStorage.GetSafeControl(typeof(EVControlName));
                se.SetObject(obj as SMControl);
                propertyPanelsContainer1.AddPanel("General", se);

                if (obj is SMMemoryGame)
                {
                    EVMemoryGame emg = (EVMemoryGame)EVStorage.GetSafeControl(typeof(EVMemoryGame));
                    emg.SetControl(obj as SMMemoryGame);
                    propertyPanelsContainer1.AddPanel("Memory Game", emg);
                }
            }
            else if (obj is MNPage)
            {
                ShowTab(0);
                InitializeControlList(obj as MNPage);
                area = (obj as MNPage).Area;
            }
            else if (obj is MNDocument)
            {
                propertyGrid1.SelectedObject = (obj as MNDocument).Book;
                ShowTab(0);
            }
            else if (obj is MNReferencedText)
            {
                ShowTab(1);
                p_editedRefText = obj as MNReferencedText;
                textPanel_name.Text = p_editedRefText.Name;
                textPanel_text.Text = p_editedRefText.Text;
            }
            else if (obj is MNMenu)
            {
                ShowTab(2);
                p_editedMenu = obj as MNMenu;
                docMenuEditView1.Menu = p_editedMenu;
            }

            if (area != null)
            {
                CurrentSelectedArea = area;
            }

            if (currStyle != null)
            {
                for (int i = 0; i < listBoxStyles.Items.Count; i++)
                {
                    if (listBoxStyles.Items[i] == currStyle)
                    {
                        listBoxStyles.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private TVItem p_action_item = null;
        private MNReferencedText p_editedRefText = null;
        private MNMenu p_editedMenu = null;

        public void ShowTab(int i)
        {
            if (p_editedRefText != null)
            {
                p_editedRefText.Name = textPanel_name.Text;
                p_editedRefText.Text = textPanel_text.Text;
                p_editedRefText = null;
                textPanel_name.Text = "";
                textPanel_text.Text = "";
            }

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

    }
}
