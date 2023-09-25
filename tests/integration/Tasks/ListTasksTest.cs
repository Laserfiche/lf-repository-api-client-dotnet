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
        public async Task ReturnStatus()
        {
            var deleteEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net GetOperationStatus").ConfigureAwait(false);

            StartDeleteEntryRequest request = new();
            var result = await client.EntriesClient.StartDeleteEntryAsync(new StartDeleteEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = deleteEntry.Id,
                Request = request
            }).ConfigureAwait(false);
            
            Assert.IsFalse(string.IsNullOrEmpty(result.TaskId));

            await Task.Delay(5000).ConfigureAwait(false);

            var taskCollectionResponse = await client.TasksClient.ListTasksAsync(new ListTasksParameters()
            {
                RepositoryId = RepositoryId,
                TaskIds = new [] { result.TaskId }
            }).ConfigureAwait(false);

            AssertCollectionResponse(taskCollectionResponse);

            var task = taskCollectionResponse.Value.First(t => t.Id == result.TaskId);

            Assert.IsNotNull(task);
            Assert.AreEqual(TaskStatus.Completed, task.Status);
            Assert.AreEqual(100, task.PercentComplete);
        }
    }
}
