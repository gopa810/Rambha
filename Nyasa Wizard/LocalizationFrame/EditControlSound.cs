using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;

using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class EditControlSound : UserControl
    {
        public LocalizationMainForm ParentFrame { get; set; }

        public MNReferencedSound Value { get; set; }

        private AudioPlayer Player = new AudioPlayer();

        public EditControlSound()
        {
            InitializeComponent();
        }

        public void SetValue(MNReferencedSound value)
        {
            labelKey.Text = value.Name;
            Value = value;
            Player.SetSound(value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Player.DisposeAll();
            ParentFrame.CloseCurrentPresentation();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Player.Play();
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (file.EndsWith(".mp3") || file.EndsWith(".wav") || file.EndsWith(".aiff"))
                    {
                        string key = Path.GetFileNameWithoutExtension(file);
                        Value.InitializeWithFile(file);
                        break;
                    }
                }
            }
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
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
