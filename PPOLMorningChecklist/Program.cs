using Comdata.AppSupport.AppTools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Comdata.AppSupport.PPOLMorningChecklist
{
    class Program
    {
        static void Main(string[] args)
        {
            var displayLog = true;
            ILog log = null;
            PPOLMorningChecklist checklist = null;

            try
            {
                ResolveObjects(ref checklist, ref log);
                log.Write("Starting PPOL Morning Checklist...");

                if (args.Count() == 1)
                    if (args[0].ToUpper() == "NODISPLAY")
                        displayLog = false;

                checklist.Execute(displayLog);
                log.Write("Finished PPOL Morning Checklist.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (log != null)
                {
                    log.Write("PPOL Morning Checklist has failed.");
                    Utilities.ReportException(ex, log);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("PPOL Morning Checklist has failed.");
                    Utilities.ReportException(ex);
                    Console.WriteLine("Press any key...");
                    Console.Read();
                    Environment.Exit(1);
                }
            }
        }

        private static void ResolveObjects(ref PPOLMorningChecklist checklist, ref ILog log)
        {
            var services = new ServiceCollection();
            services.AddSingleton<ISettings>(p => new ChecklistSettings(@".\config.xml"));
            var provider = services.BuildServiceProvider();
            var settings = provider.GetService<ISettings>();

            services.AddSingleton<ILog>(p => new TextLogger(settings.LogPath, settings.LoggingSeverityLevel));
            provider = services.BuildServiceProvider();
            log = provider.GetService<ILog>();
            log.LogUpdated += Log_LogUpdated;
            services.AddTransient<IFileSystem, FileSystemHelper>();
            services.AddTransient<PPOLMorningChecklist>();
            provider = services.BuildServiceProvider();
            checklist = provider.GetService<PPOLMorningChecklist>();
        }

        static void Log_LogUpdated(object sender, LogUpdatedEventArgs e)
        {
            Console.WriteLine(e.Mwssage);
        }
   }
}
