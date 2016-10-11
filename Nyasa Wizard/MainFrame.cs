using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using SlideMaker.Document;
using SlideMaker.DocumentViews;

namespace SlideMaker
{
    public partial class MainFrame : Form, IDocumentDelegate
    {
        private ImageProperties pvImage = null;
        private NyasaMantraProperties pvNyasaMantra = null;
        private SimpleTextProperties pvText = null;
        private LineProperties pvLine = null;

        private int printedPageCurrent = 0;
        private MNPageContext printContext = null;

        public MainFrame()
        {
            InitializeComponent();
            MNDocumentController.RegisterDelegate(pageScrollArea1);
            MNDocumentController.RegisterDelegate(this);

            InitializeControlList();

            MNDocumentController.CreateNewDocument();
        }

        public void InitializeControlList()
        {
            listToolbox.BeginUpdate();
            listToolbox.Items.Clear();
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "TextView", Data = new SMControl(null) });
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "TextEdit Short", Data = new SMControl(null) });
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "TextEdit", Data = new SMControl(null) });
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Label", Data = new SMLabel(null) });
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Picture", Data = new SMControl(null) });
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "CheckBox", Data = new SMControl(null) });
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Drawable", Data = new SMControl(null) });
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Free Drawing", Data = new SMControl(null) });
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Text Container", Data = new SMControl(null) });
            listToolbox.Items.Add(new PageEditDraggableItem() { Text = "Text Puzzle", Data = new SMControl(null) });
            listToolbox.EndUpdate();
        }

        public void InitTreeViewWithDocument(MNDocument document)
        {
            treeView1.Nodes.Clear();

            TreeNode tn1;
            TreeNode tn = treeView1.Nodes.Add("Images");
            tn.Tag = "images";

            tn = treeView1.Nodes.Add("Pages");

            int i = 1;
            foreach (MNPage page in document.Pages)
            {
                tn1 = tn.Nodes.Add(page.Title);
                tn1.Tag = page;
                page.TreeNode = tn1;
                i++;
            }

            treeView1.ExpandAll();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;
            if (e.Node != null && e.Node.Tag != null)
            {
                if (e.Node.Tag is string)
                {
                    string tag = e.Node.Tag.ToString();
                }
                else if (e.Node.Tag is MNPage)
                {
                    MNDocument document = MNDocumentController.CurrentDocument;
                    int pageNo = (e.Node.Tag as MNPage).PageIndex;
                    if (pageNo > 0 && pageNo <= document.Pages.Count)
                    {
                        //pageScrollArea1.Dock = DockStyle.Fill;
                        pageScrollArea1.Visible = true;
                        pageScrollArea1.EditView.Page = document.Pages[pageNo - 1];
                        pageScrollArea1.EditView.Document = document;
                        pageScrollArea1.Invalidate();

                        propertyGrid1.SelectedObject = document.Pages[pageNo - 1];
                    }
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);

            MNDocumentController.CreateNewDocument();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Nyasa Wizard Files (*.nwf)|*.nwf||";
            dlg.FilterIndex = 1;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MNDocument document = MNDocument.Load(dlg.FileName);
                MNDocumentController.CurrentFileName = dlg.FileName;
                if (MNDocumentController.CurrentDocument == null)
                {
                    MessageBox.Show("File could not be loaded");
                    document = new MNDocument();
                }

                MNDocumentController.CurrentDocument = document;
                MNDocumentController.SendDocumentHasChanged(document);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MNDocumentController.CurrentFileName == null || MNDocumentController.CurrentFileName.Length == 0)
            {
                if (MNDocumentController.CurrentDocument != null && MNDocumentController.CurrentDocument.HasContent())
                    saveAsToolStripMenuItem_Click(sender, e);
                else
                    return;
            }

            XmlDocument doc = new XmlDocument();
            MNDocumentController.CurrentDocument.Save(doc);
            doc.Save(MNDocumentController.CurrentFileName);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".smd";
            dlg.Filter = "SlideMaker Files (*.smd)|*.smd||";
            dlg.FilterIndex = 1;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MNDocumentController.CurrentFileName = dlg.FileName;
                XmlDocument doc = new XmlDocument();
                MNDocumentController.CurrentDocument.Save(doc);
                doc.Save(MNDocumentController.CurrentFileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pageEditView1_PageObjectSelected(object sender, PageEditViewArguments e)
        {
            if (e.Object != null)
            {
                if (e.Object is MNPageImage)
                {
                    if (pvImage == null)
                    {
                        pvImage = new SlideMaker.ImageProperties();
                        pvImage.Size = new Size(100, 100);
                        pvImage.Location = new Point(0, 0);
                        pvImage.Name = "pvImage";
                        //pvImage.TabIndex = splitContainer2.Panel2.Controls.Count + 1;
                        pvImage.Visible = false;
                        //splitContainer2.Panel2.Controls.Add(pvImage);
                        pvImage.Dock = DockStyle.Fill;
                    }

                    pvImage.Visible = true;
                    pvImage.PageView = e.PageView;
                    pvImage.Set(e.Document, e.Page, e.Object as MNPageImage);
                }
                else if (e.Object is MNPageTextWithImage)
                {
                    if (pvNyasaMantra == null)
                    {
                        pvNyasaMantra = new SlideMaker.NyasaMantraProperties();
                        pvNyasaMantra.Size = new Size(100, 100);
                        pvNyasaMantra.Location = new Point(0, 0);
                        pvNyasaMantra.Name = "pvNyasa";
                        //pvNyasaMantra.TabIndex = splitContainer2.Panel2.Controls.Count + 1;
                        pvNyasaMantra.Visible = false;
                        //splitContainer2.Panel2.Controls.Add(pvNyasaMantra);
                        pvNyasaMantra.Dock = DockStyle.Fill;
                    }

                    pvNyasaMantra.Visible = true;
                    pvNyasaMantra.PageView = e.PageView;
                    pvNyasaMantra.Set(e.Document, e.Page, e.Object as MNPageTextWithImage);
                }
                else if (e.Object is MNLine)
                {
                    if (pvLine == null)
                    {
                        pvLine = new SlideMaker.LineProperties();
                        pvLine.Size = new Size(100, 100);
                        pvLine.Location = new Point(0, 0);
                        pvLine.Name = "pvLine";
                        //pvLine.TabIndex = splitContainer2.Panel2.Controls.Count + 1;
                        pvLine.Visible = false;
                        //splitContainer2.Panel2.Controls.Add(pvLine);
                        pvLine.Dock = DockStyle.Fill;
                    }

                    pvLine.Visible = true;
                    pvLine.PageView = e.PageView;
                    pvLine.Set(e.Document, e.Page, e.Object as MNLine);
                }
                else if (e.Object is MNPagePoint)
                {
                    MNPagePoint pp = e.Object as MNPagePoint;

                    if (pp.EditParentProperties)
                    {
                        e.Object = pp.ParentObject;
                        pageEditView1_PageObjectSelected(sender, e);
                    }
                }
                else
                {
                    propertyGrid1.SelectedObject = e.Object;
                }
            }
            else if (e.Page != null)
            {
                propertyGrid1.SelectedObject = e.Page;
            }
            else if (e.Document != null)
            {
                propertyGrid1.SelectedObject = e.Document;
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();

            if (pd.ShowDialog() != DialogResult.OK)
                return;

            printedPageCurrent = 0;
            printDocument1.DocumentName = MNDocumentController.CurrentDocument.Title;
            printDocument1.PrinterSettings = pd.PrinterSettings;

            if (printContext == null)
            {
                printContext = new MNPageContext();

                printContext.drawSelectionMarks = false;
                printContext.TrackedObjects = new List<SMControl>();
            }

            printContext.LastMatrix = null;

            printDocument1.Print();

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (printContext.LastMatrix == null)
            {
                printContext.PageHeight = MNDocumentController.CurrentDocument.PageHeight;
                printContext.PageWidth = MNDocumentController.CurrentDocument.PageWidth;
                Rectangle presentedRect = new Rectangle(0, 0, printContext.PageWidth, printContext.PageHeight);
                Point[] pls = new Point[3];
                pls[0] = new Point(0, 0);
                pls[1] = new Point(e.PageBounds.Width - 1, 0);
                pls[2] = new Point(0, e.PageBounds.Height - 1);
                printContext.LastMatrix = new System.Drawing.Drawing2D.Matrix(presentedRect, pls);
                printContext.LastInvertMatrix = new System.Drawing.Drawing2D.Matrix(presentedRect, pls);
                printContext.LastInvertMatrix.Invert();
            }

            printContext.g = e.Graphics;
            e.Graphics.Transform = printContext.LastMatrix;

            MNDocumentController.CurrentDocument.Pages[printedPageCurrent].Paint(printContext);
            printedPageCurrent++;

            e.HasMorePages = (printedPageCurrent < MNDocumentController.CurrentDocument.Pages.Count);
        }

        private void pageEditView1_NewPageRequested(object sender, PageEditViewArguments e)
        {
            MNDocumentController.CurrentDocument.InsertPageAfter(e.Page);
            InitTreeViewWithDocument(MNDocumentController.CurrentDocument);
        }

        public void documentHasChanged(MNDocument document)
        {
            InitTreeViewWithDocument(document);
        }

        private void listToolbox_DrawItem(object sender, DrawItemEventArgs e)
        {

        }

        private void listToolbox_MeasureItem(object sender, MeasureItemEventArgs e)
        {

        }

        private void listToolbox_MouseDown(object sender, MouseEventArgs e)
        {
            int index = listToolbox.IndexFromPoint(e.X, e.Y);
            if (index >= 0)
            {
                DragDropEffects de = listToolbox.DoDragDrop(listToolbox.Items[index], DragDropEffects.Copy);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            pageScrollArea1.ZoomIn();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            pageScrollArea1.ZoomOut();
        }
    }
}
