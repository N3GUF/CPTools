using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.FileSystemCleanup
{
    interface ISettings
    {
        string LogPath { get; set; }
        Comdata.AppSupport.AppTools.Severity LoggingSeverityLevel { get; set; }
        int Threads { get; set; }
        List<FolderSetting> FoldersToCleanup { get; set; }

        FileSystemCleanupSettings Reload(string filename);
        bool Save(string filename);

        void Create();
    }
}
