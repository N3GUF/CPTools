using Comdata.AppSupport.AppTools;

namespace Comdata.AppSupport.PPOLZeroByteCheck
{
    public partial class Settings : ISettings
    {
        public string LogPath { get; set; }
        public Severity LoggingSeverityLevel { get; set; }
        public string IncomingPath { get; set; }
        public string ProcessedPath { get; set; }
        public string InputFilePattern { get; set; }
        public string ResponsePath { get; set; }
        public bool GenerateResponse { get; set; }
    }
}
