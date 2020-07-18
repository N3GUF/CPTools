using Comdata.AppSupport.AppTools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace Comdata.AppSupport.DirectDepositFix
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
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
            var services = new ServiceCollection();
            services.AddSingleton<ILog>(new TextLogger(Properties.Settings.Default.LogPath
                                                      , Properties.Settings.Default.LoggingThreshold));
            services.AddSingleton<PPOLDirectDepositFix>();
            _serviceProvider = services.BuildServiceProvider(true);

            log = _serviceProvider.GetService<ILog>();
            log.LogUpdated += Log_LogUpdated;
            fix = _serviceProvider.GetService<PPOLDirectDepositFix>();
        }

        static void Log_LogUpdated(object sender, LogUpdatedEventArgs e)
        {
            Console.WriteLine(e.Mwssage);
        }
    }
}
