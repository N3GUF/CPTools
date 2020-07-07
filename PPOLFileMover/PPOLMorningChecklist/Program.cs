using Comdata.AppSupport.AppTools;
using Microsoft.Practices.Unity;
using System;

namespace Comdata.AppSupport.PPOLMorningChecklist
{
    class Program
    {
        static ILog Log = null;
 
        static void Main(string[] args)
        {
            try
            {
                var container = Bootstrapper.BuildUnityContainer();
                Log = container.Resolve<ILog>(); 
                var checklist = container.Resolve<PPOLMorningChecklist>();

                Log.Write("Starting PPOL Morning Checklist...");
                checklist.Execute();
                Log.Write("Finished PPOL Morning Checklist.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Write("PPOL Morning Checklist has failed.");
                    Utilities.ReportException(ex, Log);
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
   }
}
