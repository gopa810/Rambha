using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
//using System.Windows.Forms.ComponentModel;

namespace Rambha.Document.Views
{
    public partial class ImageSelectionPropertyControl : UserControl
    {
        public MNReferencedImage image { get; set; }
        private IWindowsFormsEditorService editorService = null;

        public ImageSelectionPropertyControl()
        {
            InitializeComponent();
        }

        public ImageSelectionPropertyControl(MNReferencedImage img, IWindowsFormsEditorService service)
        {
            image = img;
            editorService = service;
            InitializeComponent();
            if (img != null && img.Document != null)
            {
                foreach(MNReferencedImage rimg in img.Document.Images)
                {
                    listBox1.Items.Add(rimg);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                image = listBox1.SelectedItem as MNReferencedImage;
                editorService.CloseDropDown();
            }
        }
    }

    internal class ImageSelectionPropertyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;

            if (provider != null)
            {
                editorService =
                    provider.GetService(
                    typeof(IWindowsFormsEditorService))
                    as IWindowsFormsEditorService;
            }

            if (editorService != null)
            {
                ImageSelectionPropertyControl selectionControl =
                    new ImageSelectionPropertyControl(
                    (MNReferencedImage)value,
                    editorService);

                editorService.DropDownControl(selectionControl);

                value = selectionControl.image;
            }

            return value;
        }
    }

}
