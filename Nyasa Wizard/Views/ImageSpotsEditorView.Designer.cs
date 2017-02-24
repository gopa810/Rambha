namespace SlideMaker.Views
{
    partial class ImageSpotsEditorView
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewCircleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.addNewCircleToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(179, 48);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuItem1.Text = "Add New Rectangle";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.addNewRectToolStripMenuItem_Click);
            // 
            // addNewCircleToolStripMenuItem
            // 
            this.addNewCircleToolStripMenuItem.Name = "addNewCircleToolStripMenuItem";
            this.addNewCircleToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.addNewCircleToolStripMenuItem.Text = "Add New Circle";
            this.addNewCircleToolStripMenuItem.Click += new System.EventHandler(this.addNewCircleToolStripMenuItem_Click);
            // 
            // ImageSpotsEditorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "ImageSpotsEditorView";
            this.Size = new System.Drawing.Size(460, 435);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageSpotsEditorView_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ImageSpotsEditorView_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageSpotsEditorView_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageSpotsEditorView_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageSpotsEditorView_MouseUp);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addNewCircleToolStripMenuItem;
    }
}
