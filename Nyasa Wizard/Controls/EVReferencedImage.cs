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
        public SMImage control = null;

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
            control = null;
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
            control = null;
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

        public string HeaderText
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
            }
        }

        public Color HeaderBackColor
        {
            get
            {
                return label1.BackColor;
            }
            set
            {
                label1.BackColor = value;
            }
        }

        public void SetControl(SMImage sMImage)
        {
            SetDocument(sMImage.Document);
            SetImage(sMImage.Img);
            control = sMImage;
            if (control != null)
                trackBar1.Value = control.SourceOffsetX / 5;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (control != null)
            {
                control.SourceOffsetX = trackBar1.Value * 5;
                control.SourceOffsetY = trackBar1.Value * 5;
                EVContainer.Shared.OnValueUpdatedImmediate();
            }
        }
    }
}
