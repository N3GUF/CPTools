using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comdata.AppSupport.AppTools;

namespace Comdata.AppSupport.PartnerAllianceResubmit
{
    class Program
    {
        static Logger Log = null; 

        static void Main(string[] args)
        {

            try
            {
                if (!Directory.Exists(Properties.Settings.Default.LogPath))
                    throw new FileNotFoundException(Properties.Settings.Default.LogPath + " doesn't exist.");

                Log = new Logger(Properties.Settings.Default.LogPath);
                Log.LoggingThreashold = Properties.Settings.Default.LogThreshold;
                Log.AddSeverityLevel = true;
                Log.AddTimeStamp = true;
                Log.Write("Starting Partner Alliance Resubmit...");
                Resubmit();
                Log.Write("Finished Partner Alliance Resubmit.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    Log.Write("Partner Alliance Resubmit has failed.");
                    AppTools.Utilities.ReportException(ex, Log);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("Partner Alliance Resubmit has failed.");
                    AppTools.Utilities.ReportException(ex);
                    Console.WriteLine("Press any key...");
                    Console.Read();
                    Environment.Exit(1);
                }
            }
        }

        private static void Resubmit()
        {
            PartnerAllianceResubmit resub = new PartnerAllianceResubmit();
            resub.Log = Log;

            if (File.Exists(Properties.Settings.Default.PS14File))
                resub.PS14Pathname = Properties.Settings.Default.PS14File;
            else
                throw new FileNotFoundException(string.Format("PS14 file {0} not found."
                                                              , Properties.Settings.Default.PS14File));

            if (File.Exists(Properties.Settings.Default.PS15File))
                resub.PS15Pathname = Properties.Settings.Default.PS15File;
            else
                throw new FileNotFoundException(string.Format("PS15 file {0} not found."
                                                              , Properties.Settings.Default.PS15File));

            resub.AcctCustChangesDT.Rows.Add("MG168", "BH0ZR", "MG168", "BH0ZS");
     //       resub.AcctCustChangesDT.Rows.Add("ME507", "BK5LI", "ME507", "BK5LJ");
                            
            Log.Write("PS14 Filename: {0}", resub.PS14Pathname);
            Log.Write("PS15 Filename: {0}", resub.PS15Pathname);

            if (resub.CheckForErrors())
                resub.ExtractErrors();
             else
                Log.Write("No Errors found in: {0}.", resub.PS15Pathname);
        }
    }
}
