namespace PetProjects.Framework.Logging.Consumer.ElasticSearch
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Elasticsearch.Net;

    public interface IElasticLowLevelClientFactory
    {
        Task<IElasticLowLevelClient> BuildAsync(ElasticClientConfiguration config, IEnumerable<string> indices);
    }
}