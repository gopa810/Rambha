namespace SlideMaker.Views
{
    partial class EditSpotsEditorView
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
            this.addNewTriangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.addRectangleTaggedAreaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCircleTaggedAreaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTriangleTaggedAreaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.addNewCircleToolStripMenuItem,
            this.addNewTriangleToolStripMenuItem,
            this.toolStripMenuItem2,
            this.addRectangleTaggedAreaToolStripMenuItem,
            this.addCircleTaggedAreaToolStripMenuItem,
            this.addTriangleTaggedAreaToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(221, 164);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(220, 22);
            this.toolStripMenuItem1.Text = "Add New Rectangle";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.addNewRectToolStripMenuItem_Click);
            // 
            // addNewCircleToolStripMenuItem
            // 
            this.addNewCircleToolStripMenuItem.Name = "addNewCircleToolStripMenuItem";
            this.addNewCircleToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.addNewCircleToolStripMenuItem.Text = "Add New Circle";
            this.addNewCircleToolStripMenuItem.Click += new System.EventHandler(this.addNewCircleToolStripMenuItem_Click);
            // 
            // addNewTriangleToolStripMenuItem
            // 
            this.addNewTriangleToolStripMenuItem.Name = "addNewTriangleToolStripMenuItem";
            this.addNewTriangleToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.addNewTriangleToolStripMenuItem.Text = "Add New Triangle";
            this.addNewTriangleToolStripMenuItem.Click += new System.EventHandler(this.addNewTriangleToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(217, 6);
            // 
            // addRectangleTaggedAreaToolStripMenuItem
            // 
            this.addRectangleTaggedAreaToolStripMenuItem.Name = "addRectangleTaggedAreaToolStripMenuItem";
            this.addRectangleTaggedAreaToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.addRectangleTaggedAreaToolStripMenuItem.Text = "Add Rectangle Tagged Area";
            this.addRectangleTaggedAreaToolStripMenuItem.Click += new System.EventHandler(this.addRectangleTaggedAreaToolStripMenuItem_Click);
            // 
            // addCircleTaggedAreaToolStripMenuItem
            // 
            this.addCircleTaggedAreaToolStripMenuItem.Name = "addCircleTaggedAreaToolStripMenuItem";
            this.addCircleTaggedAreaToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.addCircleTaggedAreaToolStripMenuItem.Text = "Add Circle Tagged Area";
            this.addCircleTaggedAreaToolStripMenuItem.Click += new System.EventHandler(this.addCircleTaggedAreaToolStripMenuItem_Click);
            // 
            // addTriangleTaggedAreaToolStripMenuItem
            // 
            this.addTriangleTaggedAreaToolStripMenuItem.Name = "addTriangleTaggedAreaToolStripMenuItem";
            this.addTriangleTaggedAreaToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.addTriangleTaggedAreaToolStripMenuItem.Text = "Add Triangle Tagged Area";
            this.addTriangleTaggedAreaToolStripMenuItem.Click += new System.EventHandler(this.addTriangleTaggedAreaToolStripMenuItem_Click);
            // 
            // EditSpotsEditorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "EditSpotsEditorView";
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
        private System.Windows.Forms.ToolStripMenuItem addNewTriangleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem addRectangleTaggedAreaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCircleTaggedAreaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTriangleTaggedAreaToolStripMenuItem;
    }
}
