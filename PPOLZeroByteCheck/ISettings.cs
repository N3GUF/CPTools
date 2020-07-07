namespace Comdata.AppSupport.PPOLZeroByteCheck
{
    public interface ISettings
    {
        string LogPath { get; set; }
        Comdata.AppSupport.AppTools.Severity LoggingSeverityLevel { get; set; }
        string IncomingPath { get; set; }
        string ProcessedPath { get; set; }
        string InputFilePattern { get; set; }
        string ResponsePath { get; set; }
        bool GenerateResponse { get; set; }
    }
}
