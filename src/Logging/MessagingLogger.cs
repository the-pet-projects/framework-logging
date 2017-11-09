namespace PetProjects.Framework.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    internal class MessagingLogger : ILogger
    {
        private LogLevel minimumLevel;

        public MessagingLogger(LogLevel minimumLevel)
        {
            this.minimumLevel = minimumLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= this.minimumLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}