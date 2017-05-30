namespace SlideMaker.Views
{
    partial class PageView
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
            this.timerLongClick = new System.Windows.Forms.Timer(this.components);
            this.timerRuntext = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerLongClick
            // 
            this.timerLongClick.Interval = 500;
            this.timerLongClick.Tick += new System.EventHandler(this.timerLongClick_Tick);
            // 
            // timerRuntext
            // 
            this.timerRuntext.Tick += new System.EventHandler(this.timerRuntext_Tick);
            // 
            // PageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "PageView";
            this.Size = new System.Drawing.Size(709, 496);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PageView_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PageView_KeyDown);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.PageView_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PageView_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PageView_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PageView_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerLongClick;
        private System.Windows.Forms.Timer timerRuntext;
    }
}
