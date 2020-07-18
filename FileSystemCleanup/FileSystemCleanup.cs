using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Comdata.AppSupport.FileSystemCleanup
{
    class FileSystemCleanup
    {
        private object _lockObj = new object();
        private AppTools.ILog _log;
        private ISettings _settings;
        private IFileInfoList _fiList;

        public FileSystemCleanup(AppTools.ILog log, ISettings settings, IFileInfoList fiList)
        {
            // TODO: Complete member initialization
            this._log = log;
            this._settings = settings;
            this._fiList = fiList;
        }

        public void Execute()
        {
            foreach (var folder in _settings.FoldersToCleanup)
            {
                if (Directory.Exists(folder.Folder))
                {
                    _log.Write(Severity.Debug, "Cleaning up: {0}", folder.Folder);

                    if (folder.FoldersToCollapse.Count > 0)
                        DispatchCleanup(folder, operation.Comlapse, _settings.Threads);

                    if (folder.DeleteAfterDays >= 0)
                        DispatchCleanup(folder, operation.Delete, _settings.Threads);
                    deleteEmptyFolders(folder);

                    if (folder.ArchiveAfterDays >= 0)
                        DispatchCleanup(folder, operation.Archive, _settings.Threads);
                    deleteEmptyFolders(folder);

                    if (folder.CompressAfterDays >= 0)
                        DispatchCleanup(folder, operation.Compress, _settings.Threads);
                }
                else
                    _log.Write(Severity.Warning, "Folder: {0} doesn't exist.", folder.Folder);
            }
        }
    
        private void deleteEmptyFolders(FolderSetting folder)
        {
            var backupFolderNamePattern = @"\d{4}-?\d{2}-?\d{2}|\d{4}|\d{2}$";
            var otherFolders = Directory.GetDirectories(folder.Folder);

            foreach (var otherFolder in otherFolders)
            {
                if (Regex.IsMatch(otherFolder, backupFolderNamePattern))
                {
                    var di = new DirectoryInfo(otherFolder);

                    if ((di.GetFiles().Length == 0) && (di.GetDirectories().Length == 0))
                    {
                        _log.Write(Severity.Debug, "Removing empty directory: {0}.", otherFolder);
                        di.Delete();
                    }
                }
            }
        }

        private void DispatchCleanup(FolderSetting folder, operation operation, int threads)
        {
            var maxDate = DateTime.Today;
            var count = 0;
            var operationText = "";


            switch (operation)
            {
                case operation.Archive:
                    maxDate = DateTime.Today.AddDays(-1 * folder.ArchiveAfterDays);
                    operationText = "Archiving";
                    break;

                case operation.Compress:
                    maxDate = DateTime.Today.AddDays(-1 * folder.CompressAfterDays);
                    operationText = "Compressing";
                    break;

                case operation.Delete:
                    maxDate = DateTime.Today.AddDays(-1 * folder.DeleteAfterDays);
                    operationText = "Deleting";
                    break;
            }

            var threadLists = this._fiList.CreateLists(folder, out count, threads, maxDate);
            _log.Write("{0}: {1} {2:N0} files, last updated on or before {3:MM/dd/yyyy}."
                      , folder.Folder, operationText, count, maxDate);

            Parallel.ForEach(threadLists, (list) => Cleanup(folder, list, operation));
            threadLists.Clear();

            if (folder.DeleteEmptyFolders)
                deleteEmptyDirectory(folder.Folder, folder.IncludeSubfolders);
        }

        private object Cleanup(FolderSetting Folder, List<FileInfo> list, operation operation)
        {
            object ob = new object();

            try
            {
                switch (operation)
                {
                    case operation.Archive: Archive(Folder, list);
                        break;
                    case operation.Compress: Compress(Folder, list);
                        break;
                    case operation.Delete: Delete(Folder, list);
                        break;
                }
            }
            catch (Exception ex)
            {
                Utilities.ReportException(ex, _log);
            }

            return ob;
        }

        private void Archive(FolderSetting Folder, List<FileInfo> list)
        {
            var prevLastWritetime = DateTime.Today;
            var archivePath = "";

            foreach (var File in list)
            {
                if (File.LastWriteTime.ToShortDateString() != prevLastWritetime.ToShortDateString())
                {
                    archivePath = GetArchivePath(Folder, File.FullName);
                    _log.Write(Severity.Debug, "Created directory: {0}", archivePath);
                }

                _log.Write(Severity.Debug, "Moving {0} to {1}", File.FullName, archivePath);
                File.MoveTo(Path.Combine(archivePath, File.Name));
            }
        }

        private string GetArchivePath(FolderSetting Folder, string filename)
        {
            string archivePath = GetAbsoluteArchivePath(Folder, filename);
            var fi = new FileInfo(filename);
            var yyyy = fi.LastWriteTime.Year.ToString();
            var MM = fi.LastWriteTime.ToString("MM");
            var yyyyMMdd = fi.LastWriteTime.ToString("yyyy-MM-dd");
            archivePath = Path.Combine(archivePath, yyyy, MM, yyyyMMdd);

            lock (_lockObj)
            {
                if (!Directory.Exists(archivePath))
                {
                    Directory.CreateDirectory(archivePath);
                    Directory.SetCreationTime(archivePath, fi.LastWriteTime);
                    Directory.SetLastWriteTime(archivePath, fi.LastWriteTime);
                }
            }

            return archivePath;
        }

        private static string GetAbsoluteArchivePath(FolderSetting Folder, string filename)
        {
            var archivePath = "";

            if (Folder.ArchivePath.StartsWith(@".\"))
            {
                archivePath = Path.GetDirectoryName(filename);
                archivePath = Path.Combine(archivePath, Folder.ArchivePath.Substring(2));
            }
            else
            if (Folder.ArchivePath.StartsWith(@"..\"))
            {
                archivePath = Path.GetDirectoryName(filename);
                archivePath = Directory.GetParent(archivePath).FullName;
                archivePath = Path.Combine(archivePath, Folder.ArchivePath.Substring(3));
            }
            else
                archivePath = Folder.ArchivePath;

            return archivePath;
        }

        private void Compress(FolderSetting Folder, List<FileInfo> list)
        {
            throw new NotImplementedException();
        }

        private void Delete(FolderSetting Folder, List<FileInfo> list)
        {
            foreach (var File in list)
            {
                _log.Write(Severity.Debug, "Deleting {0}", File.FullName);
                File.Delete();
            }
        }

        private void deleteEmptyDirectory(string startLocation, bool includeSubdirectories)
        {
            if (includeSubdirectories)
                foreach (var directory in Directory.GetDirectories(startLocation))
                {
                    deleteEmptyDirectory(directory, includeSubdirectories);

                    if (Directory.GetFiles(directory).Length == 0 &&
                        Directory.GetDirectories(directory).Length == 0)
                        Directory.Delete(directory, false);
                }
        }     

        enum operation {
            [Description("Deleting")]
            Delete,
            [Description("Collapsing")]
            Comlapse,
            [Description("Compressing")]
            Compress,
            [Description("Archiving")]
            Archive
        };
    }
}
