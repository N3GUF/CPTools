using System.Collections.Generic;

namespace Comdata.AppSupport.PPOLMCFileRename
{
    public interface ISettings
    {
        string LogPath { get; set; }
        AppTools.Severity LoggingSeverityLevel { get; set; }
        string IncomingPath { get; set; }
        string DestinationPath { get; set; }
        List<string> ExpectedBulkTypeList { get; set; }
        List<AdditionalInstruction> AdditionalInstructionList { get; set; }
    }
}
