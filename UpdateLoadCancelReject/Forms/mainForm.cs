using Comdata.AppSupport.AppTools;
using Comdata.AppSupport.UpdateLoadCancelReject.Classes;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Linq;


namespace Comdata.AppSupport.UpdateLoadCancelReject.Forms
{
    public partial class formMain : Form
    {
        string OutgoingPayrollPath = string.Empty;
        string ProcessedPath = string.Empty;
        bool UpdateRequired = false;
        bool UpdateCompleted = false;
        ILog log;
        Classes.LoadCancelReject util;
        Classes.DailyLCRReport report;
        string achFiles = null;
        string postingDates = null;
        string sendScript;
        string currentDirectory;

        public formMain()
        {
            InitializeComponent();
            log = new TextLogger(Properties.Settings.Default.LogPath, Properties.Settings.Default.LogThreshold);
            log.AddSeverityLevel = true;
            log.AddTimeStamp = true;

            util = new Classes.LoadCancelReject(log);
            util.ProcessedPath = Properties.Settings.Default.ProcessedPath;

            Session.Dal = new DAL.OpenWayDB(Properties.Settings.Default.Connection);

            report = new Classes.DailyLCRReport(this.log, Session.Dal, @".\PPOLBinNumbers.txt.", @".\PPOLEmail.txt.");
 
            this.sendScript = Properties.Settings.Default.SendScript;
            this.currentDirectory = Environment.CurrentDirectory;
            this.btnUpdatePayrollRejects.Enabled = false;
            this.btnGenerateEmail.Enabled = false;
            this.btnSendPayrollRejects.Enabled = false;
            this.log.Write("Started Update Load Cancel Reject...");
        }

        /// <summary>
        /// click handler for Browse button, opens file dialog for loading the reject file, and loads any REGIONS files in the working dir
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {   
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.InitialDirectory = Properties.Settings.Default.OutgoingPayrollPath;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                this.tbRejectFileName.Text = this.openFileDialog1.FileName;            
        }

        private void tbRejectFileName_TextChanged(object sender, EventArgs e)
        {
            this.tbRejectFile.Clear();

            if (File.Exists(tbRejectFileName.Text))
            {
                util.LoadRejectData(tbRejectFileName.Text);
                tbRejectFile.AppendText(util.RejectData);
                this.UpdateRequired = willUpdateBeRequired();
            }
        }

        private void tbPostingDates_Leave(object sender, EventArgs e)
        {
            var shortDates = "";
            DateTime testDate;

            foreach (var date in this.tbPostingDates.Text.Split(Environment.NewLine.ToCharArray(),
                                          StringSplitOptions.RemoveEmptyEntries))
                if (DateTime.TryParse(date, out testDate))
                    shortDates += testDate.Date.ToShortDateString() + Environment.NewLine;

            this.tbPostingDates.Text = shortDates;
        }

        private void tbCreditAmounts_Leave(object sender, EventArgs e)
        {
            var amounts = "";
            var formattedAmount = "";
            Decimal testAmount;

            foreach (var credit in this.tbCreditAmounts.Text.Split(Environment.NewLine.ToCharArray(),
                                          StringSplitOptions.RemoveEmptyEntries))
                if (Decimal.TryParse(credit, out testAmount))
                {
                    formattedAmount = testAmount.ToString("#########.00");
                    amounts += formattedAmount + Environment.NewLine;
                }
            this.tbCreditAmounts.Text = amounts;
        }

        private bool willUpdateBeRequired()
        {
            string messageBoxText = "Do you need to add credits to this file?";
            string caption = "Is an Update Required";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);

            if (result == MessageBoxResult.Yes)
            {
                this.log.Write("User indicated an update to the load cancel file is required.");
                this.btnUpdatePayrollRejects.Enabled = true;
                this.btnGenerateEmail.Enabled = false;
                this.btnSendPayrollRejects.Enabled = false;
                return true;
            }
            else
            {
                this.log.Write("User indicated an update to the load cancel file is not required.");
                this.btnUpdatePayrollRejects.Enabled = false;
                this.btnGenerateEmail.Enabled = true;
                this.btnSendPayrollRejects.Enabled = false;
                createBackupAndSentCopies();
                return false;
            }
        }

        private void createBackupAndSentCopies()
        {
            createBackupCopy(tbRejectFileName.Text.Trim());
            createSentCopy(tbRejectFileName.Text.Trim());
            updateStatus("Backup and sent copies created.");
        }

        /// <summary>
        /// click handler for Create button and main processing logic
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (util.CreditList.Count == 0)
            {
                updateStatus("Load Cancel Reject file has been backed up and then updated.");
                this.btnUpdatePayrollRejects.Enabled = false;
                this.btnGenerateEmail.Enabled = true;
                UpdateCompleted = true;
            }
            else
                updateStatus("Couldn't update the Load Cancel Reject file, bcause not all credits were found. Please try again.");
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadData();
            var rejectData = tbRejectFile.Text;
            var fname = tbRejectFileName.Text.Trim();
            var result = util.Update(tbPostingDates.Text, tbContractNumbers.Text, tbCreditAmounts.Text, ref rejectData);

