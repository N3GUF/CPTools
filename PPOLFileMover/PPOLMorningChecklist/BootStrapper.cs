using Comdata.AppSupport.AppTools;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.PPOLMorningChecklist
{
    public static class Bootstrapper
    {
        public static IUnityContainer BuildUnityContainer()
        {
            ISettings settings;
            var container = new UnityContainer();
            container.RegisterType<PPOLMorningChecklist>();
            container.RegisterType<ISettings, ChecklistSettings>(new InjectionConstructor(@".\config.xml"));
            settings = container.Resolve<ISettings>();
            container.RegisterType<ILog, TextLogger>(new InjectionConstructor(settings.LogPath, settings.LoggingSeverityLevel));
            return container;
        }
    }
}
