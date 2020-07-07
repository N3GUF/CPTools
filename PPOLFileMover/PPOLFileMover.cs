using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.PPOLFileMover
{
    class PPOLFileMover
    {
        public ILog Log { get; set; }
        public ISettings Settings { get; set; }

        public PPOLFileMover (ILog log, ISettings settings)
        {
            this.Log = log;
            this.Settings = settings;
         }

        public void Execute()
        {
            foreach (FileToMove file in this.Settings.FilesToMove)
                locate(file);
        }

        private void locate(FileToMove file)
        {
            var path = Path.GetDirectoryName(file.Source);
            var pattern = Path.GetFileName(file.Source);
            var minDate = DateTime.Today;
            this.Log.Write(Severity.Debug, "Searching for {0} in {1} created after {2}.", pattern, path, minDate);

            var di = new DirectoryInfo(path);
            var list = from f in di.GetFiles(pattern, SearchOption.TopDirectoryOnly)
                       where f.LastWriteTime.Date >= minDate
                       select f;

            if (list.Count() > 0)
            {
                foreach (FileInfo fi in list)
                {
                    this.Log.Write(Severity.Debug, "Found {0} created on {1}.", fi.Name, fi.LastWriteTime);
                    process(fi.FullName, file);
                }
            }
        }

        private void process(string source, FileToMove file)
        {
            var destination = transform(file, source);

            switch (file.Operation)
            {
                case FileOperation.Copy:
                    this.Log.Write("Copying {0} to {1}.", source, destination);
                    File.Copy(source, destination, true);
                    break;

                 case FileOperation.Delete:
                    this.Log.Write("Deleting {0}.", source);
                    File.Delete(source);
                    break;

                case FileOperation.Move:
                    this.Log.Write("Moving {0} to {1}.", source, destination);
                    File.Move(source, destination);
                    break;
                    
                default:
                    this.Log.Write(Severity.Warning,"Invalid Operation: {0}.", file.Operation);
                    break;
            }
        }

        private string transform(FileToMove file, string source)
        {
            var output = file.Destination.Replace("*", Path.GetFileName(source));
            return output;
        }
    }
}
