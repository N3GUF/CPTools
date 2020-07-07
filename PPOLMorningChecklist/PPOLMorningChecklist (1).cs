using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comdata.AppSupport.PPOLMorningChecklist
{
    class PPOLMorningChecklist
    {
        public ILog Log { get; set; }
        public ISettings Settings { get; set; }
        public IFileSystem FileSystem { get; set; }

        public PPOLMorningChecklist (ILog log, ISettings settings, IFileSystem fileSystem)
        {
            this.Log = log;
            this.Settings = settings;
            this.FileSystem = fileSystem;
         }

        public void Execute()
        {
            var problemsFound = 0;

            problemsFound += checkCorpBalRptAndLogs();              // Step  1: Check for Current Corp Balance Report and other Daily Logs
            problemsFound += checkForFilesInHold();                 // Step  2: Check for Files in Hold
            problemsFound += checkIncommingCarholder();             // Step  3: Check Incomming Cardholder
            problemsFound += checkIncommingPayroll();               // Step  4: Check Incomming Payroll
            problemsFound += cleanupInprocess();                    // Step  5: Check files in InProcess folder
            problemsFound += checkProcessed();                      // Step  7: Check files in Processed folder
            problemsFound += checkIpmFiles();                       // Step  8: Check for IPM files in \OWS_WORK\Data\Interchange\IPM_Inc
            problemsFound += checkMdsFiles();                       // Step  9: Check for MDS files in \OWS_WORK\Data\Interchange\MDS_Inc
            problemsFound += checkForDailyInvoices();               // Step 10: Check for invoices in \Outbound\Invoices & Invoices_xl
            problemsFound += checkVariousFolders();                 // Step 11: Check Various folders for files left behind

            Process.Start(this.Log.LogPathname);                    // Display the log
        }

        /// <summary>
        /// This method checks various logs to ensure that they were created as expected and they don't have a zero length
        /// </summary>
        /// <returns>The number of problems found</returns>
        private int checkCorpBalRptAndLogs()
        {
            var problemsFound = 0;
            var files = Settings.FilesToCheck.FindAll(x => x.ChecklistStep == 1);
            var folders = Settings.FoldersToCheck.FindAll(x => x.ChecklistStep == 1);

            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 1: Checking for Daily Files");

            foreach (PathnameAndSchedule file in files)
            {
                if (File.Exists(file.Pathname))
                    if (File.GetLastWriteTime(file.Pathname).ToShortDateString() == DateTime.Now.ToShortDateString())
                    {
                        this.Log.Write(Severity.Debug, "{0} was created today at: {1}."
                                                     , file.Pathname
                                                     , File.GetLastWriteTime(file.Pathname));

                        var di = new DirectoryInfo(Path.GetDirectoryName(file.Pathname));
                        var fi = di.GetFiles().First(x => x.Name.ToLower() == Path.GetFileName(file.Pathname).ToLower());

                        if (fi.Length == 0)
                        {
                            this.Log.Write(Severity.Warning, "*** PROBLEM: {0} has a 0 byte length. ***"
                                                         , file.Pathname);
                            problemsFound++;
                        }
                        
                        //if (File.ReadAllText(file.Pathname).StartsWith("0 File(s) copied"))
                        //{
                        //    this.Log.Write(Severity.Info, "*** PROBLEM: {0} has 0 File(s) copied. ***"
                        //                                 , file.Pathname);
                        //    problemsFound++;
                        //}

                     }
                    else
                    {
                        this.Log.Write(Severity.Warning, "*** PROBLEM: {0} was created at: {1}. ***"
                                                     , file.Pathname
                                                     , File.GetLastWriteTime(file.Pathname));
                        problemsFound++;
                    }
                else
                { 
                    this.Log.Write(Severity.Warning, "*** PROBLEM: {0} was not created. ***"
                                             , file.Pathname
                                             , File.GetLastWriteTime(file.Pathname));
                    problemsFound++;
                }
            }

            if (problemsFound == 0)
                this.Log.Write("*** Step 1 Complete.  No problems found. ***");

            return problemsFound;
        }
        
        /// <summary>
        /// This method checks for old files in the hold directory. Zero dollar files will automatically be backed up and removed.
        /// </summary>
        /// <returns>>The number of problems found</returns>
        private int checkForFilesInHold()
        {
            var problemsFound = 0;
            var files = Settings.FilesToCheck.FindAll(x => x.ChecklistStep == 2);
            var folders = Settings.FoldersToCheck.FindAll(x => x.ChecklistStep == 2);

            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 2: Checking for Files in Hold");

            problemsFound = checkFolderForOldFiles(problemsFound, folders);

            if (problemsFound == 0)
                this.Log.Write("*** Step 2 Complete.  No problems found. ***");

            return problemsFound;
        }

        /// <summary>
        /// This method checks for old files in the incoming cardholder directory.
        /// </summary>
        /// <returns>The number of problems found</returns>
        private int checkIncommingCarholder()
        {
            var problemsFound = 0;
            var files = Settings.FilesToCheck.FindAll(x => x.ChecklistStep == 3);
            var folders = Settings.FoldersToCheck.FindAll(x => x.ChecklistStep == 3);

            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 3: Checking for Files in Incoming Cardholder");

            problemsFound = checkFolderForOldFiles(problemsFound, folders);

            if (problemsFound == 0)
                this.Log.Write("*** Step 3 Complete.  No problems found. ***");

            return problemsFound;
        }

        /// <summary>
        /// This method checks for old files in the incoming payroll directory.
        /// </summary>
        /// <returns>The number of problems found</returns>
        private int checkIncommingPayroll()
        {
            var problemsFound = 0;
            var files = Settings.FilesToCheck.FindAll(x => x.ChecklistStep == 4);
            var folders = Settings.FoldersToCheck.FindAll(x => x.ChecklistStep == 4);

            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 4: Checking for Files in Incoming Payroll");

            problemsFound = checkFolderForOldFiles(problemsFound, folders);

            if (problemsFound == 0)
                this.Log.Write("*** Step 4 Complete.  No problems found. ***");

            return problemsFound;
        }

        /// <summary>
        /// This method will check for files remaining InProcess directory older than one hour. It will back up any files found.
        /// For each file found, this method will check to see if the same file exists in the processed directory. If it does it 
        /// will be deleted from the Inprocess directory. If it finds that the file has not been moved to processed, 
        /// a message will be logged to go determine why the file has not been moved.
        /// XML files are considered to be invoices and they are moved to\processed\invoice\mmddyyyy.
        /// </summary>
        /// <returns>The number of problems found</returns>
        private int cleanupInprocess()
        {
            var inProcessDir = this.Settings.CleanupInProcessFolder.InProcessDir;
            var processedDir = this.Settings.CleanupInProcessFolder.ProcessedDir;
            var processedInvoiceDir = this.Settings.CleanupInProcessFolder.ProcessedInvoiceDir;
            var backupDir = this.Settings.CleanupInProcessFolder.ProcessedBackupDir;
            var hourOfset = this.Settings.CleanupInProcessFolder.CheckFilesOlderThanHours;
            var dateFormat = this.Settings.CleanupInProcessFolder.DateFormat;
            var backupDateFormat = this.Settings.CleanupInProcessFolder.BackupDateFormat;

            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 5: Cleaning up: {0}", inProcessDir);
            this.Log.Write(Severity.Debug, "In process Directory: {0}", inProcessDir);
            this.Log.Write(Severity.Debug, "Backup Directory:     {0}", backupDir);
            this.Log.Write(Severity.Debug, "Processed Directory:  {0}", processedDir);
            this.Log.Write(Severity.Info, "Checking for files older than {0} hour(s).", hourOfset);

            var backupDate = DateTime.Now.ToString(backupDateFormat);
            var filesFound = 0;
            var filesNotMoved = 0;
            var filesDeleted = 0;
            var dest = "";
            var filename = "";
           
            if (!Directory.Exists(inProcessDir))
                throw new DirectoryNotFoundException(inProcessDir + " doesn't exist.");

            if (!Directory.Exists(processedDir))
                throw new DirectoryNotFoundException(processedDir + " doesn't exist.");

            if (!Directory.Exists(processedInvoiceDir))
                throw new DirectoryNotFoundException(processedInvoiceDir + " doesn't exist.");

            if (!Directory.Exists(backupDir))
                throw new DirectoryNotFoundException(backupDir + " doesn't exist.");

            foreach (var file in Directory.GetFiles(inProcessDir))
            {
                FileInfo info = new FileInfo(file);

                if (info.LastWriteTime > DateTime.Now.AddHours(-1 * hourOfset))      // Only check files older than 1 hour
                    continue;

                filesFound++;
                filename = Path.GetFileName(file);
                dest = Path.Combine(backupDir, filename);

                if (File.Exists(dest))                                              // Backup File
                    dest = dest + backupDate;  
  
                this.Log.Write(Severity.Debug, "Backing up {0} to {1}.", file, dest);
                FileSystem.Copy(file, dest);

                if (filename.EndsWith(".xml"))
                {
                    dest = Path.Combine(processedInvoiceDir, info.LastWriteTime.AddDays(-1).ToString(dateFormat), filename);

                    if (!Directory.Exists(Path.GetDirectoryName(dest)))
                    {
                        this.Log.Write(Severity.Debug, "Creating directory: {0}", Path.GetDirectoryName(dest));
                        Directory.CreateDirectory(Path.GetDirectoryName(dest));
                    }
                }
                else
                    dest = Path.Combine(processedDir, filename);

                if (File.Exists(dest))                                              // Move or Delete File
                {
                    this.Log.Write(Severity.Debug, "{0} already exists.", dest);
                    this.Log.Write(Severity.Debug, "Deleting: {0}.", file);

                    if (filename.EndsWith(".xml"))
                        this.Log.Write(Severity.Warning, "Check into why {0}, created on {1:MM/dd/yyyy} at {1:HH:mm} didn't process normally."
                                                    , file, info.LastWriteTime);

                    FileSystem.Delete(file);
                    filesDeleted++;
                }
                else
                {
                    this.Log.Write(Severity.Debug, "Moving {0} to {1}.", file, dest);
                    FileSystem.Move(file, dest);
                    this.Log.Write(Severity.Warning, "Check into why {0}, created on {1:MM/dd/yyyy} at {1:HH:mm} didn't process normally."
                                                , file, info.LastWriteTime);
                    filesNotMoved++;
                }
            }

            this.Log.Write(Severity.Debug,"");
            this.Log.Write(Severity.Debug, "Files older than {0} hour(s) found: {1}", hourOfset, filesFound);
            this.Log.Write(Severity.Debug, "Files older than {0} hour(s) deleted: {1}", hourOfset, filesDeleted);

            if (filesNotMoved > 0)
                this.Log.Write(Severity.Warning, "*** PROBLEM: Found {0} file(s) older than {1} hour(s) that weren't moved to {2} from {3}. ***"
                              , filesNotMoved, hourOfset, processedDir, inProcessDir);
            else
                this.Log.Write("*** Step 5 Complete.  No problems found. ***");

            return filesNotMoved;
        }
 
        /// <summary>
        /// This method checks for various files to be created each day in the process directory.
        /// </summary>
        /// <returns>The number of problems found</returns>
        private int checkProcessed()
        {
            var problemsFound = 0;
            var files = Settings.FilesToCheck.FindAll(x => x.ChecklistStep == 7);
            var folders = Settings.FoldersToCheck.FindAll(x => x.ChecklistStep == 7);

            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 7: Checking for Current files in processed");

            foreach (PathnameAndSchedule file in files)
            {
                var path = Path.GetDirectoryName(file.Pathname);
                var pattern = Path.GetFileName(file.Pathname);
                this.Log.Write(Severity.Debug, "Searching for {0} in {1}.", pattern, path);
                var di = new DirectoryInfo(path);
                var list = from f in di.GetFiles(pattern, SearchOption.TopDirectoryOnly)
                             where f.LastWriteTime.Date == DateTime.Now.Date
                             select f;

                if (list.Count() == 0)
                {
                    this.Log.Write(Severity.Warning, "*** PROBLEM: Unable to find a {0} created today.", pattern);
                    problemsFound++;
                }
                else
                    this.Log.Write(Severity.Debug, "Found {0} created at {1}", list.First().Name, list.First().LastWriteTime);
            }
            
            if (problemsFound == 0)
                this.Log.Write("*** Step 7 Complete.  No problems found. ***");

            return problemsFound;
        }

        /// <summary>
        /// This method checks for IPM files to be processed successfully for each of the last few days.
        /// </summary>
        /// <returns>The number of problems found</returns>
        private int checkIpmFiles()
        {
            var problemsFound = 0;
            var files = Settings.FilesToCheck.FindAll(x => x.ChecklistStep == 8);
            var folders = Settings.FoldersToCheck.FindAll(x => x.ChecklistStep == 8);

            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 8: Checking for Current IPM files");

            problemsFound = checkIpmAndMdsFiles(problemsFound, files);

            if (problemsFound == 0)
                this.Log.Write("*** Step 8 Complete.  No problems found. ***");

            return problemsFound;
        }

        /// <summary>
        /// This method checks for MDS files to be processed successfully for each of the last few days.
        /// </summary>
        /// <returns>The number of problems found</returns>
        private int checkMdsFiles()
        {
            var problemsFound = 0;
            var files = Settings.FilesToCheck.FindAll(x => x.ChecklistStep == 9);
            var folders = Settings.FoldersToCheck.FindAll(x => x.ChecklistStep == 9);

            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 9: Checking for Current MDS files");

            problemsFound = checkIpmAndMdsFiles(problemsFound, files);

            if (problemsFound == 0)
                this.Log.Write("*** Step 9 Complete.  No problems found. ***");

            return problemsFound;
        }

        private int checkIpmAndMdsFiles(int problemsFound, List<PathnameAndSchedule> files)
        {
            foreach (PathnameAndSchedule file in files)
            {
                var path = Path.GetDirectoryName(file.Pathname);
                var pattern = Path.GetFileName(file.Pathname);
                var minDate = DateTime.Now.Date.AddDays(-7);
                this.Log.Write(Severity.Debug, "Searching for {0} in {1} created after {2}.", pattern, path, minDate);

                var di = new DirectoryInfo(path);
                var list = from f in di.GetFiles(pattern, SearchOption.TopDirectoryOnly)
                           where f.LastWriteTime.Date >= minDate
                           select f;

                if (list.Count() > 0)
                {
                    foreach (FileInfo fi in list)
                    {
                        this.Log.Write(Severity.Warning, "*** PROBLEM: Found {0} created on {1}.", fi.Name, fi.LastWriteTime);
                        problemsFound++;
                    }
                }

                pattern = "X" + pattern.Substring(1);
                var date = DateTime.Now.Date.AddDays(-1);

                while (date >= minDate)
                {
                    //if (date.DayOfWeek == DayOfWeek.Sunday && pattern.StartsWith("XPM"))
                    //{                                   // we don't receive IPM files on Sunday's
                    //    date = date.AddDays(-1);
                    //    continue;
                    //}

                    this.Log.Write(Severity.Debug, "Searching for {0} in {1} created on {2}.", pattern, path, date);
                    list = from f in di.GetFiles(pattern, SearchOption.TopDirectoryOnly)
                           where f.LastWriteTime.Date >= date && f.LastWriteTime <= date.AddDays(1)
                           select f;

                    if (list.Count() > 0)
                        this.Log.Write(Severity.Debug, "Found {0} created at {1}", list.First().Name, list.First().LastWriteTime);
                    else
                    {
                        this.Log.Write(Severity.Warning, "*** PROBLEM: Unable to find a {0} created on {1}.", pattern, date);
                        problemsFound++;
                    }

                    date = date.AddDays(-1);
                }
            }
            return problemsFound;
        }

        private int checkFolderForOldFiles(int problemsFound, List<PathnameAndSchedule> folders)
        {
            foreach (PathnameAndSchedule folder in folders)
            {
                if (!Directory.Exists(folder.Pathname))
                {
                    this.Log.Write(Severity.Error, "Directory: {0} not found.", folder.Pathname);
                    continue;
                }

                List<ProblemFile> problems = filterResults(folder);
                problemsFound += problems.Count();

                if (problems.Count() > 0)
                {
                    this.Log.Write(Severity.Warning, "*** PROBLEM: Files found in: {0}", folder.Pathname);

                    foreach (ProblemFile file in problems)
                        this.Log.Write(Severity.Warning, "\t{0:-20}\t{1}\t{2:5} kb\t{3} min. old"
                                                     , file.Filename, file.Timestamp, file.Size / 1024,file.Age);
                }
            }

            return problemsFound;
        }

        /// <summary>
        /// This utility method is responsible for filtering the results based on scheduled times, 
        /// and avoiding template files. It will also handle zero dollars files found in hold.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private List<ProblemFile> filterResults(PathnameAndSchedule folder)
        {
            List<ProblemFile> problems = new List<ProblemFile>();
            var windowStart = DateTime.Today.AddHours(folder.StartTime.Hour).AddMinutes(folder.StartTime.Minute);
            var windowEnd = DateTime.Today.AddHours(folder.EndTime.Hour).AddMinutes(folder.EndTime.Minute);
            var di = new DirectoryInfo(folder.Pathname);
            var fileInfos = di.GetFiles();
            var age = 0;

            if (folder.IntervalMins > 0)
                this.Log.Write(Severity.Debug, "Checking for files created between {0:MM/dd/yyyy hh:mm} and {0:MM/dd/yyyy hh:mm}.", windowStart, windowEnd);

            foreach (FileInfo fi in fileInfos)
            {
                if (folder.Pathname.ToLower().Contains("cardholder")
                  && fi.Name.ToLower().Contains("template"))
                    continue;

                if (folder.Pathname.ToLower().Contains(@"\hold")
                  && fi.Name.ToLower().Contains("zerodoller"))
                {
                    var holdPathname = @"W:\hold\Zero Files" + @"\" + fi.Name;

                    if (File.Exists(holdPathname))
                        holdPathname = Path.GetDirectoryName(holdPathname) 
                                     + Path.GetFileNameWithoutExtension(holdPathname) 
                                     + DateTime.Now.ToString("Hmmss") 
                                     + Path.GetExtension(holdPathname);

                    this.Log.Write(Severity.Info, "Moving {0} to {1}.", fi.Name, holdPathname);
                    FileSystem.Move(fi.FullName, holdPathname);
                    continue;
                }

                if (folder.IntervalMins > 0)
                    if (fi.LastWriteTime < windowStart)
                        continue;

                if (folder.IntervalMins > 0)
                    if (fi.LastWriteTime > windowEnd)
                    continue;

                age = calculateAge(folder, fi.LastWriteTime);

                if (age > folder.IntervalMins)
                    problems.Add(new ProblemFile(fi.Name, fi.LastWriteTime, fi.Length, age));
            }

            return problems;
        }

        /// <summary>
        /// This method checks to verify that daily invoices are created as expected each day. 
        /// It also checks for a minimum number of invoices to be created. It will also verify that 
        /// these invoices are available to ICD.
        /// </summary>
        /// <returns>The number of problems found</returns>

        private int checkForDailyInvoices()
        {
            var problemsFound = 0;
            var files = Settings.FilesToCheck.FindAll(x => x.ChecklistStep == 10);
            var folders = Settings.FoldersToCheck.FindAll(x => x.ChecklistStep == 10);
            
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 10: Checking for Daily Invoices");

            foreach (var folder in folders)
            {
                var date = DateTime.Today.AddDays(-1 * this.Settings.CheckDailyInvoices.PriorDaysToCheck);

                while (date < DateTime.Today)
                {
                     var invoicePath = Path.Combine(folder.Pathname, date.ToString(this.Settings.CheckDailyInvoices.DateFormat));

                    if (!Directory.Exists(invoicePath))
                    {
                        this.Log.Write(Severity.Warning, "*** PROBLEM: {0} was not found.", invoicePath);
                        problemsFound++;
                        date = date.AddDays(1);
                        continue;
                    }

                    var invoices = Directory.GetFiles(invoicePath, "*.*", SearchOption.TopDirectoryOnly).Count();
                    this.Log.Write(Severity.Debug, "found {0} inoices in {1} for {2}.", invoices, invoicePath, date.ToShortDateString());

                    if (invoices < this.Settings.CheckDailyInvoices.MinimumInvoices)
                        if (invoicePath.StartsWith("W:"))
                        {
                            this.Log.Write(Severity.Warning, "*** PROBLEM: Only {0} invoices were generated for {1} in {2}.", invoices, date.ToShortDateString(), invoicePath);
                            problemsFound++;
                        }
                        else
                            if (invoicePath.StartsWith("R:"))
                                if (invoicePath.Contains("xl"))
                                {
                                    this.Log.Write(Severity.Warning, "*** PROBLEM: Only {0} Excel invoices are available to ICD for {1}.", invoices, date.ToShortDateString());
                                    problemsFound++;
                                }
                                else
                                {
                                    this.Log.Write(Severity.Warning, "*** PROBLEM: Only {0} PDF invoices are available to ICD for {1}.", invoices, date.ToShortDateString());
                                    problemsFound++;
                                }
                            
                    date = date.AddDays(1);
                }
            }

            if (problemsFound == 0)
                this.Log.Write("*** Step 10 Complete.  No problems found. ***");

            return problemsFound;
        }
        /// <summary>
        /// This method will check for files left behind in various folders.
        /// </summary>
        /// <returns>The number of problems found</returns>
        private int checkVariousFolders()
        {
            var problemsFound = 0;
            var files = Settings.FilesToCheck.FindAll(x => x.ChecklistStep == 11);
            var folders = Settings.FoldersToCheck.FindAll(x => x.ChecklistStep == 11);

            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "");
            this.Log.Write(Severity.Info, "Starting Step 11: Checking for Files in Various Folders");

            problemsFound = checkFolderForOldFiles(problemsFound, folders);

            if (problemsFound == 0)
                this.Log.Write("*** Step 11 Complete.  No problems found. ***");

            return problemsFound;
        }

        /// <summary>
        /// This method will copulate the age of a file based on its interval and last scheduled time.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private int calculateAge(PathnameAndSchedule folder, DateTime dateTime)
        {
            if (folder.IntervalMins == 0)
                return 99;

            TimeSpan ts = lastScheduledTime(folder).Subtract(dateTime);
            var age = ts.Days * 1440 + ts.Hours * 60 + ts.Minutes;
 
            return age;
        }

        /// <summary>
        /// This method will determine the last scheduled time items in a folder should've been processed
        /// based on the folders start time, and time, and interval.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>Last scheduled time</returns>
        private DateTime lastScheduledTime(PathnameAndSchedule folder)
        {
            var lastRun = DateTime.Today.AddHours(folder.StartTime.Hour);
            lastRun = lastRun.AddMinutes(folder.StartTime.Minute);

            if (DateTime.Now < lastRun)
            {
                lastRun = DateTime.Today.AddDays(-1);
                lastRun = lastRun.AddHours(folder.EndTime.Hour);
                lastRun = lastRun.AddMinutes(folder.EndTime.Minute);
                return lastRun;
            }

            while(lastRun < DateTime.Now)
                lastRun = lastRun.AddMinutes(folder.IntervalMins);

            lastRun = lastRun.AddMinutes(-1*folder.IntervalMins);
            return lastRun;
        }

        class ProblemFile
        {
            public string Filename { get; set; }
            public DateTime Timestamp { get; set; }
            public long Size { get; set; }
            public int Age { get; set; }

            public ProblemFile(string filename, DateTime timestamp, long size, int age)
            {
                this.Filename = filename;
                this.Timestamp = timestamp;
                this.Size = size;
                this.Age = age;
            }
        }
    }
}
