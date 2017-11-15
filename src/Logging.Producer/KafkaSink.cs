namespace PetProjects.Framework.Logging.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Serilog.Events;
    using Serilog.Sinks.PeriodicBatching;

    internal class KafkaSink : PeriodicBatchingSink
    {
        public KafkaSink(PeriodicSinkConfiguration sinkConfig, KafkaConfiguration kafkaConfig) : base(sinkConfig.BatchSizeLimit, sinkConfig.Period)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            throw new NotImplementedException();
        }

        protected override Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            throw new NotImplementedException();
        }
    }
}