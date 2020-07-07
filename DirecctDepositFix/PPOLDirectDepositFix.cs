using System;
using Comdata.AppSupport.AppTools;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Comdata.AppSupport.DirectDepositFix
{
    public class PPOLDirectDepositFix
    {
        public string ProcessedDir { get; internal set; }
        public string FilenamePattern { get; internal set; }
        public string BackupDir { get; internal set; }
        public string SendDir { get; internal set; }
        public string SendScript { get; internal set; }

        private ILog log;
        private IFileSystem fs;

        public PPOLDirectDepositFix(ILog log, IFileSystem fs)
        {
            this.log = log;
            this.fs = fs;
        }

        public void Execute()
        {
            var file = fs.FindCurrentFile(this.ProcessedDir, this.FilenamePattern);

            if (file == null)
            {
                log.Write(Severity.Error, "Unable to find a file matching the pattern {0} in {1} created today."
                                        , this.FilenamePattern
                                        , this.ProcessedDir);
                return;
            }

            if (validAccountTypes(file))
            {
                log.Write(Severity.Info, "No Problems found in: {0}.", file);
                return;
            }

            backup(file);
            edit(file);
            fs.Delete(file);
            sendAndVerify(file);
        }

        private bool validAccountTypes(string file)
        {
            var returnValue = true;

            try
            {
                using (var sr = new StreamReader(file))
                {
                    string record;

                    while ((record = sr.ReadLine()) != null)
                    {
                        if (record.Substring(1, 4) == "null")
                        {
                            log.Write(Severity.Info, "Contract {0} - {1} has an invalid account type."
                                                    , record.Substring(41, 9)
                                                    , record.Substring(56, 24));
                            log.Write(Severity.Debug, "Found: {0}", record.Substring(0, 50));
                            returnValue = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occurred while checking {0} for invalid account types."
                                                , Path.GetFileName(file)), ex);
            }

            return returnValue;
        }

        private void backup(string file)
        {
            var backupPathname = Path.Combine(this.BackupDir, Path.GetFileName(file));
            log.Write(Severity.Info, "Backing up {0} to {1}.", file, backupPathname);
            fs.Copy(file, backupPathname);
        }

        private void edit(string file)
        {
            var outputPathname = Path.Combine(this.SendDir, Path.GetFileName(file));
            log.Write(Severity.Info, "Creating {0}.", outputPathname);

            try
            {
                using (var sw = new StreamWriter(outputPathname))
                using (var sr = new StreamReader(file))
                {
                    string record;

                    while ((record = sr.ReadLine()) != null)
                    {
                        if (record.Substring(1, 4) == "null")
                            record = record.Replace("null", "22");

                        sw.WriteLine(record);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occurred while editting {0}.", file), ex);
            }

        }

        private void sendAndVerify(string file)
        {
            log.Write("Sending {0} to Connect Enterprise", Path.GetFileName(file));
            Process.Start(this.SendScript);
            Thread.Sleep(5000);
            file = fs.FindCurrentFile(this.SendDir, "*.*");

            if (file == null)
                log.Write("Sent to Connect Enterprise");
            else
                log.Write(Severity.Error, "The send directory is not empty.");
        }
    }
}