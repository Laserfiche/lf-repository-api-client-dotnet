using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class ListSearchResultsTest : BaseTest
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
            if (!string.IsNullOrEmpty(taskId))
            {
                await client.TasksClient.CancelTasksAsync(new CancelTasksParameters()
                {
                    RepositoryId = RepositoryId,
                    TaskIds = new [] { taskId }
                }).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ForEachPaging()
        {
            int maxPageSize = 90;

            // Create search
            var request = new StartSearchEntryRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"NLT\"})"
            };
            
            var taskResponse = await client.SearchesClient.StartSearchEntryAsync(new StartSearchEntryParameters()
            {
                RepositoryId = RepositoryId,
                Request = request
            }).ConfigureAwait(false);
            
            taskId = taskResponse.TaskId;
            
            AssertIsNotNullOrEmpty(taskId);

            await Task.Delay(10000).ConfigureAwait(false);

            Task<bool> PagingCallback(EntryCollectionResponse data)
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

            await client.SearchesClient.ListSearchResultsForEachAsync(PagingCallback, new ListSearchResultsParameters()
            {
                RepositoryId = RepositoryId,
                TaskId = taskId
            }, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SimplePaging()
        {
            int maxPageSize = 1;

            // Create search first
            var request = new StartSearchEntryRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"NLT\"})"
            };
            var taskResponse = await client.SearchesClient.StartSearchEntryAsync(new StartSearchEntryParameters()
            {
                RepositoryId = RepositoryId,
                Request = request
            }).ConfigureAwait(false);
            taskId = taskResponse.TaskId;
            
            Assert.IsTrue(!string.IsNullOrEmpty(taskId));
            AssertIsNotNullOrEmpty(taskId);

            await Task.Delay(10000).ConfigureAwait(false);

            // Initial request
            var entryCollectionResponse = await client.SearchesClient.ListSearchResultsAsync(new ListSearchResultsParameters()
            {
                RepositoryId = RepositoryId,
                TaskId = taskId,
                Prefer = $"maxpagesize={maxPageSize}"
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(entryCollectionResponse);

            if (entryCollectionResponse.Value.Count <= maxPageSize)
            {
                return; // There's no point testing if the items do not need more than one page or do not exist.
            }

            var nextLink = entryCollectionResponse.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(entryCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            entryCollectionResponse = await client.SearchesClient.ListSearchResultsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(entryCollectionResponse);
            Assert.IsTrue(entryCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
