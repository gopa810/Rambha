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
using System.Windows.Forms.ComponentModel;

namespace Rambha.Document.Views
{
    public partial class FontSizePropertyEditor : UserControl
    {
        public int fontSize = 10;
        private IWindowsFormsEditorService editorService = null;
        public static int[] FontSizes = { 40, 50, 60, 70, 80, 90, 100, 120, 140 };

        public FontSizePropertyEditor(int fontSizeIn, IWindowsFormsEditorService service)
        {
            fontSize = fontSizeIn;
            editorService = service;
            InitializeComponent();
            numericUpDown1.Value = fontSize;
            foreach (int c in FontSizes)
            {
                listBox1.Items.Add(c.ToString());
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < listBox1.Items.Count)
            {
                numericUpDown1.Value = Convert.ToDecimal(int.Parse((string)listBox1.SelectedItem));
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            fontSize = Convert.ToInt32(numericUpDown1.Value);
        }
    }

    internal class FontSizeEditor : UITypeEditor
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
                FontSizePropertyEditor selectionControl =
                    new FontSizePropertyEditor(
                    (int)value,
                    editorService);

                editorService.DropDownControl(selectionControl);

                value = selectionControl.fontSize;
            }

            return value;
        }
    }
}
