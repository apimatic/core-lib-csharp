using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core.Utilities.Logger
{
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Returns the shared instance of <see cref="T:Microsoft.Extensions.Logging.Abstractions.NullLogger" />.
        /// </summary>
        public static ConsoleLogger Instance { get; } = new ConsoleLogger();


        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Extensions.Logging.Abstractions.NullLogger" /> class.
        /// </summary>
        private ConsoleLogger()
        {
        }

        public IDisposable BeginScope<TState>(TState state) => 
            NullLogger.Instance.BeginScope(state);

        /// <inheritdoc />
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            
            LogEntry<TState> logEntry = new LogEntry<TState>(logLevel, "Console", eventId, state, exception, formatter);
            var logString = WriteLogString(logEntry);
            Console.WriteLine(logString);
        }

        private static string WriteLogString<TState>(LogEntry<TState> logEntry)
        {
            var stringWriter = new StringWriter();
            var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && message == null) return string.Empty;

            var logLevelString = GetLogLevelString(logEntry.LogLevel);
            if (logLevelString != null)
            {
                stringWriter.Write(logLevelString);
                stringWriter.Write(':');
            }
            stringWriter.Write(' ');
            stringWriter.Write(logEntry.Category);
            stringWriter.Write('[');
            stringWriter.Write(logEntry.EventId.Id.ToString());
            stringWriter.Write(']');

            // scope information
            if (!string.IsNullOrEmpty(message))
            {
                stringWriter.Write(' ');
                stringWriter.Write(message);
            }
            if (logEntry.Exception == null)
            {
                return stringWriter.ToString();
            }

            var exceptionMessage = logEntry.Exception.ToString();
            stringWriter.Write(' ');
            stringWriter.Write(exceptionMessage);
            return stringWriter.ToString();
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                case LogLevel.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }
}
