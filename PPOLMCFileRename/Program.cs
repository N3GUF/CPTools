using Comdata.AppSupport.AppTools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace Comdata.AppSupport.PPOLMCFileRename
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
            PPOLMCFileRename rename = null;

            try
            {
                ResolveObjects("config.xml", ref rename, ref log);
                log.LogUpdated += Log_LogUpdated;
                rename.Execute();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (log != null)
                {
                    log.Write("PPOL MasterCard File Rename has failed.");
                    Utilities.ReportException(ex, log);
                    Environment.Exit(2);
                }
                else
                {
                    Console.WriteLine("PPOL MasterCard File Rename has failed.");
                    Utilities.ReportException(ex);
                    Environment.Exit(2);
                }
            }
        }

        private static void Log_LogUpdated(object sender, LogUpdatedEventArgs e)
        {
            Console.WriteLine(e.Mwssage);
        }

        /// <summary>
        /// Register and resolve all Objects with a unity container
        /// </summary>
        /// <param name="configFile">Configuration Name</param>
        /// <param name="rename">Zero byte checker instance</param>
        /// <param name="log">logger instance</param>
        private static void ResolveObjects(string configFile, ref PPOLMCFileRename rename, ref ILog log)
        {
            if (!File.Exists(configFile))
            {
                var asmDir = Assembly.GetEntryAssembly().Location;
                configFile = Path.Combine(asmDir, configFile);

                if (!File.Exists(configFile))
                    throw new FileNotFoundException(string.Format("{0} not found.", configFile));
            }

            var services = new ServiceCollection();
            services.AddSingleton<ISettings>(p => new Settings(configFile));
            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<ISettings>();

            services.AddSingleton<ILog>(p => new TextLogger(settings.LogPath, settings.LoggingSeverityLevel));
            provider = services.BuildServiceProvider();
            log = provider.GetService<ILog>();

            log.Write(Severity.Debug, "Configuration settings have been loaded from: {0}.", configFile);

            services.AddTransient<PPOLMCFileRename>();
            provider = services.BuildServiceProvider();
            rename = provider.GetService<PPOLMCFileRename>();
        }
    }
}
