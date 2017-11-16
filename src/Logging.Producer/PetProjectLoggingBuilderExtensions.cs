namespace PetProjects.Framework.Logging.Producer
{
    using System;
    using Microsoft.Extensions.Logging;
    using Serilog;

    public static class PetProjectLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddPetProjectLogging(this ILoggingBuilder builder, PeriodicSinkConfiguration sinkConfig, KafkaConfiguration kafkaConfig, bool dispose = false)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Kafka(sinkConfig, kafkaConfig)
                .CreateLogger();

            builder.AddSerilog(logger, dispose);

            return builder;
        }
    }
}