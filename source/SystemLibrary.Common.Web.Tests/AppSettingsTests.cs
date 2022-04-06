using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.HttpBaseClientTests
{
    partial class HttpBaseClientTests
    {
        [TestMethod]
        public void AppSettingsTests()
        {
            AppSettingsConfig config = new AppSettingsConfig();

            Assert.IsTrue(config.SystemLibraryCommonWeb.HttpBaseClient != null, "Null");
            Assert.IsTrue(config.SystemLibraryCommonWeb.HttpBaseClient.TimeoutMilliseconds == 60000, "Not 60k");

            config = AppSettingsConfig.Current;

            Assert.IsTrue(config.SystemLibraryCommonWeb.HttpBaseClient != null, "Current null");
            Assert.IsTrue(config.SystemLibraryCommonWeb.HttpBaseClient.TimeoutMilliseconds == 20000, "Not 20k");
            Assert.IsTrue(config.SystemLibraryCommonWeb.HttpBaseClient.RetryRequestTimeoutSeconds == 8, "Not 8");
            Assert.IsTrue(config.SystemLibraryCommonWeb.HttpBaseClient.CacheClientConnectionSeconds == 100, "Not 100");
        }
    }
}
