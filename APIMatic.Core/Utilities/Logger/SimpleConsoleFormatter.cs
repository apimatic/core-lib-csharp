using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core.Utilities.Logger
{
    internal sealed class SimpleConsoleFormatter
    {
        private const string LoglevelPadding = ": ";

        public static void Write<TState>(in LogEntry<TState> logEntry, TextWriter textWriter)
        {
            var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && message == null)
            {
                return;
            }

            var logLevel = logEntry.LogLevel;
            var logLevelString = GetLogLevelString(logLevel);
            var dateTimeOffset = DateTimeOffset.Now;
            textWriter.Write(dateTimeOffset.ToString("s"));
            if (logLevelString != null)
            {
                textWriter.Write(logLevelString);
            }

            CreateDefaultLogMessage(textWriter, logEntry, message);
        }

        private static void CreateDefaultLogMessage<TState>(TextWriter textWriter, in LogEntry<TState> logEntry,
            string message)
        {
            var eventId = logEntry.EventId.Id;
            var exception = logEntry.Exception;

            // Example:
            // info: ConsoleApp.Program[10]
            //       Request received

            // category and event id
            textWriter.Write(LoglevelPadding);
            textWriter.Write(logEntry.Category);
            textWriter.Write('[');

            textWriter.Write(eventId.ToString());

            textWriter.Write(']');

            // scope information
            WriteMessage(textWriter, message);

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                WriteMessage(textWriter, exception.ToString());
            }

            textWriter.Write(Environment.NewLine);
        }

        private static void WriteMessage(TextWriter textWriter, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            textWriter.Write(' ');
            var newMessage = message.Replace(Environment.NewLine, " ");
            textWriter.Write(newMessage);
        }

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
