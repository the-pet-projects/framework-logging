namespace PetProjects.Framework.Logging.Producer
{
    using System;
    using System.Linq;
    using PetProjects.Framework.Logging.Contracts;
    using Serilog.Events;

    public static class LogEventExtensions
    {
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
            switch (@this)
            {
                case LogEventLevel.Fatal:
                    return LogLevel.Fatal;

                case LogEventLevel.Error:
                    return LogLevel.Error;

                case LogEventLevel.Warning:
                    return LogLevel.Warning;

                case LogEventLevel.Information:
                    return LogLevel.Information;

                case LogEventLevel.Verbose:
                    return LogLevel.Verbose;

                case LogEventLevel.Debug:
                    return LogLevel.Debug;

                default:
                    throw new ArgumentException("LogEventLevel not supported");
            }
        }
    }
}