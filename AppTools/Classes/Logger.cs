using System;
using System.IO;

namespace Comdata.AppSupport.AppTools
{
    /// <summary>
    /// This will create and manage an application log.
    /// </summary>
    public class Logger
    {
        #region Fields

        StreamWriter _sw;
        string _logPath;
        string _logFilename;
        string _logPathname;
        bool _logIsOpen;
        bool _addTimeStamp;
        bool _addSeverityLevel         = false;
        Severity _loggingThreashold    = Severity.Info;
        Severity _currentSeverityLevel = Severity.Info;
        bool _keepLogOpen              = false;
        Object _logLock                = new object();
        int _writeWriteRetryCount      = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Sets or gets the location of the log.
        /// </summary>
        public string LogPath
        {
            get { return _logPath; }
            set { _logPath = value; }
        }

        /// <summary>
        /// gets the full log pathname
        /// </summary>
        public string LogPathname
        {
            get { return _logPathname; }
        }

        /// <summary>
        /// automatically timestamp log entries
        /// </summary>
        public bool AddTimeStamp
        {
            get { return _addTimeStamp; }
            set { _addTimeStamp = value; }
        }

        /// <summary>
        /// add Severity Level to log Entries
        /// </summary>
        public bool AddSeverityLevel
        {
            get { return _addSeverityLevel; }
            set { _addSeverityLevel = value; }
        }

        /// <summary>
        /// sets or gets the severity level logging threashold
        /// </summary>
        public Severity LoggingThreashold
        {
            get { return _loggingThreashold; }
            set { _loggingThreashold = value; }
        }

        /// <summary>
        /// sets or gets the current severity level
        /// </summary>
        public Severity CurrentSeverityLevel
        {
            get { return _currentSeverityLevel; }
            set { _currentSeverityLevel = value; }
        }

