namespace PetProjects.Framework.Logging.Consumer.ElasticSearch.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Elasticsearch.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using PetProjects.Framework.Logging.Contracts;

    [TestClass]
    public class ElasticLogEventV1StoreTests
    {
        private ElasticLogEventV1Store target;
        private ElasticStoreConfiguration storeConfig;
        private Mock<IElasticLowLevelClient> clientMock;
        private PostData<object> callBack = null;

        [TestInitialize]
        public void Init()
        {
            this.storeConfig = new ElasticStoreConfiguration
            {
                AppLogsIndex = "appLogIndex"
            };

            this.clientMock = new Mock<IElasticLowLevelClient>();
            this.clientMock
                .Setup(cl => cl.Bulk<string>(It.IsAny<PostData<object>>(), It.IsAny<Func<BulkRequestParameters, BulkRequestParameters>>()))
                .Returns(new ElasticsearchResponse<string>(200, new List<int> { }))
                .Callback<PostData<object>, Func<BulkRequestParameters, BulkRequestParameters>>((c1, c2) =>
                {
                    this.callBack = c1;
                });
            this.target = new ElasticLogEventV1Store(this.clientMock.Object, this.storeConfig);
        }

        [TestMethod]
        public void Store_ValidLog_MapsCorrectlyToBulkApiFormat()
        {
            // Arrange
            var dt = DateTimeOffset.UtcNow;

            // Act
            this.target.Store(new List<LogEventV1> { new LogEventV1 { Type = "typ123", BatchId = "123", Timestamp = dt, Level = LogLevel.Error } });

            // Assert
            Assert.IsNotNull(this.callBack);
            var objects = (GetInstanceField<IEnumerable<object>>(this.callBack, "_enumerableOfObject") ?? throw new InvalidOperationException()).ToList();
            var firstObject = objects[0];
            var secondObject = objects[1];
            Assert.AreEqual("appLogIndex", GetInstanceProperty<string>(GetInstanceProperty<object>(firstObject, "index"), "_index"));
            Assert.AreEqual("typ123", GetInstanceProperty<string>(GetInstanceProperty<object>(firstObject, "index"), "_type"));
            Assert.AreEqual(dt.ToString("o"), GetInstanceProperty<string>(secondObject, "Timestamp"));
            Assert.AreEqual("Error", GetInstanceProperty<string>(secondObject, "LogLevel"));
            Assert.AreEqual("123", GetInstanceProperty<string>(secondObject, "BatchId"));
        }

        internal static T GetInstanceField<T>(object instance, string fieldName)
        {
            const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var field = instance.GetType().GetField(fieldName, BindFlags);
            return (T)field.GetValue(instance);
        }

        internal static T GetInstanceProperty<T>(object instance, string fieldName)
        {
            const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var prop = instance.GetType().GetProperty(fieldName, BindFlags);
            return (T)prop.GetValue(instance);
        }
    }
}