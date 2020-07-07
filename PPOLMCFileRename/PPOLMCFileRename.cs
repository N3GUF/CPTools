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
            foreach (DirectoryPair dirSettings in settings.DirectoryPairList)
                processDirectoryPair(dirSettings); 
        }
        #endregion

        #region Private Methods
        private void LogSettingsValues()
        {
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Log Path:", settings.LogPath);
            log.Write(Severity.Debug, "\t{0,-25}{1}.", "Log Level:", settings.LoggingSeverityLevel);

            foreach (DirectoryPair dirSettings in settings.DirectoryPairList)
            {
                log.Write(Severity.Debug, "\t{0,-25}{1}.", "Source Path:", dirSettings.SourcePath);
                log.Write(Severity.Debug, "\t{0,-25}{1}.", "Destination Path:", dirSettings.DestinationPath);
                log.Write(Severity.Debug, "\t{0,-25}{1}.", "Archive Path:", Path.Combine(dirSettings.ArchivePath,DateTime.Today.ToString("yyyy-MM-dd")));
            }
        }

        private void processDirectoryPair(DirectoryPair dirSettings)
        {
            DirectoryInfo di = new DirectoryInfo(dirSettings.SourcePath);
            FileInfo[] files = di.GetFiles();
            string dest;
            if (files.Length == 0)  // If directory is empty, exit.
                return;

            foreach (FileInfo fi in files)
            {
                this.log.Write(Severity.Debug, "Examining: {0}.", fi.Name);

                if (dirSettings.SourcePath.ToLower().Contains("incoming"))
                    dest = processIncomingFile(fi, dirSettings);
                else
                    dest = processOutgoingFile(fi, dirSettings);

                if (dest == null)
                    continue;

                this.log.Write(Severity.Info, "Moving {0} to {1}, Size: {2:N0} bytes.", fi.FullName, dest, fi.Length);

                if (!move(fi.FullName, dest))  // Skip file if unavailable
                    continue;

                capyToAArchive(dirSettings, dest);
                additionalInstructions(dirSettings, dest);
            }
        }

        private string processIncomingFile(FileInfo fi, DirectoryPair dirSettings)
        {
            var nodes = fi.Name.Split(new char[] { '.' });

            if (fi.Name.Length != 43)  // Skip files that don't match the expected pattern.
            {
                this.log.Write(Severity.Info, "Skipping: {0} because it doesn't match the expected length.", fi.Name);
                return null;
            }

            if (nodes.Length != 8)  // Skip files that don't match the expected pattern.
            {
                this.log.Write(Severity.Info, "Skipping: {0} because it doesn't have 8 nodes.", fi.Name);
                return null;
            }

            if (!dirSettings.ExpectedBulkTypeList.Contains(nodes[2]))
            {
                this.log.Write(Severity.Info, "Skipping: {0} because it is not an expected bulk type.", fi.Name);
                return null;
            }

            return Rename(dirSettings.DestinationPath, fi.FullName);
        }

        private string processOutgoingFile(FileInfo fi, DirectoryPair dirSettings)
        {
            var nodes = fi.Name.Split(new char[] { '.' });

            if (fi.Name.Length != 24)  // Skip files that don't match the expected pattern.
            {
                this.log.Write(Severity.Info, "Skipping: {0} because it doesn't match the expected length.", fi.Name);
                return null;
            }

            if (nodes.Length != 2)  // Skip files that don't match the expected pattern.
            {
                this.log.Write(Severity.Info, "Skipping: {0} because it doesn't have 2 nodes.", fi.Name);
                return null;
            }

            if (!fi.Name.ToLower().StartsWith("ipm_"))
            {
                this.log.Write(Severity.Info, "Skipping: {0} because it doesn't have the correct filename", fi.Name);
                return null;
            }

            return Rename(dirSettings.DestinationPath, fi.FullName);
        }

        private string Rename(string destination, string source)
        {
            string newFilename;

            if (destination.ToLower().Contains("incoming"))
                newFilename = string.Format("T{0:4}T0.{1}{2}-{3}-{4}-{5}-{6}-{7}.{8}", Path.GetFileName(source).Substring(7, 4)
                                                                                     , DateTime.Now.ToString("yyyy").Substring(0, 2)
                                                                                     , Path.GetFileName(source).Substring(24, 2)
                                                                                     , Path.GetFileName(source).Substring(26, 2)
                                                                                     , Path.GetFileName(source).Substring(28, 2)
                                                                                     , Path.GetFileName(source).Substring(32, 2)
                                                                                     , Path.GetFileName(source).Substring(34, 2)
                                                                                     , Path.GetFileName(source).Substring(36, 2)
                                                                                     , Path.GetFileName(source).Substring(40, 3));
            else
                newFilename = string.Format("MCI.AR.R111.M.E0086401.D{0}.T{1}.A{2}"  , Path.GetFileName(source).Substring(6, 6)
                                                                                     , Path.GetFileName(source).Substring(12, 6)
                                                                                     , Path.GetFileName(source).Substring(21, 3));
            return Path.Combine(destination, newFilename);
        }

        private void capyToAArchive(DirectoryPair dirSettings, string source)
        {
            var archivePath = Path.Combine(dirSettings.ArchivePath, DateTime.Today.ToString("yyyy-MM-dd"));

            if (!Directory.Exists(archivePath))
            {
                this.log.Write(Severity.Info, "Creating directory: {0}.", archivePath);
                Directory.CreateDirectory(archivePath);
            }

            var destination = Path.Combine(archivePath, Path.GetFileName(source));
            this.log.Write(Severity.Info, "Archiving {0} to {1}.", source, destination);
            copy(source, destination);
        }

        private void additionalInstructions(DirectoryPair dirSettings, string source)
        {
            var bulkType = Path.GetFileName(source).Substring(1, 4);
            var instructions = dirSettings.AdditionalInstructionList.Where(i => i.BulkType == bulkType);

            foreach (var instr in instructions)
                executeInstruction(source, instr);
        }

        private void executeInstruction(string source, AdditionalInstruction instr)
        {
            var destination = Path.Combine(instr.Path, Path.GetFileName(source));

            switch (instr.AdditionalOperation)
            {
                case Operation.Copy:
                    this.log.Write(Severity.Info, "Copying {0} to {1}.", source, destination);
                    copy(source, destination);
                    break;

                case Operation.Move:
                    this.log.Write(Severity.Info, "Moving {0} to {1}.", source, destination);
                    move(source, destination);
                    break;

                case Operation.Delete:
                    this.log.Write(Severity.Info, "Deleting: {0}.", destination);
                    delete(source);
                    break;
            }

            return;
        }

        private bool copy(string source, string destination)
        {
            if (File.Exists(destination))
            {
                this.log.Write(Severity.Warning, "{0} already exists, and will be replaced.", destination);
            }

            try
            {
                File.Copy(source, destination, true);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The process cannot access the file because it is being used by another process"))
                    log.Write(Severity.Warning, "The file is in use by another process, and it will be processed next time.");
                else 
                    throw ex;

                return false;
            } 

            return true;
        }

        private bool move(string source, string destination)
        {
            if (File.Exists(destination))
            {
                this.log.Write(Severity.Warning, "{0} already exists, and will be replaced.", destination);
                delete(destination);
            }

            try
            {
                File.Move(source, destination);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The process cannot access the file because it is being used by another process"))
                    log.Write(Severity.Warning, "The file is in use by another process, and it will be processed next time.");
                else
                    throw ex;

                return false;
            }

            return true;
        }

        private bool delete(string destination)
        {
            try
            {
                File.Delete(destination);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The process cannot access the file because it is being used by another process"))
                    log.Write(Severity.Warning, "The file is in use by another process, and it will be processed next time.");
                else
                    throw ex;

                return false;
            }

            return true;
        }
        #endregion
    }
}
