namespace SlideMaker.Views
{
    partial class ReviewFrame
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
            this.label1 = new System.Windows.Forms.Label();
            this.labelBookTitle = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabBook = new System.Windows.Forms.TabPage();
            this.textBookNotes = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabItem = new System.Windows.Forms.TabPage();
            this.labelControlId = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textItemNotes = new System.Windows.Forms.RichTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textItemText = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabBook.SuspendLayout();
            this.tabPage.SuspendLayout();
            this.tabItem.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Book";
            // 
            // labelBookTitle
            // 
            this.labelBookTitle.AutoSize = true;
            this.labelBookTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBookTitle.Location = new System.Drawing.Point(50, 9);
            this.labelBookTitle.Name = "labelBookTitle";
            this.labelBookTitle.Size = new System.Drawing.Size(41, 13);
            this.labelBookTitle.TabIndex = 1;
            this.labelBookTitle.Text = "label2";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabBook);
            this.tabControl1.Controls.Add(this.tabPage);
            this.tabControl1.Controls.Add(this.tabItem);
            this.tabControl1.Location = new System.Drawing.Point(2, 39);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(371, 433);
            this.tabControl1.TabIndex = 2;
            // 
            // tabBook
            // 
            this.tabBook.Controls.Add(this.textBookNotes);
            this.tabBook.Controls.Add(this.label3);
            this.tabBook.Location = new System.Drawing.Point(4, 22);
            this.tabBook.Name = "tabBook";
            this.tabBook.Padding = new System.Windows.Forms.Padding(3);
            this.tabBook.Size = new System.Drawing.Size(363, 407);
            this.tabBook.TabIndex = 0;
            this.tabBook.Text = "Book";
            this.tabBook.UseVisualStyleBackColor = true;
            // 
            // textBookNotes
            // 
            this.textBookNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBookNotes.Location = new System.Drawing.Point(9, 41);
            this.textBookNotes.Name = "textBookNotes";
            this.textBookNotes.Size = new System.Drawing.Size(347, 359);
            this.textBookNotes.TabIndex = 1;
            this.textBookNotes.Text = "";
            this.textBookNotes.TextChanged += new System.EventHandler(this.textBookNotes_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "General notes for Book";
            // 
            // tabPage
            // 
            this.tabPage.Controls.Add(this.label9);
            this.tabPage.Controls.Add(this.label2);
            this.tabPage.Controls.Add(this.richTextBox3);
            this.tabPage.Controls.Add(this.richTextBox2);
            this.tabPage.Controls.Add(this.richTextBox1);
            this.tabPage.Controls.Add(this.label6);
            this.tabPage.Controls.Add(this.label5);
            this.tabPage.Controls.Add(this.label4);
            this.tabPage.Location = new System.Drawing.Point(4, 22);
            this.tabPage.Name = "tabPage";
            this.tabPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage.Size = new System.Drawing.Size(363, 407);
            this.tabPage.TabIndex = 1;
            this.tabPage.Text = "Page";
            this.tabPage.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(148, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(13, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(120, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "ID:";
            // 
            // richTextBox3
            // 
            this.richTextBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox3.Location = new System.Drawing.Point(9, 251);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.Size = new System.Drawing.Size(347, 150);
            this.richTextBox3.TabIndex = 5;
            this.richTextBox3.Text = "<general notes>";
            this.richTextBox3.TextChanged += new System.EventHandler(this.richTextBox3_TextChanged);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox2.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox2.Location = new System.Drawing.Point(9, 104);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(347, 111);
            this.richTextBox2.TabIndex = 4;
            this.richTextBox2.Text = "";
            this.richTextBox2.TextChanged += new System.EventHandler(this.richTextBox2_TextChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(9, 32);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(347, 43);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 235);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "General Notes";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Help Text";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Page Title";
            // 
            // tabItem
            // 
            this.tabItem.Controls.Add(this.labelControlId);
            this.tabItem.Controls.Add(this.label10);
            this.tabItem.Controls.Add(this.textItemNotes);
            this.tabItem.Controls.Add(this.label8);
            this.tabItem.Controls.Add(this.textItemText);
            this.tabItem.Controls.Add(this.label7);
            this.tabItem.Location = new System.Drawing.Point(4, 22);
            this.tabItem.Name = "tabItem";
            this.tabItem.Size = new System.Drawing.Size(363, 407);
            this.tabItem.TabIndex = 2;
            this.tabItem.Text = "Item";
            this.tabItem.UseVisualStyleBackColor = true;
            // 
            // labelControlId
            // 
            this.labelControlId.AutoSize = true;
            this.labelControlId.Location = new System.Drawing.Point(140, 13);
            this.labelControlId.Name = "labelControlId";
            this.labelControlId.Size = new System.Drawing.Size(13, 13);
            this.labelControlId.TabIndex = 5;
            this.labelControlId.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(112, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(21, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "ID:";
            // 
            // textItemNotes
            // 
            this.textItemNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textItemNotes.Location = new System.Drawing.Point(9, 248);
            this.textItemNotes.Name = "textItemNotes";
            this.textItemNotes.Size = new System.Drawing.Size(347, 152);
            this.textItemNotes.TabIndex = 3;
            this.textItemNotes.Text = "";
            this.textItemNotes.TextChanged += new System.EventHandler(this.richTextBox5_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 232);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "General notes";
            // 
            // textItemText
            // 
            this.textItemText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textItemText.Location = new System.Drawing.Point(9, 30);
            this.textItemText.Name = "textItemText";
            this.textItemText.Size = new System.Drawing.Size(347, 181);
            this.textItemText.TabIndex = 1;
            this.textItemText.Text = "";
            this.textItemText.TextChanged += new System.EventHandler(this.richTextBox4_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Item text";
            // 
            // ReviewFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 473);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.labelBookTitle);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ReviewFrame";
            this.Text = "ReviewFrame";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReviewFrame_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReviewFrame_FormClosed);
            this.Load += new System.EventHandler(this.ReviewFrame_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabBook.ResumeLayout(false);
            this.tabBook.PerformLayout();
            this.tabPage.ResumeLayout(false);
            this.tabPage.PerformLayout();
            this.tabItem.ResumeLayout(false);
            this.tabItem.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelBookTitle;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabBook;
        private System.Windows.Forms.RichTextBox textBookNotes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabItem;
        private System.Windows.Forms.RichTextBox textItemNotes;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RichTextBox textItemText;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelControlId;
        private System.Windows.Forms.Label label10;
    }
}