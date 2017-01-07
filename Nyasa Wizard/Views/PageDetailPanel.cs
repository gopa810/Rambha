using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

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

        }

        public MNReferencedImage DefaultDocImage
        {
            get
            {
                MNReferencedImage image = new MNReferencedImage(null);
                image.Title = "(default)";
                image.ImageData = Properties.Resources.DefaultImage;
                return image;
            }
        }

        public void InitializeControlList(MNPage currentPage)
        {
            listToolbox.BeginUpdate();
            listToolbox.Items.Clear();
            if (currentPage != null)
            {
                listToolbox.Items.Add(new PageEditDraggableItem() { Text = "TextView", Data = currentPage.ObjectTypeToTag(typeof(SMTextView)) });
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
                    SMStyle sm = doc.GetDefaultStyle().CreateCopy();
                    sm.Name = dlg.StyleName;
                    doc.Styles.Add(sm);
                    listBoxStyles.Items.Add(sm);
                }
            }
        }

        private void buttonAddPage_Click(object sender, EventArgs e)
        {
            int index = listBoxPages.SelectedIndex;
            if (index < 0) index = 0;
            MNNotificationCenter.CurrentDocument.CreateNewPage();
            RefreshListboxes(MNNotificationCenter.CurrentDocument);
        }

        private void addTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            MNPage nt = new MNPage(doc);
            nt.Id = doc.GetNextId();
            nt.IsTemplate = true;
            nt.Title = "Template " + doc.Templates.Count.ToString();
            nt.Description = "template for pages";
            doc.Templates.Add(nt);
            RefreshListboxes(MNNotificationCenter.CurrentDocument);
        }
        private void buttonAddImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;

            MNDocument Document = MNNotificationCenter.CurrentDocument;
            if (Document == null)
                return;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string sFileName in dlg.FileNames)
                    {
                        Document.AcceptFile(sFileName);
                    }
                    listBoxImages.Items.Clear();
                    foreach (MNReferencedImage img in Document.Images)
                    {
                        listBoxImages.Items.Add(img);
                    }
                }
                catch (BadImageFormatException bfe)
                {
                    MessageBox.Show("Invalid format of image.\nImage is not loaded.\n\n" + bfe.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during loading of image.\n\n" + ex.Message);
                }

            }
        }

        private void buttonDeleteImage_Click(object sender, EventArgs e)
        {
            MNDocument Document = MNNotificationCenter.CurrentDocument;
            if (Document == null)
                return;

            if (listBoxImages.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Delete images?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<MNReferencedImage> imgs = new List<MNReferencedImage>();
                    foreach (MNReferencedImage im in listBoxImages.SelectedItems)
                    {
                        imgs.Add(im);
                    }
                    foreach (MNReferencedImage im in imgs)
                    {
                        Document.Images.Remove(im);
                    }
                }
            }
        }

        private void listBoxImages_DrawItem(object sender, DrawItemEventArgs e)
        {
            if ((e.State & DrawItemState.Selected) > 0)
            {
                e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
            }

            if (e.Index >= 0)
            {
                MNReferencedImage image = listBoxImages.Items[e.Index] as MNReferencedImage;
                SizeF sizeOfText = e.Graphics.MeasureString("M", SystemFonts.MenuFont);
                if (image != null && image.ImageData != null)
                {
                    int sz = Math.Max(image.ImageData.Height, image.ImageData.Width);
                    double change = image.ItemHeight / Convert.ToDouble(sz);
                    Rectangle rect = new Rectangle(0, 0, Convert.ToInt32(image.ImageData.Width * change),
                        Convert.ToInt32(image.ImageData.Height * change));
                    if (rect.Width > rect.Height)
                    {
                        rect.Y = (image.ItemHeight - rect.Height) / 2;
                    }
                    else
                    {
                        rect.X = (image.ItemHeight - rect.Width) / 2;
                    }

                    rect.X += e.Bounds.X + 8;
                    rect.Y += e.Bounds.Y + 4;

                    e.Graphics.DrawImage(image.ImageData, rect);
                    e.Graphics.DrawString(image.Title, SystemFonts.MenuFont, Brushes.Black, e.Bounds.X + image.ItemHeight + 16, e.Bounds.Y + 4);
                    e.Graphics.DrawString(image.Description, SystemFonts.MenuFont, Brushes.Gray, e.Bounds.X + image.ItemHeight + 8, e.Bounds.Y + 8 + sizeOfText.Height);
                }
            }
        }

        private void listBoxImages_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            MNReferencedImage image = listBoxImages.Items[e.Index] as MNReferencedImage;
            SizeF sizeOfText = e.Graphics.MeasureString("M", SystemFonts.MenuFont);
            e.ItemHeight = Convert.ToInt32(sizeOfText.Height * 2 + 12);
            image.ItemHeight = e.ItemHeight;
            image.ItemTextHeight = Convert.ToInt32(sizeOfText.Height);
        }

        private void listBoxPages_SelectedIndexChanged(object sender, EventArgs e)
        {
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
            }
        }

        private void listBoxPages_MouseDown(object sender, MouseEventArgs e)
        {
            int index = listBoxImages.IndexFromPoint(e.X, e.Y);
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            if (index >= doc.Pages.Count && index < (doc.Pages.Count + doc.Templates.Count))
            {
                DragDropEffects de = listBoxImages.DoDragDrop(listBoxPages.Items[index], DragDropEffects.Copy);
            }
        }

        private void listBoxPages_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            MNPage image = listBoxPages.Items[e.Index] as MNPage;
            SizeF sizeOfText = e.Graphics.MeasureString("M", SystemFonts.MenuFont);
            e.ItemHeight = Convert.ToInt32(sizeOfText.Height * 2 + 12);
            image.ItemHeight = e.ItemHeight;
            image.ItemTextHeight = Convert.ToInt32(sizeOfText.Height);

        }

        private void listBoxPages_DrawItem(object sender, DrawItemEventArgs e)
        {
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
            /*if (image != null && image.ImageData != null)
            {
                int sz = Math.Max(image.ImageData.Height, image.ImageData.Width);
                double change = image.ItemHeight / Convert.ToDouble(sz);
                Rectangle rect = new Rectangle(0, 0, Convert.ToInt32(image.ImageData.Width * change),
                    Convert.ToInt32(image.ImageData.Height * change));
                if (rect.Width > rect.Height)
                {
                    rect.Y = (image.ItemHeight - rect.Height) / 2;
                }
                else
                {
                    rect.X = (image.ItemHeight - rect.Width) / 2;
                }

                rect.X += e.Bounds.X + 8;
                rect.Y += e.Bounds.Y + 4;

                e.Graphics.DrawImage(image.ImageData, rect);
            }*/

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
        }

        private void listBoxImages_MouseDown(object sender, MouseEventArgs e)
        {
            int index = listBoxImages.IndexFromPoint(e.X, e.Y);
            if (index >= 0)
            {
                DragDropEffects de = listBoxImages.DoDragDrop(listBoxImages.Items[index], DragDropEffects.Copy);
            }
        }

        private bool bStyle_OmitSetting = false;

        private void listBoxStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGridStyle.SelectedObject = listBoxStyles.SelectedItem;
            SMStyle ss = listBoxStyles.SelectedItem as SMStyle;
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
            listBoxPages.Items.Clear();
            foreach (MNPage page in document.Pages)
            {
                listBoxPages.Items.Add(page);
            }
            foreach (MNPage template in document.Templates)
            {
                listBoxPages.Items.Add(template);
            }

            listBoxImages.Items.Clear();
            foreach (MNReferencedImage img in document.Images)
            {
                listBoxImages.Items.Add(img);
            }

            listBoxStyles.Items.Clear();
            foreach (SMStyle style in document.Styles)
            {
                listBoxStyles.Items.Add(style);
            }
        }

        void INotificationTarget.OnNotificationReceived(object sender, string msg, params object[] args)
        {
            switch (msg)
            {
                case "FilesAdded":
                    listBoxImages.Items.Clear();
                    MNDocument document = MNNotificationCenter.CurrentDocument;
                    foreach (MNReferencedImage img in document.Images)
                    {
                        listBoxImages.Items.Add(img);
                    }
                    break;
                case "ObjectSelected":
                    if (args != null && args.Length > 0)
                        objectWasSelected(sender, args[0]);
                    break;
                case "DocumentChanged":
                    if (args != null && args.Length > 0 && args[0] is MNDocument)
                    {
                        RefreshListboxes(args[0] as MNDocument);
                        if (listBoxPages.Items.Count > 0)
                            listBoxPages.SelectedIndex = 0;
                    }
                    break;
                case "PagesChanged":
                    RefreshListboxes(MNNotificationCenter.CurrentDocument);
                    break;
            }
        }

        private void objectWasSelected(object sender, object obj)
        {
            propertyGrid1.SelectedObject = obj;

            // initialization of event scripts
            SMRectangleArea area = null;
            SMStyle currStyle = null;
            if (obj is SMRectangleArea)
            {
                area = (SMRectangleArea)obj;
            }
            else if (obj is SMControl)
            {
                CurrentSelectedControl = obj as SMControl;
                currStyle = CurrentSelectedControl.Style;
                MNPage page = MNNotificationCenter.CurrentPage;
                if (page != null)
                {
                    area = page.GetArea(CurrentSelectedControl.Id);
                }
            }
            else if (obj is MNPage)
            {
                InitializeControlList(obj as MNPage);
                area = (obj as MNPage).Area;
            }
            else if (obj is MNDocument)
            {
                area = (obj as MNDocument).Area;
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

    }
}
