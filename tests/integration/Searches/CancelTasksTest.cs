using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class CancelTasksTest : BaseTest
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
                await client.TasksClient.CancelTasksAsync(taskId).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task CancelTasks_CancelSearchTasks()
        {
            // Create search
            var request = new StartSearchEntryRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var taskResponse = await client.SearchesClient.StartSearchEntryAsync(RepositoryId, request).ConfigureAwait(false);

            AssertIsNotNullOrEmpty(taskResponse.TaskId);
        }
    }
}
