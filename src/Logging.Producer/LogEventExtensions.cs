namespace PetProjects.Framework.Logging.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PetProjects.Framework.Logging.Contracts;
    using Serilog.Events;

    public static class LogEventExtensions
    {
        private static IDictionary<LogEventLevel, LogLevel> mappingsDictionary;

        static LogEventExtensions()
        {
            LogEventExtensions.mappingsDictionary = new Dictionary<LogEventLevel, LogLevel>
            {
                { LogEventLevel.Fatal, LogLevel.Fatal },
                { LogEventLevel.Error, LogLevel.Error },
                { LogEventLevel.Warning, LogLevel.Warning },
                { LogEventLevel.Information, LogLevel.Information },
                { LogEventLevel.Verbose, LogLevel.Verbose },
                { LogEventLevel.Debug, LogLevel.Debug }
            };
        }

        public static LogEventV1 BuildLogEventV1(this LogEvent @this)
        {
            return new LogEventV1
            {
                Exception = @this.Exception,
                Level = @this.Level.MapToLogLevel(),
                MessageTemplate = @this.MessageTemplate.Text,
                Timestamp = @this.Timestamp,
                RenderedMessage = @this.MessageTemplate.Render(@this.Properties),
                Properties = @this.Properties.ToDictionary(kv => kv.Key, kv => kv.Value.ToString())
            };
        }

        private static LogLevel MapToLogLevel(this LogEventLevel @this)
        {
            if (LogEventExtensions.mappingsDictionary.TryGetValue(@this, out var value))
            {
                return value;
            }

            throw new ArgumentException("LogEventLevel not supported");
        }
    }
}