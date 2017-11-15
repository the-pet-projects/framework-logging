namespace PetProjects.Framework.Logging.Producer
{
    using System;
    using Serilog;
    using Serilog.Configuration;

    internal static class SerilogLoggerConfigurationExtensions
    {
        public static LoggerConfiguration Kafka(this LoggerSinkConfiguration loggerConfiguration, PeriodicSinkConfiguration sinkConfig, KafkaConfiguration kafkaConfig)
        {
            var sink = new KafkaSink(sinkConfig, kafkaConfig);
            return loggerConfiguration.Sink(sink);
        }
    }
}