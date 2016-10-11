namespace SlideMaker.DocumentViews
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
            this.pageEditView1 = new SlideMaker.PageEditView();
            this.SuspendLayout();
            // 
            // pageEditView1
            // 
            this.pageEditView1.BackColor = System.Drawing.SystemColors.Window;
            this.pageEditView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pageEditView1.Document = null;
            this.pageEditView1.LastRelativePoint = new System.Drawing.Point(0, 0);
            this.pageEditView1.LastUserPoint = new System.Drawing.Point(0, 0);
            this.pageEditView1.Location = new System.Drawing.Point(3, 3);
            this.pageEditView1.Name = "pageEditView1";
            this.pageEditView1.Page = null;
            this.pageEditView1.Size = new System.Drawing.Size(594, 300);
            this.pageEditView1.TabIndex = 0;
            this.pageEditView1.ViewSize = new System.Drawing.Size(1024, 768);
            this.pageEditView1.ZoomRatio = 1F;
            this.pageEditView1.PageObjectSelected += new SlideMaker.PageChangedEventHandler(this.pageEditView1_PageObjectSelected);
            this.pageEditView1.NewPageRequested += new SlideMaker.PageChangedEventHandler(this.pageEditView1_NewPageRequested);
            // 
            // PageScrollArea
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pageEditView1);
            this.Name = "PageScrollArea";
            this.Size = new System.Drawing.Size(668, 459);
            this.SizeChanged += new System.EventHandler(this.PageScrollArea_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.PageScrollArea_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.PageScrollArea_DragEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private PageEditView pageEditView1;
    }
}
