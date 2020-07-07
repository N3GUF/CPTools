using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Comdata.AppSupport.CDNFileSystemMonitor;

namespace Comdata.AppSupport.CDNFileSystemMonitor.Monitors
{
    class MonitorBase
    {
        protected ILog log;
        protected TimeSpan monitorTs;
        protected delegate void MonitorTaskHandler();
        protected MonitorTaskHandler MonitorTask;

        public string MonitorName { get; protected set; }
        public bool IsRunning { get; protected set; }
        public bool IsActive { get; protected set; }
        public DateTime LastRunTs { get; private set; }
        public DateTime NextRunTs { get; private set; }
        public Timer Timer { get; set; }

        protected TimeSpan initialTimeSpan(TimeSpan ts)
        {
            DateTime start = DateTime.Today;

            while (start <= DateTime.Now)
                start = start.Add(ts);

            TimeSpan returedTs = start - DateTime.Now;
            this.NextRunTs=DateTime.Now.Add(returedTs);
            return returedTs;
        }

        protected void doWork(object state)
        {
            if (this.IsRunning)
            {
                this.log.Write(Severity.Warning, "{0} is already running.", this.MonitorName);
                return;
            }

            this.IsRunning = true;
            this.LastRunTs = DateTime.Now;
            this.NextRunTs = DateTime.Now.Add(this.monitorTs);

            try
            {
                using (new Classes.Impersonator("dbernhardy", "comdata", "?Camer0n2"))
                {
                    if (!NetworkDriveManager.IsDriveMapped("t"))
                        NetworkDriveManager.MapNetworkDrive("t", @"\\BWFS\Dept\AppSupport\Docs");
                    if (!NetworkDriveManager.IsDriveMapped("w"))
                        NetworkDriveManager.MapNetworkDrive("w", @"\\bw68b0f12\ppol");
                    
                    this.MonitorTask();

                    if (NetworkDriveManager.IsDriveMapped("t"))
                        NetworkDriveManager.DisconnectNetworkDrive("t", true);
                    if (NetworkDriveManager.IsDriveMapped("w"))
                        NetworkDriveManager.DisconnectNetworkDrive("w", true);
                }
            }
            catch (Exception ex)
            {
                Utilities.ReportException(new Exception("Error executing " + this.MonitorName, ex), this.log);
            }
            finally
            {
                this.IsRunning = false;
            }
        }
    }
}
