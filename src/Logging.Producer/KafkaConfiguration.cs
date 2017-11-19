namespace PetProjects.Framework.Logging.Producer
{
    using System.Collections.Generic;

    public class KafkaConfiguration
    {
        public IEnumerable<string> Brokers { get; set; }

        public string Topic { get; set; }
    }
}