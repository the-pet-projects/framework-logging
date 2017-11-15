namespace PetProjects.Framework.Logging.Producer.Tests
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            new MessagingLoggingProvider().CreateLogger("asd").LogCritical("asd", "asd");
        }
    }
}