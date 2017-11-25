namespace PetProjects.Framework.Logging.Producer
{
    using System;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Events;

    public static class PetProjectLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddPetProjectLogging(this ILoggingBuilder builder, LogEventLevel minLevel, PeriodicSinkConfiguration sinkConfig, KafkaConfiguration kafkaConfig, string type, bool dispose = false)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var logger = new LoggerConfiguration()
                .MinimumLevel.Is(minLevel)
                .Enrich.FromLogContext()
                .WriteTo.Kafka(sinkConfig, kafkaConfig, type)
                .CreateLogger();

            builder.AddSerilog(logger, dispose);

            return builder;
        }
    }
}