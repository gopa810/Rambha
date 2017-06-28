using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
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
        private bool bOmitFinalSave;

        public Timer autoSave = null;

        public MainFrame()
        {
            InitializeComponent();

            MNSharedObjects.Load();

            pageDetailPanel1.Dock = DockStyle.Fill;
            pageDetailPanel1.Visible = true;
            pageDetailPanel1.SetSharedDocument();

            MNNotificationCenter.CreateNewDocument();
            autoSave = new Timer();
            autoSave.Interval = 900000;
            autoSave.Tick += new EventHandler(autoSave_Tick);
            autoSave.Start();

            ViewForm.Shared.Show();
        }

        void autoSave_Tick(object sender, EventArgs e)
        {
            if (File.Exists(MNNotificationCenter.CurrentFileName))
            {
                TimeSpan ts = DateTime.Now - lastSaved;
                if (ts.TotalMinutes > 15.0)
                    SaveDocument(null);
            }
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

                    ApplyUpdatesAndChanges();

                    MNNotificationCenter.BroadcastMessage(this, "DocumentChanged", MNNotificationCenter.CurrentDocument);
                }

                lastSaved = DateTime.Now;
            }

            pageDetailPanel1.RefreshView();
        }

        private void ApplyUpdatesAndChanges()
        {
            //MNSharedObjects.CopyToDocument(MNNotificationCenter.CurrentDocument);

            /*if (MNNotificationCenter.CurrentDocument.Book.Version < 2)
            {
                MNNotificationCenter.CurrentDocument.ReapplyStyles();
            }*/
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
                        document.Book.FilePath = fileName;
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

            MNDocument doc = MNNotificationCenter.CurrentDocument;
            if (doc.Data.Pages.Count == 0)
            {
                doc.CreateNewPage();
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

            MNDocument doc = MNNotificationCenter.CurrentDocument;
            if (doc.DefaultLanguage.Styles.Count == 0)
            {
                doc.InitialiseDefaultStyles();
            }

            foreach (MNReferencedImage ri in doc.DefaultLanguage.Images)
            {
                if (ri.Id < 1)
                    ri.Id = doc.Data.GetNextId();
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

        private DateTime lastSaved = DateTime.Now;

        private void SaveDocument(string filePath)
        {
            if (filePath != null)
                MNNotificationCenter.CurrentFileName = filePath;
            else
                filePath = MNNotificationCenter.CurrentFileName;

            RSFileWriter fw;
            string fileName = filePath;
            MNDocument doc = MNNotificationCenter.CurrentDocument;

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\save_book.txt"))
            {
                using (BinaryWriter bw = new BinaryWriter(File.Create(fileName)))
                {
                    fw = new RSFileWriter(bw);
                    fw.logStream = sw;
                    doc.Book.Save(fw);
                }
            }

            fileName = fileName.Replace(".smb", ".smd");
            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\save_data.txt"))
            {
                using (BinaryWriter bw = new BinaryWriter(File.Create(fileName)))
                {
                    fw = new RSFileWriter(bw);
                    fw.logStream = sw;
                    doc.Data.Save(fw);
                }
            }

            if (doc.DefaultLanguage.IsModified())
            {
                fileName = fileName.Replace(".smd", ".sme");
                using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\save_lang.txt"))
                {
                    using (BinaryWriter bw = new BinaryWriter(File.Create(fileName)))
                    {
                        fw = new RSFileWriter(bw);
                        fw.logStream = sw;
                        MNLocalisation loc = doc.DefaultLanguage;
                        loc.SetProperty("BookCode", doc.Book.BookCode);
                        loc.SetProperty("LanguageName", "Default");
                        loc.Save(fw);
                        loc.Modified = false;
                    }
                }
            }

            lastSaved = DateTime.Now;
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
                    tc.Text = "Here is text for dropping <drop text=\"______\" tag=\"word1\">. Be sure to place here correct word";
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
            viewerSlidesToolStripMenuItem.Enabled = !ViewForm.IsVisible();
            reviewFrameToolStripMenuItem.Enabled = !ReviewFrame.IsVisible();
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
            if (ViewForm.IsVisible())
                ViewForm.Shared.SavePosition();
            if (ReviewFrame.IsVisible())
                ReviewFrame.Shared.SavePosition();

            MNNotificationCenter.BroadcastMessage(this, "AppWillClose");

            if (bOmitFinalSave) return;
            autoSave.Stop();
            saveToolStripMenuItem_Click(sender, EventArgs.Empty);
            MNSharedObjects.Save();
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
                        file.Modified = true;
                    }
                }

                file.Save(outFile);
            }
        }

        private void showRTFConvertorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRtfConvertor form = new FormRtfConvertor();
            form.Show();
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static void ResizeImage(Image image, int width, int height, string fileName)
        {
            var destRect = new Rectangle(0, 0, width, height);
            using (var destImage = new Bitmap(width, height))
            {

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                destImage.Save(fileName, ImageFormat.Png);
            }
        }

        private void shrinkImagesToTheirCanvasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            if (doc == null)
                return;

            Dictionary<long, Size> dsz = new Dictionary<long, Size>();
            Dictionary<long, MNReferencedImage> dri = new Dictionary<long, MNReferencedImage>();

            foreach (MNPage p in doc.Data.Pages)
            {
                foreach (SMControl c in p.Objects)
                {
                    if (c is SMImage)
                    {
                        MNReferencedImage img = (c as SMImage).Img.Image;
                        if (img != null && img.ImageData != null)
                        {
                            if (!img.Name.StartsWith("Image Pasted"))
                                continue;
                            SMRectangleArea area = c.Area;
                            Rectangle bounds = area.GetBounds(PageEditDisplaySize.LandscapeBig);
                            Size imageSize = img.ImageData.Size;
                            Size imageDrawSize = SMImage.GetImageDrawSize(bounds, img.ImageData);
                            if (imageSize.Width > imageDrawSize.Width || imageSize.Height > imageDrawSize.Height)
                            {
                                if (!dsz.ContainsKey(img.Id))
                                {
                                    Debugger.Log(0, "", "Image " + img.Id + " shrinked from " + imageSize.ToString() + " to " + imageDrawSize.ToString() + "\n");
                                    dsz[img.Id] = imageDrawSize;
                                    dri[img.Id] = img;
                                }
                                else
                                {
                                    Size storedSize = dsz[img.Id];
                                    if (storedSize.Width < imageDrawSize.Width ||
                                        storedSize.Height < imageDrawSize.Height)
                                    {
                                        dsz[img.Id] = imageDrawSize;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (long imageId in dsz.Keys)
            {
                string tempFileName = Path.GetTempFileName();
                if (dri.ContainsKey(imageId))
                {
                    Size newSize = dsz[imageId];
                    MNReferencedImage img = dri[imageId];

                    //Debugger.Log(0, "", "  Size before shrink: " + GetImageSize(img.ImageData) + "\n");
                    ResizeImage(img.ImageData, newSize.Width, newSize.Height, tempFileName);

                    Image newImage = Image.FromFile(tempFileName);
                    img.ImageData = newImage;
                    //Debugger.Log(0, "", "  Size after shrink: " + GetImageSize(img.ImageData) + "\n");
                    doc.DefaultLanguage.Modified = true;

                    Debugger.Log(0, "", "Used file " + tempFileName + "\n");
                }
            }

        }

        public int GetImageSize(Image image)
        {
            int size = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                size = (int)ms.Length;
            }
            return size;
        }

        private void exitWithoutSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bOmitFinalSave = true;
            Close();
        }

        private void saveOnlyHeaderFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = MNNotificationCenter.CurrentFileName;

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\save_book.txt"))
            {
                using (BinaryWriter bw = new BinaryWriter(File.Create(fileName)))
                {
                    RSFileWriter fw = new RSFileWriter(bw);
                    fw.logStream = sw;
                    MNNotificationCenter.CurrentDocument.Book.Save(fw);
                }
            }

        }

        private void saveOnlyDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = MNNotificationCenter.CurrentFileName;
            fileName = fileName.Replace(".smb", ".smd");

            using (StreamWriter sw = new StreamWriter(@"d:\LearnToRead\save_data.txt"))
            {
                using (BinaryWriter bw = new BinaryWriter(File.Create(fileName)))
                {
                    RSFileWriter fw = new RSFileWriter(bw);
                    fw.logStream = sw;
                     MNNotificationCenter.CurrentDocument.Data.Save(fw);
                }
            }
        }

        private void generateInitialPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            if (doc == null) return;

            MNBookData data = doc.Data;

            MNLocalisation lang = doc.DefaultLanguage;

            if (data == null || lang == null)
                return;

            if (data.Pages.Count != 1)
                return;

            data.Pages[0].Title = "Title";
            SetPageHeaderControls(0, data.Pages[0]);
            SetControlsToPage("Title page", data.Pages[0]);

            AddNewPage(doc, "start", "startPage", 0);
            AddNewPage(doc, "tp", "tpPage", 0);
            AddNewPage(doc, "new_letters", "tpNewLetters", 1);
            AddNewPage(doc, "teaching_plan", "tpTeachingPlan", 1);
            List<int> pageNos = new List<int>();
            foreach (MNReferencedSound snd in lang.Sounds)
            {
                if (snd.Name.StartsWith("Page "))
                {
                    int i;
                    if (int.TryParse(snd.Name.Substring(5).Trim(), out i))
                    {
                        pageNos.Add(i);
                    }
                }
            }
            pageNos.Sort();
            foreach (int a in pageNos)
            {
                AddNewPage(doc, "Page " + a, "mainAbove", 3);
            }
            AddNewPage(doc, "ex_before", "", 2);
            AddNewPage(doc, "ex_after", "", 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="templateName"></param>
        /// <param name="type">0 - without header, 1 - header without fwd and audio,
        /// 2 - header without audio, 3 - full header</param>
        private void AddNewPage(MNDocument doc, string title, string templateName, int type)
        {
            AddNewPage(doc, title, templateName, type, -1);

        }

        private MNPage AddNewPage(MNDocument doc, string title, string templateName, int type, int pageAfter)
        {
            Debugger.Log(0, "", "AddnewPage: " + title + ", " + templateName + ", " + type + "\n");

            MNPage np = new MNPage(doc);
            np.Title = title;
            np.Id = doc.Data.GetNextId();

            SetControlsToPage(templateName, np);

            SetPageHeaderControls(type, np);

            if (pageAfter == -1)
                doc.Data.Pages.Add(np);
            else
                doc.Data.Pages.Insert(pageAfter + 1, np);

            return np;
        }

        private static void SetControlsToPage(string templateName, MNPage np)
        {
            MNPage t = MNSharedObjects.internalDocument.FindTemplateName(templateName);
            if (t != null)
            {
                MNPage.CopyControlsFrom(t, np);
            }
        }

        private static void SetPageHeaderControls(int type, MNPage np)
        {
            switch (type)
            {
                case 0:
                    np.ShowBackNavigation = false;
                    np.ShowForwardNavigation = false;
                    np.ShowAudio = false;
                    np.DefaultAudioState = true;
                    np.ShowTitle = false;
                    np.ShowHome = false;
                    np.ShowHelp = false;
                    break;
                case 1:
                    np.ShowBackNavigation = true;
                    np.ShowForwardNavigation = false;
                    np.ShowAudio = false;
                    np.DefaultAudioState = true;
                    np.ShowTitle = true;
                    np.ShowHome = true;
                    np.ShowHelp = false;
                    break;
                case 2:
                    np.ShowBackNavigation = true;
                    np.ShowForwardNavigation = true;
                    np.ShowAudio = false;
                    np.DefaultAudioState = true;
                    np.ShowTitle = true;
                    np.ShowHome = true;
                    np.ShowHelp = false;
                    break;
                case 3:
                    np.ShowBackNavigation = true;
                    np.ShowForwardNavigation = true;
                    np.ShowAudio = true;
                    np.DefaultAudioState = false;
                    np.ShowTitle = true;
                    np.ShowHome = true;
                    np.ShowHelp = false;
                    break;
            }
        }

        private void viewerSlidesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewForm.Shared.Show();
            ViewForm.Shared.Visible = true;
        }

        private void reviewFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReviewFrame.Shared.Show();
            ReviewFrame.Shared.Visible = true;
        }

        private void checkTransitionPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogInsertTransPages dlg = new DialogInsertTransPages();
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            dlg.SetDocument(doc);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                for (int i = 2; i >= 0; i--)
                {
                    int afterPage, pageType;
                    dlg.GetPageToInsert(i, out afterPage, out pageType);
                    MNPage p = null;
                    switch(pageType)
                    {
                        case 1:
                            doc.Data.Pages[afterPage].NextPage = "";
                            p = AddNewPage(doc, "End of Book", "EOPBook", 0, afterPage);
                            break;
                        case 2:
                            doc.Data.Pages[afterPage].NextPage = "";
                            p = AddNewPage(doc, "End of Exercises Before", "EOPExBefore", 0, afterPage);
                            break;
                        case 3:
                            doc.Data.Pages[afterPage].NextPage = "";
                            p = AddNewPage(doc, "End of Exercises After", "EOPExAfter", 0, afterPage);
                            break;
                    }
                }
            }
        }

        private void convertAllFilesToVersion3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (string s in Directory.EnumerateFiles(@"c:\Users\Gopa702\Documents\"))
            {
                if (Path.GetExtension(s) == ".smb")
                {
                    Debugger.Log(0, "", "File: " + s + "\n");
                    MNDocument doc = new MNDocument();

                    using (Stream str = File.OpenRead(s))
                    {
                        using (BinaryReader br = new BinaryReader(str))
                        {
                            RSFileReader fr = new RSFileReader(br);
                            doc.Book.Load(fr);
                        }
                    }
                    using (Stream str = File.OpenWrite(s))
                    {
                        using (BinaryWriter br = new BinaryWriter(str))
                        {
                            RSFileWriter fr = new RSFileWriter(br);
                            doc.Book.Save(fr);
                        }
                    }
                }
                else if (Path.GetExtension(s) == ".smd")
                {
                    Debugger.Log(0, "", "File: " + s + "\n");
                    MNDocument doc = new MNDocument();
                    using (Stream str = File.OpenRead(s))
                    {
                        using (BinaryReader br = new BinaryReader(str))
                        {
                            RSFileReader fr = new RSFileReader(br);
                            doc.Data.Load(fr);
                        }
                    }
                    using (Stream str = File.OpenWrite(s))
                    {
                        using (BinaryWriter bw = new BinaryWriter(str))
                        {
                            RSFileWriter fw = new RSFileWriter(bw);
                            doc.Data.Save(fw);
                        }
                    }
                }
            }
        }


    }
}
