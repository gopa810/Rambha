using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using SlideMaker.Views;

namespace SlideMaker
{
    public partial class EVContainer : UserControl
    {
        public static EVContainer Shared = null;

        public int counter = 0;

        public EVContainer()
        {
            InitializeComponent();
            tm.Interval = 500;
            tm.Tick += new EventHandler(tm_Tick);
            Shared = this;
        }

        ~EVContainer()
        {
            tm.Stop();
        }

        private Timer tm = new Timer();

        public List<Control> Panels = new List<Control>();

        public PageEditView EditView = null;

        public void OnPanelVisibilityChange(ControlSeparator sender)
        {
            RecalculatePositions();
        }

        void tm_Tick(object sender, EventArgs e)
        {
            if (EditView != null)
            {
                if (counter > 0)
                {
                    counter--;
                }
                else
                {
                    EditView.Invalidate();
                    tm.Stop();
                }
            }
        }

        public void OnValueUpdated()
        {
            if (EditView != null && !tm.Enabled)
                tm.Start();
            counter = 5;
        }

        public void OnValueUpdatedImmediate()
        {
            if (EditView != null)
                EditView.Invalidate();
        }

        public void RecalculatePositions()
        {
            int x = 0;
            foreach (Control uc in Panels)
            {
//                Debugger.Log(0, "", string.Format("CONTROL {0} \nBounds {1}\n\n", uc.GetType().Name, uc.Bounds));
                //if (uc.Visible)
                {
                    uc.Location = new Point(0, x);
                    uc.Width = panel1.ClientSize.Width;
                    uc.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                    x += uc.Height + 2;
                }
            }

            panel1.Height = x + 22;
        }

        public void ClearPanels()
        {
            Panels.Clear();
            panel1.Controls.Clear();
        }

        public void AddPanel(string name, UserControl uc)
        {
            /*ControlSeparator cs = new ControlSeparator(this, uc);
            cs.Title = name;
            cs.Visible = true;
            cs.Parent = panel1;
            uc.Visible = true;
            panel1.Controls.Add(cs);*/
            panel1.Controls.Add(uc);
            Panels.Add(uc);

            RecalculatePositions();
        }

    }
}
