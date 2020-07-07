namespace Comdata.AppSupport.FileMover
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.MoveRbtn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.deleteRbtn = new System.Windows.Forms.RadioButton();
            this.copyRbtn = new System.Windows.Forms.RadioButton();
            this.CpyDirStructOnlyChkb = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.CopyMoveBtn = new System.Windows.Forms.Button();
            this.ResetFormBtn = new System.Windows.Forms.Button();
            this.sourceTb = new System.Windows.Forms.TextBox();
            this.destinationTb = new System.Windows.Forms.TextBox();
            this.sourceBrowseBtn = new System.Windows.Forms.Button();
            this.destinationBrowseBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.sourceDirectoriesLbl = new System.Windows.Forms.Label();
            this.sourceFilesLbl = new System.Windows.Forms.Label();
            this.sourceGb = new System.Windows.Forms.GroupBox();
            this.destinationGb = new System.Windows.Forms.GroupBox();
            this.destinationFilesLbl = new System.Windows.Forms.Label();
            this.destinationDirectoriesLbl = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.leaveDirStructChkb = new System.Windows.Forms.CheckBox();
            this.overwriteExistFilesChkb = new System.Windows.Forms.CheckBox();
            this.sourceTotalsBw = new System.ComponentModel.BackgroundWorker();
            this.destinationTotalsBw = new System.ComponentModel.BackgroundWorker();
            this.copyMoveBw = new System.ComponentModel.BackgroundWorker();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.refreshBtn = new System.Windows.Forms.Button();
            this.onlyDirectoriesWithDataChkb = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.sourceGb.SuspendLayout();
            this.destinationGb.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source:";
            // 
            // folderBrowserDlg
            // 
            this.folderBrowserDlg.SelectedPath = "```";
            this.folderBrowserDlg.ShowNewFolderButton = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Destination:";
            // 
            // MoveRbtn
            // 
            this.MoveRbtn.AutoSize = true;
            this.MoveRbtn.Location = new System.Drawing.Point(20, 63);
            this.MoveRbtn.Name = "MoveRbtn";
            this.MoveRbtn.Size = new System.Drawing.Size(52, 17);
            this.MoveRbtn.TabIndex = 5;
            this.MoveRbtn.TabStop = true;
            this.MoveRbtn.Text = "Move";
            this.MoveRbtn.UseVisualStyleBackColor = true;
            this.MoveRbtn.CheckedChanged += new System.EventHandler(this.operationChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.deleteRbtn);
            this.groupBox1.Controls.Add(this.copyRbtn);
            this.groupBox1.Controls.Add(this.MoveRbtn);
            this.groupBox1.Location = new System.Drawing.Point(528, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(87, 90);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operation";
            // 
            // deleteRbtn
            // 
            this.deleteRbtn.AutoSize = true;
            this.deleteRbtn.Location = new System.Drawing.Point(20, 40);
            this.deleteRbtn.Name = "deleteRbtn";
            this.deleteRbtn.Size = new System.Drawing.Size(56, 17);
            this.deleteRbtn.TabIndex = 6;
            this.deleteRbtn.TabStop = true;
            this.deleteRbtn.Text = "Delete";
            this.deleteRbtn.UseVisualStyleBackColor = true;
            this.deleteRbtn.CheckedChanged += new System.EventHandler(this.operationChanged);
            // 
            // copyRbtn
            // 
            this.copyRbtn.AutoSize = true;
            this.copyRbtn.Location = new System.Drawing.Point(20, 18);
            this.copyRbtn.Name = "copyRbtn";
            this.copyRbtn.Size = new System.Drawing.Size(49, 17);
            this.copyRbtn.TabIndex = 5;
            this.copyRbtn.TabStop = true;
            this.copyRbtn.Text = "Copy";
            this.copyRbtn.UseVisualStyleBackColor = true;
            this.copyRbtn.CheckedChanged += new System.EventHandler(this.operationChanged);
            // 
            // CpyDirStructOnlyChkb
            // 
            this.CpyDirStructOnlyChkb.AutoSize = true;
            this.CpyDirStructOnlyChkb.Location = new System.Drawing.Point(548, 160);
            this.CpyDirStructOnlyChkb.Name = "CpyDirStructOnlyChkb";
            this.CpyDirStructOnlyChkb.Size = new System.Drawing.Size(138, 17);
            this.CpyDirStructOnlyChkb.TabIndex = 9;
            this.CpyDirStructOnlyChkb.Text = "Directory Structure Only";
            this.CpyDirStructOnlyChkb.UseVisualStyleBackColor = true;
            this.CpyDirStructOnlyChkb.Visible = false;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // CopyMoveBtn
            // 
            this.CopyMoveBtn.Location = new System.Drawing.Point(639, 220);
            this.CopyMoveBtn.Name = "CopyMoveBtn";
            this.CopyMoveBtn.Size = new System.Drawing.Size(75, 23);
            this.CopyMoveBtn.TabIndex = 10;
            this.CopyMoveBtn.UseVisualStyleBackColor = true;
            this.CopyMoveBtn.Click += new System.EventHandler(this.CopyMoveBtn_Click);
            // 
            // ResetFormBtn
            // 
            this.ResetFormBtn.Location = new System.Drawing.Point(558, 220);
            this.ResetFormBtn.Name = "ResetFormBtn";
            this.ResetFormBtn.Size = new System.Drawing.Size(75, 23);
            this.ResetFormBtn.TabIndex = 11;
            this.ResetFormBtn.Text = "Reset Form";
            this.ResetFormBtn.UseVisualStyleBackColor = true;
            this.ResetFormBtn.Click += new System.EventHandler(this.ResetFormBtn_Click);
            // 
            // sourceTb
            // 
            this.sourceTb.Location = new System.Drawing.Point(82, 31);
            this.sourceTb.Name = "sourceTb";
            this.sourceTb.Size = new System.Drawing.Size(332, 20);
            this.sourceTb.TabIndex = 13;
            this.sourceTb.Validating += new System.ComponentModel.CancelEventHandler(this.sourceTb_Validating);
            // 
            // destinationTb
            // 
            this.destinationTb.Location = new System.Drawing.Point(82, 73);
            this.destinationTb.Name = "destinationTb";
            this.destinationTb.Size = new System.Drawing.Size(332, 20);
            this.destinationTb.TabIndex = 14;
            this.destinationTb.Validating += new System.ComponentModel.CancelEventHandler(this.destinationTb_Validating);
            // 
            // sourceBrowseBtn
            // 
            this.sourceBrowseBtn.Location = new System.Drawing.Point(421, 27);
            this.sourceBrowseBtn.Name = "sourceBrowseBtn";
            this.sourceBrowseBtn.Size = new System.Drawing.Size(75, 23);
            this.sourceBrowseBtn.TabIndex = 15;
            this.sourceBrowseBtn.Text = "Browse";
            this.sourceBrowseBtn.UseVisualStyleBackColor = true;
            this.sourceBrowseBtn.Click += new System.EventHandler(this.sourceBrowseBtn_Click);
            // 
            // destinationBrowseBtn
            // 
            this.destinationBrowseBtn.Location = new System.Drawing.Point(421, 71);
            this.destinationBrowseBtn.Name = "destinationBrowseBtn";
            this.destinationBrowseBtn.Size = new System.Drawing.Size(75, 23);
            this.destinationBrowseBtn.TabIndex = 16;
            this.destinationBrowseBtn.Text = "Browse";
            this.destinationBrowseBtn.UseVisualStyleBackColor = true;
            this.destinationBrowseBtn.Click += new System.EventHandler(this.destinationBrowseBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Directories:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Files:";
            // 
            // sourceDirectoriesLbl
            // 
            this.sourceDirectoriesLbl.AutoSize = true;
            this.sourceDirectoriesLbl.Location = new System.Drawing.Point(78, 31);
            this.sourceDirectoriesLbl.Name = "sourceDirectoriesLbl";
            this.sourceDirectoriesLbl.Size = new System.Drawing.Size(39, 13);
            this.sourceDirectoriesLbl.TabIndex = 19;
            this.sourceDirectoriesLbl.Text = "Src Dir";
            this.sourceDirectoriesLbl.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sourceFilesLbl
            // 
            this.sourceFilesLbl.AutoSize = true;
            this.sourceFilesLbl.Location = new System.Drawing.Point(78, 54);
            this.sourceFilesLbl.Name = "sourceFilesLbl";
            this.sourceFilesLbl.Size = new System.Drawing.Size(47, 13);
            this.sourceFilesLbl.TabIndex = 20;
            this.sourceFilesLbl.Text = "Src Files";
            this.sourceFilesLbl.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sourceGb
            // 
            this.sourceGb.Controls.Add(this.label3);
            this.sourceGb.Controls.Add(this.sourceFilesLbl);
            this.sourceGb.Controls.Add(this.label4);
            this.sourceGb.Controls.Add(this.sourceDirectoriesLbl);
            this.sourceGb.Location = new System.Drawing.Point(16, 114);
            this.sourceGb.Name = "sourceGb";
            this.sourceGb.Size = new System.Drawing.Size(161, 85);
            this.sourceGb.TabIndex = 21;
            this.sourceGb.TabStop = false;
            this.sourceGb.Text = "Source Directory";
            this.sourceGb.Visible = false;
            // 
            // destinationGb
            // 
            this.destinationGb.Controls.Add(this.destinationFilesLbl);
            this.destinationGb.Controls.Add(this.destinationDirectoriesLbl);
            this.destinationGb.Controls.Add(this.label6);
            this.destinationGb.Controls.Add(this.label5);
            this.destinationGb.Location = new System.Drawing.Point(199, 114);
            this.destinationGb.Name = "destinationGb";
            this.destinationGb.Size = new System.Drawing.Size(161, 85);
            this.destinationGb.TabIndex = 22;
            this.destinationGb.TabStop = false;
            this.destinationGb.Text = "Destination Directory";
            this.destinationGb.Visible = false;
            // 
            // destinationFilesLbl
            // 
            this.destinationFilesLbl.AutoSize = true;
            this.destinationFilesLbl.Location = new System.Drawing.Point(81, 54);
            this.destinationFilesLbl.Name = "destinationFilesLbl";
            this.destinationFilesLbl.Size = new System.Drawing.Size(35, 13);
            this.destinationFilesLbl.TabIndex = 3;
            this.destinationFilesLbl.Text = "label8";
            this.destinationFilesLbl.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // destinationDirectoriesLbl
            // 
            this.destinationDirectoriesLbl.AutoSize = true;
            this.destinationDirectoriesLbl.Location = new System.Drawing.Point(81, 31);
            this.destinationDirectoriesLbl.Name = "destinationDirectoriesLbl";
            this.destinationDirectoriesLbl.Size = new System.Drawing.Size(35, 13);
            this.destinationDirectoriesLbl.TabIndex = 2;
            this.destinationDirectoriesLbl.Text = "label7";
            this.destinationDirectoriesLbl.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Files:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Directories:";
            // 
            // leaveDirStructChkb
            // 
            this.leaveDirStructChkb.AutoSize = true;
            this.leaveDirStructChkb.Location = new System.Drawing.Point(548, 160);
            this.leaveDirStructChkb.Name = "leaveDirStructChkb";
            this.leaveDirStructChkb.Size = new System.Drawing.Size(147, 17);
            this.leaveDirStructChkb.TabIndex = 23;
            this.leaveDirStructChkb.Text = "Leave Directory Structure";
            this.leaveDirStructChkb.UseVisualStyleBackColor = true;
            // 
            // overwriteExistFilesChkb
            // 
            this.overwriteExistFilesChkb.AutoSize = true;
            this.overwriteExistFilesChkb.Location = new System.Drawing.Point(548, 183);
            this.overwriteExistFilesChkb.Name = "overwriteExistFilesChkb";
            this.overwriteExistFilesChkb.Size = new System.Drawing.Size(136, 17);
            this.overwriteExistFilesChkb.TabIndex = 24;
            this.overwriteExistFilesChkb.Text = "Overwrite Existing Filws";
            this.overwriteExistFilesChkb.UseVisualStyleBackColor = true;
            this.overwriteExistFilesChkb.CheckedChanged += new System.EventHandler(this.overwriteExistFilesChkb_CheckedChanged);
            // 
            // sourceTotalsBw
            // 
            this.sourceTotalsBw.WorkerSupportsCancellation = true;
            this.sourceTotalsBw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.sourceTotalsBw_DoWork);
            this.sourceTotalsBw.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.SourceTotalsBw_RunWorkerCompleted);
            // 
            // destinationTotalsBw
            // 
            this.destinationTotalsBw.WorkerSupportsCancellation = true;
            this.destinationTotalsBw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.destinationTotalsBw_DoWork);
            this.destinationTotalsBw.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.destinationTotalsThread_RunWorkerCompleted);
            // 
            // copyMoveBw
            // 
            this.copyMoveBw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.copyMoveBw_DoWork);
            this.copyMoveBw.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.copyMoveBw_RunWorkerCompleted);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 233);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(308, 23);
            this.progressBar.TabIndex = 25;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.updateProgressbar);
            // 
            // refreshBtn
            // 
            this.refreshBtn.Location = new System.Drawing.Point(40, 204);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(75, 23);
            this.refreshBtn.TabIndex = 26;
            this.refreshBtn.Text = "Refresh";
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // onlyDirectoriesWithDataChkb
            // 
            this.onlyDirectoriesWithDataChkb.AutoSize = true;
            this.onlyDirectoriesWithDataChkb.Location = new System.Drawing.Point(548, 137);
            this.onlyDirectoriesWithDataChkb.Name = "onlyDirectoriesWithDataChkb";
            this.onlyDirectoriesWithDataChkb.Size = new System.Drawing.Size(142, 17);
            this.onlyDirectoriesWithDataChkb.TabIndex = 27;
            this.onlyDirectoriesWithDataChkb.Text = "Only Directories w/ Data";
            this.onlyDirectoriesWithDataChkb.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 268);
            this.Controls.Add(this.onlyDirectoriesWithDataChkb);
            this.Controls.Add(this.refreshBtn);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.overwriteExistFilesChkb);
            this.Controls.Add(this.leaveDirStructChkb);
            this.Controls.Add(this.destinationGb);
            this.Controls.Add(this.sourceGb);
            this.Controls.Add(this.destinationBrowseBtn);
            this.Controls.Add(this.sourceBrowseBtn);
            this.Controls.Add(this.destinationTb);
            this.Controls.Add(this.sourceTb);
            this.Controls.Add(this.ResetFormBtn);
            this.Controls.Add(this.CopyMoveBtn);
            this.Controls.Add(this.CpyDirStructOnlyChkb);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "File Mover";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.sourceGb.ResumeLayout(false);
            this.sourceGb.PerformLayout();
            this.destinationGb.ResumeLayout(false);
            this.destinationGb.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDlg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton MoveRbtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton copyRbtn;
        private System.Windows.Forms.CheckBox CpyDirStructOnlyChkb;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Button ResetFormBtn;
        private System.Windows.Forms.Button CopyMoveBtn;
        private System.Windows.Forms.TextBox destinationTb;
        private System.Windows.Forms.TextBox sourceTb;
        private System.Windows.Forms.Button destinationBrowseBtn;
        private System.Windows.Forms.Button sourceBrowseBtn;
        private System.Windows.Forms.Label sourceFilesLbl;
        private System.Windows.Forms.Label sourceDirectoriesLbl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox sourceGb;
        private System.Windows.Forms.GroupBox destinationGb;
        private System.Windows.Forms.Label destinationFilesLbl;
        private System.Windows.Forms.Label destinationDirectoriesLbl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox leaveDirStructChkb;
        private System.Windows.Forms.CheckBox overwriteExistFilesChkb;
        private System.ComponentModel.BackgroundWorker sourceTotalsBw;
        private System.ComponentModel.BackgroundWorker destinationTotalsBw;
        private System.ComponentModel.BackgroundWorker copyMoveBw;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button refreshBtn;
        private System.Windows.Forms.CheckBox onlyDirectoriesWithDataChkb;
        private System.Windows.Forms.RadioButton deleteRbtn;
    }
}

