using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;

using Rambha.Document;


namespace SlideMaker.Views
{
    public partial class EditControlAudioText : UserControl
    {
        public LocalizationMainForm ParentFrame { get; set; }

        public MNReferencedAudioText Value { get; set; }

        public AudioPlayer player = new AudioPlayer();

        public DateTime startTicks = DateTime.Now;

        public EditControlAudioText()
        {
            InitializeComponent();
        }


        public void SetValue(MNReferencedAudioText val)
        {
            labelKey.Text = val.Name;
            Value = val;
            richTextBox1_Locked = true;
            richTextBox1.Text = val.Text;
            richTextBox1_Locked = false;
            UpdateListboxWithWords();
            player.SetSound(val.Sound);
        }

        private bool richTextBox1_Locked = false;

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1_Locked)
            {
                UpdateWordsList();
                UpdateListboxWithWords();
                Value.Text = richTextBox1.Text;
            }
        }

        private void UpdateWordsList()
        {
            if (Value != null)
                Value.ClearWords();
            bool bFirst = true;
            foreach (string s in richTextBox1.Lines)
            {
                GOFRunningTextItem rti = new GOFRunningTextItem() { Text = s, TimeOffset = 0 };
                if (bFirst)
                {
                    rti.Valid = true;
                    rti.TimeOffset = 0;
                }
                if (Value != null)
                    Value.AddWord(rti);
                bFirst = false;
            }

        }

        private void UpdateListboxWithWords()
        {
            listBox1.Items.Clear();
            foreach (GOFRunningTextItem rti in Value.Words)
            {
                listBox1.Items.Add(rti);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ParentFrame.CloseCurrentPresentation();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            player.Play();
            listBox1.SelectedIndex = -1;
            startTicks = DateTime.Now;
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
                        Value.Sound = new MNReferencedSound();
                        Value.Sound.InitializeWithFile(file);
                        player.SetSound(Value.Sound);
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                GOFRunningTextItem gi = (GOFRunningTextItem)(listBox1.SelectedItem);
                if (gi != null && player.Playing)
                {
                    gi.TimeOffset = Convert.ToInt64((DateTime.Now - startTicks).TotalMilliseconds);
                    gi.Valid = true;
                }
            }
        }

        private StringFormat p_sformat = null;

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0 && e.Index < listBox1.Items.Count)
            {
                Rectangle rect = e.Bounds;
                rect.Width = rect.Width / 2 - e.Bounds.Height;
                rect.X += e.Bounds.Height;

                e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
                if (p_sformat == null)
                {
                    p_sformat = new StringFormat();
                    p_sformat.Alignment = StringAlignment.Near;
                    p_sformat.LineAlignment = StringAlignment.Center;
                    p_sformat.Trimming = StringTrimming.EllipsisCharacter;
                }

                GOFRunningTextItem gi = (GOFRunningTextItem)(listBox1.Items[e.Index]);

                if (gi != null)
                {
                    e.Graphics.DrawString(gi.Text, listBox1.Font, SystemBrushes.ControlText, rect, p_sformat);
                    rect.X += rect.Width;
                    if (gi.Valid)
                    {
                        string time = string.Format("{0} ms", (long)(gi.TimeOffset));
                        e.Graphics.DrawString(time, listBox1.Font, Brushes.DarkBlue, rect, p_sformat);
                    }
                    else
                    {
                        e.Graphics.DrawString("??", listBox1.Font, Brushes.Gray, rect);
                    }
                }

                if ((e.State & DrawItemState.Selected) != 0)
                {
                    rect.Width = rect.Height;
                    rect.X = 0;
                    rect.Inflate(-2,-2);
                    e.Graphics.FillRectangle(Brushes.DarkCyan, rect);
                }
            }
        }
    }
}
