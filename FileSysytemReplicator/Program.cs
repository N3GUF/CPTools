using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comdata.AppSupport.FileSystemReplicater
{
    class Program
    {
        static ILog Log = null;
        static ISettings Settings = null;

        static void Main(string[] args)
        {
            try
            {
                var settingsFile = "";
                var hoursOld = 24;
                var threads = 1;
                parseCommandLineArgs(args, out settingsFile, out hoursOld, out threads);

                //Settings = new Settings();
                Settings = new Settings(settingsFile + ".xml");
                Log = new TextLogger(Settings.LogPath, settingsFile, Settings.LoggingSeverityLevel);
                Log.Write("Starting FileSystemReplicater...");
                Console.WriteLine("Logging output to: {0}.", Log.LogPathname);
                //createDirectory(@"\\usbldppdbatwp01.pci.fleetcor.com\plsdreport\", @"\\usbldppdbatwp01.pci.fleetcor.com\plsdreports\invoices\20070624");
                Log.Write("Will be using settings from {0}.xml, and running {1} threads, and replicating files older than {2} hours.", settingsFile, threads, hoursOld);
                FileSystemReplicater Replicator = new FileSystemReplicater(Log, Settings);

                Replicator.Replicate(threads, hoursOld);
                Log.Write("Finished FileSystemReplicater.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Write("FileSystemReplicater has failed.");
                    Utilities.ReportException(ex, Log);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("FileSystemReplicater has failed.");
                    Utilities.ReportException(ex);
                    Console.WriteLine("Press any key...");
                    Console.Read();
                    Environment.Exit(1);
                }
            }
        }

        private static void parseCommandLineArgs(string[] args, out string settingsFile, out int hoursOld, out int threads)
        {
            settingsFile = "";
            hoursOld = 24;
            threads = 1;

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: FileSystemReplicater [Settings File] <# of Hours Old> <# of Threads>");
                Environment.Exit(-1);
            }

            if (args.Length > 0)
                settingsFile = args[0];

            if (args.Length > 1)
                int.TryParse(args[1], out hoursOld);

            if (args.Length > 2)
                int.TryParse(args[2], out threads);
        }
    }
}
