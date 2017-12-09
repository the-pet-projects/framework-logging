namespace PetProjects.Framework.Logging.Producer
{
    using System.Collections.Generic;
    using System.Text;
    using Confluent.Kafka.Serialization;
    using Newtonsoft.Json;

    internal class JsonSerializer<T> : ISerializer<T>
    {
        private readonly ISerializer<string> stringSerializer;

        public JsonSerializer()
        {
            this.stringSerializer = new StringSerializer(Encoding.UTF8);
        }

        public byte[] Serialize(string topic, T data)
        {
            return this.stringSerializer.Serialize(topic, JsonConvert.SerializeObject(data));
        }

        public IEnumerable<KeyValuePair<string, object>> Configure(IEnumerable<KeyValuePair<string, object>> config, bool isKey)
        {
            return config;
        }
    }
}