namespace PetProjects.Framework.Logging.Producer
{
    using Serilog;
    using Serilog.Configuration;

    internal static class SerilogLoggerConfigurationExtensions
    {
        public static LoggerConfiguration Kafka(this LoggerSinkConfiguration loggerConfiguration, PeriodicSinkConfiguration sinkConfig, KafkaConfiguration kafkaConfig, string type)
        {
            var sink = new KafkaSink(sinkConfig, kafkaConfig, type);
            return loggerConfiguration.Sink(sink);
        }
    }
}