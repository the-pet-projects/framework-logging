namespace PetProjects.Framework.Logging.Consumer
{
    using System.Collections.Generic;

    public class KafkaConfiguration
    {
        public IEnumerable<string> Brokers { get; set; }

        public string Topic { get; set; }

        public string ConsumerGroupId { get; set; }
    }
}