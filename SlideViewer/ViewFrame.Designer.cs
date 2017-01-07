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
            Rambha.Document.MNPageContext mnPageContext1 = new Rambha.Document.MNPageContext();
            System.Drawing.Drawing2D.Matrix matrix1 = new System.Drawing.Drawing2D.Matrix();
            System.Drawing.Drawing2D.Matrix matrix2 = new System.Drawing.Drawing2D.Matrix();
            Rambha.Script.GSExecutor gsExecutor1 = new Rambha.Script.GSExecutor();
            SlideViewer.Views.PageViewController pageViewController1 = new SlideViewer.Views.PageViewController();
            this.panelBook = new System.Windows.Forms.Panel();
            this.panelFiles = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.pageView1 = new SlideViewer.Views.PageView();
            this.panelBook.SuspendLayout();
            this.panelFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBook
            // 
            this.panelBook.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBook.Controls.Add(this.pageView1);
            this.panelBook.Location = new System.Drawing.Point(0, 1);
            this.panelBook.Name = "panelBook";
            this.panelBook.Size = new System.Drawing.Size(377, 332);
            this.panelBook.TabIndex = 2;
            this.panelBook.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            this.panelBook.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // panelFiles
            // 
            this.panelFiles.Controls.Add(this.buttonBrowse);
            this.panelFiles.Controls.Add(this.buttonPlay);
            this.panelFiles.Controls.Add(this.listBox1);
            this.panelFiles.Location = new System.Drawing.Point(225, 244);
            this.panelFiles.Name = "panelFiles";
            this.panelFiles.Size = new System.Drawing.Size(355, 261);
            this.panelFiles.TabIndex = 3;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(278, 254);
            this.listBox1.TabIndex = 0;
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlay.Location = new System.Drawing.Point(287, 4);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(65, 46);
            this.buttonPlay.TabIndex = 1;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(287, 214);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(65, 41);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // pageView1
            // 
            this.pageView1.BackColor = System.Drawing.SystemColors.Window;
            this.pageView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mnPageContext1.CurrentPage = null;
            mnPageContext1.DisplaySize = Rambha.Document.PageEditDisplaySize.LandscapeBig;
            mnPageContext1.LastInvertMatrix = matrix1;
            mnPageContext1.LastMatrix = matrix2;
            this.pageView1.Context = mnPageContext1;
            this.pageView1.CurrentPage = null;
            this.pageView1.CurrentDocument = null;
            this.pageView1.DocExec = null;
            this.pageView1.Location = new System.Drawing.Point(0, 3);
            this.pageView1.Name = "pageView1";
            this.pageView1.Size = new System.Drawing.Size(303, 290);
            this.pageView1.TabIndex = 0;
            pageViewController1.View = this.pageView1;
            this.pageView1.ViewController = pageViewController1;
            // 
            // ViewFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 511);
            this.Controls.Add(this.panelFiles);
            this.Controls.Add(this.panelBook);
            this.Name = "ViewFrame";
            this.Text = "Page Views";
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
    }
}