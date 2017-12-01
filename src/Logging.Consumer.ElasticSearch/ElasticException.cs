namespace PetProjects.Framework.Logging.Consumer.ElasticSearch
{
    using System;
    using Elasticsearch.Net;

    public class ElasticException : Exception
    {
        private readonly ElasticsearchResponse<VoidResponse> response;

        public ElasticException(ElasticsearchResponse<VoidResponse> response) : 
            base(response.ToString() + "\n" + (response.DebugInformation ?? string.Empty) + "\n" + (response.OriginalException?.ToString() ?? string.Empty), response.OriginalException ?? new Exception())
        {
            this.response = response;
        }
    }
}