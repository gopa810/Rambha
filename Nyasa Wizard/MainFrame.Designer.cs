﻿namespace SlideMaker
{
    partial class MainFrame
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrame));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.makeGroupListLabels = new System.Windows.Forms.ToolStripMenuItem();
            this.makeGroupSortPic = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playSlideshowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testAction1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDefaultLanguageDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageFlowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageDynamicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageDetailPanel1 = new SlideMaker.Views.PageDetailPanel();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem4,
            this.windowToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1109, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripMenuItem2,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.printToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.printToolStripMenuItem.Text = "Print";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.printToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripSeparator2,
            this.makeGroupListLabels,
            this.makeGroupSortPic});
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(39, 20);
            this.toolStripMenuItem4.Text = "Edit";
            this.toolStripMenuItem4.DropDownOpening += new System.EventHandler(this.toolStripMenuItem4_DropDownOpening);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(228, 22);
            this.toolStripMenuItem5.Text = "Image List";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(225, 6);
            // 
            // makeGroupListLabels
            // 
            this.makeGroupListLabels.Name = "makeGroupListLabels";
            this.makeGroupListLabels.Size = new System.Drawing.Size(228, 22);
            this.makeGroupListLabels.Text = "Make Group: List of Labels";
            this.makeGroupListLabels.Click += new System.EventHandler(this.makeGroupListLabels_Click);
            // 
            // makeGroupSortPic
            // 
            this.makeGroupSortPic.Name = "makeGroupSortPic";
            this.makeGroupSortPic.Size = new System.Drawing.Size(228, 22);
            this.makeGroupSortPic.Text = "Make Group: Sorting Pictures";
            this.makeGroupSortPic.Click += new System.EventHandler(this.makeGroupSortPic_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playSlideshowToolStripMenuItem,
            this.testAction1ToolStripMenuItem,
            this.toolStripSeparator3,
            this.showObjectsToolStripMenuItem,
            this.showDefaultLanguageDataToolStripMenuItem,
            this.toolStripMenuItem6});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // playSlideshowToolStripMenuItem
            // 
            this.playSlideshowToolStripMenuItem.Name = "playSlideshowToolStripMenuItem";
            this.playSlideshowToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.playSlideshowToolStripMenuItem.Text = "Play Slideshow";
            this.playSlideshowToolStripMenuItem.Click += new System.EventHandler(this.playSlideshowToolStripMenuItem_Click);
            // 
            // testAction1ToolStripMenuItem
            // 
            this.testAction1ToolStripMenuItem.Name = "testAction1ToolStripMenuItem";
            this.testAction1ToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.testAction1ToolStripMenuItem.Text = "Test Action 1";
            this.testAction1ToolStripMenuItem.Click += new System.EventHandler(this.testAction1ToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(246, 6);
            // 
            // showObjectsToolStripMenuItem
            // 
            this.showObjectsToolStripMenuItem.Name = "showObjectsToolStripMenuItem";
            this.showObjectsToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.showObjectsToolStripMenuItem.Text = "Show Objects";
            this.showObjectsToolStripMenuItem.Click += new System.EventHandler(this.showObjectsToolStripMenuItem_Click);
            // 
            // showDefaultLanguageDataToolStripMenuItem
            // 
            this.showDefaultLanguageDataToolStripMenuItem.Name = "showDefaultLanguageDataToolStripMenuItem";
            this.showDefaultLanguageDataToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.showDefaultLanguageDataToolStripMenuItem.Text = "Show Default Language Data";
            this.showDefaultLanguageDataToolStripMenuItem.Click += new System.EventHandler(this.showDefaultLanguageDataToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pageFlowToolStripMenuItem,
            this.pageDynamicsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.DropDownOpening += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpening);
            // 
            // pageFlowToolStripMenuItem
            // 
            this.pageFlowToolStripMenuItem.Name = "pageFlowToolStripMenuItem";
            this.pageFlowToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.pageFlowToolStripMenuItem.Text = "Page Flow";
            // 
            // pageDynamicsToolStripMenuItem
            // 
            this.pageDynamicsToolStripMenuItem.Name = "pageDynamicsToolStripMenuItem";
            this.pageDynamicsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.pageDynamicsToolStripMenuItem.Text = "Page Dynamics";
            this.pageDynamicsToolStripMenuItem.Click += new System.EventHandler(this.pageDynamicsToolStripMenuItem_Click);
            // 
            // pageDetailPanel1
            // 
            this.pageDetailPanel1.Location = new System.Drawing.Point(51, 47);
            this.pageDetailPanel1.Name = "pageDetailPanel1";
            this.pageDetailPanel1.Size = new System.Drawing.Size(325, 257);
            this.pageDetailPanel1.TabIndex = 3;
            this.pageDetailPanel1.BackToParentView += new SlideMaker.Views.NormalEventHandler(this.pageScrollArea1_BackToParentView);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(249, 22);
            this.toolStripMenuItem6.Text = "Create Language File from Folder";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
            // 
            // MainFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1109, 485);
            this.Controls.Add(this.pageDetailPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainFrame";
            this.Text = "Slide Maker";
            this.Activated += new System.EventHandler(this.MainFrame_Activated);
            this.Deactivate += new System.EventHandler(this.MainFrame_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrame_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem makeGroupListLabels;
        private System.Windows.Forms.ToolStripMenuItem makeGroupSortPic;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playSlideshowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testAction1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem showObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pageFlowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pageDynamicsToolStripMenuItem;
        private Views.PageDetailPanel pageDetailPanel1;
        private System.Windows.Forms.ToolStripMenuItem showDefaultLanguageDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
    }
}

