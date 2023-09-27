using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class StartSearchEntryTest : BaseTest
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
        public async Task ReturnTaskId()
        {
            var request = new StartSearchEntryRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var taskResponse = await client.SearchesClient.StartSearchEntryAsync(new StartSearchEntryParameters()
            {
                RepositoryId = RepositoryId,
                Request = request
            }).ConfigureAwait(false);

            AssertIsNotNullOrEmpty(taskResponse.TaskId);
        }
    }
}
