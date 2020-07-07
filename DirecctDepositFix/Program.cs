using Comdata.AppSupport.AppTools;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.DirectDepositFix
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog log = null;
            PPOLDirectDepositFix fix = null;

            try
            {
                ResolveObjects(ref fix, ref log);
                fix.FilenamePattern = Properties.Settings.Default.FilemePattern;
                fix.ProcessedDir    = Properties.Settings.Default.ProcessedDir;
                fix.BackupDir       = Properties.Settings.Default.BackupDir;
                fix.SendDir         = Properties.Settings.Default.SendDir;
                fix.SendScript      = Properties.Settings.Default.SendScript;

                log.Write("Starting PPOL Direct Deposit Fix...");
                fix.Execute();
                log.Write("Finished PPOL Direct Deposit Fix.");
                Process.Start(log.LogPathname);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (log != null)
                {
                    log.Write("PPOL Direct Deposit Fix has failed.");
                    Utilities.ReportException(ex, log);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("PPOL Direct Deposit Fix has failed.");
                    Utilities.ReportException(ex);
                    Console.WriteLine("Press any key...");
                    Console.Read();
                    Environment.Exit(1);
                }
            }
        }

        private static void ResolveObjects(ref PPOLDirectDepositFix fix, ref ILog log)
        {
            var container = new UnityContainer();
            container.RegisterType<ILog, TextLogger>(new ContainerControlledLifetimeManager()
                                                   , new InjectionConstructor(Properties.Settings.Default.LogPath
                                                                            , Properties.Settings.Default.LoggingThreshold));
            container.RegisterType<IFileSystem, FileSystemHelper>(new ContainerControlledLifetimeManager());
            container.RegisterType<PPOLDirectDepositFix>();
            log = container.Resolve<ILog>();
            log.LogUpdated += Log_LogUpdated;
            fix = container.Resolve<PPOLDirectDepositFix>();
        }

        static void Log_LogUpdated(object sender, LogUpdatedEventArgs e)
        {
            Console.WriteLine(e.Mwssage);
        }
    }
}
