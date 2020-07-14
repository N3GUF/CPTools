using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Comdata.AppSupport.FileSystemCleanup
{
    public class FolderSetting
    {
        public string Folder { get; set; }
        public string ArchivePath { get; set; }
        public bool IncludeSubfolders { get; set; }
        public int ArchiveAfterDays { get; set; }
        public int CompressAfterDays { get; set; }
        public int DeleteAfterDays { get; set; }
        public bool DeleteEmptyFolders { get; set; }
        public List<string> Exclusions { get; set; }
        public List<string> FoldersToCollapse { get; set; }

        public DateTime parse(string time)
        {
            DateTime dt = DateTime.Today; ;
            var hour = 0;
            var minutes = 0;

            if (time.Length != 5)
                return dt;

            if (!int.TryParse(time.Substring(0, 2), out hour))
                return dt;

            if (!int.TryParse(time.Substring(3, 2), out minutes))
                return dt;

            if (hour >= 0 && hour <= 23)
                if (minutes >= 0 && minutes <= 59)
                    dt = dt.Add(new TimeSpan(hour, minutes, 0));

            return dt;
        }
        public FolderSetting() { }
        public FolderSetting(string folder, string archivePath, bool includeSubfolders, int archiveAfterDays, int compressAfterDays
                             , int deleteAfterDays, bool deleteEmptyFolders, List<string> exclusions, List<string> foldersToCallapse)
        {
            this.Folder = folder;
            this.ArchivePath = archivePath;
            this.IncludeSubfolders = includeSubfolders;
            this.ArchiveAfterDays = archiveAfterDays;
            this.CompressAfterDays = compressAfterDays;
            this.DeleteAfterDays = deleteAfterDays;
            this.DeleteEmptyFolders = deleteEmptyFolders;
            this.Exclusions = exclusions;
            this.FoldersToCollapse = foldersToCallapse;
        }
    }

    public class FileSystemCleanupSettings : ISettings
    {
        public string LogPath { get; set; }
        public Severity LoggingSeverityLevel { get; set; }
        public int Threads { get; set; }
        public List<FolderSetting> FoldersToCleanup { get; set; }

        public void Create()
        {
            this.LogPath = @"C:\Logs\FileSystemCleanup";
            this.LoggingSeverityLevel = Severity.Debug;
            this.Threads = 4;
            this.FoldersToCleanup=new List<FolderSetting>();
            List<string> exclusions = new List<string>();
            exclusions.Add("test");
            List<string> foldersToCollapse = new List<string>();
            foldersToCollapse.Add("test");
            this.FoldersToCleanup.Add(new FolderSetting(@"C:\Users\dbernhardy\Downloads", @".\Archive", false, 1, 30, 24, true, exclusions, foldersToCollapse));
            this.Save(@".\FileSystemCleanupConfig.xml");
        }

        public FileSystemCleanupSettings()
        {
        }

        public FileSystemCleanupSettings(string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(FileSystemCleanupSettings));
                TextReader tr = new StreamReader(filename);
                var tmp = (FileSystemCleanupSettings)serializer.Deserialize(tr);
                tr.Close();

                foreach (var property in GetType().GetProperties())
                    if (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).GetLength(0) == 0)
                        property.SetValue(this, property.GetValue(tmp, null), null);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error loading FileSystemCleanup settings from {0}.", filename), ex);
            }

        }
        public FileSystemCleanupSettings Reload(string filename)
        {
            FileSystemCleanupSettings returnValue = new FileSystemCleanupSettings();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(FileSystemCleanupSettings));
                TextReader tr = new StreamReader(filename);
                returnValue = (FileSystemCleanupSettings)serializer.Deserialize(tr);
                tr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error loading FileSystemCleanup settings from {0}.", filename), ex);
            }

            return (returnValue);
        }

        public bool Save(string filename)
        {
            bool returnValue = false;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(FileSystemCleanupSettings));
                TextWriter tw = new StreamWriter(filename);
                serializer.Serialize(tw, this);
                tw.Close();
                returnValue = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving FileSystemCleanup settings.", ex);
            }

            return (returnValue);
        }
    }
}
