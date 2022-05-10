using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class GetSearchStatusTest : BaseTest
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
            if (!string.IsNullOrEmpty(token))
            {
                await client.SearchesClient.CancelOrCloseSearchAsync(TestConfig.RepositoryId, token);
                Thread.Sleep(5000);
            }
            await Logout(client);
        }

        [TestMethod]
        public async Task GetSearchStatus_ReturnSearchStatus()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(5000);

            // Get search status
            var searchStatusResponse = await client.SearchesClient.GetSearchStatusAsync(TestConfig.RepositoryId, token);
            var searchStatus = searchStatusResponse.Result;
            Assert.IsNotNull(searchStatus);
            Assert.AreEqual(token, searchStatus.OperationToken);
        }
    }
}
