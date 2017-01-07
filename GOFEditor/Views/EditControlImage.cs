using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Rambha.GOF;

namespace GOFEditor.Views
{
    public partial class EditControlImage : UserControl
    {
        public MainForm ParentFrame { get; set; }
        public GOFImage Value { get; set; }


        public EditControlImage()
        {
            InitializeComponent();
        }

        public void SetValues(string key, GOFImage img)
        {
            labelKey.Text = key;
            Value = img;
            pictureBox1.Image = img.Image;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ParentFrame.CloseCurrentPresentation();
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".tif"))
                    {
                        string key = Path.GetFileNameWithoutExtension(file);
                        Value.SetData(File.ReadAllBytes(file));
                        pictureBox1.Image = Value.Image;
                        break;
                    }
                }
            }
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
