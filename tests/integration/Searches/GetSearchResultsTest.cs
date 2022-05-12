using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class GetSearchResultsTest : BaseTest
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
        public async Task GetSearchResults_ReturnSearchResults()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(10000);

            // Get search results
            var searchResultsResponse = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token);
            var searchResults = searchResultsResponse.Result?.Value;
            Assert.IsNotNull(searchResults);
        }

        [TestMethod]
        public async Task GetSearchResults_ForEachPaging()
        {
            int maxPageSize = 10;

            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"NLT\"})"
            };
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(10000);

            Task<bool> PagingCallback(SwaggerResponse<ODataValueContextOfIListOfODataGetSearchResults> data)
            {
                Assert.AreNotEqual(0, data.Result.Value.Count);
                Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                return Task.FromResult(true);
            }

            await client.SearchesClient.GetSearchResultsForEachAsync(PagingCallback, RepositoryId, token, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetSearchResults_SimplePaging()
        {
            int maxPageSize = 1;

            // Create search first
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"NLT\"})"
            };
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(10000);

            // Initial request
            var response = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.SearchesClient.GetSearchResultsNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
