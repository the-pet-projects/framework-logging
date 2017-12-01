namespace PetProjects.Framework.Logging.Producer.Tests
{
    using System;
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using PetProjects.Framework.Logging.Contracts;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;

    [TestClass]
    public class LogEventExtensionsTests
    {
        private Logger log;
        private Mock<ILogEventSink> sinkMock;

        public LogEventExtensionsTests()
        {
            this.sinkMock = new Mock<ILogEventSink>();
            this.log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Sink(this.sinkMock.Object)
                .CreateLogger();
        }

        [TestMethod]
        public void LogError_ValidMessageWithProperties_MapsCorrectlyToLogEventV1()
        {
            // Arrange
            LogEvent result = null;
            this.sinkMock.Setup(x => x.Emit(It.IsAny<LogEvent>())).Callback<LogEvent>(e => result = e);
            var id = Guid.NewGuid();
            var dt = DateTime.UtcNow;
            this.log.Error("Getting item {ID} at {RequestTime}", id, dt);

            // Act
            var logEventV1 = result.BuildLogEventV1("typ1", "123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("typ1", logEventV1.Type);
            Assert.AreEqual("123", logEventV1.BatchId);
            Assert.AreEqual("Getting item {ID} at {RequestTime}", logEventV1.MessageTemplate);
            Assert.IsNull(logEventV1.Exception);
            Assert.AreEqual(LogLevel.Error, logEventV1.Level);
            Assert.AreEqual(result.Timestamp, logEventV1.Timestamp);
            Assert.AreEqual(string.Format("Getting item {0} at {1}", id, dt.ToString(DateTimeFormatInfo.InvariantInfo)), logEventV1.RenderedMessage);
            Assert.AreEqual(2, logEventV1.Properties.Count);
            Assert.AreEqual(id.ToString(), logEventV1.Properties["ID"]);
            Assert.AreEqual(dt.ToString(DateTimeFormatInfo.InvariantInfo), logEventV1.Properties["RequestTime"]);
        }

        [TestMethod]
        public void LogFatal_ValidMessageWithProperties_MapsLogLevelCorrectly()
        {
            // Arrange
            LogEvent result = null;
            this.sinkMock.Setup(x => x.Emit(It.IsAny<LogEvent>())).Callback<LogEvent>(e => result = e);
            this.log.Fatal("Getting item");

            // Act
            var logEventV1 = result.BuildLogEventV1("typ1", "123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(LogLevel.Fatal, logEventV1.Level);
        }

        [TestMethod]
        public void LogInfo_ValidMessageWithException_MapsExceptionCorrectly()
        {
            // Arrange
            LogEvent result = null;
            var ex = new Exception("123");
            this.sinkMock.Setup(x => x.Emit(It.IsAny<LogEvent>())).Callback<LogEvent>(e => result = e);
            this.log.Fatal(ex, "Getting item");

            // Act
            var logEventV1 = result.BuildLogEventV1("typ1", "123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ex, logEventV1.Exception);
        }
    }
}