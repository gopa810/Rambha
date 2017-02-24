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
    public partial class EVReferencedImage : UserControl
    {
        private MNDocument doc = null;
        private MNLazyImage image = null;

        public EVReferencedImage()
        {
            InitializeComponent();
        }
        
        public delegate void NormalEventDeleg(object sender, EventArgs e);

        public event NormalEventDeleg OnSelectionChanged;

        public void SetDocument(MNDocument doci)
        {
            doc = doci;
            comboBox1.Items.Clear();
            foreach (MNReferencedImage img in doc.DefaultLanguage.Images)
            {
                comboBox1.Items.Add(img);
            }
        }

        public void SetImage(MNLazyImage li)
        {
            doc = li.Document;
            image = li;
            comboBox1.Items.Clear();
            int i = 0;
            int selected = -1;
            foreach (MNReferencedImage img in doc.DefaultLanguage.Images)
            {
                comboBox1.Items.Add(img);
                if (img == li.Image)
                    selected = i;
                i++;
            }
            if (selected >= 0)
                comboBox1.SelectedIndex = selected;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedIndex < comboBox1.Items.Count)
            {
                if (image != null)
                {
                    image.Image = (MNReferencedImage)comboBox1.Items[comboBox1.SelectedIndex];
                    if (OnSelectionChanged != null)
                        OnSelectionChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool HasSelectedImage
        {
            get
            {
                return comboBox1.SelectedIndex >= 0 && comboBox1.SelectedIndex < comboBox1.Items.Count;
            }
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            if (image != null)
                SetImage(image);
            else if (doc != null)
                SetDocument(doc);
        }


    }
}
