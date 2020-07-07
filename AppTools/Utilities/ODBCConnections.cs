using System.IO;
using System.Text.RegularExpressions;

namespace Comdata.AppSupport.AppTools
{
    partial class Utilities
    {
        public static string GetConnectionString (string pathname)
        {
            return (getConnectionString(pathname));
        }

        private static string getConnectionString(string pathname)
        {
            string connString = null;

            if (!File.Exists(pathname))
                throw new FileNotFoundException("Unable to find connection string.", pathname);

            using (StreamReader sr = new StreamReader(pathname))
                connString = sr.ReadLine();

            return connString;
        }
    }
}
