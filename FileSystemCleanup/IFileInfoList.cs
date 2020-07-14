using System;
using System.Collections.Generic;
using System.IO;

namespace Comdata.AppSupport.FileSystemCleanup
{
    interface IFileInfoList
    {
        List<List<FileInfo>> CreateLists(FolderSetting folder, out int fileCount, int threads, DateTime maxDate);
    }
}