using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.AppTools
{
    public interface ILog
    {
        string LogPathname  { get; }
        bool AddTimeStamp { get; set; }
        bool AddSeverityLevel { get; set; }
        Severity CurrentSeverityLevel { get; set; }
        Severity LoggingThreshold { get; set; }
        event EventHandler<LogUpdatedEventArgs> LogUpdated;

        /// <summary>
        /// write a formatted message to the log with a severity level
        /// </summary>
        /// <param name="severity"></param> severity level override
        /// <param name="format"></param> format string
        /// <param name="args"></param> arguments
        void Write(Severity severity, string format, params object[] args);

        /// <summary>
        /// write a formatted message to the log with a soecific severity level
        /// </summary>
        /// <param name="severity"></param> severity level override
        /// <param name="format"></param> format string
        /// <param name="args"></param> arguments
        void Write(string format, params object[] args);

        /// <summary>
        /// write a message to the log with a sppecific severity level
        /// </summary>
        /// <param name="severity"></param> severity level override
        /// <param name="message"></param> message
        void Write(Severity severity, string message);

        /// <summary>
        /// write a message to the log
        /// </summary>
        /// <param name="message"></param> message
        void Write(string message);
    }
}
