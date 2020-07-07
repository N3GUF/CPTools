using System;
using System.IO;

namespace Comdata.AppSupport.AppTools
{
    public class FileMover
    {
        #region Private Vaiables

        private string sourceDir = string.Empty;
        private string destDir = string.Empty;
        private bool copyDirStructureOnly = false;
        private bool leaveDirStructure = false;
        private bool overwrite = false;
        private bool onlyDirectoriesWithData = false;

        private int sourceFiles;
        private int sourceFolders;
        private int affetedFiles;
        private int affectedFolders;

    
        #endregion

        #region Properties
        /// <summary>
        /// Sets or Gets Source Directory
        /// </summary>
        public string SourceDir                     
        { 
            set { sourceDir = value; }
            get { return sourceDir; }
        }

        /// <summary>
        /// Sets or Gets Destination Directory
        /// </summary>
        public string DestDir                       
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

        public int SourceFiles
        {
            get { return sourceFiles; }
            set { sourceFiles = value; }
        }

        public int SourceFolders
        {
            get { return sourceFolders; }
            set { sourceFolders = value; }
        }

        public int AffectedFiles
        {
            get { return affetedFiles; }
            set { affetedFiles = value; }
        }

        public int AffectedFolders
        {
            get { return affectedFolders; }
            set { affectedFolders = value; }
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

        public void GetDirectoryTotals(string source, ref int directories, ref int files)
        {
            directories = 0;
            files = 0;
            getDirectoryTotals(source, ref directories, ref files);

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
            this.AffectedFiles = 0;
            this.affectedFolders = 0;
            copyDirectory(this.sourceDir, this.destDir);
        }

        public void MoveDirectory()
        {
            this.AffectedFiles = 0;
            this.affectedFolders = 0;
            moveDirectory(this.sourceDir, this.destDir);
        }

        public void MoveDirectory(string source, string destination)
        {
            this.sourceDir = source;
            this.destDir = destination;
            this.AffectedFiles = 0;
            this.affectedFolders = 0;
            moveDirectory(this.sourceDir, this.destDir);
        }

        public void RemoveDirectory()
        {
            removeDirectory(this.sourceDir);
        }

        public void RemoveDirectory(string source)
        {
            removeDirectory(source);
        }
         
        #endregion 
       
        #region Private Nethods

        private void getDirectoryTotals(string source)
        {
            if (!Directory.Exists(source))
                throw new Exception(string.Format("Target Directory: {0} not found.", source));

            this.SourceFiles += Directory.GetFiles(source).Length;
            this.SourceFolders++;

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(source);

            foreach (string subdirectory in subdirectoryEntries)
                getDirectoryTotals(subdirectory);
        }

        private void getDirectoryTotals(string source, ref int directories, ref int files)
        {
            if (!Directory.Exists(source))
                throw new Exception(string.Format("Target Directory: {0} not found.", source));

            files += Directory.GetFiles(source).Length;
            directories++;
            this.SourceFiles = files;
            this.SourceFolders = directories;

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(source);

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
                        this.AffectedFiles++;
                    }

                    catch (Exception e)
                    {
                        if (e.Message.StartsWith("Access to the path") && e.Message.EndsWith("is denied."))
                            continue;
                        else
                            throw new Exception(e.Message);
                    } 
                }

            // Recurse into subdirectories of this directory.
            foreach (DirectoryInfo sdi in subdirectories)
            {
                copyDirectory(sdi.FullName, sdi.FullName.Replace(source, destination));
                this.affectedFolders++;
            }
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
                    this.AffectedFiles++;
                }

                catch (Exception e)
                {
                    if (e.Message.StartsWith("Access to the path") && e.Message.EndsWith("is denied."))
                        continue;
                    else
                        throw new Exception(e.Message);
                } 
            }

            // Recurse into subdirectories of this directory.
            foreach (DirectoryInfo sdi in subdirectories)
            {
                moveDirectory(sdi.FullName, sdi.FullName.Replace(source, destination));
                this.affectedFolders++;
            }

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

        private void removeDirectory(string source)
        {
            if (!Directory.Exists(source))
                throw new Exception(string.Format("Target Directory: {0} not found.", source));

            DirectoryInfo di = new DirectoryInfo(source);
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

                this.AffectedFiles++;
            }

            // Recurse into subdirectories of this directory.
            foreach (DirectoryInfo sdi in subdirectories)
                removeDirectory(sdi.FullName);

            if (!this.leaveDirStructure)
                try
                {
                    Directory.Delete(source);
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
