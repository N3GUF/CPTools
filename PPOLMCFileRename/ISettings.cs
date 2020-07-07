using System.Collections.Generic;

namespace Comdata.AppSupport.PPOLMCFileRename
{
    public interface ISettings
    {
        string LogPath { get; set; }
        AppTools.Severity LoggingSeverityLevel { get; set; }
        List<DirectoryPair> DirectoryPairList { get; set; }
    }
}
