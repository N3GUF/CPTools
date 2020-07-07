using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Comdata.AppSupport.FileSystemCleanup
{
    class FileInfoList : IFileInfoList
    {
        ILog _log;

        public FileInfoList(ILog log)
        {
            this._log = log;
        }

        public List<List<FileInfo>> CreateLists(FolderSetting folder, out int fileCount, int threads, DateTime maxDate)
        {
            var di = new DirectoryInfo(folder.Folder);
            IEnumerable<FileInfo> fileList;

            if (folder.IncludeSubfolders)
                fileList = from f in di.GetFiles("*.*", SearchOption.AllDirectories)
                           where f.LastWriteTime.Date <= maxDate
                           orderby f.LastWriteTime.Date
                           select f;
            else
                fileList = from f in di.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                           where f.LastWriteTime.Date <= maxDate
                           orderby f.LastWriteTime.Date
                           select f;

            var files = fileList.ToList();
            _log.Write(Severity.Debug, "Found {0:N0} files created before {1:MM/dd/yyyy}", files.Count, maxDate);

            foreach (var file in fileList.ToList())
                if (isExcluded(folder, file))
                    files.Remove(file);

            fileCount = files.Count();
            var threadLists = split(files, threads);
            files.Clear();
            return threadLists;
        }

        private bool isExcluded(FolderSetting Folder, FileInfo File)
        {
            var excluded = false;

            foreach (var pattern in Folder.Exclusions)
                if (excluded = Regex.IsMatch(File.FullName, pattern, RegexOptions.IgnoreCase))
                {
                    _log.Write("{0} will be excluded.", File.FullName);
                    break;
                }

            return excluded;
        }

        private List<List<FileInfo>> split(IEnumerable<FileInfo> list, int threads)
        {
            var threadLists = new List<List<FileInfo>>();

            for (int i = 0; i < threads; i++)
                threadLists.Add(new List<FileInfo>());

            var thread = 0;

            foreach (var file in list)
            {
                threadLists[thread].Add(file);

                if (++thread > threads - 1)
                    thread = 0;
            }

            return threadLists;
        }
    }
}
