using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class ListTasksTest : BaseTest
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
                await client.TasksClient.CancelTasksAsync(RepositoryId, new List<string> { taskId }).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ReturnTaskStatus()
        {
            // Create search
            var request = new StartSearchEntryRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var taskResponse = await client.SearchesClient.StartSearchEntryAsync(RepositoryId, request).ConfigureAwait(false);

            Assert.IsNotNull(taskResponse);

            taskId = taskResponse.TaskId;

            await Task.Delay(5000).ConfigureAwait(false);

            // Get search status
            var taskCollectionResponse = await client.TasksClient.ListTasksAsync(RepositoryId, new List<string> { taskId }).ConfigureAwait(false);

            AssertCollectionResponse(taskCollectionResponse);

            var searchTaskProgress = taskCollectionResponse.Value.First(t => t.Id == taskId);

            Assert.IsNotNull(searchTaskProgress);
        }
    }
}
