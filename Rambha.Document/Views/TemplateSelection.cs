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

namespace Rambha.Document.Views
{
    public partial class TemplateSelection : UserControl
    {
        public MNPage template = null;
        private IWindowsFormsEditorService editorService = null;

        public TemplateSelection(MNPage tmp, IWindowsFormsEditorService service)
        {
            template = tmp;
            editorService = service;
            InitializeComponent();
            MNDocument doc = MNNotificationCenter.CurrentDocument;
            int i = 1;
            int seld = 0;
            listBox1.Items.Add("<none>");
            foreach(MNPage rimg in doc.Templates)
            {
                listBox1.Items.Add(rimg);
                if (rimg == tmp) seld = i;
                i++;
            }
            listBox1.SelectedIndex = seld;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                if (listBox1.SelectedItem is string)
                {
                    template = null;
                }
                else if (listBox1.SelectedItem is MNPage)
                {
                    template = listBox1.SelectedItem as MNPage;
                }
                editorService.CloseDropDown();
            }
        }
    }

    internal class TemplateSelectionEditor : UITypeEditor
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
                TemplateSelection selectionControl =
                    new TemplateSelection(
                    (MNPage)value,
                    editorService);

                editorService.DropDownControl(selectionControl);

                value = selectionControl.template;
            }

            return value;
        }
    }

}
