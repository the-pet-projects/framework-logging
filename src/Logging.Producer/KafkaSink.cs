namespace PetProjects.Framework.Logging.Producer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Confluent.Kafka;
    using PetProjects.Framework.Logging.Contracts;

    using Serilog.Events;
    using Serilog.Sinks.PeriodicBatching;

    internal class KafkaSink : PeriodicBatchingSink
    {
        private readonly Producer<Null, List<LogEventV1>> producer;
        private readonly string topic;

        public KafkaSink(PeriodicSinkConfiguration sinkConfig, KafkaConfiguration kafkaConfig)
            : base(sinkConfig.BatchSizeLimit, sinkConfig.Period)
        {
            var config = new Dictionary<string, object> { { "bootstrap.servers", string.Join(",", kafkaConfig.Brokers) } };
            this.topic = kafkaConfig.Topic;
            this.producer = new Producer<Null, List<LogEventV1>>(config, null, new JsonSerializer<List<LogEventV1>>());
        }

        protected override void Dispose(bool disposing)
        {
            this.producer.Dispose();
            base.Dispose(disposing);
        }

        protected override Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            return this.producer.ProduceAsync(this.topic, null, events.Select(e => e.BuildLogEventV1()).ToList(), false);
        }
    }
}