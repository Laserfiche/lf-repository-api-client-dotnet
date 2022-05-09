using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class GetSearchResultsTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;
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
                await client.CancelOrCloseSearchAsync(TestConfig.RepositoryId, token);
                Thread.Sleep(5000);
            }
            await Logout(client);
        }

        [TestMethod]
        public async Task GetSearchResults_ReturnSearchResults()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(10000);

            // Get search results
            var searchResultsResponse = await client.GetSearchResultsAsync(TestConfig.RepositoryId, token);
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
            var searchResponse = await client.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(10000);

            bool PagingCallback(SwaggerResponse<ODataValueContextOfIListOfODataGetEntryChildren> data)
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

            await client.GetSearchResultsForEachAsync(PagingCallback, TestConfig.RepositoryId, token, maxPageSize: maxPageSize);
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
            var searchResponse = await client.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(10000);

            // Initial request
            var response = await client.GetSearchResultsAsync(TestConfig.RepositoryId, token, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.GetEntryListingNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
