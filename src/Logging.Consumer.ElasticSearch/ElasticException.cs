namespace PetProjects.Framework.Logging.Consumer.ElasticSearch
{
    using System;
    using Elasticsearch.Net;

    public class ElasticException : Exception
    {
        private readonly ElasticsearchResponse<VoidResponse> response;

        public ElasticException(ElasticsearchResponse<VoidResponse> response) : base("Elastic Error")
        {
            this.response = response;
        }
    }
}