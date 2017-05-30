using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SlideMaker.Views
{
    public class KeyPageActions
    {
        public class KPAction
        {
            public int page;
            public Keys key;
            public char chr;
            public string name;
            public int nextPage = -1;
            public Del del = null;
        }

        public List<KPAction> acts = new List<KPAction>();

        public PageScrollArea pageScroll;
        public PageEditView editView;
        public TabControl tabControl;
        public TabPage actionTabPage;
        public ListView listView;

        private int previousTabIndex = 0;

        private int currActions = 0;

        private bool b_is_key_action_mode = false;

        public bool IsKeyActionMode
        {
            get { return b_is_key_action_mode; }
            set { b_is_key_action_mode = value; }
        }

        public KeyPageActions()
        {
            InitializeActions();
        }

        public void Act(int page, Keys k, char c, string name, int np)
        {
            KPAction kpa = new KPAction();
            kpa.chr = c;
            kpa.del = null;
            kpa.nextPage = np;
            kpa.key = k;
            kpa.name = name;
            kpa.page = page;
            acts.Add(kpa);
        }

        public void Act(int page, Keys k, char c, string name, Del d)
        {
            KPAction kpa = new KPAction();
            kpa.chr = c;
            kpa.del = d;
            kpa.nextPage = -1;
            kpa.key = k;
            kpa.name = name;
            kpa.page = page;
            acts.Add(kpa);
        }

        public void StartKeyActionMode()
        {
            Debugger.Log(0, "", "StartKeyActionMode\n");
            previousTabIndex = tabControl.SelectedIndex;
            tabControl.SelectedIndex = 2;
            IsKeyActionMode = true;
            currActions = 0;
            editView.Focus();
            FillListView();
        }

        public void StopKeyActionMode()
        {
            Debugger.Log(0, "", "StopKeyActionMode\n");
            tabControl.SelectedIndex = previousTabIndex;
            IsKeyActionMode = false;
            editView.Focus();
        }

        public void KeyActionMode(Keys a)
        {
            if (IsKeyActionMode)
            {
                if (a == Keys.Escape)
                {
                    StopKeyActionMode();
                    return;
                }
                KPAction kpa = FindAction(currActions, a);
                if (kpa == null)
                {
                    StopKeyActionMode();
                    return;
                }
                else
                {
                    if (kpa.nextPage >= 0)
                    {
                        currActions = kpa.nextPage;
                        FillListView();
                    }
                    else if (kpa.del != null)
                    {
                        kpa.del.Invoke();
                        StopKeyActionMode();
                    }
                }
            }
        }

        public KPAction FindAction(int page, Keys a)
        {
            foreach (KPAction kpa in acts)
            {
                if (page == kpa.page && a == kpa.key)
                    return kpa;
            }
            return null;
        }

        public delegate void Del();

        public void FillListView()
        {
            listView.Items.Clear();
            foreach (KPAction kpa in acts)
            {
                if (kpa.page == currActions)
                {
                    ListViewItem lvi = new ListViewItem(kpa.chr.ToString());
                    lvi.SubItems.Add(kpa.name);
                    lvi.Tag = kpa;
                    listView.Items.Add(lvi);
                }
            }

            Del d = delegate() { currActions = 0; };
        }

        private void InitializeActions()
        {
            Act(0, Keys.A, 'A', "Align...", 1);
            Act(0, Keys.M, 'M', "Make...", 2);
            Act(0, Keys.S, 'S', "Set...", 5);
            Act(0, Keys.I, 'I', "Insert...", 8);
            Act(0, Keys.D, 'D', "Duplicate", delegate() { editView.DuplicateItems(); });

            Act(1, Keys.Y, 'Y', "Top & Bottom", delegate() { editView.AlignHorizontal(); });
            Act(1, Keys.X, 'X', "Left & Right", delegate() { editView.AlignVertical(); });
            Act(1, Keys.W, 'W', "Width", delegate() { editView.AlignWidth(); });
            Act(1, Keys.H, 'H', "Height", delegate() { editView.AlignHeight(); });

            Act(2, Keys.G, 'G', "Group...", 3);

            Act(3, Keys.D0, '0', "Group selectable min 0 max 1", delegate() { editView.MakeGroup(0, null); });
            Act(3, Keys.D1, '1', "Group selectable min 1 max 1", delegate() { editView.MakeGroup(1, null); });
            Act(3, Keys.D2, '2', "Group selectable min 0 max N", delegate() { editView.MakeGroup(2, null); });

            Act(5, Keys.C, 'C', "Expected Checked...", 4);
            Act(5, Keys.D, 'D', "Drag Effect...", 6);
            Act(5, Keys.A, 'A', "Cardinality...", 7);
            Act(5, Keys.L, 'L', "Drag Line Align...", 9);
            Act(5, Keys.N, 'N', "Names", delegate() { editView.SetSelectionProperty("names", 0); });
            Act(5, Keys.T, 'T', "Tags", delegate() { editView.SetSelectionProperty("tags", 0); });

            Act(4, Keys.D0, '0', "Unchecked (expected check status)", delegate() { editView.SetExpectedChecked(0); });
            Act(4, Keys.D1, '1', "Checked (expected check status)", delegate() { editView.SetExpectedChecked(1); });
            Act(4, Keys.N, 'N', "Not expected", delegate() { editView.SetExpectedChecked(2); });

            Act(6, Keys.N, 'N', "None", delegate() { editView.SetSelectionProperty("drag", 0); });
            Act(6, Keys.L, 'L', "Line", delegate() { editView.SetSelectionProperty("drag", 1); });
            Act(6, Keys.D, 'D', "Drag", delegate() { editView.SetSelectionProperty("drag", 2); });
            Act(6, Keys.U, 'U', "Undefined", delegate() { editView.SetSelectionProperty("drag", 3); });

            Act(7, Keys.U, 'U', "Undef", delegate() { editView.SetSelectionProperty("cardinality", 0); });
            Act(7, Keys.D0, '0', "None", delegate() { editView.SetSelectionProperty("cardinality", 1); });
            Act(7, Keys.D1, '1', "One", delegate() { editView.SetSelectionProperty("cardinality", 2); });
            Act(7, Keys.D9, '9', "Many", delegate() { editView.SetSelectionProperty("cardinality", 3); });

            Act(8, Keys.L, 'L', "Layout (of Page)", delegate() { editView.InsertPageLayout(); });

            Act(9, Keys.U, 'U', "Undef", delegate() { editView.SetSelectionProperty("dragline", 0); });
            Act(9, Keys.T, 'T', "Top-Bottom", delegate() { editView.SetSelectionProperty("dragline", 1); });
            Act(9, Keys.L, 'L', "Left-Right", delegate() { editView.SetSelectionProperty("dragline", 2); });

        }



    }
}
