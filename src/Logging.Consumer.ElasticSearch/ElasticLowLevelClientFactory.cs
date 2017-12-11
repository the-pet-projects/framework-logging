namespace PetProjects.Framework.Logging.Consumer.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Elasticsearch.Net;

    public class ElasticLowLevelClientFactory : IElasticLowLevelClientFactory
    {
        public async Task<IElasticLowLevelClient> BuildAsync(ElasticClientConfiguration config, IEnumerable<string> indices)
        {
            var settings = new ConnectionConfiguration(new Uri(config.Address));
            var client = new ElasticLowLevelClient(settings);
            var errors = new List<Exception>();

            foreach (var index in indices)
            {
                var indexExists = await client.IndicesExistsAsync<string>(index).ConfigureAwait(false);

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

                    var response = await client.IndicesCreateAsync<VoidResponse>(index, postData).ConfigureAwait(false);

                    if (!response.Success)
                    {
                        errors.Add(new ElasticException(response));
                    }
                }
            }

            if (errors.Count > 0)
            {
                throw new AggregateException(errors);
            }

            return client;
        }
    }
}