using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FileMover
{
    public partial class MainForm
    {
        private void updateSourceDirectoryTotals()
        /********************************************************************
         * Get the number of directories and files in the source directory  
         * from a background worker thread.  The totals will be posted      
         * back to the main form.                                           
         ********************************************************************/
        {
            FileMover.FileSystem fs = new FileMover.FileSystem();
            int directories = 0, files = 0;

            try
            {
                fs.GetDirectoryTotals(sourceTb.Text, ref directories, ref files);
            }

            catch (Exception e)
            {
                if (e.Message.StartsWith("Target Directory:") && e.Message.EndsWith("not found."))
                { }
                else
                    repoprtException(e);
            }

            lock (Session.locker)
            {
                Session.FilesToProcess = files;
            }

            if (InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { updateSourceTotals(directories, files); });
            else
                updateSourceTotals(directories, files);
        }

        private void updateDestinationDirectoryTotals()
        /********************************************************************
        * Get the number of directories and files in the destination directory 
        * from a background worker thread.  The totals will be posted          
        * back to the main form.                                               
        *********************************************************************/
        {
            FileMover.FileSystem fs = new FileMover.FileSystem();
            int directories = 0, files = 0;

            try
            {
                fs.GetDirectoryTotals(destinationTb.Text, ref directories, ref files);
            }

            catch (Exception e)
            {
                if (e.Message.StartsWith("Target Directory:") && e.Message.EndsWith("not found."))
                {
                }
                else
                    repoprtException(e);
            }

            if (InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { updateDestinationTotals(directories, files); });
            else
                updateDestinationTotals(directories, files);
        }

        private void copyMoveFiles()
        /********************************************************************
         * Copy or move all directories and files contained in the source
         * directory to the specified destination directory according to the 
         * options selected. this message is designed to work from a 
         * background worker thread and will post its progress back to the 
         * main form.
         *********************************************************************/
        {
            FileMover.FileSystem fs = new FileMover.FileSystem();

            fs.SourceDir = sourceTb.Text;
            fs.DestDir = destinationTb.Text;
            fs.Overwrite = overwriteExistFilesChkb.Checked;
            fs.OnlyDirectoriesWithData = onlyDirectoriesWithDataChkb.Checked;

            lock (Session.locker)
            {
                Session.FilesProcessed = 0;
            }

            if (copyRbtn.Checked)
            {
                fs.CopyDirStructureOnly = CpyDirStructOnlyChkb.Checked;

                try
                {
                    fs.CopyDirectory();
                }

                catch (Exception e)
                {
                    repoprtException(e);
                }

                updateDestinationDirectoryTotals();
            }

            if (deleteRbtn.Checked)
            {
                DialogResult dr = MessageBox.Show("This directory contains data.  Would you like to delete this directory?",
                                       "Delete Directory", MessageBoxButtons.YesNo);

                if (dr != DialogResult.Yes)
                    return;

                fs.LeaveDirStructure = leaveDirStructChkb.Checked;

                try
                {
                    fs.RemoveDirectory();
                }

                catch (Exception e)
                {
                    repoprtException(e);
                }

                updateSourceDirectoryTotals();
            }

            if (MoveRbtn.Checked)
            {
                fs.LeaveDirStructure = leaveDirStructChkb.Checked;

                try
                {
                    fs.MoveDirectory();
                }

                catch (Exception e)
                {
                    repoprtException(e);
                }

                updateSourceDirectoryTotals();
                updateDestinationDirectoryTotals();
            }
        }
    }
}
