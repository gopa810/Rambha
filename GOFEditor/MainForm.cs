using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Rambha.GOF;
using GOFEditor.Views;

namespace GOFEditor
{
    public partial class MainForm : Form
    {
        GOFile data = null;

        string fileName = null;

        EditControlDirectory dirView = null;
        EditControlString stringView = null;
        EditControlImage imageView = null;
        EditControlSound soundView = null;
        EditControlRunningText runtextView = null;

        private List<UserControl> viewStack = new List<UserControl>();

        public MainForm()
        {
            InitializeComponent();

            InitializeLanguageList();

            OnNew();
        }

        public void InitializeLanguageList()
        {
            foreach (SVLanguage lang in SVLanguageCollection.List)
            {
                toolStripComboBox1.Items.Add(lang);
            }
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
            d.Filter = "Language Pack Files (*.rpl)|*.rpl||";
            if (Properties.Settings.Default.LastDirectory.Length > 0)
                d.InitialDirectory = Properties.Settings.Default.LastDirectory;
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                data = new GOFile();
                fileName = d.FileName;
                data.Load(d.FileName, true);
                UpdateUIWithData();
                SelectedLanguageCode = data.GetProperty("LanguageCode");
                toolStripTextBox1.Text = data.GetProperty("BookCode");
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

        private void OnNew()
        {
            OnSave();
            data = new GOFile();
            UpdateUIWithData();
            PresentData(data, "");
        }

        private void OnSave()
        {
            if (data != null && data.IsModified())
            {
                if (fileName == null)
                {
                    SaveFileDialog d = new SaveFileDialog();
                    d.RestoreDirectory = true;
                    if (Properties.Settings.Default.LastDirectory.Length > 0)
                        d.InitialDirectory = Properties.Settings.Default.LastDirectory;
                    d.FileName = (SelectedBookCode +"_" + SelectedLanguageCode + ".rpl").ToLower();
                    d.DefaultExt = ".rpl";
                    if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Properties.Settings.Default.LastDirectory = Path.GetDirectoryName(d.FileName);
                        Properties.Settings.Default.Save();
                        fileName = d.FileName;
                    }
                    else
                    {
                        return;
                    }
                }

                data.SetProperty("BookCode", SelectedBookCode);
                data.SetProperty("LanguageCode", SelectedLanguageCode);
                data.SetProperty("LanguageName", SVLanguageCollection.GetName(SelectedLanguageCode));
                data.Save(fileName);
            }
        }

        private string SelectedBookCode
        {
            get { return toolStripTextBox1.Text.ToUpper(); }
            set { toolStripTextBox1.Text = value.ToUpper(); }
        }

        private string SelectedLanguageCode
        {
            get
            {
                if (toolStripComboBox1.SelectedIndex >= 0)
                {
                    SVLanguage lang = toolStripComboBox1.Items[toolStripComboBox1.SelectedIndex] as SVLanguage;
                    return lang.ISOCode2;
                }

                return "ENG";
            }
            set
            {
                for (int i = 0; i < toolStripComboBox1.Items.Count; i++)
                {
                    SVLanguage lang = toolStripComboBox1.Items[i] as SVLanguage;
                    if (lang.ISOCode2.Equals(value))
                    {
                        toolStripComboBox1.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void UpdateUIWithData()
        {
            if (data == null)
                return;

            toolStripTextBox1.Text = data.GetProperty("BookCode");
        }

        public void PresentData(object data, string key)
        {
            if (data is GOFile)
            {
                if (dirView == null)
                {
                    dirView = PrepareControl(new EditControlDirectory()) as EditControlDirectory;
                }
                AddToStack(dirView);

                dirView.ParentFrame = this;
                dirView.SetLanguageData(data as GOFile);
                dirView.Visible = true;
            }
            else if (data is GOFRunningText)
            {
                if (runtextView == null)
                {
                    runtextView = PrepareControl(new EditControlRunningText()) as EditControlRunningText;
                }
                AddToStack(runtextView);

                runtextView.ParentFrame = this;
                runtextView.SetValue(key, data as GOFRunningText);
                runtextView.Visible = true;
            }
            else if (data is GOFSound)
            {
                if (soundView == null)
                {
                    soundView = PrepareControl(new EditControlSound()) as EditControlSound;
                }
                AddToStack(soundView);

                soundView.ParentFrame = this;
                soundView.SetValue(key, data as GOFSound);
                soundView.Visible = true;
            }
            else if (data is GOFString)
            {
                if (stringView == null)
                {
                    stringView = PrepareControl(new EditControlString()) as EditControlString;
                }
                AddToStack(stringView);

                stringView.ParentFrame = this;
                stringView.SetValue(key, data as GOFString);
                stringView.Visible = true;
            }
            else if (data is GOFImage)
            {
                if (imageView == null)
                    imageView = PrepareControl(new EditControlImage()) as EditControlImage;
                AddToStack(imageView);

                imageView.ParentFrame = this;
                imageView.SetValues(key, data as GOFImage);
                imageView.Visible = true;
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

        public GOFNodes GetCurrentDirectory()
        {
            return dirView.GetPathLeaf();
        }

        public void ReplaceType(string key, GOFString oldValue, GOFRunningText newValue)
        {
            GOFNodes nodes = GetCurrentDirectory();
            nodes.SetValue(key, newValue);
            CloseCurrentPresentation();
            PresentData(newValue, key);
            dirView.ReplaceNode(key, oldValue, newValue);
        }
    }
}
