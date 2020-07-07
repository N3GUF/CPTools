using Comdata.AppSupport.AppTools;
using System.IO;

namespace Comdata.AppSupport.PPOLZeroByteCheck
{
    /// <summary>
    /// This class will check for zero byte files in an inbound directory.
    /// If an empty file is found; then, the file will be moved to a processed 
    /// folder, in a corresponding response file will be written to an outbound 
    /// directory.
    /// </summary>
    public class PPOLZeroByteFileChecker
    {
        #region Private fields
        private ILog log;
        private ISettings settings;
        #endregion

        #region Public Methods
        public PPOLZeroByteFileChecker(ILog log, ISettings settings)
        {
            this.log = log;
            this.settings = settings;
            logSettingsValues();
        }  

        public void Execute()
        {
            DirectoryInfo di = new DirectoryInfo(this.settings.IncomingPath);
            FileInfo[] files = di.GetFiles();

            if (files.Length == 0)  // If directory is empty, exit.
                return;

            this.log.Write("Starting PPOL Zero Byte File Checker...");
  
            foreach (FileInfo fi in files)
            {
                this.log.Write(Severity.Debug, "Examining: {0}.", fi.Name);

                if (!fi.Name.Contains(this.settings.InputFilePattern))  // Skip files that don't match the expected pattern.
                {
                    this.log.Write(Severity.Info, "Skipping: {0} because it doesn't match the expected pattern.", fi.Name);
                    continue;
                }

                if (fi.Length == 0)  //  If empty file; acknolege and move it.
                {
                    this.log.Write(Severity.Info, "Removing {0}, because it is an empty file.", fi.Name);
                    acknolegeAndRemove(fi.Name);
                }
            }

            this.log.Write("Finished PPOL Zero Byte File Checker.");
        }
        #endregion

        #region Private Methods
        private void logSettingsValues()
        {
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Log Path:", settings.LogPath);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Log Level:", settings.LogPath);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Incoming Path:", settings.IncomingPath);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Processed Path:", settings.ProcessedPath);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Response Path:", settings.ResponsePath);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Input Filename Pattern:", settings.InputFilePattern);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Generate Response File:", settings.GenerateResponse.ToString());
        }
        private void acknolegeAndRemove(string name)
        {
            var source = Path.Combine(this.settings.IncomingPath, name);
            var destination = Path.Combine(this.settings.ProcessedPath, name);
            this.log.Write("Moving {0} to {1}.", source, destination);
            File.Move(source, destination);

            if (this.settings.GenerateResponse)
                createResponseFile(name);
        }
        private void createResponseFile(string name)
        {
            var splitPos = name.IndexOf("_");

            if (splitPos == 0)
                splitPos = name.IndexOf("_", 2);

            var ackFilename = name.Substring(0, splitPos) + "Response" + name.Substring(splitPos);
            ackFilename = Path.GetFileNameWithoutExtension(ackFilename) + ".tsv";
            ackFilename = Path.Combine(this.settings.ResponsePath, ackFilename);
            this.log.Write("Creating acknolegement file: {0}", ackFilename);
            var ackFormat = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}";
            var ackHeader = string.Format(ackFormat
                                        , "Record No"
                                        , "Response Code"
                                        , "Response Message"
                                        , "Account Number"
                                        , "Employee Id"
                                        , "CardHolder Reference Number"
                                        , "Card Number (Last 8 digits)"
                                        , "");
            var ackTrailerFormat = "{0}\t{1}";
            var ackTrailer = string.Format(ackTrailerFormat, "Total", 0);

            using (StreamWriter sw = new StreamWriter(ackFilename))
            {
                sw.WriteLine(ackHeader);
                sw.WriteLine(ackFormat,"", "100", "FILE HAS NO DATA TO PROCESS", "", "", "", "", "");
                sw.WriteLine(ackTrailer);
            }
        }
        #endregion
    }
}