using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comdata.AppSupport.AppTools;
using System.IO;
using System.Diagnostics;

namespace Comdata.AppSupport.LoadCancelRejectReport
{
    class Program
    {
        static Logger Log = null;

        static void Main(string[] args)
        {

            try
            {
                if (!Directory.Exists(Properties.Settings.Default.LogPath))
                    throw new FileNotFoundException(Properties.Settings.Default.LogPath + " doesn't exist.");

                Log = new Logger(Properties.Settings.Default.LogPath);
                Log.LoggingThreashold = Properties.Settings.Default.LogThreshold;
                Log.AddSeverityLevel = true;
                Log.AddTimeStamp = true;
                Log.Write("Starting Load Cancel Reject Report....");
                createReport();
                Log.Write("Finished Starting Load Cancel Reject Report.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Write("Load Cancel Reject Report has failed.");
                    AppTools.Utilities.ReportException(ex, Log);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("Load Cancel Reject Report has failed.");
                    AppTools.Utilities.ReportException(ex);
                    Console.WriteLine("Press any key...");
                    Console.Read();
                    Environment.Exit(1);
                }
            }
        }

        private static void createReport()
        {
            DailyLCRReport report = new DailyLCRReport();
            report.Log = Log;

            if (File.Exists(Properties.Settings.Default.PPOLBINInput))
                report.BINPathname = Properties.Settings.Default.PPOLBINInput;
            else
                throw new FileNotFoundException(string.Format("BIN input file {0} not found."
                                                              , Properties.Settings.Default.PPOLBINInput));

            if (Directory.Exists(Path.GetDirectoryName(Properties.Settings.Default.InputPattern)))
                report.InputPathname = findNewestLoadCancelRejectFile(Properties.Settings.Default.InputPattern);
            else
                throw new FileNotFoundException(string.Format("Input directory {0} not found."
                                                              , Path.GetDirectoryName(Properties.Settings.Default.InputPattern)));

            Log.Write("Current Regions Load Cancel Reject Filename: {0}", report.InputPathname);
            report.CreateReport();
            Process.Start(report.EmailFileName);
        }

        private static string findNewestLoadCancelRejectFile(string inputPattern)
        {
            var files = Directory.GetFiles(Path.GetDirectoryName(inputPattern), Path.GetFileName(inputPattern));
            var mostRecentDate = DateTime.Now.AddYears(-100);
            var mostRecentFilename = "";

            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTime > mostRecentDate)
                {
                    mostRecentDate = fi.CreationTime;
                    mostRecentFilename = file;
                }

            }

            return mostRecentFilename;
        }
    }
}
