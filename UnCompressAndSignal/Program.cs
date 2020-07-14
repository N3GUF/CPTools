using Comdata.AppSupport.AppTools;
using System;
using System.Linq;

namespace UnCompressAndSignal
{
    class Program
    {
        static int Main(string[] args)
        {
            var path = string.Empty;
            var triggerPathname = string.Empty;

            if (args.Count() == 0)
            {
                Console.WriteLine("Usage: UnCompressAndSignal path [trigger pathname]");
                Console.WriteLine("\twhere:\tpath is the location to search for zip archives (Requierd)");
                Console.WriteLine("\t\ttrigger pathname is the full filename and location of the trigger filename (optional)");
                return -1;
            }
      
            if (args.Count() > 0)
                path = args[0];

            if (args.Count() > 1)
                triggerPathname = args[1];

            try
            {
                var extract = new ExtractAndSignal();
                extract.ExtractAllFiles(path, triggerPathname);
                return 0;
            }
            catch (Exception ex)
            {
                Utilities.ReportException(ex);
                return -2;
            }
        }
    }
}