using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Rambha.Document;

namespace SlideMaker
{
    public partial class DialogInsertTransPages : Form
    {
        private MNDocument doc = null;
        private int[] pnum = new int[3];

        public DialogInsertTransPages()
        {
            InitializeComponent();

            groupBox1.Visible = false;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
        }

        public void SetDocument(MNDocument d)
        {
            int groupIndex = 0;
            doc = d;
            int m = doc.Data.Pages.Count;

            for (int i = 0; i < m; i++)
            {
                comboPage1.Items.Add(doc.Data.Pages[i].Title);
                comboPage2.Items.Add(doc.Data.Pages[i].Title);
                comboPage3.Items.Add(doc.Data.Pages[i].Title);
            }

            for (int pageNo = 0; pageNo < m; pageNo++)
            {
                if (doc.Data.Pages[pageNo].NextPage.Equals(doc.Book.HomePage))
                {
                    SetPage(groupIndex, pageNo, doc.Data.Pages[pageNo]);
                    groupIndex++;
                }
            }
        }


        public void SetPage(int groupIndex, int pageNo, MNPage page)
        {
            switch (groupIndex)
            {
                case 0:
                    groupBox1.Visible = true;
                    comboPage1.SelectedIndex = pageNo;
                    comboType1.SelectedIndex = 1;
                    pnum[groupIndex] = pageNo;
                    break;
                case 1:
                    groupBox2.Visible = true;
                    comboPage2.SelectedIndex = pageNo;
                    comboType2.SelectedIndex = 2;
                    pnum[groupIndex] = pageNo;
                    break;
                case 2:
                    groupBox3.Visible = true;
                    comboPage3.SelectedIndex = pageNo;
                    comboType3.SelectedIndex = 3;
                    pnum[groupIndex] = pageNo;
                    break;
                default:
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void GetPageToInsert(int index, out int afterPage, out int pageType)
        {
            switch (index)
            {
                case 0:
                    afterPage = comboPage1.SelectedIndex;
                    pageType = comboType1.SelectedIndex;
                    break;
                case 1:
                    afterPage = comboPage2.SelectedIndex;
                    pageType = comboType2.SelectedIndex;
                    break;
                case 2:
                    afterPage = comboPage3.SelectedIndex;
                    pageType = comboType3.SelectedIndex;
                    break;
                default:
                    afterPage = -1;
                    pageType = -1;
                    break;
            }
        }
    }
}
