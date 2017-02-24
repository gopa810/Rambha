namespace SlideMaker.Views
{
    partial class PageScrollArea
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageScrollArea));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.alignHorizontalyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignVerticalyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.alignHeightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignWidthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.useAverageValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useMinimumValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useMaximumValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageEditView1 = new SlideMaker.Views.PageEditView();
            this.useFirstValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.toolStripComboBox1,
            this.toolStripSeparator2,
            this.toolStripSplitButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(668, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(54, 22);
            this.toolStripButton1.Text = "< Graph";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(71, 22);
            this.toolStripLabel1.Text = "Display Size:";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "Landscape 10\"  4:3",
            "Landspace 7\" 4:3",
            "Portait 10\" 3:4",
            "Portait 7\" 3:4"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 25);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.pageEditView1);
            this.panel1.Location = new System.Drawing.Point(3, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(662, 428);
            this.panel1.TabIndex = 4;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alignHorizontalyToolStripMenuItem,
            this.alignVerticalyToolStripMenuItem,
            this.toolStripMenuItem1,
            this.alignHeightToolStripMenuItem,
            this.alignWidthToolStripMenuItem,
            this.toolStripMenuItem2,
            this.useAverageValueToolStripMenuItem,
            this.useMinimumValueToolStripMenuItem,
            this.useMaximumValueToolStripMenuItem,
            this.useFirstValueToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(51, 22);
            this.toolStripSplitButton1.Text = "Align";
            this.toolStripSplitButton1.DropDownOpening += new System.EventHandler(this.toolStripSplitButton1_DropDownOpening);
            // 
            // alignHorizontalyToolStripMenuItem
            // 
            this.alignHorizontalyToolStripMenuItem.Name = "alignHorizontalyToolStripMenuItem";
            this.alignHorizontalyToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.alignHorizontalyToolStripMenuItem.Text = "Align Horizontaly";
            this.alignHorizontalyToolStripMenuItem.Click += new System.EventHandler(this.alignHorizontalyToolStripMenuItem_Click);
            // 
            // alignVerticalyToolStripMenuItem
            // 
            this.alignVerticalyToolStripMenuItem.Name = "alignVerticalyToolStripMenuItem";
            this.alignVerticalyToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.alignVerticalyToolStripMenuItem.Text = "Align Verticaly";
            this.alignVerticalyToolStripMenuItem.Click += new System.EventHandler(this.alignVerticalyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(178, 6);
            // 
            // alignHeightToolStripMenuItem
            // 
            this.alignHeightToolStripMenuItem.Name = "alignHeightToolStripMenuItem";
            this.alignHeightToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.alignHeightToolStripMenuItem.Text = "Align Height";
            this.alignHeightToolStripMenuItem.Click += new System.EventHandler(this.alignHeightToolStripMenuItem_Click);
            // 
            // alignWidthToolStripMenuItem
            // 
            this.alignWidthToolStripMenuItem.Name = "alignWidthToolStripMenuItem";
            this.alignWidthToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.alignWidthToolStripMenuItem.Text = "Align Width";
            this.alignWidthToolStripMenuItem.Click += new System.EventHandler(this.alignWidthToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(178, 6);
            // 
            // useAverageValueToolStripMenuItem
            // 
            this.useAverageValueToolStripMenuItem.Name = "useAverageValueToolStripMenuItem";
            this.useAverageValueToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.useAverageValueToolStripMenuItem.Text = "Use Average Value";
            this.useAverageValueToolStripMenuItem.Click += new System.EventHandler(this.useAverageValueToolStripMenuItem_Click);
            // 
            // useMinimumValueToolStripMenuItem
            // 
            this.useMinimumValueToolStripMenuItem.Name = "useMinimumValueToolStripMenuItem";
            this.useMinimumValueToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.useMinimumValueToolStripMenuItem.Text = "Use Minimum Value";
            this.useMinimumValueToolStripMenuItem.Click += new System.EventHandler(this.useMinimumValueToolStripMenuItem_Click);
            // 
            // useMaximumValueToolStripMenuItem
            // 
            this.useMaximumValueToolStripMenuItem.Name = "useMaximumValueToolStripMenuItem";
            this.useMaximumValueToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.useMaximumValueToolStripMenuItem.Text = "Use Maximum Value";
            this.useMaximumValueToolStripMenuItem.Click += new System.EventHandler(this.useMaximumValueToolStripMenuItem_Click);
            // 
            // pageEditView1
            // 
            this.pageEditView1.BackColor = System.Drawing.SystemColors.Window;
            this.pageEditView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pageEditView1.DisplaySize = Rambha.Document.PageEditDisplaySize.LandscapeBig;
            this.pageEditView1.LastRelativePoint = new System.Drawing.Point(0, 0);
            this.pageEditView1.LastUserPoint = new System.Drawing.Point(0, 0);
            this.pageEditView1.Location = new System.Drawing.Point(89, 98);
            this.pageEditView1.Name = "pageEditView1";
            this.pageEditView1.Page = null;
            this.pageEditView1.Size = new System.Drawing.Size(217, 124);
            this.pageEditView1.TabIndex = 0;
            this.pageEditView1.ViewSize = new System.Drawing.Size(1024, 768);
            this.pageEditView1.ZoomRatio = 1F;
            this.pageEditView1.NewPageRequested += new SlideMaker.Views.PageChangedEventHandler(this.pageEditView1_NewPageRequested);
            // 
            // useFirstValueToolStripMenuItem
            // 
            this.useFirstValueToolStripMenuItem.Name = "useFirstValueToolStripMenuItem";
            this.useFirstValueToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.useFirstValueToolStripMenuItem.Text = "Use First Value";
            this.useFirstValueToolStripMenuItem.Click += new System.EventHandler(this.useFirstValueToolStripMenuItem_Click);
            // 
            // PageScrollArea
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "PageScrollArea";
            this.Size = new System.Drawing.Size(668, 459);
            this.SizeChanged += new System.EventHandler(this.PageScrollArea_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.PageScrollArea_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.PageScrollArea_DragEnter);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PageScrollArea_MouseClick);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PageEditView pageEditView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem alignHorizontalyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alignVerticalyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem alignHeightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alignWidthToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem useAverageValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useMinimumValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useMaximumValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useFirstValueToolStripMenuItem;
    }
}
