using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using Object = System.Object;

// #define LOG_HEADER

namespace SciRev
{
    // A message logging class to replace Debug.Log
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Logger
    {
        // Is the logger initialized?
        private static readonly Boolean IsInitialized;

        // Logger output path
        public static String LogDirectory
        {
            get { return KSPUtil.ApplicationRootPath + "Logs/" + typeof (Logger).Assembly.GetName().Name + "/"; }
        }

        // ==> Implement own version
        #if LOG_HEADER
        private static String Version
        {
            get { return typeof(Logger).Assembly.GetName().Name;  }
        }
        #endif

        // Default logger
        private static Logger _defaultLogger;

        public static Logger Default
        {
            get
            {
                if (_defaultLogger == null) {
                    _defaultLogger = new Logger(typeof(Logger).Assembly.GetName().Name);
                    Debug.Log("[Kopernicus] Default logger initialized as " + typeof(Logger).Assembly.GetName().Name);
                }
                return _defaultLogger;
            }
        }

        // Currently active logger
        private static Logger _activeLogger;

        public static Logger Active
        {
            get
            {
                if (_activeLogger._loggerStream == null)
                    return _defaultLogger;
                return _activeLogger;
            }
            private set { _activeLogger = value; }
        }

        // The complete path of this log
        private TextWriter _loggerStream;

        // Write text to the log
        public void Log(Object o)
        {
            if (_loggerStream == null)
                return;

            _loggerStream.WriteLine("[LOG " + DateTime.Now.ToString("HH:mm:ss") + "]: " + o);
            _loggerStream.Flush();
        }

        // Write text to the log
        public void LogException(Exception e)
        {
            if (_loggerStream == null)
                return;

            _loggerStream.WriteLine("[LOG " + DateTime.Now.ToString("HH:mm:ss") + "]: Exception Was Recorded: " +
                                   e.Message + "\n" + e.StackTrace);

            if (e.InnerException != null)
                _loggerStream.WriteLine("[LOG " + DateTime.Now.ToString("HH:mm:ss") + "]: Inner Exception Was Recorded: " +
                                       e.InnerException.Message + "\n" + e.InnerException.StackTrace);
            _loggerStream.Flush();
        }

        // Set logger as the active logger
        public void SetAsActive()
        {
            Active = this;
        }

        public void Flush()
        {
            _loggerStream?.Flush();
        }

        // Close the logger
        public void Close()
        {
            if (_loggerStream == null)
                return;

            _loggerStream.Flush();
            _loggerStream.Close();
            _loggerStream = null;
        }

        // Create a logger
        public Logger(String logFileName = null)
        {
            SetFilename(logFileName);
        }

        // Set/Change the filename we log to
        public void SetFilename(String logFileName)
        {
            Close();

            if (!IsInitialized)
                return;

            if (String.IsNullOrEmpty(logFileName))
                return; //effectively makes this logger a black hole

            try
            {
                // Open the log file (overwrite existing logs)
                String logFile = LogDirectory + logFileName + ".log";
                Directory.CreateDirectory(Path.GetDirectoryName(logFile) ?? throw new InvalidOperationException());
                _loggerStream = new StreamWriter(logFile);

                // Write an opening message
                #if LOG_HEADER
                String logVersion = "//=====  " + Version + "  =====//";
                String logHeader = new string('=', logVersion.Length - 4);
                logHeader = "//" + logHeader + "//";

                // Don't use Log() because we don't want a date time in front of the Versioning.
                _loggerStream.WriteLine(logHeader + "\n" + logVersion + "\n" + logHeader);
                #endif
                Log("Logger \"" + logFileName + "\" was created");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        // Cleanup the logger
        ~Logger()
        {
            Close();
        }

        // Initialize the Logger (i.e. delete old logs) 
        static Logger()
        {
            // Attempt to clean the log directory
            try
            {
                if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);

                // Clear out the old log files
                foreach(String file in Directory.GetFiles(LogDirectory))
                {
                    File.Delete(file);
                }
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
                return;
            }

            IsInitialized = true;
        }
    }
}