        /// <summary>
        /// Sets or gets whether or not the Log remains open after it has been updated.
        /// </summary>
        public bool KeepLogOpen
        {
            get { return _keepLogOpen; }
            set { _keepLogOpen = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// open an application log with a standard name *(ie: ApplicationName_yyyyMMdd.log)
        /// 
        /// <paramref name="logPath"/> The path where the lag will be saved  
        /// </summary>
        /// <param name="logPath"></param>
        public Logger(string logPath)
        {
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            _logPath = logPath;
            _logFilename = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            _logFilename = _logFilename.Replace(".exe", "");
            _logFilename = _logFilename.Replace(".EXE", "");
            _logFilename = _logFilename.Replace(".vshost", "");
            _logIsOpen = false;
            _addTimeStamp = false;
            _addSeverityLevel = false;
            _currentSeverityLevel = Severity.Info;
        }

        /// <summary>
        /// open an application log with a specicified path and filename *(ie: filename_yyyyMMdd.log)
        /// 
        /// <paramref name="logPath"/> The path where the lag will be saved  
        /// <paramref name="filename"/> The filename where the lag will be saved  
        /// </summary>
        /// <param name="logPath"></param>
        public Logger(string logPath, string logFilename)
        {
            if (!Directory.Exists(logPath))
                 Directory.CreateDirectory(logPath);
            
            _logPath = logPath;
            _logFilename = logFilename;
            _logIsOpen = false;
            _addTimeStamp = false;
            _addSeverityLevel = false;
            _currentSeverityLevel = Severity.Info;
        }

        /// <summary>
        /// write a formatted message to the log with a severity level
        /// </summary>
        /// <param name="severity"></param> severity level override
        /// <param name="format"></param> format string
        /// <param name="args"></param> arguments
        public void Write(Severity severity, string format, params object[] args)
        {
            var message = string.Format(format, args);
            Write(severity, message);
        }

        /// <summary>
        /// write a formatted message to the log with a soecific severity level
        /// </summary>
        /// <param name="severity"></param> severity level override
        /// <param name="format"></param> format string
        /// <param name="args"></param> arguments
        public void Write(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Write(_currentSeverityLevel, message);
        }

        /// <summary>
        /// write a message to the log with a sppecific severity level
        /// </summary>
        /// <param name="severity"></param> severity level override
        /// <param name="message"></param> message
        public void Write(Severity severity, string message)
        {
            if (severity < _loggingThreashold)
                return;

            if (message.Contains("\n") || message.Contains("\n"))
            {
                string[] lines = message.Split(new char[] { '\n', '\r' });
                foreach (var line in lines)
                    if (line.Length > 0)
                        Write(severity, line);
                return;
            }

            string logEntry;
            string shortLogEntry;
            buildLogEntry(severity, message, out logEntry, out shortLogEntry);

            lock (_logLock)
            {
                write(logEntry, shortLogEntry);
            }
        }

        /// <summary>
        /// write a message to the log
        /// </summary>
        /// <param name="message"></param> message
        public void Write(string message)
        {
            if (_currentSeverityLevel < _loggingThreashold)
                return;

            if (message.Contains("\n") || message.Contains("\n"))
            {
                string[] lines = message.Split(new char[] { '\n', '\r' });
                foreach (var line in lines)
                    if (line.Length > 0)
                        Write(line);
                return;
            }

            string logEntry;
            string shortLogEntry;
            buildLogEntry(_currentSeverityLevel, message, out logEntry, out shortLogEntry);
           
            lock (_logLock)
            {
                write(logEntry, shortLogEntry);
            }
        }

        /// <summary>
        /// Close Log
        /// </summary>
        public void Close()
        {
            if (this._logIsOpen)
            {
                lock (_logLock)
                {
                    _sw.Close();
                }

                _logIsOpen = false;
            }
        }

        #endregion

        #region Private Methods

        private void openLog()
        {
            var logFilename = string.Format("{0}_{1:yyyy-MM-dd}.log", _logFilename, DateTime.Now);
            _logPathname = Path.Combine(_logPath, logFilename);

            if (!Directory.Exists(_logPath))
                Directory.CreateDirectory(_logPath);

            try
            {
                lock (_logLock)
                {
                    _sw = new StreamWriter(_logPathname, true);
                    _sw.AutoFlush = true;
                }
            }

            catch (Exception ex)
            {
                var exMsg = string.Format("An Error has occrured while opening the application log: {0}.", _logFilename);
                throw new Exception(exMsg, ex);
            }

            _logIsOpen = true;
        }

        private void buildLogEntry(Severity severity, string message, out string logEntry, out string shortLogEntry)
        {
            logEntry = "";
            shortLogEntry = "";

            if (_addTimeStamp && _addSeverityLevel)
            {
                logEntry = String.Format("{0:MM/dd/yyyy hh:mm:ss tt} {1,-8}: {2}", DateTime.Now, severity.ToString("F"), message);
                shortLogEntry = String.Format("{0,-8}: {1}", severity.ToString("F"), message);
            }
            else
                if (!_addTimeStamp && _addSeverityLevel)
                {
                    logEntry = String.Format("{{0,-8}: {1}", severity.ToString("F"), message);
                    shortLogEntry = logEntry;
                }
                else
                    if (_addTimeStamp && !_addSeverityLevel)
                    {
                        logEntry = String.Format("{0:MM/dd/yyyy hh:mm:ss tt}: {1}", DateTime.Now, message);
                        shortLogEntry = message;
                    }
                    else
                    {
                        logEntry = message;
                        shortLogEntry = message;
                    }
        }

        private void write(string logEntry, string shortLogEntry)
        {
            if (_logIsOpen)
            {
                if ((File.GetCreationTime(_logPathname).Year != DateTime.Now.Year) ||
                    (File.GetCreationTime(_logPathname).DayOfYear != DateTime.Now.DayOfYear))
                {
                    Close();
                    openLog();
                }
            }
            else
                openLog();

            try
            {
                _sw.WriteLine(logEntry);
                OnLogUpdated(new LogUpdatedEventArgs(shortLogEntry));
                _writeWriteRetryCount = 0;
            }

            catch (Exception ex)
            {
                if (++_writeWriteRetryCount < 3)
                {
                    System.Threading.Thread.Sleep(5000);
                    write(logEntry, shortLogEntry);
                }
                else
                    throw new Exception("An Error has occrured while writing to the application log.", ex);
            }

            finally
            {
                if (_keepLogOpen)
                { }
                else
                    Close();
            }
        }

        #endregion

        #region Event Handlers
        public event EventHandler<LogUpdatedEventArgs> LogUpdated;

        protected virtual void OnLogUpdated(LogUpdatedEventArgs e)
        {
            var temp = LogUpdated;

            if (temp != null)
                temp(this, e);
        }
        #endregion
    }

    public enum Severity { Debug, Info, Warning, Error, Critical };
}