            if (result > 0)
            {
                return;
            }

            // finally, do the final refresh of the Text property from the ArrayList, repositioning the cursor at TOF
            if (InvokeRequired)
                tbRejectFile.BeginInvoke((Action)(() =>
                {
                    tbRejectFile.Text = "";
                    tbRejectFile.AppendText(util.RejectData);
                    tbRejectFile.Text.TrimEnd(Environment.NewLine.ToCharArray());
                }));
            else
            {
                tbRejectFile.Text = "";
                tbRejectFile.AppendText(util.RejectData);
                tbRejectFile.Text.TrimEnd(Environment.NewLine.ToCharArray());
            }

            createBackupCopy(fname);
            File.Delete(fname);

            using (StreamWriter sw = new StreamWriter(fname))
                sw.Write(util.RejectData);

            this.log.Write("The Load Cancel Reject file has been updated.");
            createSentCopy(fname);
        }
        
        private void loadData()
        {
            tbRejectFileName.Text = openFileDialog1.FileName.ToString();
            util.PostingDates = this.tbPostingDates.Text;
            postingDates = this.tbPostingDates.Text;
            util.LoadAchData(postingDates, out achFiles);

            if (InvokeRequired)

                tbREGIONSFiles.BeginInvoke((Action)(() =>
                {
                    tbREGIONSFiles.Text = achFiles;
                    tbPostingDates.Text = postingDates;
                }));
            else
            {
                tbREGIONSFiles.Text = achFiles;
                tbPostingDates.Text = postingDates;
            }
        }

        private void createSentCopy(string fname)
        {
            var sentDir = Path.Combine(Path.GetDirectoryName(fname), @"sent\SENT_TO_REGIONS");
            sentDir += DateTime.Now.ToString("_MM_dd_yy");
            var dest = Path.Combine(sentDir, Path.GetFileName(fname));

            if (!Directory.Exists(sentDir))
                Directory.CreateDirectory(sentDir);

            if (!File.Exists(dest))
            {
                File.Copy(fname, dest);
                this.log.Write("{0} has been copied up to {1}.", fname, dest);
            }
        }

        private void createBackupCopy(string fname)
        {
            var backupPath = Path.Combine(Path.GetDirectoryName(fname), "backup");
            var dest = Path.Combine(backupPath, Path.GetFileName(fname));

            if (!File.Exists(dest))
            {
                File.Copy(fname, dest);
                this.log.Write("{0} has been backed up to {1}.", fname, dest);
            }
        }

        /// <summary>
        /// Generate Daily Email Text that accompanies the Reject File sent to Regions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerateEmail_Click(object sender, EventArgs e)
        {
            this.log.Write("");
            this.log.Write("Creating Payroll Rejects Email.");
            updateStatus("Creating Payroll Rejects Email.");

            report.CreateReport(tbRejectFile.Lines);
            System.Diagnostics.Process.Start(report.EmailFileName);

            if (this.UpdateRequired && this.UpdateCompleted)
                this.btnSendPayrollRejects.Enabled = true;

            if (!this.UpdateRequired)
                this.btnSendPayrollRejects.Enabled = true;
        }

        private void btnSendPayrollRejects_Click(object sender, EventArgs e)
        {
            this.log.Write("");
            this.log.Write("Sending Payroll Rejects to Regions.");
            System.Diagnostics.Process.Start(this.sendScript);
            updateStatus("Sending Payroll Rejects to Regions.");
            this.btnSendPayrollRejects.Enabled = false;

        }

        /// <summary>
        /// error dialog box
        /// </summary>
        /// <param name="p">string</param>
        private void ShowDialog(string p)
        {
            string messageBoxText = p;
            string caption = "Error";
            MessageBoxButton button = MessageBoxButton.OK;                ;
            MessageBoxImage icon = MessageBoxImage.Stop;
            System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void formCreateRejectFile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.btnSendPayrollRejects.Enabled)
            {
                string messageBoxText = "Do you want to exit without sending Reject file to Regions?";
                string caption = "Reject File has not been Sent";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Question;
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);

                if (result == MessageBoxResult.No)
                    e.Cancel = true; 
            }
        }

        private void updateStatus(string text)
        {
            if (InvokeRequired)
                statusStrip.BeginInvoke((Action)(() =>
                {
                    this.statusLabel.Text = text;
                }));
            else
            {
                this.statusLabel.Text = text;
            }
        }

         private void formMain_Shown(object sender, EventArgs e)
        {
            Forms.LoginForm loginForm = new Forms.LoginForm();
            loginForm.Show(this);
        }
    }
}
