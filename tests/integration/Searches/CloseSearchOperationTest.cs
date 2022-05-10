using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class CloseSearchOperationTest : BaseTest
    {
        IRepositoryApiClient client = null;
        string token;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
            token = "";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            Thread.Sleep(5000);
            await Logout(client);
        }

        [TestMethod]
        public async Task CloseSearchOperation_CloseSearch()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            // Close search
            var response = await client.SearchesClient.CancelOrCloseSearchAsync(TestConfig.RepositoryId, token);
            Assert.AreEqual(true, response.Result?.Value);
        }
    }
}
