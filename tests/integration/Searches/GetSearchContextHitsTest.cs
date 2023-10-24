// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class GetSearchContextHitsTest : BaseTest
    {
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
                await client.SearchesClient.CancelOrCloseSearchAsync(RepositoryId, token).ConfigureAwait(false);
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
            var operation = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request).ConfigureAwait(false);
            token = operation.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            await Task.Delay(5000).ConfigureAwait(false);

            // Get search results
            var searchResultsResponse = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token).ConfigureAwait(false);
            var searchResults = searchResultsResponse.Value;
            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count > 0, "No search results found. Cannot get context hits.");
            int rowNumber = searchResults.First().RowNumber;

            // Get context hits
            var contextHitsResponse = await client.SearchesClient.GetSearchContextHitsAsync(RepositoryId, token, rowNumber).ConfigureAwait(false);
            var contextHits = contextHitsResponse.Value;
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
            var operation = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request).ConfigureAwait(false);
            token = operation.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            await Task.Delay(5000).ConfigureAwait(false);

            // Get search results
            var searchResultsResponse = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token).ConfigureAwait(false);
            var searchResults = searchResultsResponse.Value;
            Assert.IsNotNull(searchResults);
            Assert.IsTrue(searchResults.Count > 0, "No search results found. Cannot get context hits.");
            int rowNumber = searchResults.First().RowNumber;

            Task<bool> PagingCallback(ODataValueContextOfIListOfContextHit data)
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

            await client.SearchesClient.GetSearchContextHitsForEachAsync(PagingCallback, RepositoryId, token, rowNumber, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }
    }
}
