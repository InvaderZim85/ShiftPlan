using System;
using NLog;

namespace ShiftPlan.Core
{
    /// <summary>
    /// Provides several logging functions
    /// </summary>
    public static class ServiceLogger
    {
        /// <summary>
        /// Contains the instance of the NLog logger
        /// </summary>
        private static readonly Logger Log = LogManager.GetLogger("*");

        /// <summary>
        /// Logs an info message
        /// </summary>
        /// <param name="message">The message</param>
        public static void Info(string message)
        {
            var log = new LogEventInfo
            {
                Level = LogLevel.Info,
                Message = message
            };

            WriteLog(log);
        }

        /// <summary>
        /// Logs a warn message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">The exception (optional)</param>
        public static void Warn(string message, Exception ex = null)
        {
            var log = new LogEventInfo
            {
                Level = LogLevel.Warn,
                Message = message,
                Exception = ex
            };

            WriteLog(log);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ex">The exception (optional)</param>
        public static void Error(string message, Exception ex = null)
        {
            WriteLog(new LogEventInfo
            {
                Level = LogLevel.Error,
                Message = message,
                Exception = ex
            });
        }

        /// <summary>
        /// Writes the log
        /// </summary>
        /// <param name="log">The log data</param>
        private static void WriteLog(LogEventInfo log)
        {
            Log.Log(log);
        }
    }
}
