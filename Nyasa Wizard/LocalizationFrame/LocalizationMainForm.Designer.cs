namespace SlideMaker
{
    partial class LocalizationMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalizationMainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listBoxImages = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonAddImage = new System.Windows.Forms.ToolStripButton();
            this.tsbEdit = new System.Windows.Forms.ToolStripButton();
            this.buttonDeleteImage = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listBoxTexts = new System.Windows.Forms.ListBox();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.tsbTextAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbTextEdit = new System.Windows.Forms.ToolStripButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listBoxSounds = new System.Windows.Forms.ListBox();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.tsbSoundAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbSoundEdit = new System.Windows.Forms.ToolStripButton();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.listBoxAudioTexts = new System.Windows.Forms.ListBox();
            this.toolStrip5 = new System.Windows.Forms.ToolStrip();
            this.tsbAudiTextAdd = new System.Windows.Forms.ToolStripButton();
            this.tsbAudioTextEdit = new System.Windows.Forms.ToolStripButton();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.listBoxStyles = new System.Windows.Forms.ListBox();
            this.toolStrip6 = new System.Windows.Forms.ToolStrip();
            this.tsbStylesAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.toolStrip5.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.toolStrip6.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1,
            this.tsbRefresh,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.toolStripTextBox1,
            this.toolStripSeparator1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(699, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.selectToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(41, 22);
            this.toolStripSplitButton1.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.toolStripButton3_ClickNew);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.toolStripButton2_ClickLoad);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.toolStripButton1_ClickSave);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(102, 6);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.selectToolStripMenuItem.Text = "Select";
            this.selectToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // tsbRefresh
            // 
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsbRefresh.Image")));
            this.tsbRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(50, 22);
            this.tsbRefresh.Text = "Refresh";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(65, 22);
            this.toolStripLabel1.Text = "Book Code";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.ReadOnly = true;
            this.toolStripTextBox1.Size = new System.Drawing.Size(60, 25);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(420, 428);
            this.panel1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(699, 428);
            this.splitContainer1.SplitterDistance = 275;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(275, 428);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listBoxImages);
            this.tabPage1.Controls.Add(this.toolStrip2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(267, 402);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Images";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listBoxImages
            // 
            this.listBoxImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxImages.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxImages.FormattingEnabled = true;
            this.listBoxImages.IntegralHeight = false;
            this.listBoxImages.Location = new System.Drawing.Point(3, 31);
            this.listBoxImages.Name = "listBoxImages";
            this.listBoxImages.Size = new System.Drawing.Size(261, 368);
            this.listBoxImages.TabIndex = 5;
            this.listBoxImages.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxImages_DrawItem);
            this.listBoxImages.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxImages_MeasureItem);
            this.listBoxImages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxImages_MouseDown);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddImage,
            this.tsbEdit,
            this.buttonDeleteImage,
            this.toolStripButton5,
            this.toolStripSeparator3,
            this.toolStripButton1});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(261, 25);
            this.toolStrip2.TabIndex = 4;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // buttonAddImage
            // 
            this.buttonAddImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonAddImage.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddImage.Image")));
            this.buttonAddImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddImage.Name = "buttonAddImage";
            this.buttonAddImage.Size = new System.Drawing.Size(33, 22);
            this.buttonAddImage.Text = "Add";
            this.buttonAddImage.Click += new System.EventHandler(this.buttonAddImage_Click);
            // 
            // tsbEdit
            // 
            this.tsbEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbEdit.Image = ((System.Drawing.Image)(resources.GetObject("tsbEdit.Image")));
            this.tsbEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEdit.Name = "tsbEdit";
            this.tsbEdit.Size = new System.Drawing.Size(31, 22);
            this.tsbEdit.Text = "Edit";
            this.tsbEdit.Click += new System.EventHandler(this.tsbImagesEdit_Click);
            // 
            // buttonDeleteImage
            // 
            this.buttonDeleteImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonDeleteImage.Image = ((System.Drawing.Image)(resources.GetObject("buttonDeleteImage.Image")));
            this.buttonDeleteImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDeleteImage.Name = "buttonDeleteImage";
            this.buttonDeleteImage.Size = new System.Drawing.Size(44, 22);
            this.buttonDeleteImage.Text = "Delete";
            this.buttonDeleteImage.Click += new System.EventHandler(this.buttonDeleteImage_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(40, 22);
            this.toolStripButton5.Text = "Spots";
            this.toolStripButton5.Click += new System.EventHandler(this.tsbImagesEditSpots_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBoxTexts);
            this.tabPage2.Controls.Add(this.toolStrip3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(267, 402);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Texts";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listBoxTexts
            // 
            this.listBoxTexts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxTexts.FormattingEnabled = true;
            this.listBoxTexts.IntegralHeight = false;
            this.listBoxTexts.Location = new System.Drawing.Point(8, 31);
            this.listBoxTexts.Name = "listBoxTexts";
            this.listBoxTexts.Size = new System.Drawing.Size(253, 365);
            this.listBoxTexts.TabIndex = 1;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbTextAdd,
            this.tsbTextEdit});
            this.toolStrip3.Location = new System.Drawing.Point(3, 3);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(261, 25);
            this.toolStrip3.TabIndex = 0;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // tsbTextAdd
            // 
            this.tsbTextAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbTextAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsbTextAdd.Image")));
            this.tsbTextAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTextAdd.Name = "tsbTextAdd";
            this.tsbTextAdd.Size = new System.Drawing.Size(33, 22);
            this.tsbTextAdd.Text = "Add";
            this.tsbTextAdd.Click += new System.EventHandler(this.tsbTextAdd_Click);
            // 
            // tsbTextEdit
            // 
            this.tsbTextEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbTextEdit.Image = ((System.Drawing.Image)(resources.GetObject("tsbTextEdit.Image")));
            this.tsbTextEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbTextEdit.Name = "tsbTextEdit";
            this.tsbTextEdit.Size = new System.Drawing.Size(31, 22);
            this.tsbTextEdit.Text = "Edit";
            this.tsbTextEdit.Click += new System.EventHandler(this.tsbTextEdit_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listBoxSounds);
            this.tabPage3.Controls.Add(this.toolStrip4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(267, 402);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Sounds";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listBoxSounds
            // 
            this.listBoxSounds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxSounds.FormattingEnabled = true;
            this.listBoxSounds.Location = new System.Drawing.Point(3, 28);
            this.listBoxSounds.Name = "listBoxSounds";
            this.listBoxSounds.Size = new System.Drawing.Size(261, 368);
            this.listBoxSounds.TabIndex = 1;
            // 
            // toolStrip4
            // 
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSoundAdd,
            this.tsbSoundEdit});
            this.toolStrip4.Location = new System.Drawing.Point(0, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(267, 25);
            this.toolStrip4.TabIndex = 0;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // tsbSoundAdd
            // 
            this.tsbSoundAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSoundAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsbSoundAdd.Image")));
            this.tsbSoundAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSoundAdd.Name = "tsbSoundAdd";
            this.tsbSoundAdd.Size = new System.Drawing.Size(33, 22);
            this.tsbSoundAdd.Text = "Add";
            this.tsbSoundAdd.Click += new System.EventHandler(this.tsbSoundAdd_Click);
            // 
            // tsbSoundEdit
            // 
            this.tsbSoundEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSoundEdit.Image = ((System.Drawing.Image)(resources.GetObject("tsbSoundEdit.Image")));
            this.tsbSoundEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSoundEdit.Name = "tsbSoundEdit";
            this.tsbSoundEdit.Size = new System.Drawing.Size(31, 22);
            this.tsbSoundEdit.Text = "Edit";
            this.tsbSoundEdit.Click += new System.EventHandler(this.tsbSoundEdit_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.listBoxAudioTexts);
            this.tabPage4.Controls.Add(this.toolStrip5);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(267, 402);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "AudioTexts";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // listBoxAudioTexts
            // 
            this.listBoxAudioTexts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAudioTexts.FormattingEnabled = true;
            this.listBoxAudioTexts.IntegralHeight = false;
            this.listBoxAudioTexts.Location = new System.Drawing.Point(8, 28);
            this.listBoxAudioTexts.Name = "listBoxAudioTexts";
            this.listBoxAudioTexts.Size = new System.Drawing.Size(256, 366);
            this.listBoxAudioTexts.TabIndex = 1;
            // 
            // toolStrip5
            // 
            this.toolStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAudiTextAdd,
            this.tsbAudioTextEdit});
            this.toolStrip5.Location = new System.Drawing.Point(0, 0);
            this.toolStrip5.Name = "toolStrip5";
            this.toolStrip5.Size = new System.Drawing.Size(267, 25);
            this.toolStrip5.TabIndex = 0;
            this.toolStrip5.Text = "toolStrip5";
            // 
            // tsbAudiTextAdd
            // 
            this.tsbAudiTextAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbAudiTextAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsbAudiTextAdd.Image")));
            this.tsbAudiTextAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAudiTextAdd.Name = "tsbAudiTextAdd";
            this.tsbAudiTextAdd.Size = new System.Drawing.Size(33, 22);
            this.tsbAudiTextAdd.Text = "Add";
            this.tsbAudiTextAdd.Click += new System.EventHandler(this.tsbAudiTextAdd_Click);
            // 
            // tsbAudioTextEdit
            // 
            this.tsbAudioTextEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbAudioTextEdit.Image = ((System.Drawing.Image)(resources.GetObject("tsbAudioTextEdit.Image")));
            this.tsbAudioTextEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAudioTextEdit.Name = "tsbAudioTextEdit";
            this.tsbAudioTextEdit.Size = new System.Drawing.Size(31, 22);
            this.tsbAudioTextEdit.Text = "Edit";
            this.tsbAudioTextEdit.Click += new System.EventHandler(this.tsbAudioTextEdit_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.listBoxStyles);
            this.tabPage5.Controls.Add(this.toolStrip6);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(267, 402);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Styles";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // listBoxStyles
            // 
            this.listBoxStyles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxStyles.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxStyles.FormattingEnabled = true;
            this.listBoxStyles.IntegralHeight = false;
            this.listBoxStyles.ItemHeight = 18;
            this.listBoxStyles.Location = new System.Drawing.Point(3, 28);
            this.listBoxStyles.Name = "listBoxStyles";
            this.listBoxStyles.Size = new System.Drawing.Size(261, 372);
            this.listBoxStyles.TabIndex = 2;
            this.listBoxStyles.SelectedIndexChanged += new System.EventHandler(this.listBoxStyles_SelectedIndexChanged);
            // 
            // toolStrip6
            // 
            this.toolStrip6.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbStylesAdd,
            this.toolStripSeparator4,
            this.toolStripButton2});
            this.toolStrip6.Location = new System.Drawing.Point(0, 0);
            this.toolStrip6.Name = "toolStrip6";
            this.toolStrip6.Size = new System.Drawing.Size(267, 25);
            this.toolStrip6.TabIndex = 0;
            this.toolStrip6.Text = "toolStrip6";
            // 
            // tsbStylesAdd
            // 
            this.tsbStylesAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbStylesAdd.Image = ((System.Drawing.Image)(resources.GetObject("tsbStylesAdd.Image")));
            this.tsbStylesAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStylesAdd.Name = "tsbStylesAdd";
            this.tsbStylesAdd.Size = new System.Drawing.Size(33, 22);
            this.tsbStylesAdd.Text = "Add";
            this.tsbStylesAdd.Click += new System.EventHandler(this.tsbStylesAdd_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(86, 19);
            this.toolStripButton1.Text = "Add to Library";
            this.toolStripButton1.Click += new System.EventHandler(this.tsbAddImageToLibrary);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(86, 22);
            this.toolStripButton2.Text = "Add to Library";
            this.toolStripButton2.Click += new System.EventHandler(this.tsbAddStyleToLibrary);
            // 
            // LocalizationMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 453);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "LocalizationMainForm";
            this.Text = "Language File Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LocalizationMainForm_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.toolStrip5.ResumeLayout(false);
            this.toolStrip5.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.toolStrip6.ResumeLayout(false);
            this.toolStrip6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListBox listBoxImages;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton buttonAddImage;
        private System.Windows.Forms.ToolStripButton buttonDeleteImage;
        private System.Windows.Forms.ToolStripButton tsbEdit;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ListBox listBoxTexts;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton tsbTextAdd;
        private System.Windows.Forms.ToolStripButton tsbTextEdit;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton tsbSoundAdd;
        private System.Windows.Forms.ToolStripButton tsbSoundEdit;
        private System.Windows.Forms.ListBox listBoxSounds;
        private System.Windows.Forms.ToolStrip toolStrip5;
        private System.Windows.Forms.ToolStripButton tsbAudiTextAdd;
        private System.Windows.Forms.ToolStripButton tsbAudioTextEdit;
        private System.Windows.Forms.ListBox listBoxAudioTexts;
        private System.Windows.Forms.ToolStripButton tsbRefresh;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ToolStrip toolStrip6;
        private System.Windows.Forms.ToolStripButton tsbStylesAdd;
        private System.Windows.Forms.ListBox listBoxStyles;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}