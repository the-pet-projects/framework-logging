namespace PetProjects.Framework.Logging.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PetProjects.Framework.Logging.Contracts;
    using Serilog.Events;

    using NetCoreLogLevel = Microsoft.Extensions.Logging.LogLevel;

    public static class LogEventExtensions
    {
        private static IDictionary<LogEventLevel, LogLevel> mappingsDictionary;
        private static IDictionary<LogEventLevel, NetCoreLogLevel> netCoreMappingsDictionary;

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

            LogEventExtensions.netCoreMappingsDictionary = new Dictionary<LogEventLevel, NetCoreLogLevel>
            {
                { LogEventLevel.Fatal, NetCoreLogLevel.Critical },
                { LogEventLevel.Error, NetCoreLogLevel.Error },
                { LogEventLevel.Warning, NetCoreLogLevel.Warning },
                { LogEventLevel.Information, NetCoreLogLevel.Information },
                { LogEventLevel.Verbose, NetCoreLogLevel.Trace },
                { LogEventLevel.Debug, NetCoreLogLevel.Debug }
            };
        }

        public static LogEventV1 BuildLogEventV1(this LogEvent @this, string type, string batchId, string instanceId)
        {
            return new LogEventV1
            {
                BatchId = batchId,
                Type = type,
                Exception = @this.Exception,
                Level = @this.Level.MapToLogLevel(),
                MessageTemplate = @this.MessageTemplate.Text,
                Timestamp = @this.Timestamp,
                RenderedMessage = @this.MessageTemplate.Render(@this.Properties),
                Properties = @this.Properties.ToDictionary(kv => kv.Key, kv => kv.Value.ToString()),
                InstanceId = instanceId
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


        public static NetCoreLogLevel MapToNetCoreLogLevel(this LogEventLevel @this)
        {
            if (LogEventExtensions.netCoreMappingsDictionary.TryGetValue(@this, out var value))
            {
                return value;
            }

            throw new ArgumentException("LogEventLevel not supported");
        }
    }
}