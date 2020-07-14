using System.Collections.Generic;
using Comdata.AppSupport.AppTools;

namespace Comdata.AppSupport.PPOLMCFileRename
{
    public enum Operation { Copy, Delete, Move }
    
    public class AdditionalInstruction
    {
        public string BulkType { get; set; }
        public Operation AdditionalOperation { get; set; }
        public string Path { get; set; }
    }

    public partial class Settings : ISettings
    {
        public string LogPath { get; set; }
        public Severity LoggingSeverityLevel { get; set; }
        public string IncomingPath { get; set; }
        public string DestinationPath { get; set; }
        public List<string> ExpectedBulkTypeList { get; set; }
        public List<AdditionalInstruction> AdditionalInstructionList { get; set; }
    }
}
