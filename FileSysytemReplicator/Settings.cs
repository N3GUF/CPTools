using Comdata.AppSupport.AppTools;
using System.Collections.Generic;

namespace Comdata.AppSupport.FileSystemReplicater
{
    public partial class Settings : ISettings
    {
        public string LogPath { get; set; }
        public Severity LoggingSeverityLevel { get; set; }
        public List<Folder> FoldersToReplicate { get; set; }

        public Settings()
        {
            //create();
        }

        private void create()
        {
            this.LogPath = @"C:\Logs\FileSystemReplicater";
            this.LoggingSeverityLevel = Severity.Debug;
            this.FoldersToReplicate = new List<Folder>();
            List<string> Exclusions = new List<string>();
            Exclusions.Add(@"C:\Documents and Settings");
            this.FoldersToReplicate.Add(new Folder(@"C:\", @"D:\", false, Exclusions));
            this.Save(@".\FileSystemReplicaterConfig.xml");
        }
    }

    public class Folder
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public bool IncludeSubfolders { get; set; }
        public List<string> Exclusions { get; set; }

        public Folder()
        {
        }

        public Folder(string source, string destination, bool includeSubfolders, List<string> exclusions)
        {
            this.Source = source;
            this.Destination = destination;
            this.IncludeSubfolders = includeSubfolders;
            this.Exclusions = exclusions;
        }

    }
}
