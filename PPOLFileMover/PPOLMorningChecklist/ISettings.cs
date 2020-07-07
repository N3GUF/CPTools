using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.PPOLMorningChecklist
{
    public interface ISettings
    {
        string LogPath { get; set; }
        Comdata.AppSupport.AppTools.Severity LoggingSeverityLevel { get; set; }
        List<PathnameAndSchedule> FilesToCheck { get; set; }
        List<PathnameAndSchedule> FoldersToCheck { get; set; }
        CleanupInProcessFolderSettings CleanupInProcessFolder { set; get; }
        CheckDailyInvoicesSettings CheckDailyInvoices { set; get; }
        bool Save(string filename);
        List<PathnameAndSchedule> GetPathnamesAndSchedules(int step, List<PathnameAndSchedule> list);

    }
}
