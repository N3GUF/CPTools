using Comdata.AppSupport.AppTools;
using Microsoft.Practices.Unity;
using System;
using System.IO;

namespace Comdata.AppSupport.PPOLFileMover
{
    class Program
    {
        static ILog Log = null;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                    throw new Exception("This program expects 1 command line argument.");

                if (!File.Exists(args[0] + ".xml"))
                    throw new FileNotFoundException(string.Format("Unable to to find configuration {0}.xml.", args[0]));

                var container = Bootstrapper.BuildUnityContainer(args[0]);
                Log = container.Resolve<ILog>();
                var fileMover = container.Resolve<PPOLFileMover>();

                Log.Write("Starting PPOL File Mover...");
                fileMover.Execute();
                Log.Write("Finished PPOL File Mover.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Write("PPOL File Mover has failed.");
                    Utilities.ReportException(ex, Log);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("PPOL File Mover has failed.");
                    Utilities.ReportException(ex);
                    Console.WriteLine("Press any key...");
                    Console.Read();
                    Environment.Exit(1);
                }
            }
        }
    }
}
