using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.PPOLFileMover
{
    public interface ISettings
    {
        string LogPath { get; set; }
        Comdata.AppSupport.AppTools.Severity LoggingSeverityLevel { get; set; }
        List<FileToMove> FilesToMove { get; set; }
        bool Save(string filename);
     }
}
