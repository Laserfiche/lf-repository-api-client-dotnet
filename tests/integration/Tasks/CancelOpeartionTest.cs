using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Tasks
{
    [TestClass]
    public class CancelOpeartionTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task CancelOpeartion_OperationEndedBeforeCancel()
        {
            var deleteEntry = await CreateEntry(client, "APIServerClientIntegrationTest CancelOperation");
            DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
            var result = await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, deleteEntry.Id, body);
            var token = result.Token;
            Assert.IsFalse(string.IsNullOrEmpty(token));

            try
            {
                Thread.Sleep(5000);
                await client.TasksClient.CancelOperationAsync(RepositoryId, token);
                Assert.Fail("Long operation should have ended before cancel.");
            }
            catch (ApiException<ProblemDetails> e)
            {
                Assert.IsTrue(e.Result.Title.Contains("Cannot cancel ended operation"));
            }
        }
    }
}
