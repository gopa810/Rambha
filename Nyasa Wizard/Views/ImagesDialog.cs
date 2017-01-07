using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class ImagesDialog : Form
    {
        public ImagesDialog()
        {
            InitializeComponent();
        }

        public void SetDocument(MNDocument doc)
        {
            imagesListView1.Document = doc;
            imagesListView1.RefreshImageList();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
