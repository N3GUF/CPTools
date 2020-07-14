using Comdata.AppSupport.AppTools;
using System;
using System.Text;

namespace AppTools
{
    public partial class Utilities
    {
        public static void ReportException(Exception ex)
        {   
            Console.WriteLine(FormatException(ex));
        }

        public static void ReportException(Exception ex, Logger log)
        {
            if (log == null)
                ReportException(ex);
            else
                log.Write(Severity.Critical, FormatException(ex));
        }

        public static string FormatException(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Exception Message: {0}\n\r", ex.Message);
            Exception temp = ex;

            while (temp.InnerException != null)
            {
                temp = temp.InnerException;
                sb.AppendFormat("Inner Exception Message: {0}\n\r", temp.Message);
            }

            sb.Append("\n\rStackTrace:\n\r");
            sb.Append(temp.StackTrace);
            return sb.ToString();
        }
    }
}
