using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comdata.AppSupport.PPOLUnzipInvoices
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Count() < 1)
            {
                Console.WriteLine("Usage: PPOLUnzipInvoices <Path> [path]");
                return -1;
            }

            if (args.Count() >= 1)
                if (!Directory.Exists(args[0]))
                {
                    Console.WriteLine("Directory 1: {0} doesn't Exist.", args[0]);
                    return -2;
                }

            if (args.Count() >= 2)
                if (!Directory.Exists(args[1]))
                {
                    Console.WriteLine("Directory 2: {0} doesn't Exist.", args[1]);
                    return -3;
                }

            var di = new DirectoryInfo(args[0]);
            var fileList =  from f in di.GetFiles("*.zip", SearchOption.TopDirectoryOnly)
                           where (f.Name.ToUpper().StartsWith("PDF")
                              ||  f.Name.ToUpper().StartsWith("EXCEL"))
                          select f;

            if (fileList.Count() > 0)
                UnzipInvoices(args, fileList);

            return 0;
        }

        private static void UnzipInvoices(string [] args, IEnumerable<FileInfo> fileList)
        {
            var outputPath = "";
            
            foreach (var file in fileList)
            {
                if (file.Name.ToUpper().StartsWith("PDF"))
                    outputPath = Path.Combine(args[0], "invoices", file.Name.Substring(4, 8));
                else
                    outputPath = Path.Combine(args[0], "invoices_xl", file.Name.Substring(6, 8));

                if (Directory.Exists(outputPath))
                {
                    Console.WriteLine("Removing: {0}", outputPath);
                    Directory.Delete(outputPath, true);
                }

                Directory.CreateDirectory(outputPath);
                Console.WriteLine("Extracting: {0} to: {1}.", file.FullName, outputPath);
                ZipFile.ExtractToDirectory(file.FullName, outputPath);

                if (args.Count() > 1)
                {
                    if (file.Name.ToUpper().StartsWith("PDF"))
                        outputPath = Path.Combine(args[1], file.Name.Substring(4, 8));
                    else
                        outputPath = Path.Combine(args[1], file.Name.Substring(6, 8));

                    Directory.CreateDirectory(outputPath);
                    Console.WriteLine("Extracting: {0} to: {1}.", file.FullName, outputPath);
                    ZipFile.ExtractToDirectory(file.FullName, outputPath);
                }

                Console.WriteLine("Deleting: {0}.", file.FullName);
                File.Delete(file.FullName);
            }
        }
    }
}
