namespace F002520
{
    partial class frmMain
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
            this.panelTitleBar = new System.Windows.Forms.Panel();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnMaximize = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitleBar = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelDesktop = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelTestLog = new System.Windows.Forms.Panel();
            this.rtbTestLog = new System.Windows.Forms.RichTextBox();
            this.panelTestItem = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTestItem = new System.Windows.Forms.Label();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelButton = new System.Windows.Forms.Panel();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.panelTestResult = new System.Windows.Forms.Panel();
            this.lblTestResult = new System.Windows.Forms.Label();
            this.panelTestMode = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioBtnManual = new System.Windows.Forms.RadioButton();
            this.radioBtnAuto = new System.Windows.Forms.RadioButton();
            this.panelMDCS = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioBtnTest = new System.Windows.Forms.RadioButton();
            this.radioBtnProduction = new System.Windows.Forms.RadioButton();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.panelVersion = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panelStation = new System.Windows.Forms.Panel();
            this.lblStation = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panelProject = new System.Windows.Forms.Panel();
            this.lblProject = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.picBoxLogo = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTitleBar.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelDesktop.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelTestLog.SuspendLayout();
            this.panelTestItem.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelButton.SuspendLayout();
            this.panelTestResult.SuspendLayout();
            this.panelTestMode.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelMDCS.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panelInfo.SuspendLayout();
            this.panelVersion.SuspendLayout();
            this.panelStation.SuspendLayout();
            this.panelProject.SuspendLayout();
            this.panelLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxLogo)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTitleBar
            // 
            this.panelTitleBar.BackColor = System.Drawing.SystemColors.Highlight;
            this.panelTitleBar.Controls.Add(this.btnMinimize);
            this.panelTitleBar.Controls.Add(this.btnMaximize);
            this.panelTitleBar.Controls.Add(this.btnClose);
            this.panelTitleBar.Controls.Add(this.lblTitleBar);
            this.panelTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitleBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelTitleBar.Location = new System.Drawing.Point(0, 0);
            this.panelTitleBar.Margin = new System.Windows.Forms.Padding(2);
            this.panelTitleBar.Name = "panelTitleBar";
            this.panelTitleBar.Size = new System.Drawing.Size(900, 32);
            this.panelTitleBar.TabIndex = 0;
            // 
            // btnMinimize
            // 
            this.btnMinimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::F002520.Properties.Resources.minimize_16;
            this.btnMinimize.Location = new System.Drawing.Point(798, 0);
            this.btnMinimize.Margin = new System.Windows.Forms.Padding(0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(34, 32);
            this.btnMinimize.TabIndex = 3;
            this.btnMinimize.UseVisualStyleBackColor = false;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnMaximize
            // 
            this.btnMaximize.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnMaximize.FlatAppearance.BorderSize = 0;
            this.btnMaximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaximize.Image = global::F002520.Properties.Resources.maximize_16;
            this.btnMaximize.Location = new System.Drawing.Point(832, 0);
            this.btnMaximize.Margin = new System.Windows.Forms.Padding(0);
            this.btnMaximize.Name = "btnMaximize";
            this.btnMaximize.Size = new System.Drawing.Size(34, 32);
            this.btnMaximize.TabIndex = 2;
            this.btnMaximize.UseVisualStyleBackColor = false;
            this.btnMaximize.Click += new System.EventHandler(this.btnMaximize_Click);
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Image = global::F002520.Properties.Resources.close_20;
            this.btnClose.Location = new System.Drawing.Point(866, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(34, 32);
            this.btnClose.TabIndex = 1;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblTitleBar
            // 
            this.lblTitleBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitleBar.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleBar.Location = new System.Drawing.Point(0, 0);
            this.lblTitleBar.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitleBar.Name = "lblTitleBar";
            this.lblTitleBar.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblTitleBar.Size = new System.Drawing.Size(922, 32);
            this.lblTitleBar.TabIndex = 0;
            this.lblTitleBar.Text = "F002520 Sensor Calibration Fixture";
            this.lblTitleBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitleBar.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lblTitleBar_MouseDoubleClick);
            this.lblTitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblTitleBar_MouseDown);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelDesktop);
            this.panelMain.Controls.Add(this.menuStrip1);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 32);
            this.panelMain.Margin = new System.Windows.Forms.Padding(2);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(900, 668);
            this.panelMain.TabIndex = 2;
            // 
            // panelDesktop
            // 
            this.panelDesktop.Controls.Add(this.panelRight);
            this.panelDesktop.Controls.Add(this.panelLeft);
            this.panelDesktop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDesktop.Location = new System.Drawing.Point(0, 24);
            this.panelDesktop.Margin = new System.Windows.Forms.Padding(2);
            this.panelDesktop.Name = "panelDesktop";
            this.panelDesktop.Size = new System.Drawing.Size(900, 644);
            this.panelDesktop.TabIndex = 1;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.panelTestLog);
            this.panelRight.Controls.Add(this.panelTestItem);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(210, 0);
            this.panelRight.Margin = new System.Windows.Forms.Padding(2);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(690, 644);
            this.panelRight.TabIndex = 1;
            // 
            // panelTestLog
            // 
            this.panelTestLog.Controls.Add(this.rtbTestLog);
            this.panelTestLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTestLog.Location = new System.Drawing.Point(0, 119);
            this.panelTestLog.Margin = new System.Windows.Forms.Padding(2);
            this.panelTestLog.Name = "panelTestLog";
            this.panelTestLog.Padding = new System.Windows.Forms.Padding(8);
            this.panelTestLog.Size = new System.Drawing.Size(690, 525);
            this.panelTestLog.TabIndex = 1;
            // 
            // rtbTestLog
            // 
            this.rtbTestLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rtbTestLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbTestLog.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbTestLog.Location = new System.Drawing.Point(8, 8);
            this.rtbTestLog.Margin = new System.Windows.Forms.Padding(2);
            this.rtbTestLog.Name = "rtbTestLog";
            this.rtbTestLog.ReadOnly = true;
            this.rtbTestLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbTestLog.Size = new System.Drawing.Size(674, 509);
            this.rtbTestLog.TabIndex = 0;
            this.rtbTestLog.Text = "Sensor Calibration ";
            // 
            // panelTestItem
            // 
            this.panelTestItem.Controls.Add(this.groupBox1);
            this.panelTestItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTestItem.Location = new System.Drawing.Point(0, 0);
            this.panelTestItem.Margin = new System.Windows.Forms.Padding(2);
            this.panelTestItem.Name = "panelTestItem";
            this.panelTestItem.Padding = new System.Windows.Forms.Padding(8);
            this.panelTestItem.Size = new System.Drawing.Size(690, 119);
            this.panelTestItem.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblTestItem);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Courier New", 13.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 8);
            this.groupBox1.Size = new System.Drawing.Size(674, 103);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TestItem :";
            // 
            // lblTestItem
            // 
            this.lblTestItem.BackColor = System.Drawing.Color.Wheat;
            this.lblTestItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTestItem.Font = new System.Drawing.Font("Courier New", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTestItem.ForeColor = System.Drawing.Color.Black;
            this.lblTestItem.Location = new System.Drawing.Point(4, 25);
            this.lblTestItem.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTestItem.Name = "lblTestItem";
            this.lblTestItem.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblTestItem.Size = new System.Drawing.Size(666, 70);
            this.lblTestItem.TabIndex = 0;
            this.lblTestItem.Text = "TestInit";
            this.lblTestItem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(30)))), ((int)(((byte)(68)))));
            this.panelLeft.Controls.Add(this.panelButton);
            this.panelLeft.Controls.Add(this.panelTestResult);
            this.panelLeft.Controls.Add(this.panelTestMode);
            this.panelLeft.Controls.Add(this.panelMDCS);
            this.panelLeft.Controls.Add(this.panelInfo);
            this.panelLeft.Controls.Add(this.panelLogo);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Margin = new System.Windows.Forms.Padding(2);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(210, 644);
            this.panelLeft.TabIndex = 0;
            // 
            // panelButton
            // 
            this.panelButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelButton.Controls.Add(this.btnStop);
            this.panelButton.Controls.Add(this.btnStart);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButton.Location = new System.Drawing.Point(0, 500);
            this.panelButton.Margin = new System.Windows.Forms.Padding(2);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(210, 144);
            this.panelButton.TabIndex = 7;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStop.BackColor = System.Drawing.Color.Red;
            this.btnStop.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(19, 81);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(174, 43);
            this.btnStop.TabIndex = 0;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStart.BackColor = System.Drawing.Color.Green;
            this.btnStart.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(19, 15);
            this.btnStart.Margin = new System.Windows.Forms.Padding(2);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(174, 43);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // panelTestResult
            // 
            this.panelTestResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTestResult.Controls.Add(this.lblTestResult);
            this.panelTestResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTestResult.Location = new System.Drawing.Point(0, 376);
            this.panelTestResult.Margin = new System.Windows.Forms.Padding(2);
            this.panelTestResult.Name = "panelTestResult";
            this.panelTestResult.Padding = new System.Windows.Forms.Padding(2);
            this.panelTestResult.Size = new System.Drawing.Size(210, 124);
            this.panelTestResult.TabIndex = 6;
            // 
            // lblTestResult
            // 
            this.lblTestResult.BackColor = System.Drawing.Color.Green;
            this.lblTestResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTestResult.Font = new System.Drawing.Font("Courier New", 48F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTestResult.Location = new System.Drawing.Point(2, 2);
            this.lblTestResult.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTestResult.Name = "lblTestResult";
            this.lblTestResult.Size = new System.Drawing.Size(204, 118);
            this.lblTestResult.TabIndex = 0;
            this.lblTestResult.Text = "PASS";
            this.lblTestResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelTestMode
            // 
            this.panelTestMode.Controls.Add(this.groupBox2);
            this.panelTestMode.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTestMode.Location = new System.Drawing.Point(0, 305);
            this.panelTestMode.Margin = new System.Windows.Forms.Padding(2);
            this.panelTestMode.Name = "panelTestMode";
            this.panelTestMode.Padding = new System.Windows.Forms.Padding(4);
            this.panelTestMode.Size = new System.Drawing.Size(210, 71);
            this.panelTestMode.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioBtnManual);
            this.groupBox2.Controls.Add(this.radioBtnAuto);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.groupBox2.Location = new System.Drawing.Point(4, 4);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(202, 63);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "TestMode:";
            // 
            // radioBtnManual
            // 
            this.radioBtnManual.AutoSize = true;
            this.radioBtnManual.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioBtnManual.ForeColor = System.Drawing.SystemColors.Control;
            this.radioBtnManual.Location = new System.Drawing.Point(117, 23);
            this.radioBtnManual.Margin = new System.Windows.Forms.Padding(2);
            this.radioBtnManual.Name = "radioBtnManual";
            this.radioBtnManual.Size = new System.Drawing.Size(80, 21);
            this.radioBtnManual.TabIndex = 0;
            this.radioBtnManual.TabStop = true;
            this.radioBtnManual.Text = "Manual";
            this.radioBtnManual.UseVisualStyleBackColor = true;
            // 
            // radioBtnAuto
            // 
            this.radioBtnAuto.AutoSize = true;
            this.radioBtnAuto.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioBtnAuto.ForeColor = System.Drawing.SystemColors.Control;
            this.radioBtnAuto.Location = new System.Drawing.Point(9, 23);
            this.radioBtnAuto.Margin = new System.Windows.Forms.Padding(2);
            this.radioBtnAuto.Name = "radioBtnAuto";
            this.radioBtnAuto.Size = new System.Drawing.Size(62, 21);
            this.radioBtnAuto.TabIndex = 0;
            this.radioBtnAuto.TabStop = true;
            this.radioBtnAuto.Text = "Auto";
            this.radioBtnAuto.UseVisualStyleBackColor = true;
            // 
            // panelMDCS
            // 
            this.panelMDCS.Controls.Add(this.groupBox3);
            this.panelMDCS.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMDCS.ForeColor = System.Drawing.SystemColors.Control;
            this.panelMDCS.Location = new System.Drawing.Point(0, 214);
            this.panelMDCS.Margin = new System.Windows.Forms.Padding(2);
            this.panelMDCS.Name = "panelMDCS";
            this.panelMDCS.Padding = new System.Windows.Forms.Padding(4);
            this.panelMDCS.Size = new System.Drawing.Size(210, 91);
            this.panelMDCS.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioBtnTest);
            this.groupBox3.Controls.Add(this.radioBtnProduction);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.groupBox3.Location = new System.Drawing.Point(4, 4);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(202, 83);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MDCS:";
            // 
            // radioBtnTest
            // 
            this.radioBtnTest.AutoSize = true;
            this.radioBtnTest.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioBtnTest.ForeColor = System.Drawing.SystemColors.Control;
            this.radioBtnTest.Location = new System.Drawing.Point(134, 34);
            this.radioBtnTest.Margin = new System.Windows.Forms.Padding(2);
            this.radioBtnTest.Name = "radioBtnTest";
            this.radioBtnTest.Size = new System.Drawing.Size(62, 21);
            this.radioBtnTest.TabIndex = 0;
            this.radioBtnTest.TabStop = true;
            this.radioBtnTest.Text = "Test";
            this.radioBtnTest.UseVisualStyleBackColor = true;
            // 
            // radioBtnProduction
            // 
            this.radioBtnProduction.AutoSize = true;
            this.radioBtnProduction.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioBtnProduction.ForeColor = System.Drawing.SystemColors.Control;
            this.radioBtnProduction.Location = new System.Drawing.Point(9, 34);
            this.radioBtnProduction.Margin = new System.Windows.Forms.Padding(2);
            this.radioBtnProduction.Name = "radioBtnProduction";
            this.radioBtnProduction.Size = new System.Drawing.Size(116, 21);
            this.radioBtnProduction.TabIndex = 0;
            this.radioBtnProduction.TabStop = true;
            this.radioBtnProduction.Text = "Production";
            this.radioBtnProduction.UseVisualStyleBackColor = true;
            // 
            // panelInfo
            // 
            this.panelInfo.Controls.Add(this.panelVersion);
            this.panelInfo.Controls.Add(this.panelStation);
            this.panelInfo.Controls.Add(this.panelProject);
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelInfo.Font = new System.Drawing.Font("Courier New", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelInfo.Location = new System.Drawing.Point(0, 104);
            this.panelInfo.Margin = new System.Windows.Forms.Padding(2);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(210, 110);
            this.panelInfo.TabIndex = 1;
            // 
            // panelVersion
            // 
            this.panelVersion.Controls.Add(this.lblVersion);
            this.panelVersion.Controls.Add(this.label5);
            this.panelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVersion.Location = new System.Drawing.Point(0, 74);
            this.panelVersion.Margin = new System.Windows.Forms.Padding(2);
            this.panelVersion.Name = "panelVersion";
            this.panelVersion.Size = new System.Drawing.Size(210, 36);
            this.panelVersion.TabIndex = 2;
            // 
            // lblVersion
            // 
            this.lblVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVersion.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblVersion.Location = new System.Drawing.Point(84, 0);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(126, 36);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "01.01";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Dock = System.Windows.Forms.DockStyle.Left;
            this.label5.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.label5.Size = new System.Drawing.Size(84, 36);
            this.label5.TabIndex = 2;
            this.label5.Text = "Version:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelStation
            // 
            this.panelStation.Controls.Add(this.lblStation);
            this.panelStation.Controls.Add(this.label3);
            this.panelStation.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStation.Location = new System.Drawing.Point(0, 37);
            this.panelStation.Margin = new System.Windows.Forms.Padding(2);
            this.panelStation.Name = "panelStation";
            this.panelStation.Size = new System.Drawing.Size(210, 37);
            this.panelStation.TabIndex = 1;
            // 
            // lblStation
            // 
            this.lblStation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStation.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStation.ForeColor = System.Drawing.SystemColors.Control;
            this.lblStation.Location = new System.Drawing.Point(84, 0);
            this.lblStation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStation.Name = "lblStation";
            this.lblStation.Size = new System.Drawing.Size(126, 37);
            this.lblStation.TabIndex = 3;
            this.lblStation.Text = "SensorK";
            this.lblStation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.label3.Size = new System.Drawing.Size(84, 37);
            this.label3.TabIndex = 2;
            this.label3.Text = "Station:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelProject
            // 
            this.panelProject.Controls.Add(this.lblProject);
            this.panelProject.Controls.Add(this.label1);
            this.panelProject.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelProject.Location = new System.Drawing.Point(0, 0);
            this.panelProject.Margin = new System.Windows.Forms.Padding(2);
            this.panelProject.Name = "panelProject";
            this.panelProject.Size = new System.Drawing.Size(210, 37);
            this.panelProject.TabIndex = 0;
            // 
            // lblProject
            // 
            this.lblProject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProject.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProject.ForeColor = System.Drawing.SystemColors.Control;
            this.lblProject.Location = new System.Drawing.Point(84, 0);
            this.lblProject.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(126, 37);
            this.lblProject.TabIndex = 1;
            this.lblProject.Text = "CT45";
            this.lblProject.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(30)))), ((int)(((byte)(68)))));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(84, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelLogo
            // 
            this.panelLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLogo.Controls.Add(this.picBoxLogo);
            this.panelLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogo.Location = new System.Drawing.Point(0, 0);
            this.panelLogo.Margin = new System.Windows.Forms.Padding(2);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Padding = new System.Windows.Forms.Padding(1);
            this.panelLogo.Size = new System.Drawing.Size(210, 104);
            this.panelLogo.TabIndex = 0;
            // 
            // picBoxLogo
            // 
            this.picBoxLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBoxLogo.Image = global::F002520.Properties.Resources.HoneywellLog_150;
            this.picBoxLogo.Location = new System.Drawing.Point(1, 1);
            this.picBoxLogo.Margin = new System.Windows.Forms.Padding(2);
            this.picBoxLogo.Name = "picBoxLogo";
            this.picBoxLogo.Size = new System.Drawing.Size(206, 100);
            this.picBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picBoxLogo.TabIndex = 0;
            this.picBoxLogo.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.configToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(900, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.configToolStripMenuItem.Text = "Config";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTitleBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(900, 590);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sensor Calibration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.panelTitleBar.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panelDesktop.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelTestLog.ResumeLayout(false);
            this.panelTestItem.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelButton.ResumeLayout(false);
            this.panelTestResult.ResumeLayout(false);
            this.panelTestMode.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelMDCS.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panelInfo.ResumeLayout(false);
            this.panelVersion.ResumeLayout(false);
            this.panelStation.ResumeLayout(false);
            this.panelProject.ResumeLayout(false);
            this.panelLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxLogo)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTitleBar;
        private System.Windows.Forms.Label lblTitleBar;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnMaximize;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Panel panelDesktop;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelTestItem;
        private System.Windows.Forms.Panel panelTestLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblTestItem;
        private System.Windows.Forms.RichTextBox rtbTestLog;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Panel panelTestMode;
        private System.Windows.Forms.Panel panelMDCS;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox picBoxLogo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panelTestResult;
        private System.Windows.Forms.RadioButton radioBtnManual;
        private System.Windows.Forms.RadioButton radioBtnAuto;
        private System.Windows.Forms.RadioButton radioBtnTest;
        private System.Windows.Forms.RadioButton radioBtnProduction;
        private System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.Label lblTestResult;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel panelVersion;
        private System.Windows.Forms.Panel panelStation;
        private System.Windows.Forms.Panel panelProject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblStation;
        private System.Windows.Forms.Label label3;
    }
}

