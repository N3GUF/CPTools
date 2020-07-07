using Comdata.AppSupport.AppTools;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.PPOLFileMover
{
    public static class Bootstrapper
    {
        public static IUnityContainer BuildUnityContainer(string configName)
        {
            ISettings settings;
            var container = new UnityContainer();
            container.RegisterType<PPOLFileMover>();
            container.RegisterType<ISettings, FileMoverSettings>(new InjectionConstructor(@".\" + configName +".xml"));
            settings = container.Resolve<ISettings>();
            container.RegisterType<ILog, TextLogger>(new InjectionConstructor(settings.LogPath, settings.LoggingSeverityLevel));
            return container;
        }
    }
}
