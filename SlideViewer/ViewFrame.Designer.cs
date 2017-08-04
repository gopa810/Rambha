namespace SlideViewer
{
    partial class ViewFrame
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
            this.pageView1 = new SlideViewer.Views.PageView();
            this.panelBook = new System.Windows.Forms.Panel();
            this.panelSelectLanguage = new SlideViewer.SelectLanguageView();
            this.panelFiles = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panelUpdater = new SlideViewer.UpdaterView();
            this.button3 = new System.Windows.Forms.Button();
            this.panelBook.SuspendLayout();
            this.panelFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageView1
            // 
            this.pageView1.BackColor = System.Drawing.SystemColors.Window;
            this.pageView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pageView1.CurrentDocument = null;
            this.pageView1.CurrentPage = null;
            this.pageView1.DisplayedMenu = null;
            this.pageView1.Location = new System.Drawing.Point(0, 3);
            this.pageView1.Name = "pageView1";
            this.pageView1.Size = new System.Drawing.Size(303, 290);
            this.pageView1.TabIndex = 0;
            // 
            // panelBook
            // 
            this.panelBook.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBook.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBook.Controls.Add(this.pageView1);
            this.panelBook.Location = new System.Drawing.Point(0, 1);
            this.panelBook.Name = "panelBook";
            this.panelBook.Size = new System.Drawing.Size(229, 222);
            this.panelBook.TabIndex = 2;
            this.panelBook.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            this.panelBook.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // panelSelectLanguage
            // 
            this.panelSelectLanguage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSelectLanguage.Location = new System.Drawing.Point(18, 349);
            this.panelSelectLanguage.Name = "panelSelectLanguage";
            this.panelSelectLanguage.Size = new System.Drawing.Size(285, 227);
            this.panelSelectLanguage.TabIndex = 4;
            // 
            // panelFiles
            // 
            this.panelFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFiles.Controls.Add(this.button3);
            this.panelFiles.Controls.Add(this.button2);
            this.panelFiles.Controls.Add(this.button1);
            this.panelFiles.Controls.Add(this.buttonBrowse);
            this.panelFiles.Controls.Add(this.buttonPlay);
            this.panelFiles.Controls.Add(this.listBox1);
            this.panelFiles.Location = new System.Drawing.Point(237, 238);
            this.panelFiles.Name = "panelFiles";
            this.panelFiles.Size = new System.Drawing.Size(355, 261);
            this.panelFiles.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(285, 130);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(65, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Export";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(285, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(65, 27);
            this.button1.TabIndex = 3;
            this.button1.Text = "Update";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(285, 159);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(65, 45);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "Books Folder";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlay.Location = new System.Drawing.Point(285, 4);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(65, 46);
            this.buttonPlay.TabIndex = 1;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(276, 252);
            this.listBox1.TabIndex = 0;
            this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            this.listBox1.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox1_MeasureItem);
            // 
            // panelUpdater
            // 
            this.panelUpdater.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUpdater.Location = new System.Drawing.Point(380, 12);
            this.panelUpdater.Name = "panelUpdater";
            this.panelUpdater.Size = new System.Drawing.Size(168, 169);
            this.panelUpdater.TabIndex = 4;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(285, 210);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(65, 45);
            this.button3.TabIndex = 5;
            this.button3.Text = "Reviews Folder";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ViewFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 511);
            this.Controls.Add(this.panelFiles);
            this.Controls.Add(this.panelSelectLanguage);
            this.Controls.Add(this.panelBook);
            this.Controls.Add(this.panelUpdater);
            this.Name = "ViewFrame";
            this.Text = "Page Views";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewFrame_FormClosing);
            this.Load += new System.EventHandler(this.ViewFrame_Load);
            this.Shown += new System.EventHandler(this.ViewFrame_Shown);
            this.panelBook.ResumeLayout(false);
            this.panelFiles.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Views.PageView pageView1;
        private System.Windows.Forms.Panel panelBook;
        private System.Windows.Forms.Panel panelFiles;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonBrowse;
        private SelectLanguageView panelSelectLanguage;
        private UpdaterView panelUpdater;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}