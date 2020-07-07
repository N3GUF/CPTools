using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Comdata.AppSupport.FileSystemReplicater
{
    class FileSystemReplicater
    {
        private object _lockObj = new object(); 
        private AppTools.ILog _log;
        private ISettings _settings;
        private int _threads = 1;
        private int _hoursOld = 0;
        private DateTime _maxDate;

        public FileSystemReplicater(ILog log, ISettings settings)
        {
            // TODO: Complete member initialization
            this._log = log;
            this._settings = settings;
         }

        public void Replicate(int threads, int hoursOld)
        {
            _threads = threads;
            _hoursOld = hoursOld;
            _maxDate = DateTime.Now.AddHours(-1 * this._hoursOld);
            _log.Write("Replicating files created on or before {0:MM/dd/yyyy HH:mm}.", _maxDate);

            foreach (var folder in _settings.FoldersToReplicate)
            {
                if (Directory.Exists(Path.GetDirectoryName(folder.Source)))
                {
                    _log.Write("Replicating: {0} to {1}.", Path.GetDirectoryName(folder.Source), folder.Destination);
                    replicateDirectory(folder, folder.Source);
                }
                else
                    _log.Write(Severity.Warning, "Folder: {0} doesn't exist.", Path.GetDirectoryName(folder.Source));
            }
        }

        private void replicateDirectory(Folder folder, string source)
        {
            addExclusions(ref folder, source);
            List<FileInfo> fileList = getFiles(folder, source);

            if (fileList.Count() > 0)
                replicateFiles(folder, source, fileList);

            if (folder.IncludeSubfolders)
            {
                var list = getDirectories(Path.GetDirectoryName(source));

                foreach (var directory in list)
                {
                    if (IsExcluded(folder, directory.FullName + @"\"))
                        continue;

                    replicateDirectory(folder, directory.FullName + @"\");
                }
            }
        }

        private void addExclusions(ref Folder folder, string source)
        {
            foreach(var exclusion in folder.Exclusions.ToList())
                if (exclusion.Contains("*"))
                {
                    var count = epandWildcard(ref folder, Path.GetDirectoryName(source), Path.GetFileName(exclusion));

                    if (count > 0)
                        _log.Write("Excluding: {0:N0} files like: {1}." , count, exclusion);
                }
                else
                    _log.Write("Excluding: {0} will be excluded.", exclusion);
        }

        private int epandWildcard(ref Folder folder, string path, string fileMask)
        {
            if (fileMask == "" || fileMask == "*")
                fileMask = "*.*";

            IEnumerable<FileInfo> list;
            var di = new DirectoryInfo(path);

            if (_hoursOld == 0)
                list = from f in di.GetFiles(fileMask, SearchOption.TopDirectoryOnly)
                       orderby f.LastWriteTime.Date descending
                       select f;
            else
                list = from f in di.GetFiles(fileMask, SearchOption.TopDirectoryOnly)
                       where f.LastWriteTime.Date <= _maxDate
                       orderby f.LastWriteTime.Date descending
                       select f;

            var exclusionsAdded = 0;

            foreach (var file in list)
            {
                folder.Exclusions.Add(file.FullName);
                exclusionsAdded++;
            }

            return exclusionsAdded;
        }

        private List<FileInfo> getFiles(Folder folder, string source)
        {
            var fileMask = Path.GetFileName(source);

            if (fileMask == "")
                fileMask = "*.*";

            IEnumerable<FileInfo> list;
            var di = new DirectoryInfo(Path.GetDirectoryName(source));

            if (_hoursOld == 0)
                list = from f in di.GetFiles(fileMask, SearchOption.TopDirectoryOnly)
                           orderby f.LastWriteTime.Date descending
                           select f;
            else
                list = from f in di.GetFiles(fileMask, SearchOption.TopDirectoryOnly)
                           where f.LastWriteTime.Date <= _maxDate
                           orderby f.LastWriteTime.Date descending
                           select f;

            var files = list.ToList();

            foreach (var file in files.ToList())
            {
                if (IsExcluded(folder, file.FullName))
                    files.Remove(file);

                var dest = file.FullName.Replace(Path.GetDirectoryName(folder.Source), folder.Destination);

                if (File.Exists(dest))
                    if (File.GetLastWriteTime(dest) >= file.LastWriteTime)
                        files.Remove(file);
            }

            return files;
        }

        private List<DirectoryInfo> getDirectories(string source)
        {
            var di = new DirectoryInfo(source);
            IEnumerable<DirectoryInfo> directoryList;

            directoryList = from d in di.GetDirectories("*.*", SearchOption.TopDirectoryOnly)
                         orderby d.LastWriteTime descending
                          select d;

            return directoryList.ToList();
        }

        private void replicateFiles(Folder folder, string source, List<FileInfo> fileList)
        {
            if (_hoursOld == 0)
                _log.Write("Replicating {0:N0} files from {1}."
                      , fileList.Count()
                      , source);
            else
                _log.Write("Replicating {0:N0} files from {1} created on or before {2:MM/dd/yyyy HH:mm}."
                      , fileList.Count()
                      , source
                      , _maxDate);

            var path = source.Replace(Path.GetDirectoryName(folder.Source), folder.Destination);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var threadLists = split(fileList, _threads);
            Parallel.ForEach(threadLists, (list) => replicate(folder, list));
            threadLists.Clear();
        }

        private bool IsExcluded(Folder folder, string name)
        {
            foreach (var exclusion in folder.Exclusions)
            {
                if (exclusion.Contains("*"))
                    continue;

                if (name.ToLower().StartsWith(exclusion.ToLower()))
                    return true;
            }

            return false;
        }

        private List<List<FileInfo>> split(List<FileInfo> list, int threads)
        {
            List<FileInfo>[] threadLists = new List<FileInfo>[threads]; 

            for (int i=0; i < threads; i++)
                threadLists[i] = new List<FileInfo>();

            var thread = 0;

            foreach (var file in list.ToList())
            { 
                threadLists[thread].Add(file);
                list.Remove(file);
 
                if (++thread > threads - 1)
                    thread = 0;
            }

            return threadLists.ToList();
        }

        private void replicate(Folder folder, List<FileInfo> list)
        {
            foreach (var file in list.ToList())
            {
                var dest = file.FullName.Replace(Path.GetDirectoryName(folder.Source), folder.Destination);
                file.CopyTo(dest, true);
                list.Remove(file);
            }

            list.Clear();
            _log.Write(Severity.Debug, "Running Garbage Collection...");
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
