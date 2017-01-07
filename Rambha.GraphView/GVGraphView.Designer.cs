namespace Rambha.GraphView
{
    partial class GVGraphView
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
            this.SuspendLayout();
            // 
            // GVGraphView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "GVGraphView";
            this.Size = new System.Drawing.Size(521, 408);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.GVGraphView_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.GVGraphView_DragEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GVGraphView_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GVGraphView_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GVGraphView_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GVGraphView_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GVGraphView_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GVGraphView_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
