using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Comdata.AppSupport.FileMover
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeMainForm();
        }

        private void InitializeMainForm()
        {
            sourceTb.Clear();
            sourceTb.Focus();
            destinationTb.Clear();
            sourceGb.Visible = false;
            destinationGb.Visible = false;
            copyRbtn.Checked = true;
            CopyMoveBtn.Text = "Copy";
            MoveRbtn.Checked = false;
            leaveDirStructChkb.Checked = true;
            leaveDirStructChkb.Visible = false;
            CpyDirStructOnlyChkb.Checked = false;
            CpyDirStructOnlyChkb.Visible = true;
            overwriteExistFilesChkb.Checked = false;
            overwriteExistFilesChkb.Visible = false;
            onlyDirectoriesWithDataChkb.Checked = true;
            onlyDirectoriesWithDataChkb.Visible = true;
            progressBar.Visible = false;
            CopyMoveBtn.Enabled = false;
            ResetFormBtn.Visible = false;
            refreshBtn.Visible = false;
            timer.Stop();
            timer.Interval = 1000;           
            Session.FilesProcessed = 0;
            Session.FilesToProcess = 0;
            Session.SourcePathname = string.Empty;
            Session.DestinationPathname = string.Empty;
        }

        private void sourceTb_Validating(object sender, CancelEventArgs e)
        {
            sourceTb.Text = sourceTb.Text.Trim();

            if (!Directory.Exists(sourceTb.Text))
            {
                errorProvider.SetError(sourceTb, "Directory not Found.");
                return;
            }

            errorProvider.SetError(sourceTb, "");

            if (Session.SourcePathname != sourceTb.Text)
            {
                Session.SourcePathname = sourceTb.Text;
                startSourceTotalsBw();
            }
        }

        private void sourceBrowseBtn_Click(object sender, EventArgs e)
        {
            errorProvider.SetError(sourceTb, "");

            if (Directory.Exists(sourceTb.Text))
                folderBrowserDlg.SelectedPath = sourceTb.Text;

            folderBrowserDlg.ShowNewFolderButton = false;
            folderBrowserDlg.ShowDialog();

            if (folderBrowserDlg.SelectedPath == "```")
            {
                sourceTb.Text = string.Empty;
                return;
            }
            else
                sourceTb.Text = folderBrowserDlg.SelectedPath.Trim();

            errorProvider.SetError(sourceTb, "");

            if (Session.SourcePathname != sourceTb.Text)
            {
                Session.SourcePathname = sourceTb.Text;
                startSourceTotalsBw();
            }
        }

        private void startSourceTotalsBw()
       /********************************************************************
        * Start a background worker to report the number of directories and
        * files contained in the source directory.  if this worker is 
        * already running, cancel it, and restart.
        ********************************************************************/
        {
            if (sourceTotalsBw.IsBusy)
                sourceTotalsBw.CancelAsync();

            while (sourceTotalsBw.CancellationPending)
            { }

            sourceTotalsBw.RunWorkerAsync();
        }
      
        private void sourceTotalsBw_DoWork(object sender, DoWorkEventArgs e)
        {
            updateSourceDirectoryTotals();
        }

        private void updateSourceTotals(int directories, int files)
        {
            sourceDirectoriesLbl.Text = string.Format("{0:0,0}", directories);
            sourceFilesLbl.Text = string.Format("{0:0,0}", files);
        }

        private void SourceTotalsBw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            sourceGb.Visible = true;
            refreshBtn.Visible = true;
            enableCopyMoveBtn();
        }

        private void destinationTb_Validating(object sender, CancelEventArgs e)
        {
            destinationTb.Text = destinationTb.Text.Trim();

            if (destinationTb.Text == string.Empty)
            {
                errorProvider.SetError(destinationTb, "Directory not specified.");
                return;
            }

            if (!Directory.Exists(destinationTb.Text))
            {
                DialogResult dr = MessageBox.Show("Directory doesn't Exist.  Would you like to create it?",
                                                    "Create Directory", MessageBoxButtons.YesNo);

                if (dr == DialogResult.Yes)
                    createDestiationDirectory(destinationTb.Text);
                else
                    destinationTb.Text = string.Empty;

                if (destinationTb.Text == string.Empty)
                {
                    errorProvider.SetError(destinationTb, "Directory not specified.");
                    return;
                }
            }
            
            errorProvider.SetError(destinationTb, "");

            if (Session.DestinationPathname != destinationTb.Text)
            {
                Session.DestinationPathname = destinationTb.Text;
                startDestinationTotalsBw();
            }
        }

        private void createDestiationDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }

            catch (Exception ex)
            {
                repoprtException(ex);
                destinationTb.Text = string.Empty;
            }     
           
            
        }
 
        private void destinationBrowseBtn_Click(object sender, EventArgs e)
        {
            errorProvider.SetError(destinationTb, "");

            if (Directory.Exists(destinationTb.Text))
                folderBrowserDlg.SelectedPath = destinationTb.Text;

            folderBrowserDlg.ShowNewFolderButton = true;
            folderBrowserDlg.ShowDialog();

            if (folderBrowserDlg.SelectedPath == "```")
            {
                destinationTb.Text = string.Empty;
                return;
            }
            else
                destinationTb.Text = folderBrowserDlg.SelectedPath.Trim();

            errorProvider.SetError(destinationTb, "");

            if (Session.DestinationPathname != destinationTb.Text)
            {
                Session.DestinationPathname = destinationTb.Text;
                startDestinationTotalsBw();
            }
        }

        private void startDestinationTotalsBw()
        /********************************************************************
         * Start a background worker to report the number of directories and
         * files contained in the destination directory.  if this worker is 
         * already running, cancel it, and restart.
         ********************************************************************/
        {
            if (destinationTotalsBw.IsBusy)
                destinationTotalsBw.CancelAsync();

            while (destinationTotalsBw.CancellationPending)
            { }

            destinationTotalsBw.RunWorkerAsync();
        }

        private void destinationTotalsBw_DoWork(object sender, DoWorkEventArgs e)
        {
            updateDestinationDirectoryTotals();
        }

        private void updateDestinationTotals(int directories, int files)
        {
            destinationDirectoriesLbl.Text = string.Format("{0:0,0}", directories);
            destinationFilesLbl.Text = string.Format("{0:0,0}", files);

            lock (Session.locker)
            {
                Session.DestinationDirectories = directories;
                Session.DestinationFiles = files;
            }

            if (directories > 0 || files > 0)
                overwriteExistFilesChkb.Visible = true;
            else
                overwriteExistFilesChkb.Visible = false;
        }

        private void destinationTotalsThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            destinationGb.Visible = true;
            enableCopyMoveBtn();

        }

        private void enableCopyMoveBtn()
        /********************************************************************
         * Enable the copy/move button only after the form has been fully 
         * validated.
         ********************************************************************/
        {
            CopyMoveBtn.Enabled = false;

            if (sourceTotalsBw.IsBusy)              // no background workers           
                return;                             // are still running.

            if (destinationTotalsBw.IsBusy)
                return;

            if (!Directory.Exists(sourceTb.Text))   // source and destination 
                return;                             // directories exist

            if (deleteRbtn.Checked)
            {
                CopyMoveBtn.Enabled = true;
                return;
            }

            if (!Directory.Exists(destinationTb.Text))
                return;

            lock (Session.locker)
            {                                       // If the destination directory
                                                    // contains data, the user must 
                                                    // specify overwrite existing.
                if (Session.DestinationDirectories > 0 || Session.DestinationFiles > 0)
                    if (!overwriteExistFilesChkb.Checked)
                        return;
            }

            CopyMoveBtn.Enabled = true;
        }
 
        private void CopyMoveBtn_Click(object sender, EventArgs e)
        /********************************************************************
         * Start a background worker to copy/move the selected source 
         * rectory.  A progress bar will be updated during processing. 
         ********************************************************************/
        {
            timer.Start();
            progressBar.Visible = true;
            copyMoveBw.RunWorkerAsync();
            CopyMoveBtn.Enabled = false;
            refreshBtn.Enabled = false;
            ResetFormBtn.Enabled = false;
        }

        private void copyMoveBw_DoWork(object sender, DoWorkEventArgs e)
        {
            copyMoveFiles();
        }

        private void copyMoveBw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer.Stop();
            progressBar.Increment(100 - progressBar.Value);
            refreshBtn.Enabled = true;
            ResetFormBtn.Enabled = true;
            ResetFormBtn.Visible = true;
        }

        private void updateProgressbar(object sender, EventArgs e)
        {
            float percentComplete = 0;
            int increment = 0;

            BeginInvoke((MethodInvoker)delegate
            {
                lock (Session.locker)
                {
                    if (Session.FilesToProcess == 0)
                        percentComplete = 100;
                    else 
                        percentComplete = (float) Session.FilesProcessed / (float) Session.FilesToProcess * 100;
                }

                increment = (int) percentComplete - progressBar.Value; 
                progressBar.Increment(increment);
            });
        }

        private void operationChanged(object sender, EventArgs e)
        {
            if (copyRbtn.Checked)
            {
                CopyMoveBtn.Text = "Copy";
                onlyDirectoriesWithDataChkb.Visible = true;
                CpyDirStructOnlyChkb.Visible = true;
                leaveDirStructChkb.Visible = false;
                destinationTb.Visible = true;
                destinationBrowseBtn.Visible = true;
            }

            if (deleteRbtn.Checked)
            {
                CopyMoveBtn.Text = "Delete"; 
                onlyDirectoriesWithDataChkb.Visible = false;
                CpyDirStructOnlyChkb.Visible = false;
                leaveDirStructChkb.Visible = true;
                destinationTb.Visible = false;
                destinationBrowseBtn.Visible = false;

                if (destinationTotalsBw.IsBusy)
                    destinationTotalsBw.CancelAsync();
            }

            if (MoveRbtn.Checked)
            {
                CopyMoveBtn.Text = "Move";
                onlyDirectoriesWithDataChkb.Visible = true;
                CpyDirStructOnlyChkb.Visible = false;
                leaveDirStructChkb.Visible = true;
                destinationTb.Visible = true;
                destinationBrowseBtn.Visible = true;
            }

            enableCopyMoveBtn();
        }

        private void overwriteExistFilesChkb_CheckedChanged(object sender, EventArgs e)
        {
            enableCopyMoveBtn();
        }

        private void ResetFormBtn_Click(object sender, EventArgs e)
        {
            InitializeMainForm();
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            progressBar.Visible = false;
            startSourceTotalsBw();
        }

        private void repoprtException(Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
