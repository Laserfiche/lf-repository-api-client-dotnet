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
        public void Initialize()
        {
            client = CreateClient();
            token = "";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (!string.IsNullOrEmpty(token))
            {
                await client.SearchesClient.CancelOrCloseSearchAsync(RepositoryId, token);
            }
        }

        [TestMethod]
        public async Task GetSearchStatus_ReturnSearchStatus()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(5000);

            // Get search status
            var searchStatusResponse = await client.SearchesClient.GetSearchStatusAsync(RepositoryId, token);
            var searchStatus = searchStatusResponse.Result;
            Assert.IsNotNull(searchStatus);
            Assert.AreEqual(token, searchStatus.OperationToken);
        }
    }
}
