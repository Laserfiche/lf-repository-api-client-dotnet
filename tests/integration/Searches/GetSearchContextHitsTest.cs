using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class GetSearchContextHitsTest : BaseTest
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
        public async Task GetSearchContextHits_ReturnContextHits()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"*\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(5000);

            // Get search results
            var searchResultsResponse = await client.SearchesClient.GetSearchResultsAsync(TestConfig.RepositoryId, token);
            var searchResults = searchResultsResponse.Result?.Value;
            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count > 0, "No search results found. Cannot get context hits.");
            int rowNumber = searchResults.First().RowNumber;

            // Get context hits
            var contextHitsResponse = await client.SearchesClient.GetSearchContextHitsAsync(TestConfig.RepositoryId, token, rowNumber);
            var contextHits = contextHitsResponse.Result?.Value;
            Assert.IsNotNull(contextHits);
        }

        [TestMethod]
        public async Task GetSearchContextHits_ForEachPaging()
        {
            int maxPageSize = 10;

            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"*\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(5000);

            // Get search results
            var searchResultsResponse = await client.SearchesClient.GetSearchResultsAsync(TestConfig.RepositoryId, token);
            var searchResults = searchResultsResponse.Result?.Value;
            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count > 0, "No search results found. Cannot get context hits.");
            int rowNumber = searchResults.First().RowNumber;

            bool PagingCallback(SwaggerResponse<ODataValueContextOfIListOfContextHit> data)
            {
                if (data.Result.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Result.Value.Count);
                    Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            await client.SearchesClient.GetSearchContextHitsForEachAsync(PagingCallback, TestConfig.RepositoryId, token, rowNumber, maxPageSize: maxPageSize);
        }
    }
}
