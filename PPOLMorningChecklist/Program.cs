using Comdata.AppSupport.AppTools;
using Microsoft.Practices.Unity;
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
            var container = new UnityContainer();
            container.RegisterType<ISettings, ChecklistSettings>(new ContainerControlledLifetimeManager()
                                                               , new InjectionConstructor(@".\config.xml"));
            var settings = container.Resolve<ISettings>();
            container.RegisterType<ILog, TextLogger>(new ContainerControlledLifetimeManager()
                                                   , new InjectionConstructor(settings.LogPath
                                                                            , settings.LoggingSeverityLevel));
            container.RegisterType<IFileSystem, FileSystemHelper>(new ContainerControlledLifetimeManager());
            container.RegisterType<PPOLMorningChecklist>();
            log = container.Resolve<ILog>();
            log.LogUpdated += Log_LogUpdated;
            checklist = container.Resolve<PPOLMorningChecklist>();
        }

        static void Log_LogUpdated(object sender, LogUpdatedEventArgs e)
        {
            Console.WriteLine(e.Mwssage);
        }
   }
}
