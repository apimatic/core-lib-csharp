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


            var tStringWriter = new StringWriter();
            LogEntry<TState> logEntry = new LogEntry<TState>(logLevel, "Console", eventId, state, exception, formatter);
            SimpleConsoleFormatter.Write(in logEntry, tStringWriter);

            var sb = tStringWriter.GetStringBuilder();
            if (sb.Length == 0)
            {
                return;
            }

            string computedAnsiString = sb.ToString();
            sb.Clear();
            if (sb.Capacity > 1024)
            {
                sb.Capacity = 1024;
            }

            Console.WriteLine(computedAnsiString);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;
    }
}
