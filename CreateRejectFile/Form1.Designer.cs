namespace CreateRejectFile
{
    partial class formCreateRejectFile
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
            this.tbRejectFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnCreate = new System.Windows.Forms.Button();
            this.tbContractNumbers = new System.Windows.Forms.TextBox();
            this.lblContractnumbers = new System.Windows.Forms.Label();
            this.tbREGIONSFiles = new System.Windows.Forms.TextBox();
            this.lblREGIONSfiles = new System.Windows.Forms.Label();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.tbFileDates = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbCreditAmounts = new System.Windows.Forms.TextBox();
            this.lblCreditamounts = new System.Windows.Forms.Label();
            this.lblFileDates = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbRejectFile
            // 
            this.tbRejectFile.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRejectFile.Location = new System.Drawing.Point(299, 370);
            this.tbRejectFile.Multiline = true;
            this.tbRejectFile.Name = "tbRejectFile";
            this.tbRejectFile.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbRejectFile.Size = new System.Drawing.Size(909, 400);
            this.tbRejectFile.TabIndex = 0;
            this.tbRejectFile.WordWrap = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(12, 12);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(269, 29);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(12, 736);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(269, 34);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // tbContractNumbers
            // 
            this.tbContractNumbers.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbContractNumbers.Location = new System.Drawing.Point(935, 60);
            this.tbContractNumbers.Multiline = true;
            this.tbContractNumbers.Name = "tbContractNumbers";
            this.tbContractNumbers.Size = new System.Drawing.Size(100, 287);
            this.tbContractNumbers.TabIndex = 3;
            // 
            // lblContractnumbers
            // 
            this.lblContractnumbers.AutoSize = true;
            this.lblContractnumbers.Location = new System.Drawing.Point(852, 40);
            this.lblContractnumbers.Name = "lblContractnumbers";
            this.lblContractnumbers.Size = new System.Drawing.Size(247, 17);
            this.lblContractnumbers.TabIndex = 4;
            this.lblContractnumbers.Text = "CARDHOLDER_CONTRACT numbers";
            // 
            // tbREGIONSFiles
            // 
            this.tbREGIONSFiles.Enabled = false;
            this.tbREGIONSFiles.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbREGIONSFiles.Location = new System.Drawing.Point(299, 60);
            this.tbREGIONSFiles.Multiline = true;
            this.tbREGIONSFiles.Name = "tbREGIONSFiles";
            this.tbREGIONSFiles.Size = new System.Drawing.Size(426, 287);
            this.tbREGIONSFiles.TabIndex = 5;
            // 
            // lblREGIONSfiles
            // 
            this.lblREGIONSfiles.AutoSize = true;
            this.lblREGIONSfiles.Location = new System.Drawing.Point(296, 40);
            this.lblREGIONSfiles.Name = "lblREGIONSfiles";
            this.lblREGIONSfiles.Size = new System.Drawing.Size(100, 17);
            this.lblREGIONSfiles.TabIndex = 6;
            this.lblREGIONSfiles.Text = "REGIONS files";
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(299, 15);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(909, 22);
            this.tbFileName.TabIndex = 7;
            // 
            // tbFileDates
            // 
            this.tbFileDates.Enabled = false;
            this.tbFileDates.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbFileDates.Location = new System.Drawing.Point(731, 61);
            this.tbFileDates.Multiline = true;
            this.tbFileDates.Name = "tbFileDates";
            this.tbFileDates.ReadOnly = true;
            this.tbFileDates.Size = new System.Drawing.Size(100, 286);
            this.tbFileDates.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(296, 350);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(816, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "0         1         2         3         4         5         6         7         8" +
    "         9         1";
            // 
            // tbCreditAmounts
            // 
            this.tbCreditAmounts.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCreditAmounts.Location = new System.Drawing.Point(1108, 60);
            this.tbCreditAmounts.Multiline = true;
            this.tbCreditAmounts.Name = "tbCreditAmounts";
            this.tbCreditAmounts.Size = new System.Drawing.Size(100, 287);
            this.tbCreditAmounts.TabIndex = 10;
            // 
            // lblCreditamounts
            // 
            this.lblCreditamounts.AutoSize = true;
            this.lblCreditamounts.Location = new System.Drawing.Point(1105, 40);
            this.lblCreditamounts.Name = "lblCreditamounts";
            this.lblCreditamounts.Size = new System.Drawing.Size(103, 17);
            this.lblCreditamounts.TabIndex = 11;
            this.lblCreditamounts.Text = "Credit amounts";
            // 
            // lblFileDates
            // 
            this.lblFileDates.AutoSize = true;
            this.lblFileDates.Location = new System.Drawing.Point(731, 41);
            this.lblFileDates.Name = "lblFileDates";
            this.lblFileDates.Size = new System.Drawing.Size(69, 17);
            this.lblFileDates.TabIndex = 12;
            this.lblFileDates.Text = "File dates";
            // 
            // formCreateRejectFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 782);
            this.Controls.Add(this.lblFileDates);
            this.Controls.Add(this.lblCreditamounts);
            this.Controls.Add(this.tbCreditAmounts);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbFileDates);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.lblREGIONSfiles);
            this.Controls.Add(this.tbREGIONSFiles);
            this.Controls.Add(this.lblContractnumbers);
            this.Controls.Add(this.tbContractNumbers);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbRejectFile);
            this.Name = "formCreateRejectFile";
            this.Text = "Create Reject File";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbRejectFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.TextBox tbContractNumbers;
        private System.Windows.Forms.Label lblContractnumbers;
        private System.Windows.Forms.TextBox tbREGIONSFiles;
        private System.Windows.Forms.Label lblREGIONSfiles;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.TextBox tbFileDates;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbCreditAmounts;
        private System.Windows.Forms.Label lblCreditamounts;
        private System.Windows.Forms.Label lblFileDates;
    }
}

