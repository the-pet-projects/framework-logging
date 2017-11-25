namespace PetProjects.Framework.Logging.Consumer.ElasticSearch
{
    using System.Collections.Generic;
    using Elasticsearch.Net;
    using PetProjects.Framework.Logging.Contracts;

    public class ElasticLogEventV1Store : ILogEventV1Store
    {
        private readonly IElasticLowLevelClient client;
        private readonly ElasticStoreConfiguration storeConfig;

        public ElasticLogEventV1Store(IElasticLowLevelClient client, ElasticStoreConfiguration storeConfig)
        {
            this.client = client;
            this.storeConfig = storeConfig;
        }

        public void Store(List<LogEventV1> logs)
        {
            var logList = new List<object>();

            foreach (var log in logs)
            {
                logList.Add(new { index = new { _index = this.storeConfig.AppLogsIndex, _type = log.Type } });
                var doc = new
                {
                    Timestamp = log.Timestamp.ToString("o"),
                    LogLevel = log.Level.ToString(),
                    MessageTemplate = log.MessageTemplate,
                    RenderedMessage = log.RenderedMessage,
                    Exception = log.Exception,
                    BatchId = log.BatchId,
                    Data = log.Properties
                };
                logList.Add(doc);
            }

            var indexResponse = this.client.Bulk<string>(logList.ToArray());
            var success = indexResponse.Success;

            if (!success)
            {
                throw new ElasticException(indexResponse);
            }
        }
    }
}