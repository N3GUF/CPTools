using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMover
{
    static class Session
    {
        public static object locker = new object();
        public static int FilesToProcess = 0;
        public static int FilesProcessed = 0;
        public static string SourcePathname = string.Empty;
        public static string DestinationPathname = string.Empty;
        public static int DestinationFiles = 0;
        public static int DestinationDirectories = 0;
    }
}
