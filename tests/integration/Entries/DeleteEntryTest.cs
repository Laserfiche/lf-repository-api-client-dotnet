using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class DeleteEntryTest : BaseTest_V1
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
            Thread.Sleep(5000);
            await Logout(client);
        }

        [TestMethod]
        public async Task DeleteEntry_ReturnOperationToken()
        {
            var deleteEntry = await CreateEntry(client, "APIServerClientIntegrationTest DeleteFolder");
            DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
            var response = await client.DeleteEntryInfoAsync(TestConfig.RepositoryId, deleteEntry.Id, body);
            var token = response.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));
            var redirectUrl = response.Headers["Location"].ToList()[0];
            Assert.IsNotNull(redirectUrl);
            Assert.IsTrue(redirectUrl.Contains(token));
        }
    }
}
