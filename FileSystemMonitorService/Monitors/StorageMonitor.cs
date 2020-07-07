using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Comdata.AppSupport.AppTools;
using Comdata.AppSupport.CDNFileSystemMonitor;


namespace Comdata.AppSupport.CDNFileSystemMonitor.Monitors
{
    class StorageMonitor : MonitorBase
    {
        private Classes.StorageMonitor monitor;

        public StorageMonitor()
        {
            this.MonitorName = "StorageMonitor";
            this.IsRunning = false;
            this.IsActive = true;
            this.log = new TextLogger(@"C:\Logs\FileSystemMonitor", this.MonitorName);
            this.log.AddSeverityLevel = true;
            this.log.AddTimeStamp = true;
            this.MonitorTask = new MonitorTaskHandler(monitorTask);
            this.monitorTs = new TimeSpan(0, 1, 0);
            var initialTs = initialTimeSpan(this.monitorTs);
            TimerCallback tick = new TimerCallback(this.doWork);
            this.Timer = new Timer(tick, null, initialTs, this.monitorTs);
            this.monitor = new Classes.StorageMonitor();
            this.monitor.Log = this.log;
            this.monitor.DrivesNotToMonitor = @"d:\";
        }

        private void monitorTask()
        {
            monitor.ReportDiskUsage();
        }
    }
}
