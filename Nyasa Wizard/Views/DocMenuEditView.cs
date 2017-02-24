using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker
{
    public partial class DocMenuEditView : UserControl
    {
        public DocMenuEditView()
        {
            InitializeComponent();
        }

        private MNMenu p_editedMenu = null;
        private MNMenuItem p_editedItem = null;

        public MNMenu Menu
        {
            get
            {
                return p_editedMenu;
            }
            set
            {
                p_editedMenu = value;
                if (value != null)
                {
                    listBox1.Items.Clear();
                    textBox1.Text = p_editedMenu.APIName;
                    textBox2.Text = p_editedMenu.UserTitle;
                    foreach (MNMenuItem mi in p_editedMenu.Items)
                    {
                        listBox1.Items.Add(mi);
                    }
                    SelectMenuItem(null);
                }
            }
        }

        private void SelectMenuItem(MNMenuItem mi)
        {
            p_editedItem = mi;
            pictureBox1.Image = null;
            textBox3.Text = "";
            richTextBox1.Text = "";

            if (mi != null)
            {
                if (mi.Image != null)
                    pictureBox1.Image = mi.Image.ImageData;
                textBox3.Text = mi.Text;
                richTextBox1.Text = mi.ActionScript;
            }
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(MNReferencedImage)) && p_editedItem != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(MNReferencedImage)) && p_editedItem != null)
            {
                MNReferencedImage ri = (MNReferencedImage)e.Data.GetData(typeof(MNReferencedImage));
                if (ri != null)
                {
                    p_editedItem.Image = ri;
                    pictureBox1.Image = ri.ImageData;
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (p_editedItem != null)
                p_editedItem.ActionScript = richTextBox1.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (p_editedItem != null)
            {
                p_editedItem.Text = textBox3.Text;
                listBox1.Invalidate();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < listBox1.Items.Count)
            {
                if (p_editedMenu != null && listBox1.SelectedIndex < p_editedMenu.Items.Count)
                {
                    SelectMenuItem(p_editedMenu.Items[listBox1.SelectedIndex]);
                }
                else
                {
                    SelectMenuItem(null);
                }
            }
            else
            {
                SelectMenuItem(null);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (p_editedMenu != null)
                p_editedMenu.APIName = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (p_editedMenu != null)
            {
                p_editedMenu.UserTitle = textBox2.Text;
                listBox1.Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (p_editedMenu != null)
            {
                MNMenuItem mi = new MNMenuItem();
                p_editedMenu.Items.Add(mi);
                listBox1.Items.Add(mi);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                SelectMenuItem(mi);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < listBox1.Items.Count)
            {
                if (p_editedMenu != null)
                {
                    int index = listBox1.SelectedIndex;
                    listBox1.Items.RemoveAt(index);
                    p_editedMenu.Items.RemoveAt(index);
                }
            }
        }

        private void DocMenuEditView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(MNReferencedImage)) && p_editedItem != null)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void DocMenuEditView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(MNReferencedImage)) && p_editedItem != null)
            {
                MNReferencedImage ri = (MNReferencedImage)e.Data.GetData(typeof(MNReferencedImage));
                if (ri != null)
                {
                    p_editedItem.Image = ri;
                    pictureBox1.Image = ri.ImageData;
                }
            }
        }
    }
}
