using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Rambha.Document;
using SlideMaker.Views;

namespace SlideMaker
{
    public partial class LocalizationMainForm : Form
    {
        MNLocalisation data = null;

        string fileName = null;

        string fileExtensions = ".sme";

        EditControlString stringView = null;
        EditControlImage imageView = null;
        EditControlSound soundView = null;
        EditControlAudioText runtextView = null;
        EditSpotsEditorView imageSpots = null;
        EditControlTextStyle styleView = null;

        private List<UserControl> viewStack = new List<UserControl>();

        public LocalizationMainForm()
        {
            InitializeComponent();

            InitializeLanguageList();

            OnNew();
        }

        public void InitializeLanguageList()
        {
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_ClickLoad(object sender, EventArgs e)
        {
            OnSave();

            // load file
            OpenFileDialog d = new OpenFileDialog();
            d.RestoreDirectory = true;
            d.Filter = "Language Pack Files (*" + fileExtensions + ")|*" + fileExtensions + "||";
            if (Properties.Settings.Default.LastDirectory.Length > 0)
                d.InitialDirectory = Properties.Settings.Default.LastDirectory;
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                data = new MNLocalisation();
                fileName = d.FileName;
                data.Load(d.FileName, true);
                UpdateUIWithData();
                PresentData(data, "");
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_ClickSave(object sender, EventArgs e)
        {
            UpdateDataWithUI();
            OnSave();
        }

        /// <summary>
        /// New
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_ClickNew(object sender, EventArgs e)
        {
            OnNew();
        }

        public void SetLocalisationData(MNLocalisation ld)
        {
            data = ld;
            fileName = null;
            UpdateUIWithData();
            PresentData(data, null);
        }

        public void SetFileName(string fn)
        {
            fileName = fn;
        }

        private void OnNew()
        {
            OnSave();
            SetLocalisationData(new MNLocalisation());
        }

        public void OnSave()
        {
            if (fileName != null)
                data.Save(fileName);
        }

        private string BookCode
        {
            get { return data.GetProperty("BookCode"); }
            set { data.SetProperty("BookCode", value); }
        }

        private string LanguageName
        {
            get
            {
                return data.GetProperty("LanguageName");
            }
            set
            {
                data.SetProperty("LanguageName", value);
            }
        }

        private void UpdateDataWithUI()
        {

        }

        private void UpdateUIWithData()
        {
            if (data == null)
                return;

            toolStripTextBox1.Text = data.GetProperty("BookCode");

            this.Text = string.Format("Language Editor - {0} - {1}", data.GetProperty("BookCode"),
                data.GetProperty("LanguageName"));

            listBoxImages.Items.Clear();
            foreach (MNReferencedImage ri in data.Images)
                listBoxImages.Items.Add(ri);
            listBoxTexts.Items.Clear();
            foreach (MNReferencedText rt in data.Texts)
                listBoxTexts.Items.Add(rt);
            listBoxSounds.Items.Clear();
            foreach (MNReferencedSound rs in data.Sounds)
                listBoxSounds.Items.Add(rs);
            Console.Write("Sounds: " + data.Sounds.Count + "\n");
            listBoxAudioTexts.Items.Clear();
            foreach (MNReferencedAudioText rat in data.AudioTexts)
                listBoxAudioTexts.Items.Add(rat);
            listBoxStyles.Items.Clear();
            foreach (MNReferencedStyle st in data.Styles)
                listBoxStyles.Items.Add(st);
        }

        public void PresentData(object data, string key)
        {
            UserControl viewToShow = null;
            if (data is MNReferencedAudioText)
            {
                if (runtextView == null)
                {
                    runtextView = PrepareControl(new EditControlAudioText()) as EditControlAudioText;
                }
                AddToStack(runtextView);

                runtextView.ParentFrame = this;
                runtextView.SetValue(data as MNReferencedAudioText);
                viewToShow = runtextView;
            }
            else if (data is MNReferencedSound)
            {
                if (soundView == null)
                {
                    soundView = PrepareControl(new EditControlSound()) as EditControlSound;
                }
                AddToStack(soundView);

                soundView.ParentFrame = this;
                soundView.SetValue(data as MNReferencedSound);
                viewToShow = soundView;
            }
            else if (data is MNReferencedText)
            {
                if (stringView == null)
                {
                    stringView = PrepareControl(new EditControlString()) as EditControlString;
                }
                AddToStack(stringView);

                stringView.ParentFrame = this;
                stringView.SetValue(data as MNReferencedText);
                viewToShow = stringView;
            }
            else if (data is MNReferencedImage)
            {
                if (key == "spots")
                {
                    if (imageSpots == null)
                        imageSpots = PrepareControl(new EditSpotsEditorView()) as EditSpotsEditorView;
                    AddToStack(imageSpots);

                    imageSpots.ParentFrame = this;
                    imageSpots.Image = data as MNReferencedImage;
                    viewToShow = imageSpots;
                }
                else
                {
                    if (imageView == null)
                        imageView = PrepareControl(new EditControlImage()) as EditControlImage;
                    AddToStack(imageView);

                    imageView.ParentFrame = this;
                    imageView.SetValues(data as MNReferencedImage);
                    viewToShow = imageView;
                }
            }
            else if (data is MNReferencedStyle)
            {
                if (styleView == null)
                    styleView = PrepareControl(new EditControlTextStyle()) as EditControlTextStyle;
                AddToStack(styleView);
                styleView.ParentFrame = this;
                styleView.SetValue(data as MNReferencedStyle);
                viewToShow = styleView;
            }

            if (viewToShow != null)
            {
                foreach (UserControl uc in viewStack)
                {
                    if (uc == viewToShow)
                    {
                        uc.Visible = true;
                    }
                    else
                    {
                        uc.Visible = false;
                    }
                }
            }
        }

        private void AddToStack(UserControl uc)
        {
            if (viewStack.IndexOf(uc) < 0)
            {
                uc.Parent = panel1;
                panel1.Controls.Add(uc);
                viewStack.Add(uc);
            }

            foreach (UserControl c in viewStack)
            {
                c.Visible = false;
            }
        }

        private UserControl PrepareControl(UserControl uc)
        {
            uc.Location = new Point(0, 0);
            uc.Size = new System.Drawing.Size(100, 100);
            uc.Dock = DockStyle.Fill;
            return uc;
        }

        public void CloseCurrentPresentation()
        {
            if (viewStack.Count > 1)
            {
                int li = viewStack.Count - 1;
                viewStack[li].Visible = false;
                viewStack[li].Parent = null;
                panel1.Controls.Remove(viewStack[li]);
                viewStack.RemoveAt(li);
            }

            if (viewStack.Count > 0)
            {
                viewStack[viewStack.Count - 1].Visible = true;
            }
        }

        public void ReplaceType(string key, MNReferencedText oldValue, MNReferencedAudioText newValue)
        {
            for (int i = 0; i < data.Texts.Count; i++)
            {
                if (data.Texts[i].Name.Equals(oldValue))
                {
                    data.Texts.RemoveAt(i);
                    data.Modified = true;
                    break;
                }
            }
            data.AudioTexts.Add(newValue);
            data.Modified = true;
            CloseCurrentPresentation();
            PresentData(newValue, key);
            UpdateDataWithUI();
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
                    e.Graphics.DrawString(image.Name, SystemFonts.MenuFont, Brushes.Black, e.Bounds.X + image.ItemHeight + 16, e.Bounds.Y + 4);
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
                    foreach (MNReferencedImage img in Document.DefaultLanguage.Images)
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
                        data.Images.Remove(im);
                        data.Modified = true;
                    }
                }
            }
        }

