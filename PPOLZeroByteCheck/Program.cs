using Comdata.AppSupport.AppTools;
using Microsoft.Practices.Unity;
using System;
using System.IO;

namespace Comdata.AppSupport.PPOLZeroByteCheck
{
    class Program
    {
        /// <summary>
        /// This program will check for zero byte files in an inbound directory.
        /// If an empty file is found; then, the file will be moved to a processed 
        /// folder, in a corresponding response file will be written to an outbound 
        /// directory.
        /// </summary>
        /// <param name="args">Configuration Name</param>
        static void Main(string[] args)
        {
            ILog log = null;
            PPOLZeroByteFileChecker zeroByteCheck = null;

            try
            {
                if (args.Length != 1)
                    throw new Exception("Invalid number of Command Line arguments, Expected 1.");

                ResolveObjects(args[0], ref zeroByteCheck, ref log);
                zeroByteCheck.Execute();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (log != null)
                {
                    log.Write("PPOL Zero Byte File Checker has failed.");
                    Utilities.ReportException(ex, log);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("PPOL Zero Byte File Checker has failed.");
                    Utilities.ReportException(ex);
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Register and resolve all Objects with a unity container
        /// </summary>
        /// <param name="config">Configuration Name</param>
        /// <param name="check">Zero byte checker instance</param>
        /// <param name="log">logger instance</param>
        private static void ResolveObjects(string config, ref PPOLZeroByteFileChecker check, ref ILog log)
        {
            var configFile = @".\" + config + "_config.xml";

            if (!File.Exists(configFile))
                throw new FileNotFoundException(string.Format("{0} not found.", configFile));

            var container = new UnityContainer();
            container.RegisterType<ISettings, Settings>(new ContainerControlledLifetimeManager()
                                                               , new InjectionConstructor(configFile));
            var settings = container.Resolve<ISettings>();
 
            container.RegisterType<ILog, TextLogger>(new ContainerControlledLifetimeManager()
                                                   , new InjectionConstructor(settings.LogPath
                                                                            , settings.LoggingSeverityLevel));
            log = container.Resolve<ILog>();

            log.Write(Severity.Debug, "Confiuration settings have been loaded from: {0}.", configFile);

            container.RegisterType<PPOLZeroByteFileChecker>();
            check = container.Resolve<PPOLZeroByteFileChecker>();
        }
    }
}
