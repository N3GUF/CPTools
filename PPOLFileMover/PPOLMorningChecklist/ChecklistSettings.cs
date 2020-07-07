using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using Comdata.AppSupport.AppTools;


namespace Comdata.AppSupport.PPOLMorningChecklist
{
    public class PathnameAndSchedule                                                                                                                                                                                                                             
    {
        public string Pathname { get; set; }
        public int ChecklistStep { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int IntervalMins { get; set; }

        public static PathnameAndSchedule Init(string pathname, int checklistStep)
        {
            PathnameAndSchedule p = PathnameAndSchedule.Init(pathname, checklistStep, "", "", 0);
            return p;
        }

        public static PathnameAndSchedule Init(string pathname, int checklistStep, string startTime, string endTime, int intervalMins)
        {
            PathnameAndSchedule p = new PathnameAndSchedule();
            p.Pathname = pathname;
            p.ChecklistStep = checklistStep;
            p.StartTime = p.parse(startTime);
            p.EndTime = p.parse(endTime);
            p.IntervalMins = intervalMins;
            return p;
        }

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
    }

    public class CleanupInProcessFolderSettings
    {
        public string InProcessDir { get; set; }
        public string ProcessedDir { get; set; }
        public string ProcessedBackupDir { get; set; }
        public int CheckFilesOlderThanHours { get; set; }
        public string ProcessedInvoiceDir { get; set; }
        public string DateFormat { get; set; }
        public string BackupDateFormat { get; set; }
    }

    public class CheckDailyInvoicesSettings
    {
        public string DateFormat { get; set; }
        public int PriorDaysToCheck { get; set; }
        public int MinimumInvoices { get; set; }
    }

    public class ChecklistSettings :ISettings
    {
        public string LogPath { get; set; }
        public Severity LoggingSeverityLevel { get; set; }
        public List<PathnameAndSchedule> FilesToCheck { get; set; }
        public List<PathnameAndSchedule> FoldersToCheck { get; set; }
        public CleanupInProcessFolderSettings CleanupInProcessFolder { set; get; }
        public CheckDailyInvoicesSettings CheckDailyInvoices { set; get; }

        //public ChecklistSettings()
        //{
        //    this.LogPath = "C:\\Logs\\PPOLMorningCheclist";
        //    this.LoggingSeverityLevel = Severity.Debug;
        //    this.FilesToCheck = new List<PathnameAndSchedule>();
        //    this.FilesToCheck.Add(PathnameAndSchedule.Init(@"\\cdnbwdata1\PLSDBATCH\CorpBalances", 1));
        //    this.FilesToCheck.Add(PathnameAndSchedule.Init(@"c:\conf\log\CorpBalLog.txt", 1));
        //    this.FilesToCheck.Add(PathnameAndSchedule.Init(@"c:\conf\log\invoice2Log.txt", 1));
        //    this.FilesToCheck.Add(PathnameAndSchedule.Init(@"c:\conf\log\invoiceLog.txt", 1));
        //    this.FilesToCheck.Add(PathnameAndSchedule.Init(@"c:\conf\log\reportLog.txt", 1));
        //    this.FoldersToCheck = new List<PathnameAndSchedule>();
        //    this.FoldersToCheck.Add(PathnameAndSchedule.Init(@"w:\incomming\payroll", 5, "06:00", "18:00", 15));
        //    this.CleanupInProcessFolder = new CleanupInProcessFolderSettings();
        //    this.CleanupInProcessFolder.InProcessDir = "C:\\inprocess";
        //    this.CleanupInProcessFolder.ProcessedDir = "C:\\processed";
        //    this.CleanupInProcessFolder.ProcessedBackupDir = "C:\\inprocess\\backup";
        //    this.CleanupInProcessFolder.CheckFilesOlderThanHours = 1;
        //    this.CleanupInProcessFolder.ProcessedInvoiceDir = "C:\\processed\invoice";
        //    this.CleanupInProcessFolder.DateFormat = "YYYYMMdd";
        //    this.CheckDailyInvoices = new CheckDailyInvoicesSettings();
        //    this.CheckDailyInvoices.DateFormat = "YYYYMMdd";
        //    this.CheckDailyInvoices.MinimumInvoices = 500;
        //    this.CheckDailyInvoices.PriorDaysToCheck = 5;
        //    this.Save("config.xml");
        //}

        public ChecklistSettings()
        {
        }

        public ChecklistSettings(string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ChecklistSettings));
                TextReader tr = new StreamReader(filename);
                var tmp = (ChecklistSettings)serializer.Deserialize(tr);
                tr.Close();

                foreach (var property in GetType().GetProperties())
                    if (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).GetLength(0) == 0)
                        property.SetValue(this, property.GetValue(tmp, null), null);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error loading checklist settings from {0}.", filename), ex);
            }

        }

        public ChecklistSettings Reload(string filename)
        {
            ChecklistSettings returnValue = new ChecklistSettings();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ChecklistSettings));
                TextReader tr = new StreamReader(filename);
                returnValue = (ChecklistSettings)serializer.Deserialize(tr);
                tr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error loading checklist settings from {0}.", filename), ex);
            }

            return (returnValue);
        }

        public bool Save(string filename)
        {
            bool returnValue = false;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ChecklistSettings));
                TextWriter tw = new StreamWriter(filename);
                serializer.Serialize(tw, this);
                tw.Close();
                returnValue = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving checklist settings.", ex);
            }

            return (returnValue);
        }

        public List<PathnameAndSchedule> GetPathnamesAndSchedules(int step, List<PathnameAndSchedule> list)
        {
            var returnedList = new List<PathnameAndSchedule>();
            var test = list.FindAll(x => x.ChecklistStep == step);
            Console.WriteLine(test.ToString());
            return returnedList;
        }
    } 
}
 