        private void tsbImagesEdit_Click(object sender, EventArgs e)
        {
            ListBox lb = listBoxImages;
            if (lb.SelectedIndex < 0 || lb.SelectedIndex >= lb.Items.Count)
                return;
            PresentData(lb.Items[lb.SelectedIndex], null);
        }

        private void tsbImagesEditSpots_Click(object sender, EventArgs e)
        {
            if (listBoxImages.SelectedIndex >= 0 && listBoxImages.SelectedIndex < listBoxImages.Items.Count)
            {
                MNReferencedImage ri = (MNReferencedImage)listBoxImages.Items[listBoxImages.SelectedIndex];
                if (ri != null)
                {
                    PresentData(ri, "spots");
                }
            }
        }

        private void tsbTextAdd_Click(object sender, EventArgs e)
        {
            buttonNewObject_Click(SMContentType.Text);
        }

        private void tsbTextEdit_Click(object sender, EventArgs e)
        {
            ListBox lb = listBoxTexts;
            if (lb.SelectedIndex < 0 || lb.SelectedIndex >= lb.Items.Count)
                return;
            PresentData(lb.Items[lb.SelectedIndex], null);
        }

        private void tsbSoundEdit_Click(object sender, EventArgs e)
        {
            ListBox lb = listBoxSounds;
            if (lb.SelectedIndex < 0 || lb.SelectedIndex >= lb.Items.Count)
                return;
            PresentData(lb.Items[lb.SelectedIndex], null);
        }

        private void tsbSoundAdd_Click(object sender, EventArgs e)
        {
            buttonNewObject_Click(SMContentType.Audio);
        }

        private void tsbAudiTextAdd_Click(object sender, EventArgs e)
        {
            buttonNewObject_Click(SMContentType.AudioText);
        }

        private void tsbAudioTextEdit_Click(object sender, EventArgs e)
        {
            ListBox lb = listBoxAudioTexts;
            if (lb.SelectedIndex < 0 || lb.SelectedIndex >= lb.Items.Count)
                return;
            PresentData(lb.Items[lb.SelectedIndex], null);
        }

