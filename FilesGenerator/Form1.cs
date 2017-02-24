using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FilesGenerator
{
    public partial class Form1 : Form
    {
        public string WorkDir = @"E:\Dropbox\Books for Software";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CmdFileGenerator.Generate(WorkDir);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LangFileChecker.Check(WorkDir);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AllFileGenerator.Generate(WorkDir);
        }
    }
}
