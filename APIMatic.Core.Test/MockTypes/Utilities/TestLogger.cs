using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace APIMatic.Core.Test.MockTypes.Utilities
{
    internal class TestLogger : ILogger
    {
        public IList<LogEntry> LoggedMessages { get; } = new List<LogEntry>();

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            LoggedMessages.Add(new LogEntry(logLevel, message));
        }
        
        internal class LogEntry
        {
            public LogLevel LogLevel { get; }
            public string Message { get; }
            
            public LogEntry(LogLevel logLevel, string message)
            {
                LogLevel = logLevel;
                Message = message;
            }
        }
    }
}