        private void buttonNewObject_Click(SMContentType preferredType)
        {
            DialogNewObject d = new DialogNewObject(preferredType);
            if (d.ShowDialog() == DialogResult.OK)
            {
                switch (d.ObjectType)
                {
                    case SMContentType.Image:
                        {
                            OpenFileDialog fd = new OpenFileDialog();
                            fd.Filter = "Images (*.png,*.jpg,*.bmp)|*.png;*.jpg;*.bmp|All Files (*.*)|*.*||";
                            if (fd.ShowDialog() == DialogResult.OK)
                            {
                                MNReferencedImage img = new MNReferencedImage();
                                img.ImageData = Image.FromFile(fd.FileName);
                                img.Name = d.ObjectName.Trim().Length > 0 ? d.ObjectName.Trim() : fd.FileName;
                                data.Images.Add(img);
                                data.Modified = true;
                                UpdateDataWithUI();
                            }
                        }
                        break;
                    case SMContentType.AudioText:
                        {
                            MNReferencedAudioText ra = new MNReferencedAudioText();
                            ra.Name = d.ObjectName;
                            data.AudioTexts.Add(ra);
                            data.Modified = true;
                            UpdateDataWithUI();
                        }
                        break;
                    case SMContentType.Audio:
                        {
                            OpenFileDialog fd = new OpenFileDialog();
                            fd.Filter = "Sounds (*.wav,*.mp3,*.aiff)|*.wav;*.mp3;*.aiff|All Files (*.*)|*.*||";
                            if (fd.ShowDialog() == DialogResult.OK)
                            {
                                MNReferencedSound img = new MNReferencedSound();
                                img.InitializeWithFile(fd.FileName);
                                img.Name = d.ObjectName.Trim().Length > 0 ? d.ObjectName.Trim() : fd.FileName;
                                data.Sounds.Add(img);
                                data.Modified = true;
                                UpdateDataWithUI();
                            }
                        }
                        break;
                    case SMContentType.Text:
                        {
                            MNReferencedText rt = new MNReferencedText();
                            rt.Name = d.ObjectName;
                            data.Texts.Add(rt);
                            data.Modified = true;
                            UpdateDataWithUI();
                        }
                        break;
                }
            }
        }

        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            UpdateUIWithData();
        }

        private void listBoxStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = listBoxStyles;
            if (lb.SelectedIndex < 0 || lb.SelectedIndex >= lb.Items.Count)
                return;
            PresentData(lb.Items[lb.SelectedIndex], null);
        }

        private void tsbStylesAdd_Click(object sender, EventArgs e)
        {
            string styleName = "";
            MNReferencedStyle selectedStyle = null;
            ListBox lb = listBoxStyles;
            if (lb.SelectedIndex >= 0 && lb.SelectedIndex < lb.Items.Count)
            {
                selectedStyle = lb.Items[lb.SelectedIndex] as MNReferencedStyle;
                styleName = selectedStyle.Name + " (copy)";
            }
            else
            {
                selectedStyle = data.Styles[0];
                for (int i = 0; i < 99; i++)
                {
                    styleName = string.Format("String{0}", i);
                    if (data.FindStyle(styleName) == null)
                        break;
                }
            }

            if (styleName.Length > 0)
            {
                DialogStyleName dlg = new DialogStyleName();
                dlg.Document = data;
                dlg.StyleName = styleName;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    styleName = dlg.StyleName;
                }
                else
                {
                    styleName = "";
                }
            }

            if (styleName.Length > 0)
            {
                MNReferencedStyle sm = selectedStyle.CreateCopy();
                sm.Name = styleName;
                data.Styles.Add(sm);
                data.Modified = true;
                listBoxStyles.Items.Add(sm);
                listBoxStyles.SelectedIndex = listBoxStyles.Items.Count - 1;
                MNNotificationCenter.BroadcastMessage(this, "StyleListChanged");
            }
        }

        private void LocalizationMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnSave();

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogSelectLanguage dialog = new DialogSelectLanguage();
            dialog.mainForm = this;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
            }
        }

        private void listBoxImages_MouseDown(object sender, MouseEventArgs e)
        {
            int index = listBoxImages.IndexFromPoint(e.X, e.Y);
            if (index >= 0 && index < listBoxImages.Items.Count)
            {
                DragDropEffects de = listBoxImages.DoDragDrop(listBoxImages.Items[index], DragDropEffects.Copy);
            }
        }

        private void tsbAddImageToLibrary(object sender, EventArgs e)
        {
            if (listBoxImages.SelectedIndex >= 0 && listBoxImages.SelectedIndex < listBoxImages.Items.Count)
            {
                MNReferencedImage ri = (MNReferencedImage)listBoxImages.Items[listBoxImages.SelectedIndex];
                if (ri != null)
                {
                    MNSharedObjects.AddImage(ri);
                    MNSharedObjects.Save();
                }
            }
        }

        private void tsbAddStyleToLibrary(object sender, EventArgs e)
        {
            ListBox lb = listBoxStyles;
            if (lb.SelectedIndex >= 0 && lb.SelectedIndex < lb.Items.Count)
            {
                MNReferencedStyle selectedStyle = lb.Items[lb.SelectedIndex] as MNReferencedStyle;
                if (selectedStyle != null)
                {
                    MNSharedObjects.AddStyle(selectedStyle);
                    MNSharedObjects.Save();
                }
            }
        }


    }
}
