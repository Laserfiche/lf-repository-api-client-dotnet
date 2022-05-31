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
            var searchResult = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request);
            token = searchResult.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(10000);

            // Get search results
            var searchResultsResult = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token);
            var searchResults = searchResultsResult.Value;
            Assert.IsNotNull(searchResults);
        }

        [TestMethod]
        public async Task GetSearchResults_ForEachPaging()
        {
            int maxPageSize = 90;

            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"NLT\"})"
            };
            var operation = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request);
            token = operation.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(10000);

            Task<bool> PagingCallback(ODataValueContextOfIListOfEntry data)
            {
                if (data.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Value.Count);
                    Assert.IsTrue(data.Value.Count <= maxPageSize);
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }

            await client.SearchesClient.GetSearchResultsForEachAsync(PagingCallback, RepositoryId, token, maxPageSize: maxPageSize);
            await Task.Delay(5000);
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
            var operation = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request);
            token = operation.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(10000);

            // Initial request
            var result = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(result);

            if (result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.SearchesClient.GetSearchResultsNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
