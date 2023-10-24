// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class GetSearchResultsTest : BaseTest
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
        public async Task GetSearchResults_ReturnSearchResults()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var searchResult = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request).ConfigureAwait(false);
            token = searchResult.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            await Task.Delay(10000).ConfigureAwait(false);

            // Get search results
            var searchResultsResult = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token).ConfigureAwait(false);
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
            var operation = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request).ConfigureAwait(false);
            token = operation.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            await Task.Delay(10000).ConfigureAwait(false);

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

            await client.SearchesClient.GetSearchResultsForEachAsync(PagingCallback, RepositoryId, token, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
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
            var operation = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request).ConfigureAwait(false);
            token = operation.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            await Task.Delay(10000).ConfigureAwait(false);

            // Initial request
            var result = await client.SearchesClient.GetSearchResultsAsync(RepositoryId, token, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            Assert.IsNotNull(result);

            if (result.Value.Count <= maxPageSize)
            {
                return; // There's no point testing if the items do not need more than one page or do not exist.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.SearchesClient.GetSearchResultsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
