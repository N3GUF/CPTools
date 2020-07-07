using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Comdata.AppSupport.FileMover
{
    public class FileSystem
    {
        #region Private Vaiables

        private string sourceDir = string.Empty;
        private string destDir = string.Empty;
        private bool copyDirStructureOnly = false;
        private bool leaveDirStructure = false;
        private bool overwrite = false;
        private bool onlyDirectoriesWithData = false;
    
        #endregion

        #region Properties

        public string SourceDir                     // set or get Source Directory
        { 
            set { sourceDir = value; }
            get { return sourceDir;; }
        }

        public string DestDir                       // set or get Destination Directory
        {
            set { destDir = value; }
            get { return destDir; }
        }

        public bool CopyDirStructureOnly
        {
            set { copyDirStructureOnly = value; }
            get { return copyDirStructureOnly; }
        }

        public bool LeaveDirStructure
        {
            set { leaveDirStructure = value; }
            get { return leaveDirStructure; }
        }

        public bool Overwrite
        {
            set { overwrite = value; }
            get { return overwrite; }
        }

        public bool OnlyDirectoriesWithData
        {
            set { onlyDirectoriesWithData = value; }
            get { return onlyDirectoriesWithData; }
        }

        #endregion

        #region Public Methods

        public void GetDirectoryTotals(ref int directories, ref int files)
        {
            directories = 0;
            files = 0;
            getDirectoryTotals(this.sourceDir, ref directories, ref files);

            if (directories > 0)
                directories--;
        }

        public void GetDirectoryTotals(string targetDirectory, ref int directories, ref int files)
        {
            directories = 0;
            files = 0;
            getDirectoryTotals(targetDirectory, ref directories, ref files);

            if (directories > 0)
                directories--;
        }
       
        public void CopyDirectory()
        {
            copyDirectory(this.sourceDir, this.destDir);
        }

        public void CopyDirectory(string source, string destination)
        {
            this.sourceDir = source;
            this.destDir = destination;
            copyDirectory(this.sourceDir, this.destDir);
        }

        public void MoveDirectory()
        {
            moveDirectory(this.sourceDir, this.destDir);
        }

        public void MoveDirectory(string source, string destination)
        {
            this.sourceDir = source;
            this.destDir = destination;
            moveDirectory(this.sourceDir, this.destDir);
        }

        public void RemoveDirectory()
        {
            removeDirectory(this.sourceDir);
        }

        public void RemoveDirectory(string targetDirectory)
        {
            removeDirectory(targetDirectory);
        }
         
        #endregion 
       
        #region Private Nethods

        private void getDirectoryTotals(string targetDirectory, ref int directories, ref int files)
        {
            if (!Directory.Exists(targetDirectory))
                throw new Exception(string.Format("Target Directory: {0} not found.", targetDirectory));

            files += Directory.GetFiles(targetDirectory).Length;
            directories++;

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            foreach (string subdirectory in subdirectoryEntries)
                getDirectoryTotals(subdirectory, ref directories, ref files);
        }
        
        private void copyDirectory(string source, string destination)
        {
            if (!Directory.Exists(source))
                throw new Exception(string.Format("Source Directory: {0} not found.", source));

            if (destination == string.Empty)
                throw new Exception("Destination Directory not specified.");

            DirectoryInfo di = new DirectoryInfo(source);
            FileInfo[] files = di.GetFiles();
            DirectoryInfo[] subdirectories = di.GetDirectories();

            if (onlyDirectoriesWithData && files.Length == 0 && subdirectories.Length == 0)
                return;
            
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            if (!this.copyDirStructureOnly)
                foreach (FileInfo fi in files)
                {
                    try
                    {
                        fi.CopyTo(fi.FullName.Replace(source, destination), this.overwrite);
                    }

                    catch (Exception e)
                    {
                        if (e.Message.StartsWith("Access to the path") && e.Message.EndsWith("is denied."))
                            continue;
                        else
                            throw new Exception(e.Message);
                    } 

                    lock (Session.locker)
                    { 
                        Session.FilesProcessed++;
                    }
                }

            // Recurse into subdirectories of this directory.
            foreach (DirectoryInfo sdi in subdirectories)
                copyDirectory(sdi.FullName, sdi.FullName.Replace(source, destination));
        }

        private void moveDirectory(string source, string destination)
        {
            if (!Directory.Exists(source))
                throw new Exception(string.Format("Source Directory: {0} not found.", source));

            if (destination == string.Empty)
                throw new Exception("Destination Directory not specified.");

            DirectoryInfo di = new DirectoryInfo(source);
            FileInfo[] files = di.GetFiles();
            DirectoryInfo[] subdirectories = di.GetDirectories();

            if (onlyDirectoriesWithData && files.Length == 0 && subdirectories.Length == 0)
                return;

            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            foreach (FileInfo fi in files)
            {
                try
                {
                    fi.CopyTo(fi.FullName.Replace(source, destination), this.overwrite);
                    fi.Delete();
                }

                catch (Exception e)
                {
                    if (e.Message.StartsWith("Access to the path") && e.Message.EndsWith("is denied."))
                        continue;
                    else
                        throw new Exception(e.Message);
                } 

                lock (Session.locker)
                {
                    Session.FilesProcessed++;
                }
            }

            // Recurse into subdirectories of this directory.
            foreach (DirectoryInfo sdi in subdirectories)
                moveDirectory(sdi.FullName, sdi.FullName.Replace(source, destination));

            if (!this.leaveDirStructure)
                try
                {
                    Directory.Delete(source);
                }

                catch (Exception e)
                {
                    if (e.Message.StartsWith("The directory is not empty."))
                    {}
                    else
                        throw new Exception(e.Message);
                } 
        }

        private void removeDirectory(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
                throw new Exception(string.Format("Target Directory: {0} not found.", targetDirectory));

            DirectoryInfo di = new DirectoryInfo(targetDirectory);
            FileInfo[] files = di.GetFiles();
            DirectoryInfo[] subdirectories = di.GetDirectories();

            if (onlyDirectoriesWithData && files.Length == 0 && subdirectories.Length == 0)
                return;

            foreach (FileInfo fi in files)
            {
                try
                {
                    fi.Delete();
                }

                catch (Exception e)
                {
                    if (e.Message.StartsWith("Access to the path") && e.Message.EndsWith("is denied."))
                        continue;
                    else
                        throw new Exception(e.Message);
                } 

                
                
                lock (Session.locker)
                {
                    Session.FilesProcessed++;
                }
            }

            // Recurse into subdirectories of this directory.
            foreach (DirectoryInfo sdi in subdirectories)
                removeDirectory(sdi.FullName);

            if (!this.leaveDirStructure)
                try
                {
                    Directory.Delete(targetDirectory);
                }

                catch (Exception e)
                {
                    if (e.Message.StartsWith("The directory is not empty."))
                    { }
                    else
                        throw new Exception(e.Message);
                } 

        }

        #endregion 
    }
}
