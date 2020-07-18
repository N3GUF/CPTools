using Comdata.AppSupport.AppTools;
using System;
using System.IO;

namespace Comdata.AppSupport.FileSystemCleanup
{
    class Program
    {
        static ILog Log = null;
        static ISettings Settings = null;
        static IFileInfoList FiList = null;

        static void Main(string[] args)
        {
            var configFile = @".\FileSystemCleanupConfig.xml";

            if (args.Length == 1)
                if (File.Exists(args[0]))
                    configFile = args[0];

            Settings = new FileSystemCleanupSettings(configFile);
            Log = new TextLogger(Settings.LogPath, Settings.LoggingSeverityLevel);
            FiList = new FileInfoList(Log);

            try
            {
                FileSystemCleanup cleanup = new FileSystemCleanup(Log, Settings, FiList);

                Log.Write("Configuration loaded from: {0}.", configFile);
                Log.Write("Starting FileSystemCleanup...");
                cleanup.Execute();
                Log.Write("Finished FileSystemCleanup.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Write("FileSystemCleanup has failed.");
                    Utilities.ReportException(ex, Log);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("FileSystemCleanup has failed.");
                    Utilities.ReportException(ex);
                    Console.WriteLine("Press any key...");
                    Console.Read();
                    Environment.Exit(1);
                }
            }
        }
    }
}
