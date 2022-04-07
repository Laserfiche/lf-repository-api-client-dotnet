using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Tasks
{
    [TestClass]
    public class CancelOpeartionTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await Logout(client);
        }

        [TestMethod]
        public async Task CancelOpeartion_OperationEndedBeforeCancel()
        {
            var deleteEntry = await CreateEntry(client, "APIServerClientIntegrationTest CancelOperation");
            DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
            var response = await client.DeleteEntryInfoAsync(TestConfig.RepositoryId, deleteEntry.Id, body);
            var token = response.Result?.Token;
            Assert.IsFalse(string.IsNullOrEmpty(token));

            try
            {
                Thread.Sleep(5000);
                await client.CancelOperationAsync(TestConfig.RepositoryId, token);
                Assert.Fail("Long operation should have ended before cancel.");
            }
            catch (ApiException<ProblemDetails> e)
            {
                Assert.IsTrue(e.Result.Title.Contains("Cannot cancel ended operation"));
            }
        }
    }
}
