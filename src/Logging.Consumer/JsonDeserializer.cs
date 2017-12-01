namespace PetProjects.Framework.Logging.Consumer
{
    using System.Text;
    using Confluent.Kafka.Serialization;
    using Newtonsoft.Json;

    internal class JsonDeserializer<T> : IDeserializer<T>
    {
        private readonly IDeserializer<string> stringDeserializer;

        public JsonDeserializer()
        {
            this.stringDeserializer = new StringDeserializer(Encoding.UTF8);
        }

        public T Deserialize(byte[] data)
        {
            var str = this.stringDeserializer.Deserialize(data);
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}