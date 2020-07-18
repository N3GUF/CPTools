using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace UnCompressAndSignal
{
    public class ExtractAndSignal
    {
        private int filesExtracted = 0;
        private int totalFilesExtracted = 0;

        /// <summary>
        /// Extract all files from all zip archives found in the path specified.  
        /// The zip archives found will then be deleted.
        /// And finally, if provided, a trigger file will be created using the trigger pathname.
        /// </summary>
        /// <param name="path">The path to serach for zip archives to be extracted.
        /// <param name="triggerPathname">The pathname for the trigger.</param>
        /// <returns> 0 Success</returns>
        /// <returns>-2 No files to unzipSuccess</returns>
        public int ExtractAllFiles(string path, string triggerPathname)
        {
            var files = extractZipFiles(path);
 
            if (files == 0)
                return 0;

            if (triggerPathname == string.Empty)
                return 0;

            createTriggerFile(triggerPathname);

            return 0;
        }

        /// <summary>
        /// Extract all files from all zip archives found in the path specified.  
        /// The zip archives found will then be deleted.
        /// </summary>
        /// <param name="path">The path to serach for zip archives to be extracted.</param>
        /// <returns></returns>
        private int extractZipFiles(string path)
        {
            var zipFiles = Directory.GetFiles(path, "*.zip");

            if (zipFiles.Count() == 0)
            {
                Console.WriteLine("No files to process.");
                return 0;
            }

            Console.WriteLine("Unzipping {0} files to {1}.\r\n", zipFiles.Count(), path);

            foreach (var zipFile in zipFiles)
            {
                unzip(zipFile, path);
                Console.WriteLine("\tUnzipped {0,-50}, containing {1,7:N0} files.", Path.GetFileName(zipFile)
                                                                                , this.filesExtracted);
            }

            Console.WriteLine("\r\n\t{0,-50} containing {1,7:N0} files.", string.Format("Unzipped {0} archives,", zipFiles.Count())
                                                                      , this.totalFilesExtracted);
            return zipFiles.Count();
        }

        /// <summary>
        /// Extract all files in the zip archive.
        /// Delete the zip archive.
        /// List all extracted files.
        /// </summary>
        /// <param name="zipArchive">Zip Pathname</param>
        /// <param name="destinationPath">Destination Path for Extracted Files</param>
        private void unzip(string zipArchive, string path)
        {
            var tempDir = Path.Combine(path, "unzippedFiles");

            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);

            Directory.CreateDirectory(tempDir);
            filesExtracted = 0;
            ZipFile.ExtractToDirectory(zipArchive, tempDir);
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                var dest = Path.Combine(path, Path.GetFileName(file));
                File.Move(file, dest);
                this.filesExtracted++;
            }

            Directory.Delete(tempDir, true);
            File.Delete(zipArchive);
            this.totalFilesExtracted += this.filesExtracted;
        }

        /// <summary>
        /// A trigger file will be created using the trigger pathname to signal that files are ready
        /// for processing.
        /// </summary>  
        /// <param name="triggerPathname">The pathname for the trigger file.</param>
        private void createTriggerFile(string triggerPathname)
        {
            Console.WriteLine("\r\nCreating Trigger File: {0}.", triggerPathname);

            using (var sw = new StreamWriter(triggerPathname))
                sw.WriteLine("This is a flag file to trigger a job.");
        }
    }
}
