using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Tasks
{
    [TestClass]
    public class GetOperationStatusTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetOperationStatus_ReturnStatus()
        {
            var deleteEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net GetOperationStatus");

            DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
            var result = await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, deleteEntry.Id, body);
            var token = result.Token;
            Assert.IsFalse(string.IsNullOrEmpty(token));

            await Task.Delay(5000);

            var operationProgress = await client.TasksClient.GetOperationStatusAndProgressAsync(RepositoryId, token);
            Assert.IsNotNull(operationProgress);
            Assert.AreEqual(OperationStatus.Completed, operationProgress.Status);
            Assert.AreEqual(100, operationProgress.PercentComplete);
        }
    }
}
