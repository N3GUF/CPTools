using System;
using System.Text;
using System.IO;

namespace AppTools
{
    partial class Utilities
    {
        public static string GetOdbcConnString(string pathname)
        {
            string connString = null;

            if (!File.Exists(pathname))
                throw new FileNotFoundException(string.Format("Unable to open ODBC connection string: {0}.", pathname));

            using (StreamReader sr = new StreamReader(pathname))
                connString = sr.ReadLine();

            return connString;
        }

    }
}
