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
            RegisterServices();
            IServiceScope scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<ConsoleApplication>().Run();
            DisposeServices();

        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILog> (new TextLogger(Properties.Settings.Default.LogPath
                                                      , Properties.Settings.Default.LoggingThreshold));
            services.AddSingleton<ConsoleApplication>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
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
