using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class ImagesListView : UserControl
    {
        public MNDocument Document { get; set; }

        public ImagesListView()
        {
            InitializeComponent();
        }


        public void RefreshImageList()
        {
            listView1.Items.Clear();
            if (Document != null)
            {
                foreach (MNReferencedImage img in Document.Images)
                {
                    ListViewItem lvi = new ListViewItem();
                    UpdateListViewItem(lvi, img);
                    listView1.Items.Add(lvi);
                }
            }
        }

        public void UpdateListViewItem(ListViewItem lvi, MNReferencedImage img)
        {
            lvi.SubItems.Clear();
            lvi.Text = img.Title;
            lvi.Tag = img;
            lvi.SubItems.Add(string.Format("{0}x{1}", img.ImageData.Size.Width, img.ImageData.Size.Height));
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Delete images?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<MNReferencedImage> imgs = new List<MNReferencedImage>();
                    foreach (ListViewItem im in listView1.SelectedItems)
                    {
                        imgs.Add((MNReferencedImage)im.Tag);
                    }
                    foreach (MNReferencedImage im in imgs)
                    {
                        Document.Images.Remove(im);
                    }
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemove.Enabled = true;
        }
    }
}
