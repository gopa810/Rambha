using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker.Views
{
    public partial class EditControlTextStyle : UserControl
    {
        public LocalizationMainForm ParentFrame { get; set; }

        public EditControlTextStyle()
        {
            InitializeComponent();
        }

        public void SetValue(MNReferencedStyle value)
        {
            propertyGrid1.SelectedObject = value;
        }
    }
}
