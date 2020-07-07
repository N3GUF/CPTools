using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Comdata.AppSupport.AppTools;

namespace Comdata.AppSupport.PPOLMorningChecklist
{
    class FileSystemHelper :IFileSystem
    {
        public ILog Log { get; set; }

        public FileSystemHelper(ILog log)
        {
            this.Log = log;
        }

        public void Copy(string source, string dest)
        {
            try
            {
                File.Copy(source, dest);
            }

            catch (IOException ex)
            {
                Log.Write(Severity.Debug, "Unable to copy {0} to {1}.  {2}.", source, dest, ex.Message);
                return;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Move(string source, string dest)
        {
            try
            {
                File.Move(source, dest);
            }

            catch (IOException ex)
            {
                Log.Write(Severity.Debug, "Unable to move {0} to {1}.  {2}.", source, dest, ex.Message);
                return;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(string source)
        {
            try
            {
                File.Delete(source);
            }

            catch (IOException ex)
            {
                Log.Write(Severity.Debug, "Unable to delete {0}.  {1}.", source, ex.Message);
                return;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
