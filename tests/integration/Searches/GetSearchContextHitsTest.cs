﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public async Task GetSearchContextHits_ReturnContextHits()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"*\", option=\"DFANLT\"})"
            };
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(5000);

            // Get search results
            var searchResultsResponse = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token);
            var searchResults = searchResultsResponse.Result?.Value;
            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count > 0, "No search results found. Cannot get context hits.");
            int rowNumber = searchResults.First().RowNumber;

            // Get context hits
            var contextHitsResponse = await client.SearchesClient.GetSearchContextHitsAsync(RepositoryId, token, rowNumber);
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
            var searchResponse = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request);
            token = searchResponse.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            Thread.Sleep(5000);

            // Get search results
            var searchResultsResponse = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token);
            var searchResults = searchResultsResponse.Result?.Value;
            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count > 0, "No search results found. Cannot get context hits.");
            int rowNumber = searchResults.First().RowNumber;

            Task<bool> PagingCallback(SwaggerResponse<ODataValueContextOfIListOfContextHit> data)
            {
                Assert.AreNotEqual(0, data.Result.Value.Count);
                Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                return Task.FromResult(true);
            }

            await client.SearchesClient.GetSearchContextHitsForEachAsync(PagingCallback, RepositoryId, token, rowNumber, maxPageSize: maxPageSize);
        }
    }
}
