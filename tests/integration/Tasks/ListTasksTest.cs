using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Tasks
{
    [TestClass]
    public class ListTasksTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ListTasks_ReturnStatus()
        {
            var deleteEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net GetOperationStatus").ConfigureAwait(false);

            StartDeleteEntryRequest request = new();
            var result = await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, deleteEntry.Id, request).ConfigureAwait(false);
            
            Assert.IsFalse(string.IsNullOrEmpty(result.TaskId));

            await Task.Delay(5000).ConfigureAwait(false);

            var taskCollectionResponse = await client.TasksClient.ListTasksAsync(RepositoryId).ConfigureAwait(false);

            AssertCollectionResponse(taskCollectionResponse);

            var task = taskCollectionResponse.Value.First(t => t.Id == result.TaskId);

            Assert.IsNotNull(task);
            Assert.AreEqual(TaskStatus.Completed, task.Status);
            Assert.AreEqual(100, task.PercentComplete);
        }
    }
}
