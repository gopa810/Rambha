namespace SlideMaker.Views
{
    partial class PageDetailPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageDetailPanel));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControlDetails = new System.Windows.Forms.TabControl();
            this.tabPageProperties = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageProject = new System.Windows.Forms.TabPage();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.addPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBoxPages = new System.Windows.Forms.ListBox();
            this.tabPageToolbox = new System.Windows.Forms.TabPage();
            this.listToolbox = new System.Windows.Forms.ListBox();
            this.tabPageImages = new System.Windows.Forms.TabPage();
            this.listBoxImages = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonAddImage = new System.Windows.Forms.ToolStripButton();
            this.buttonDeleteImage = new System.Windows.Forms.ToolStripButton();
            this.tabPageStyles = new System.Windows.Forms.TabPage();
            this.propertyGridStyle = new System.Windows.Forms.PropertyGrid();
            this.listBoxStyles = new System.Windows.Forms.ListBox();
            this.toolStrip5 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pageScrollArea1 = new SlideMaker.Views.PageScrollArea();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControlDetails.SuspendLayout();
            this.tabPageProperties.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageProject.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.tabPageToolbox.SuspendLayout();
            this.tabPageImages.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabPageStyles.SuspendLayout();
            this.toolStrip5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pageScrollArea1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControlDetails);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Size = new System.Drawing.Size(626, 330);
            this.splitContainer2.SplitterDistance = 450;
            this.splitContainer2.TabIndex = 3;
            // 
            // tabControlDetails
            // 
            this.tabControlDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlDetails.Controls.Add(this.tabPageProperties);
            this.tabControlDetails.Location = new System.Drawing.Point(3, 22);
            this.tabControlDetails.Name = "tabControlDetails";
            this.tabControlDetails.SelectedIndex = 0;
            this.tabControlDetails.Size = new System.Drawing.Size(169, 308);
            this.tabControlDetails.TabIndex = 4;
            // 
            // tabPageProperties
            // 
            this.tabPageProperties.Controls.Add(this.propertyGrid1);
            this.tabPageProperties.Location = new System.Drawing.Point(4, 22);
            this.tabPageProperties.Name = "tabPageProperties";
            this.tabPageProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProperties.Size = new System.Drawing.Size(161, 282);
            this.tabPageProperties.TabIndex = 0;
            this.tabPageProperties.Text = "Properties";
            this.tabPageProperties.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(155, 276);
            this.propertyGrid1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(8, 2, 8, 2);
            this.label2.Size = new System.Drawing.Size(166, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "Details";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageProject);
            this.tabControl1.Controls.Add(this.tabPageToolbox);
            this.tabControl1.Controls.Add(this.tabPageImages);
            this.tabControl1.Controls.Add(this.tabPageStyles);
            this.tabControl1.Location = new System.Drawing.Point(3, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(189, 308);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageProject
            // 
            this.tabPageProject.Controls.Add(this.toolStrip3);
            this.tabPageProject.Controls.Add(this.listBoxPages);
            this.tabPageProject.Location = new System.Drawing.Point(4, 22);
            this.tabPageProject.Name = "tabPageProject";
            this.tabPageProject.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProject.Size = new System.Drawing.Size(181, 282);
            this.tabPageProject.TabIndex = 0;
            this.tabPageProject.Text = "Pages";
            this.tabPageProject.UseVisualStyleBackColor = true;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.toolStrip3.Location = new System.Drawing.Point(3, 3);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(175, 25);
            this.toolStrip3.TabIndex = 2;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPageToolStripMenuItem,
            this.addTemplateToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(51, 22);
            this.toolStripDropDownButton1.Text = "Add...";
            // 
            // addPageToolStripMenuItem
            // 
            this.addPageToolStripMenuItem.Name = "addPageToolStripMenuItem";
            this.addPageToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.addPageToolStripMenuItem.Text = "Add Page";
            this.addPageToolStripMenuItem.Click += new System.EventHandler(this.buttonAddPage_Click);
            // 
            // addTemplateToolStripMenuItem
            // 
            this.addTemplateToolStripMenuItem.Name = "addTemplateToolStripMenuItem";
            this.addTemplateToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.addTemplateToolStripMenuItem.Text = "Add Template";
            this.addTemplateToolStripMenuItem.Click += new System.EventHandler(this.addTemplateToolStripMenuItem_Click);
            // 
            // listBoxPages
            // 
            this.listBoxPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxPages.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxPages.FormattingEnabled = true;
            this.listBoxPages.IntegralHeight = false;
            this.listBoxPages.Location = new System.Drawing.Point(3, 31);
            this.listBoxPages.Name = "listBoxPages";
            this.listBoxPages.Size = new System.Drawing.Size(172, 243);
            this.listBoxPages.TabIndex = 1;
            this.listBoxPages.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxPages_DrawItem);
            this.listBoxPages.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxPages_MeasureItem);
            this.listBoxPages.SelectedIndexChanged += new System.EventHandler(this.listBoxPages_SelectedIndexChanged);
            this.listBoxPages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxPages_MouseDown);
            // 
            // tabPageToolbox
            // 
            this.tabPageToolbox.Controls.Add(this.listToolbox);
            this.tabPageToolbox.Location = new System.Drawing.Point(4, 22);
            this.tabPageToolbox.Name = "tabPageToolbox";
            this.tabPageToolbox.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageToolbox.Size = new System.Drawing.Size(181, 282);
            this.tabPageToolbox.TabIndex = 1;
            this.tabPageToolbox.Text = "Toolbox";
            this.tabPageToolbox.UseVisualStyleBackColor = true;
            // 
            // listToolbox
            // 
            this.listToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listToolbox.FormattingEnabled = true;
            this.listToolbox.Location = new System.Drawing.Point(3, 3);
            this.listToolbox.Name = "listToolbox";
            this.listToolbox.Size = new System.Drawing.Size(175, 276);
            this.listToolbox.TabIndex = 0;
            this.listToolbox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listToolbox_MouseDown);
            // 
            // tabPageImages
            // 
            this.tabPageImages.Controls.Add(this.listBoxImages);
            this.tabPageImages.Controls.Add(this.toolStrip2);
            this.tabPageImages.Location = new System.Drawing.Point(4, 22);
            this.tabPageImages.Name = "tabPageImages";
            this.tabPageImages.Size = new System.Drawing.Size(181, 282);
            this.tabPageImages.TabIndex = 2;
            this.tabPageImages.Text = "Images";
            this.tabPageImages.UseVisualStyleBackColor = true;
            // 
            // listBoxImages
            // 
            this.listBoxImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxImages.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxImages.FormattingEnabled = true;
            this.listBoxImages.IntegralHeight = false;
            this.listBoxImages.Location = new System.Drawing.Point(6, 28);
            this.listBoxImages.Name = "listBoxImages";
            this.listBoxImages.Size = new System.Drawing.Size(169, 234);
            this.listBoxImages.TabIndex = 3;
            this.listBoxImages.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxImages_DrawItem);
            this.listBoxImages.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxImages_MeasureItem);
            this.listBoxImages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxImages_MouseDown);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddImage,
            this.buttonDeleteImage});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(181, 25);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // buttonAddImage
            // 
            this.buttonAddImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonAddImage.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddImage.Image")));
            this.buttonAddImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddImage.Name = "buttonAddImage";
            this.buttonAddImage.Size = new System.Drawing.Size(33, 22);
            this.buttonAddImage.Text = "Add";
            this.buttonAddImage.Click += new System.EventHandler(this.buttonAddImage_Click);
            // 
            // buttonDeleteImage
            // 
            this.buttonDeleteImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonDeleteImage.Image = ((System.Drawing.Image)(resources.GetObject("buttonDeleteImage.Image")));
            this.buttonDeleteImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDeleteImage.Name = "buttonDeleteImage";
            this.buttonDeleteImage.Size = new System.Drawing.Size(44, 22);
            this.buttonDeleteImage.Text = "Delete";
            this.buttonDeleteImage.Click += new System.EventHandler(this.buttonDeleteImage_Click);
            // 
            // tabPageStyles
            // 
            this.tabPageStyles.Controls.Add(this.propertyGridStyle);
            this.tabPageStyles.Controls.Add(this.listBoxStyles);
            this.tabPageStyles.Controls.Add(this.toolStrip5);
            this.tabPageStyles.Location = new System.Drawing.Point(4, 22);
            this.tabPageStyles.Name = "tabPageStyles";
            this.tabPageStyles.Size = new System.Drawing.Size(181, 282);
            this.tabPageStyles.TabIndex = 3;
            this.tabPageStyles.Text = "Styles";
            this.tabPageStyles.UseVisualStyleBackColor = true;
            // 
            // propertyGridStyle
            // 
            this.propertyGridStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridStyle.HelpVisible = false;
            this.propertyGridStyle.Location = new System.Drawing.Point(5, 220);
            this.propertyGridStyle.Name = "propertyGridStyle";
            this.propertyGridStyle.Size = new System.Drawing.Size(173, 54);
            this.propertyGridStyle.TabIndex = 2;
            // 
            // listBoxStyles
            // 
            this.listBoxStyles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxStyles.FormattingEnabled = true;
            this.listBoxStyles.Location = new System.Drawing.Point(5, 28);
            this.listBoxStyles.Name = "listBoxStyles";
            this.listBoxStyles.Size = new System.Drawing.Size(173, 186);
            this.listBoxStyles.TabIndex = 1;
            this.listBoxStyles.SelectedIndexChanged += new System.EventHandler(this.listBoxStyles_SelectedIndexChanged);
            // 
            // toolStrip5
            // 
            this.toolStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton3});
            this.toolStrip5.Location = new System.Drawing.Point(0, 0);
            this.toolStrip5.Name = "toolStrip5";
            this.toolStrip5.Size = new System.Drawing.Size(181, 25);
            this.toolStrip5.TabIndex = 0;
            this.toolStrip5.Text = "toolStrip5";
            this.toolStrip5.Click += new System.EventHandler(this.onButtonClick_StyleAdd);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(33, 22);
            this.toolStripButton3.Text = "Add";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(8, 2, 8, 2);
            this.label1.Size = new System.Drawing.Size(192, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Navigation";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(825, 330);
            this.splitContainer1.SplitterDistance = 195;
            this.splitContainer1.TabIndex = 2;
            // 
            // pageScrollArea1
            // 
            this.pageScrollArea1.AllowDrop = true;
            this.pageScrollArea1.AutoScroll = true;
            this.pageScrollArea1.DisplaySize = Rambha.Document.PageEditDisplaySize.LandscapeBig;
            this.pageScrollArea1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageScrollArea1.Location = new System.Drawing.Point(0, 0);
            this.pageScrollArea1.Name = "pageScrollArea1";
            this.pageScrollArea1.Size = new System.Drawing.Size(450, 330);
            this.pageScrollArea1.TabIndex = 1;
            this.pageScrollArea1.NewPageRequested += new SlideMaker.Views.PageChangedEventHandler(this.pageScrollArea1_NewPageRequested);
            this.pageScrollArea1.BackToParentView += new SlideMaker.Views.NormalEventHandler(this.pageScrollArea1_BackToParentView);
            // 
            // PageDetailPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PageDetailPanel";
            this.Size = new System.Drawing.Size(825, 330);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControlDetails.ResumeLayout(false);
            this.tabPageProperties.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageProject.ResumeLayout(false);
            this.tabPageProject.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.tabPageToolbox.ResumeLayout(false);
            this.tabPageImages.ResumeLayout(false);
            this.tabPageImages.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPageStyles.ResumeLayout(false);
            this.tabPageStyles.PerformLayout();
            this.toolStrip5.ResumeLayout(false);
            this.toolStrip5.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private PageScrollArea pageScrollArea1;
        private System.Windows.Forms.TabControl tabControlDetails;
        private System.Windows.Forms.TabPage tabPageProperties;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageProject;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem addPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTemplateToolStripMenuItem;
        private System.Windows.Forms.ListBox listBoxPages;
        private System.Windows.Forms.TabPage tabPageToolbox;
        private System.Windows.Forms.ListBox listToolbox;
        private System.Windows.Forms.TabPage tabPageImages;
        private System.Windows.Forms.ListBox listBoxImages;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton buttonAddImage;
        private System.Windows.Forms.ToolStripButton buttonDeleteImage;
        private System.Windows.Forms.TabPage tabPageStyles;
        private System.Windows.Forms.PropertyGrid propertyGridStyle;
        private System.Windows.Forms.ListBox listBoxStyles;
        private System.Windows.Forms.ToolStrip toolStrip5;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
