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
    public partial class EVLabel : UserControl
    {
        public SMLabel Object = null;

        public EVLabel()
        {
            InitializeComponent();
        }

        public void SetObject(SMLabel obj)
        {
            Object = obj;
            comboBox1.SelectedIndex = (int)obj.Area.BackType;
            switch (obj.Area.Dock)
            {
                case SMControlSelection.None:
                    comboBox2.SelectedIndex = 0;
                    break;
                case SMControlSelection.Left:
                    comboBox2.SelectedIndex = 1;
                    break;
                case SMControlSelection.Top:
                    comboBox2.SelectedIndex = 2;
                    break;
                case SMControlSelection.Right:
                    comboBox2.SelectedIndex = 3;
                    break;
                case SMControlSelection.Bottom:
                    comboBox2.SelectedIndex = 4;
                    break;
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Object.Area.BackType = (SMBackgroundType)comboBox1.SelectedIndex;
            EVContainer.Shared.OnValueUpdatedImmediate();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    Object.Area.Dock = SMControlSelection.None;
                    break;
                case 1:
                    Object.Area.Dock = SMControlSelection.Left;
                    break;
                case 2:
                    Object.Area.Dock = SMControlSelection.Top;
                    break;
                case 3:
                    Object.Area.Dock = SMControlSelection.Right;
                    break;
                case 4:
                    Object.Area.Dock = SMControlSelection.Bottom;
                    break;
            }
            EVContainer.Shared.OnValueUpdatedImmediate();
        }
    }
}
