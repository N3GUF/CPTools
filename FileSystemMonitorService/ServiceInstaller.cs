using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration.Install;
using System.ComponentModel;

namespace Comdata.AppSupport.CDNFileSystemMonitor
{
    [RunInstaller(true)]
    public class FileSystemMonitorService : Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        public FileSystemMonitorService()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "CDN File System Monitor";
            serviceInstaller.DisplayName = "CDN File System Monitor";
            serviceInstaller.Description = "Monitors Filesystems and Utilization, and Filesystem Maintenance";
            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}
