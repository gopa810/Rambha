using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class EditControlImage : UserControl
    {
        public LocalizationMainForm ParentFrame { get; set; }
        public MNReferencedImage Value { get; set; }


        public EditControlImage()
        {
            InitializeComponent();
        }

        public void SetValues(MNReferencedImage img)
        {
            labelKey.Text = img.Name;
            Value = img;
            pictureBox1.Image = img.ImageData;
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
                        Value.ImageData = Image.FromFile(file);
                        pictureBox1.Image = Value.ImageData;
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
