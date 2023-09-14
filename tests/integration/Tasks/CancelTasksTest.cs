using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Tasks
{
    [TestClass]
    public class CancelTasksTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task OperationEndedBeforeCancel()
        {
            var deleteEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net CancelOperation").ConfigureAwait(false);
            StartDeleteEntryRequest body = new();
            var taskResponse = await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, deleteEntry.Id, body).ConfigureAwait(false);
            
            AssertIsNotNullOrEmpty(taskResponse.TaskId);

            try
            {
                await Task.Delay(5000).ConfigureAwait(false);
                await client.TasksClient.CancelTasksAsync(RepositoryId, new List<string> { taskResponse.TaskId }).ConfigureAwait(false);
                
                Assert.Fail("Long operation should have ended before cancel.");
            }
            catch (ApiException e)
            {
                Assert.IsTrue(e.ProblemDetails.Title.Contains("Cannot cancel completed operation"));
            }
        }
    }
}
