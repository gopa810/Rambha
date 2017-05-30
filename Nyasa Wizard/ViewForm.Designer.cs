namespace SlideMaker
{
    partial class ViewForm
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
            this.pageView1 = new SlideMaker.Views.PageView();
            this.SuspendLayout();
            // 
            // pageView1
            // 
            this.pageView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pageView1.CurrentDocument = null;
            this.pageView1.CurrentPage = null;
            this.pageView1.DisplayedMenu = null;
            this.pageView1.Location = new System.Drawing.Point(0, 0);
            this.pageView1.Name = "pageView1";
            this.pageView1.Size = new System.Drawing.Size(588, 522);
            this.pageView1.TabIndex = 0;
            this.pageView1.SizeChanged += new System.EventHandler(this.pageView1_SizeChanged);
            // 
            // ViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 522);
            this.Controls.Add(this.pageView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ViewForm_FormClosing);
            this.Load += new System.EventHandler(this.ViewForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Views.PageView pageView1;
    }
}