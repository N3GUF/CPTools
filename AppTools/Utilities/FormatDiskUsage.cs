using System;

namespace Comdata.AppSupport.AppTools
{
    partial class Utilities
    {
        public static string FormatDiskSpace(long sizeInBytes)
        {
            double size = 0.0;
            string units = "b";
            string returnValue;

            for (int i = 5; i > 0; i--)
            {
                if (sizeInBytes > Math.Pow(1024, i))
                {
                    size = sizeInBytes / Math.Pow(1024, i);

                    switch (i)
                    {
                        case 1: units = "kb";
                            break;
                        case 2: units = "MB";
                            break;
                        case 3: units = "GB";
                            break;
                        case 4: units = "TB";
                            break;
                        case 5: units = "PB";
                            break;
                    }

                    break;
                }
            }

            returnValue = String.Format("{0:##0.00} {1}", size, units);
            return returnValue;
        }

    }
}
