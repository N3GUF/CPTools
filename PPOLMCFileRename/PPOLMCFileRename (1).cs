using Comdata.AppSupport.AppTools;
using System;
using System.IO;
using System.Linq;

namespace Comdata.AppSupport.PPOLMCFileRename
{
    class PPOLMCFileRename
    {
        #region Private fields
        private ILog log;
        private ISettings settings;
        #endregion

        #region Public Methods
        public PPOLMCFileRename(ILog log, ISettings settings)
        {
            this.log = log;
            this.settings = settings;
            LogSettingsValues();
        }

        public void Execute()
        {
            DirectoryInfo di = new DirectoryInfo(this.settings.IncomingPath);
            FileInfo[] files = di.GetFiles();

            if (files.Length == 0)  // If directory is empty, exit.
                return;

            foreach (FileInfo fi in files)
            {
                this.log.Write(Severity.Debug, "Examining: {0}.", fi.Name);
                var nodes = fi.Name.Split(new char[] { '.' });

                if (fi.Name.Length != 43)  // Skip files that don't match the expected pattern.
                {
                    this.log.Write(Severity.Info, "Skipping: {0} because it doesn't match the expected length.", fi.Name);
                    continue;
                }

                if (nodes.Length != 8)  // Skip files that don't match the expected pattern.
                {
                    this.log.Write(Severity.Info, "Skipping: {0} because it doesn't have 8 nodes.", fi.Name);
                    continue;
                }

                if (!this.settings.ExpectedBulkTypeList.Contains(nodes[2]))
                {
                    this.log.Write(Severity.Info, "Skipping: {0} because it is not an expected bulk type.", fi.Name);
                    continue;
                }

                var dest = Rename(settings.DestinationPath, fi.FullName);

                if (File.Exists(dest))
                {
                    this.log.Write(Severity.Warning, "{0} already exists, and will be replaced.", dest);
                    File.Delete(dest);
                }

                this.log.Write(Severity.Info, "Moving {0} to {1}, Size: {2:N0} bytes.", fi.FullName, dest, fi.Length);
                File.Move(fi.FullName, dest);
                AdditionalInstructions(dest);

            }
        }

        private void AdditionalInstructions(string dest)
        {
            var bulkType = Path.GetFileName(dest).Substring(1, 4);
            var instructions = this.settings.AdditionalInstructionList.Where(i => i.BulkType == bulkType);

            foreach (var instr in instructions)
                switch (instr.AdditionalOperation)
                {
                    case Operation.Copy:
                        this.log.Write(Severity.Info, "Copying {0} to {1}.", dest, Path.Combine(instr.Path, Path.GetFileName(dest)));
                        File.Copy(dest, Path.Combine(instr.Path, Path.GetFileName(dest)));
                        break;

                    case Operation.Delete:
                        this.log.Write(Severity.Info, "Deleting {0}.", dest);
                        File.Delete(dest);
                        break;

                    case Operation.Move:
                        this.log.Write(Severity.Info, "Moving {0} to {1}.", dest, Path.Combine(instr.Path, Path.GetFileName(dest)));
                        File.Move(dest, Path.Combine(instr.Path, Path.GetFileName(dest)));
                        break;
                }
        }
        #endregion

        #region Private Methods
        private void LogSettingsValues()
        {
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Log Path:", settings.LogPath);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Log Level:", settings.LogPath);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Incoming Path:", settings.IncomingPath);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Processed Path:", settings.DestinationPath);
         }

        private string Rename(string destination, string source)
        {
            var newFilename = string.Format("T{0:4}T0.{1}{2}-{3}-{4}-{5}-{6}-{7}.{8}", Path.GetFileName(source).Substring(7, 4)
                                                                                     , DateTime.Now.ToString("yyyy").Substring(0, 2)
                                                                                     , Path.GetFileName(source).Substring(24, 2)
                                                                                     , Path.GetFileName(source).Substring(26, 2)
                                                                                     , Path.GetFileName(source).Substring(28, 2)
                                                                                     , Path.GetFileName(source).Substring(32, 2)
                                                                                     , Path.GetFileName(source).Substring(34, 2)
                                                                                     , Path.GetFileName(source).Substring(36, 2)
                                                                                     , Path.GetFileName(source).Substring(40, 3));
            return Path.Combine(destination, newFilename);
        }

        #endregion
    }
}
