using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Comdata.AppSupport.CDNFileSystemMonitor.Classes
{
    class DriveStats
    {
        #region Private Fields
        private long usedSpace       = 0;
        private double percecentFree = 0;
        private double percecentUsed = 0;
        private string uncPath;
        #endregion

        #region Properties
        public System.IO.DriveInfo Drive { set; get; }
        public string Name { get { return Drive.Name; } }
        public DriveType DriveType { get { return Drive.DriveType; } }
        public string VolumeLabel { get { return Drive.VolumeLabel; } }
        public string DriveFormat { get { return Drive.DriveFormat; } }
        public long UsedSpace { get { return usedSpace; } }
        public long AvailableFreeSpace { get { return Drive.AvailableFreeSpace; } }
        public long TotalSize { get { return Drive.TotalSize; } }
        public string UsedSpaceText { get { return formatDiskSpace(usedSpace); } }
        public string AvailableFreeSpaceText { get { return formatDiskSpace(Drive.AvailableFreeSpace); } }
        public string TotalSizeText { get { return formatDiskSpace(Drive.TotalSize); } }
        public double PercecentFree { get { return percecentFree; } }
        public double PercecentUsed { get { return percecentUsed; } }
        public string UncPath { get { return uncPath; } }
        #endregion

        #region Public Methods
        public DriveStats(DriveInfo driveInfo)
        {
            Drive         = driveInfo;
            usedSpace     = Drive.TotalSize - Drive.AvailableFreeSpace;
            percecentFree = Math.Round((Drive.AvailableFreeSpace * 1.0 / Drive.TotalSize * 1.0) * 100.0);
            percecentUsed = Math.Round((usedSpace * 1.0 / Drive.TotalSize * 1.0) * 100.0);
            uncPath = GetUNCPath(Drive.Name);

            if (uncPath == Drive.Name)
                uncPath = "";
        }
        #endregion

        #region Private Methods
        private string formatDiskSpace(long sizeInBytes)
        {
            double size = 0.0;
            string units = "b";
            string returnValue;

            for (int i = 5; i > 0; i--)
            {
                if (sizeInBytes > Math.Pow(1024, i))
                {
                    size = sizeInBytes / Math.Pow(1024, i);

                    switch (i)
                    {
                        case 1: units = "kb";
                            break;
                        case 2: units = "MB";
                            break;
                        case 3: units = "GB";
                            break;
                        case 4: units = "TB";
                            break;
                        case 5: units = "PB";
                            break;
                    }

                    break;
                }
            }

            returnValue = String.Format("{0:##0.00} {1}", size, units);
            return returnValue;
        }

         [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
            ref int length);
        /// <summary>
        /// Given a path, returns the UNC path or the original. (No exceptions
        /// are raised by this function directly). For example, "P:\2008-02-29"
        /// might return: "\\networkserver\Shares\Photos\2008-02-09"
        /// </summary>
        /// <param name="originalPath">The path to convert to a UNC Path</param>
        /// <returns>A UNC path. If a network drive letter is specified, the
        /// drive letter is converted to a UNC or network path. If the
        /// originalPath cannot be converted, it is returned unchanged.</returns>
        private static string GetUNCPath(string originalPath)
        {
            StringBuilder sb = new StringBuilder(512);
            int size = sb.Capacity;

            // look for the {LETTER}: combination ...
            if (originalPath.Length > 2 && originalPath[1] == ':')
            {
                // don't use char.IsLetter here - as that can be misleading
                // the only valid drive letters are a-z && A-Z.
                char c = originalPath[0];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    int error = WNetGetConnection(originalPath.Substring(0, 2), sb, ref size);

                    if (error == 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(originalPath);

                        string path = Path.GetFullPath(originalPath)
                            .Substring(Path.GetPathRoot(originalPath).Length);
                        return Path.Combine(sb.ToString().TrimEnd(), path);
                    }
                }
            }

            if (originalPath.Length == 2) 
            {
                // don't use char.IsLetter here - as that can be misleading
                // the only valid drive letters are a-z && A-Z.
                char c = originalPath[0];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    int error = WNetGetConnection(originalPath.Substring(0, 2), sb, ref size);
                    
                    if (error == 0)
                        return (sb.ToString());
                }
            }

            return originalPath;
        }
        #endregion
    }

    class  StorageMonitor
    {
        #region Private Fields
        private List<DriveStats> volumes = new List<DriveStats>();
        private string[] drivesToSkip;
        private string alert = null;
        private bool emailMessageRequired = false;
        #endregion

        #region Properties
        public int EmailTheshold { get; set; }
        public int CriticalTheshold { get; set; }

        public string DrivesNotToMonitor
        {
            get
            {
                return this.DrivesNotToMonitor;
            }

            set
            {
                this.drivesToSkip = value.Split(new char[] { ',' });

                for (int i = 0; i < this.drivesToSkip.Length; i++)
                    this.drivesToSkip[i] = this.drivesToSkip[i].Substring(0, 1).ToUpper();
            }
        }

        public List<DriveStats> Volumes { get { return volumes; } }
        public ILog Log { get; set; }
        public string Alert { get { return this.alert; } }
        public bool EmailMessageRequired { get { return emailMessageRequired; } }
        #endregion

        #region Public Methods
        public StorageMonitor()
        {
            this.EmailTheshold    = 75;
            this.CriticalTheshold = 85;
         }

        public void GetVolumeInfo()
        {
            this.volumes.Clear();
            this.alert                = null;
            this.emailMessageRequired = false;
            var maxUtilizationRate    = 0;
            DriveInfo[] allDrives     = DriveInfo.GetDrives();
            DriveStats ds;

            foreach (var d in allDrives)
            {
                if (!d.IsReady)
                {
                    Log.Write(Severity.Debug, "Drive {0} Is not ready.", d.Name);
                    continue;
                }

                if (this.drivesToSkip != null)
                    if (this.drivesToSkip.Contains(d.Name.Substring(0, 1).ToUpper()))
                    {
                        Log.Write(Severity.Debug, "Skipping drive {0}.", d.Name);
                        continue;
                    }

                ds = new DriveStats(d);
                volumes.Add(ds);

                if (ds.PercecentUsed >= this.EmailTheshold)
                {
                    Log.Write(Severity.Debug, "{0,-5} {1,-10} {2,-25} {3,15} {4,15} {5,15} {6,9}%"
                                            , ds.Drive.Name
                                            , ds.DriveType
                                            , ds.UncPath
                                            , ds.TotalSizeText
                                            , ds.UsedSpaceText
                                            , ds.AvailableFreeSpaceText
                                            , ds.PercecentFree);
                    Log.Write(Severity.Debug, "Email threshold exceeded.");
                    this.emailMessageRequired = true;
                }
                
                if (ds.PercecentUsed >= this.CriticalTheshold)
                {
                    Log.Write(Severity.Debug, "Critical threshold exceeded.");
                    if (ds.PercecentUsed > maxUtilizationRate)
                    {
                        this.alert = string.Format("IFP drive {0} {1} is {2}% utilized.", ds.Drive.Name, ds.UncPath, ds.PercecentUsed);
                        maxUtilizationRate = (int)ds.PercecentUsed;
                    }
                }
            }
        }
        
        public void ReportDiskUsage()
        {
            if (this.volumes.Count == 0)
                GetVolumeInfo();

            Log.Write("Creating daily utilization report:\n\r");
            Log.Write(Severity.Info, "{0,-5} {1,-10} {2,-25} {3,15} {4,15} {5,15} {6,10}"
                                    , "Drive"
                                    , "Type"
                                    , "UNC Share"
                                    , "Total Size"
                                    , "Used"
                                    , "Available"
                                    , "% Free");

            foreach (var ds in this.volumes)
                Log.Write(Severity.Info, "{0,-5} {1,-10} {2,-25} {3,15} {4,15} {5,15} {6,9}%"
                                        , ds.Drive.Name
                                        , ds.DriveType
                                        , ds.UncPath
                                        , ds.TotalSizeText
                                        , ds.UsedSpaceText
                                        , ds.AvailableFreeSpaceText
                                        , ds.PercecentFree);
        }

        public DataTable GetkDiskUsage()
        {
            System.Data.DataTable table = new DataTable();
            table.Columns.Add("Drive");
            table.Columns.Add("Type");
            table.Columns.Add("UNC Share");
            table.Columns.Add("Total Size");
            table.Columns.Add("Total Used");
            table.Columns.Add("Total Preespace");
            table.Columns.Add("Percent Utilized");
            GetVolumeInfo();

            foreach (var d in this.volumes)
                table.Rows.Add(d.Name
                    , d.DriveType
                    , d.UncPath
                    , d.TotalSizeText
                    , d.UsedSpaceText
                    , d.AvailableFreeSpaceText
                    , string.Format("{0}%", d.PercecentUsed));

            return table;
        }

        public string FormatEmail(DateTime currentExecutionTime, EmailType emailType)
        {
            DataTable utilizationTable = new DataTable();
            utilizationTable.Columns.Add("Drive");
            utilizationTable.Columns.Add("Type");
            utilizationTable.Columns.Add("UNC Share");
            utilizationTable.Columns.Add("Total Size");           
            utilizationTable.Columns.Add("Used Space");
            utilizationTable.Columns.Add("Free Space");
            utilizationTable.Columns.Add("% Free");

            if (emailType == EmailType.Monitor)
                Log.Write(Severity.Info, "{0,-5} {1,-10} {2,-25} {3,15} {4,15} {5,15} {6,10}"
                                        , "Drive"
                                        , "Type"
                                        , "UNC Share"
                                        , "Total Size"
                                        , "Used"
                                        , "Available"
                                        , "% Free");
    

            foreach (var vol in this.volumes)
            {
                if (emailType == EmailType.Monitor)
                    if (vol.PercecentUsed >= this.EmailTheshold)
                    {
                        utilizationTable.Rows.Add(vol.Drive
                                                , vol.DriveType
                                                , vol.UncPath
                                                , vol.TotalSizeText
                                                , vol.UsedSpaceText
                                                , vol.AvailableFreeSpaceText
                                                , string.Format("{0}%", vol.PercecentFree));
                        Log.Write(Severity.Info, "{0,-5} {1,-10} {2,-25} {3,15} {4,15} {5,15} {6,9}%"
                                                , vol.Drive
                                                , vol.DriveType
                                                , vol.UncPath
                                                , vol.TotalSizeText
                                                , vol.UsedSpaceText
                                                , vol.AvailableFreeSpaceText
                                                , vol.PercecentFree);
                    }

                if (emailType == EmailType.Report)
                    utilizationTable.Rows.Add(vol.Drive
                                            , vol.DriveType
                                            , vol.UncPath
                                            , vol.TotalSizeText
                                            , vol.UsedSpaceText
                                            , vol.AvailableFreeSpaceText
                                            , string.Format("{0}%", vol.PercecentFree));
            }
            StringBuilder emailBody = new StringBuilder();

            if (emailType == EmailType.Monitor)
            {
                emailBody.AppendLine("<div align='center'><b>IFP Storage Monitor<b> - Run Time: " + currentExecutionTime + " <br><br/><br/>");
                emailBody.AppendLine("<div align='Left'>The following volumes are low on storage: <br><br/>");
            }
            else
            {
                emailBody.AppendLine("<div align='center'><b>IFP Storage Report<b> - Run Time: " + currentExecutionTime + " <br><br/><br/>");
                emailBody.AppendLine("<div align='Left'>Current volume summary: <br><br/>");
            }

            emailBody.AppendLine("<div align='center'>"); 
       //     AppFramework.Web.WebTools webTool = new AppFramework.Web.WebTools();

            if (utilizationTable.Rows.Count > 0)
         //       emailBody.AppendLine(webTool.GenerateHTMLTable(utilizationTable, true, true, true));

            emailBody.AppendLine("</div>");
            return emailBody.ToString();
        }

        #endregion

        #region Private Methods

        #endregion
    }

    public enum EmailType {Monitor, Report};
}
