namespace PetProjects.Framework.Logging.Consumer.ElasticSearch.Tests
{
    using System;
    using System.Threading.Tasks;
    using Elasticsearch.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ElasticLogEventV1StoreIntegrationTests
    {
        private ElasticClientConfiguration config;
        private ElasticLowLevelClientFactory target;
        private string index = "applogintegrationtest";

        [TestInitialize]
        public void Init()
        {
            this.config = new ElasticClientConfiguration
            {
                Address = "http://localhost:9200"
            };

            this.target = new ElasticLowLevelClientFactory();
        }
        
        [Ignore]
        [TestMethod]
        public async Task BuildClient_IndexDoesNotExist_CreatesIndex()
        {
            // Arrange
            var settings = new ConnectionConfiguration(new Uri(this.config.Address));
            var client = new ElasticLowLevelClient(settings);
            await client.IndicesDeleteAsync<VoidResponse>(this.index).ConfigureAwait(false);

            // Act
            await this.target.BuildAsync(this.config, new[]{ index }).ConfigureAwait(false);

            // Assert
            var getIndexResult = await client.IndicesGetAsync<byte[]>(this.index).ConfigureAwait(false);
            Assert.IsTrue(getIndexResult.Success);
        }

        [Ignore]
        [TestMethod]
        public async Task BuildClient_IndexDoesNotExist_CreatesIndexWithTimestampMapping()
        {
            // Arrange
            var settings = new ConnectionConfiguration(new Uri(this.config.Address));
            var client = new ElasticLowLevelClient(settings);
            await client.IndicesDeleteAsync<VoidResponse>(this.index).ConfigureAwait(false);

            // Act
            await this.target.BuildAsync(this.config, new[] { index }).ConfigureAwait(false);

            // Assert
            var getMapping = await client.IndicesGetMappingAsync<dynamic>(this.index, "_default_").ConfigureAwait(false);
            Assert.IsTrue(getMapping.Success);
            Assert.AreEqual("date", getMapping.Body.applogintegrationtest.mappings._default_.properties.Timestamp.type);
        }
    }
}