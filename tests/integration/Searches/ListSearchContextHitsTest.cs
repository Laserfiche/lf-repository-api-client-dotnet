using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class ListSearchContextHitsTest : BaseTest
    {
        string taskId;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            taskId = "";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (!string.IsNullOrEmpty(RepositoryId))
            {
                await client.TasksClient.CancelTasksAsync(RepositoryId).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ReturnContextHits()
        {
            // Create search
            var request = new StartSearchEntryRequest()
            {
                SearchCommand = "({LF:Basic ~= \"*\", option=\"DFANLT\"})"
            };
            var taskResponse = await client.SearchesClient.StartSearchEntryAsync(RepositoryId, request).ConfigureAwait(false);
            taskId = taskResponse.TaskId;

            AssertIsNotNullOrEmpty(taskId);

            await Task.Delay(5000).ConfigureAwait(false);

            // Get search results
            var entryCollectionResponse = await client.SearchesClient.ListSearchResultsAsync(RepositoryId, taskId).ConfigureAwait(false);

            AssertCollectionResponse(entryCollectionResponse);

            var searchResults = entryCollectionResponse.Value;

            Assert.IsNotNull(searchResults.First().RowNumber);

            var rowNumber = (int)searchResults.First().RowNumber;

            // Get context hits
            var contextHitsCollectionResponse = await client.SearchesClient.ListSearchContextHitsAsync(RepositoryId, taskId, rowNumber).ConfigureAwait(false);

            Assert.IsNotNull(contextHitsCollectionResponse);
        }

        [TestMethod]
        public async Task ForEachPaging()
        {
            int maxPageSize = 10;

            // Create search
            var request = new StartSearchEntryRequest()
            {
                SearchCommand = "({LF:Basic ~= \"*\", option=\"DFANLT\"})"
            };
            
            var taskResponse = await client.SearchesClient.StartSearchEntryAsync(RepositoryId, request).ConfigureAwait(false);
            taskId = taskResponse.TaskId;

            AssertIsNotNullOrEmpty(taskId);

            await Task.Delay(5000).ConfigureAwait(false);

            // Get search results
            var entryCollectionResponse = await client.SearchesClient.ListSearchResultsAsync(RepositoryId, taskId).ConfigureAwait(false);
            var searchResults = entryCollectionResponse.Value;

            AssertCollectionResponse(entryCollectionResponse);
            Assert.IsNotNull(searchResults.First().RowNumber);

            var rowNumber = (int)searchResults.First().RowNumber;

            Task<bool> PagingCallback(SearchContextHitCollectionResponse data)
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

            await client.SearchesClient.ListSearchContextHitsForEachAsync(PagingCallback, RepositoryId, taskId, rowNumber, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }
    }
}
