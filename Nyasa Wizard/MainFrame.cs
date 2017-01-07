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
using System.IO;

using Rambha.Document;
using SlideMaker.Views;
using Rambha.Serializer;


namespace SlideMaker
{
    public partial class MainFrame : Form
    {
        private int printedPageCurrent = 0;
        private MNPageContext printContext = null;

        private NavigationFrame navigationFrame = null;

        public MainFrame()
        {
            InitializeComponent();

            pageDetailPanel1.Dock = DockStyle.Fill;
            pageDetailPanel1.Visible = true;


            MNNotificationCenter.CreateNewDocument();

            navigationFrame = new NavigationFrame();
            navigationFrame.Show();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);

            MNNotificationCenter.CreateNewDocument();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "SlideMaker (*.smd)|*.smd||";
            dlg.FilterIndex = 1;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\load.txt"))
                {
                    using (BinaryReader br = new BinaryReader(File.OpenRead(dlg.FileName)))
                    {
                        MNDocument document = new MNDocument();
                        RSFileReader fr = new RSFileReader(br);
                        fr.logStream = sw;
                        try
                        {
                            if (document.Load(fr, true))
                            {
                                MNNotificationCenter.CurrentFileName = dlg.FileName;
                            }
                            else
                            {
                                MessageBox.Show("File could not be loaded");
                                document = new MNDocument();
                            }

                            MNNotificationCenter.CurrentDocument = document;
                        }
                        catch (Exception ex)
                        {
                            sw.Flush();
                            sw.WriteLine("Exception:");
                            sw.WriteLine(ex.Message);
                            sw.Flush();
                        }
                    }
                }
            }

            pageDetailPanel1.RefreshView();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MNNotificationCenter.CurrentFileName == null || MNNotificationCenter.CurrentFileName.Length == 0)
            {
                if (MNNotificationCenter.CurrentDocument != null && MNNotificationCenter.CurrentDocument.HasContent())
                    saveAsToolStripMenuItem_Click(sender, e);
                else
                    return;
            }

            SaveDocument(null);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".smd";
            dlg.Filter = "SlideMaker Document (*.smd)|*.smd||";
            dlg.FilterIndex = 1;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveDocument(dlg.FileName);
            }
        }

        private static void SaveDocument(string filePath)
        {
            if (filePath != null)
                MNNotificationCenter.CurrentFileName = filePath;
            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\save.txt"))
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(MNNotificationCenter.CurrentFileName)))
                {
                    RSFileWriter fw = new RSFileWriter(bw);
                    fw.logStream = sw;
                    MNNotificationCenter.CurrentDocument.Save(fw);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();

            if (pd.ShowDialog() != DialogResult.OK)
                return;

            printedPageCurrent = 0;
            printDocument1.DocumentName = MNNotificationCenter.CurrentDocument.BookTitle;
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
                printContext.PageHeight = MNNotificationCenter.CurrentDocument.PageHeight;
                printContext.PageWidth = MNNotificationCenter.CurrentDocument.PageWidth;
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

            MNNotificationCenter.CurrentDocument.Pages[printedPageCurrent].Paint(printContext);
            printedPageCurrent++;

            e.HasMorePages = (printedPageCurrent < MNNotificationCenter.CurrentDocument.Pages.Count);
        }


        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            ImagesDialog dlg = new ImagesDialog();
            dlg.SetDocument(MNNotificationCenter.CurrentDocument);
            dlg.ShowDialog();
        }


        private void makeGroupListLabels_Click(object sender, EventArgs e)
        {

        }

        private void makeGroupSortPic_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Opening EDIT dropdown menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem4_DropDownOpening(object sender, EventArgs e)
        {
            MNPage current = MNNotificationCenter.CurrentPage;
            int labels = 0;
            int images = 0;
            int ceb = 0;
            int total = 0;
            if (current != null)
            {
                foreach (SMControl c in current.SelectedObjects)
                {
                    if (c is SMLabel) labels++;
                    if (c is SMImage) images++;
                    if (c is SMCheckBox) ceb++;
                    total++;
                }
            }
        }





        private void playSlideshowToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }




        private void testAction1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            if (doc == null) return;

            MNPage page = MNNotificationCenter.CurrentPage;
            if (page == null) return;

            foreach (SMControl c in page.Objects)
            {
                if (c is SMTextContainer)
                {
                    SMTextContainer tc = c as SMTextContainer;
                    tc.SetText("Here is text for dropping <drop text=\"______\" tag=\"word1\">. Be sure to place here correct word");
                }
            }
            
        }

        private void showObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectDumpFrame frame = new ObjectDumpFrame();
            frame.SetDocument(MNNotificationCenter.CurrentDocument);
            frame.Show();
        }

        private void pageScrollArea1_BackToParentView(object sender, EventArgs e)
        {
        }

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            pageFlowToolStripMenuItem.Checked = false;
            pageDynamicsToolStripMenuItem.Checked = true;
        }


        void pageFlowFrame_OnClose(object sender, EventArgs e)
        {
        }

        private void pageDynamicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


    }
}
