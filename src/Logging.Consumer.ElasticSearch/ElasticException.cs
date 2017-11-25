namespace PetProjects.Framework.Logging.Consumer.ElasticSearch
{
    using System;
    using Elasticsearch.Net;

    public class ElasticException : Exception
    {
        private readonly ElasticsearchResponse<string> response;

        public ElasticException(ElasticsearchResponse<string> response) : base("Elastic Error")
        {
            this.response = response;
        }
    }
}