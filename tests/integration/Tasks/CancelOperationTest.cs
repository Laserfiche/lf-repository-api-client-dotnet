using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Tasks
{
    [TestClass]
    public class CancelOperationTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task CancelOpeartion_OperationEndedBeforeCancel()
        {
            var deleteEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net CancelOperation").ConfigureAwait(false);
            DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
            var result = await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, deleteEntry.Id, body).ConfigureAwait(false);
            var token = result.Token;
            Assert.IsFalse(string.IsNullOrEmpty(token));

            try
            {
                await Task.Delay(5000).ConfigureAwait(false);
                await client.TasksClient.CancelOperationAsync(RepositoryId, token).ConfigureAwait(false);
                Assert.Fail("Long operation should have ended before cancel.");
            }
            catch (ApiException e)
            {
                Assert.IsTrue(e.ProblemDetails.Title.Contains("Cannot cancel completed operation"));
            }
        }
    }
}
