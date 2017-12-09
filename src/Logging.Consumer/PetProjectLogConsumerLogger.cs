namespace PetProjects.Framework.Logging.Consumer
{
    using System;
    using Microsoft.Extensions.Logging;

    public class PetProjectLogConsumerLogger : IPetProjectLogConsumerLogger
    {
        private readonly ILogger logger;

        public PetProjectLogConsumerLogger(ILoggerProvider loggerProvider)
        {
            this.logger = loggerProvider.CreateLogger("PetProjectLogFallbackLogger");
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return this.logger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this.logger.BeginScope<TState>(state);
        }
    }
}