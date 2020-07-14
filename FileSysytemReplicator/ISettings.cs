using Comdata.AppSupport.AppTools;
using System.Collections.Generic;

namespace Comdata.AppSupport.FileSystemReplicater
{
    public interface ISettings
    {
        string LogPath { get; set; }
        Severity LoggingSeverityLevel { get; set; }
        List<Folder> FoldersToReplicate { get; set; }
    }
}
