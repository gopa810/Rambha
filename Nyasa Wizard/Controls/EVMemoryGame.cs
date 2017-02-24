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
    public partial class EVMemoryGame : UserControl
    {
        public EVMemoryGame()
        {
            InitializeComponent();
        }

        private SMMemoryGame control = null;
        private MNLazyImage tempImg = null;

        public void SetControl(SMMemoryGame mg)
        {
            control = mg;
            if (mg != null)
            {
                evReferencedImage1.SetImage(mg.BackImage);
                tempImg = new MNLazyImage(mg.Document);
                evReferencedImage2.SetImage(tempImg);
                listBox1.Items.Clear();
                for (int i = 0; i < mg.GetCardCount(); i++)
                {
                    listBox1.Items.Add(mg.GetReferencedImageAt(i));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (control != null)
            {
                control.AddCard(textBox1.Text, tempImg.Image);
                EVContainer.Shared.OnValueUpdated();
                textBox1.Text = "";
                listBox1.Items.Add(tempImg.Image);
                tempImg.ImageId = -1;
                tempImg.Image = null;
                evReferencedImage2.SetImage(tempImg);

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateAddButtonStatus();
        }

        private void UpdateAddButtonStatus()
        {
            button1.Enabled = (textBox1.Text.Length > 0 && evReferencedImage2.HasSelectedImage);
        }

        private void evReferencedImage2_OnSelectionChanged(object sender, EventArgs e)
        {
            UpdateAddButtonStatus();
            //PropertyPanelsContainer.Shared.OnValueUpdated();
        }

        private void evReferencedImage1_OnSelectionChanged(object sender, EventArgs e)
        {
            EVContainer.Shared.OnValueUpdated();
        }
    }
}
