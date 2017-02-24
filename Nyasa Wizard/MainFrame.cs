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
        private LocalizationMainForm localeForm = null;

        public MainFrame()
        {
            InitializeComponent();

            pageDetailPanel1.Dock = DockStyle.Fill;
            pageDetailPanel1.Visible = true;

            MNNotificationCenter.CreateNewDocument();
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
            dlg.Filter = "SlideMaker Book (*.smb)|*.smb||";
            dlg.FilterIndex = 1;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = dlg.FileName;

                if (LoadBookHeader(fileName))
                {
                    MNNotificationCenter.CurrentDocument.Book.FindLanguageFiles(Path.GetDirectoryName(fileName));

                    fileName = fileName.Replace(".smb", ".smd");
                    LoadBookData(fileName);

                    fileName = fileName.Replace(".smd", ".sme");
                    LoadBookLang(fileName);

                    MNNotificationCenter.BroadcastMessage(this, "DocumentChanged", MNNotificationCenter.CurrentDocument); 
                }
            }

            pageDetailPanel1.RefreshView();
        }

        private static bool LoadBookHeader(string fileName)
        {
            bool r = true;

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\load_book.txt"))
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(fileName)))
                {
                    MNDocument document = new MNDocument();
                    RSFileReader fr = new RSFileReader(br);
                    fr.logStream = sw;
                    try
                    {
                        document.Book.Load(fr);
                        MNNotificationCenter.CurrentFileName = fileName;
                        MNNotificationCenter.CurrentDocument = document;
                    }
                    catch (Exception ex)
                    {
                        sw.Flush();
                        sw.WriteLine("Exception:");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine(ex.StackTrace);
                        sw.Flush();
                        r = false;
                    }
                }
            }

            return r;
        }

        private static void LoadBookData(string fileName)
        {
            if (MNNotificationCenter.CurrentDocument == null)
                return;

            if (!File.Exists(fileName))
                return;

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\load_data.txt"))
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(fileName)))
                {
                    MNDocument document = MNNotificationCenter.CurrentDocument;
                    RSFileReader fr = new RSFileReader(br);
                    fr.logStream = sw;
                    try
                    {
                        document.Data.Load(fr);
                    }
                    catch (Exception ex)
                    {
                        sw.Flush();
                        sw.WriteLine("Exception:");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine(ex.StackTrace);
                        sw.Flush();
                    }
                }
            }

        }

        private static void LoadBookLang(string fileName)
        {
            if (MNNotificationCenter.CurrentDocument == null)
                return;

            if (!File.Exists(fileName))
                return;

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\load_lang.txt"))
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(fileName)))
                {
                    MNDocument document = MNNotificationCenter.CurrentDocument;
                    RSFileReader fr = new RSFileReader(br);
                    fr.logStream = sw;
                    try
                    {
                        document.DefaultLanguage.Load(fr, true);
                    }
                    catch (Exception ex)
                    {
                        sw.Flush();
                        sw.WriteLine("Exception:");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine(ex.StackTrace);
                        sw.Flush();
                    }
                }
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MNNotificationCenter.CurrentFileName == null
                || MNNotificationCenter.CurrentFileName.Length == 0)
            {
                if (MNNotificationCenter.CurrentDocument != null
                    && MNNotificationCenter.CurrentDocument.HasContent())
                {
                    saveAsToolStripMenuItem_Click(sender, e);
                }
            }
            else
            {
                SaveDocument(null);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".smb";
            dlg.Filter = "SlideMaker Book (*.smb)|*.smb||";
            dlg.FilterIndex = 1;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MNNotificationCenter.CurrentFileName = dlg.FileName;
                SaveDocument(dlg.FileName);
            }
        }

        private static void SaveDocument(string filePath)
        {
            if (filePath != null)
                MNNotificationCenter.CurrentFileName = filePath;
            else
                filePath = MNNotificationCenter.CurrentFileName;

            RSFileWriter fw;
            string fileName = filePath;

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\save_book.txt"))
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileName)))
                {
                    fw = new RSFileWriter(bw);
                    fw.logStream = sw;
                    MNNotificationCenter.CurrentDocument.Book.Save(fw);
                }
            }

            fileName = fileName.Replace(".smb", ".smd");
            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\save_data.txt"))
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileName)))
                {
                    fw = new RSFileWriter(bw);
                    fw.logStream = sw;
                    MNNotificationCenter.CurrentDocument.Data.Save(fw);
                }
            }

            fileName = fileName.Replace(".smd", ".sme");
            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\save_lang.txt"))
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileName)))
                {
                    fw = new RSFileWriter(bw);
                    fw.logStream = sw;
                    MNDocument doc = MNNotificationCenter.CurrentDocument;
                    MNLocalisation loc = doc.DefaultLanguage;
                    loc.SetProperty("BookCode", doc.Book.BookCode);
                    loc.SetProperty("LanguageName", "Default");
                    loc.Save(fw);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

        private void MainFrame_Activated(object sender, EventArgs e)
        {
            /*if (navigationFrame != null)
                navigationFrame.Visible = true;*/
        }

        private void MainFrame_Deactivate(object sender, EventArgs e)
        {
            /*if (navigationFrame != null)
                navigationFrame.Visible = false;*/
        }

        private void MainFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveToolStripMenuItem_Click(sender, EventArgs.Empty);
        }

        private void showDefaultLanguageDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (localeForm == null)
            {
                localeForm = new LocalizationMainForm();
                localeForm.Show();
            }
            else
            {
                localeForm.Show();
                localeForm.BringToFront();
            }

            localeForm.SetLocalisationData(MNNotificationCenter.CurrentDocument.DefaultLanguage);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            // create language file from folder
            // we need:
            // - book code
            // - directory path
            DialogGenerateLangFile d = new DialogGenerateLangFile();

            d.SetBookCode(MNNotificationCenter.CurrentDocument.Book.BookCode);
            d.SetBookFileName(MNNotificationCenter.CurrentFileName);

            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string inDir = d.GetInputDirectory();
                string outFile = d.GetOutputFileName();

                MNLocalisation file = new MNLocalisation();
                file.SetProperty("BookCode", MNNotificationCenter.CurrentDocument.Book.BookCode);
                file.SetProperty("LanguageName", Path.GetFileName(inDir));

                foreach (string objectFileName in Directory.GetFiles(inDir))
                {
                    string extension = Path.GetExtension(objectFileName);
                    if (extension.Equals(".mp3"))
                    {
                        MNReferencedSound sound = new MNReferencedSound();
                        sound.InitializeWithFile(objectFileName);
                        sound.Name = Path.GetFileNameWithoutExtension(objectFileName);
                        file.Sounds.Add(sound);
                    }
                }

                file.Save(outFile);
            }
        }


    }
}
