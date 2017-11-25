namespace PetProjects.Framework.Logging.Consumer.ElasticSearch
{
    using System;
    using System.Threading.Tasks;
    using Elasticsearch.Net;

    public class ElasticLowLevelClientFactory : IElasticLowLevelClientFactory
    {
        public async Task<IElasticLowLevelClient> BuildAsync(ElasticClientConfiguration config)
        {
            var settings = new ConnectionConfiguration(new Uri(config.Address));
            var client = new ElasticLowLevelClient(settings);
            var indexExists = await client.IndicesExistsAsync<string>(config.AppLogsIndex).ConfigureAwait(false);

            if (indexExists.HttpStatusCode == null || indexExists.HttpStatusCode.Value == 404)
            {
                var postData = new
                {
                    mappings = new
                    {
                        _default_ = new
                        {
                            properties = new
                            {
                                Timestamp = new
                                {
                                    type = "date"
                                }
                            }
                        }
                    }
                };

                var response = await client.IndicesCreateAsync<string>(config.AppLogsIndex, postData).ConfigureAwait(false);

                if (!response.Success)
                {
                    throw new ElasticException(response);
                }
            }

            return client;
        }
    }
}