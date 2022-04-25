using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class GetSearchContextHitsTest : BaseTest_V1
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
        public async Task GetSearchContextHits_ReturnContextHits()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"*\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(5000);

            // Get search results
            var searchResultsResponse = await client.GetSearchResultsAsync(TestConfig.RepositoryId, token);
            var searchResults = searchResultsResponse.Result?.Value;
            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count > 0, "No search results found. Cannot get context hits.");
            int rowNumber = searchResults.First().RowNumber;

            // Get context hits
            var contextHitsResponse = await client.GetSearchContextHitsAsync(TestConfig.RepositoryId, token, rowNumber);
            var contextHits = contextHitsResponse.Result?.Value;
            Assert.IsNotNull(contextHits);
        }

        [TestMethod]
        public async Task GetSearchContextHits_Paging()
        {
            int maxMageSize = 10;

            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"*\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(5000);

            // Get search results
            var searchResultsResponse = await client.GetSearchResultsAsync(TestConfig.RepositoryId, token);
            var searchResults = searchResultsResponse.Result?.Value;
            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count > 0, "No search results found. Cannot get context hits.");
            int rowNumber = searchResults.First().RowNumber;

            bool PagingCallback(ODataValueContextOfIListOfContextHit data)
            {
                if (data.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Value.Count);
                    Assert.IsTrue(data.Value.Count <= maxMageSize);
                    return true; // If data aren't exhusted, keep asking.
                }
                else
                {
                    return false;
                }
            }

            await client.GetSearchContextHitsForEachAsync(PagingCallback, TestConfig.RepositoryId, token, rowNumber, string.Format("maxpagesize={0}", maxMageSize));
        }
    }
}
