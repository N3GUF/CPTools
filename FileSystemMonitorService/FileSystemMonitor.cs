using System;
using System.ServiceProcess;
using System.Threading;

namespace Comdata.AppSupport.CDNFileSystemMonitor
{
    class FileSystemMonitor :ServiceBase
    {
        private Monitors.StorageMonitor storageMon;
       // private FolderMoitor folderMon = new FolderMoitor();
         
        public FileSystemMonitor()
        {
            this.ServiceName = "CDN File System Monitor";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            this.storageMon = new Monitors.StorageMonitor();
        }

        protected override void OnStop()
        {
            if (this.storageMon.Timer != null)
                this.storageMon.Timer.Dispose();
        }

        public static void Main()
        {
            ServiceBase.Run(new FileSystemMonitor());
        }
    }
}
