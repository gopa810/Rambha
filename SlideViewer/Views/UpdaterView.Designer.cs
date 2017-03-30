namespace SlideViewer
{
    partial class UpdaterView
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
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.updaterDownloader1 = new SlideViewer.UpdaterDownloader();
            this.updaterSelectFiles1 = new SlideViewer.UpdaterSelectFiles();
            this.updaterGetStatus1 = new SlideViewer.UpdaterGetStatus();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Silver;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(460, 47);
            this.label1.TabIndex = 0;
            this.label1.Text = "Update Manager";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.updaterDownloader1);
            this.panel1.Controls.Add(this.updaterSelectFiles1);
            this.panel1.Controls.Add(this.updaterGetStatus1);
            this.panel1.Location = new System.Drawing.Point(20, 90);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(460, 373);
            this.panel1.TabIndex = 1;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // updaterDownloader1
            // 
            this.updaterDownloader1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.updaterDownloader1.Location = new System.Drawing.Point(5, 205);
            this.updaterDownloader1.Name = "updaterDownloader1";
            this.updaterDownloader1.Size = new System.Drawing.Size(196, 151);
            this.updaterDownloader1.TabIndex = 2;
            this.updaterDownloader1.OnDownloadComplete += new SlideViewer.GeneralArgsEvent(this.updaterDownloader1_OnDownloadComplete);
            // 
            // updaterSelectFiles1
            // 
            this.updaterSelectFiles1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.updaterSelectFiles1.Location = new System.Drawing.Point(370, 228);
            this.updaterSelectFiles1.Name = "updaterSelectFiles1";
            this.updaterSelectFiles1.Size = new System.Drawing.Size(87, 142);
            this.updaterSelectFiles1.TabIndex = 1;
            this.updaterSelectFiles1.OnApplyChanges += new SlideViewer.GeneralArgsEvent(this.updaterSelectFiles1_OnApplyChanges);
            this.updaterSelectFiles1.OnDiscardChanges += new SlideViewer.GeneralArgsEvent(this.updaterSelectFiles1_OnDiscardChanges);
            // 
            // updaterGetStatus1
            // 
            this.updaterGetStatus1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.updaterGetStatus1.Location = new System.Drawing.Point(5, 3);
            this.updaterGetStatus1.Name = "updaterGetStatus1";
            this.updaterGetStatus1.Size = new System.Drawing.Size(231, 196);
            this.updaterGetStatus1.TabIndex = 0;
            this.updaterGetStatus1.OnShowManagerButtonPressed += new SlideViewer.GeneralArgsEvent(this.updaterGetStatus1_OnShowManagerButtonPressed);
            this.updaterGetStatus1.OnRetryFetchRemote += new SlideViewer.GeneralArgsEvent(this.updaterGetStatus1_OnRetryFetchRemote);
            // 
            // UpdaterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Name = "UpdaterView";
            this.Size = new System.Drawing.Size(498, 490);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private UpdaterSelectFiles updaterSelectFiles1;
        private UpdaterGetStatus updaterGetStatus1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private UpdaterDownloader updaterDownloader1;
    }
}
