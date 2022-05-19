using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class DeleteEntryTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task DeleteEntry_ReturnOperationToken()
        {
            var deleteEntry = await CreateEntry(client, "APIServerClientIntegrationTest DeleteFolder");
            DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
            var result = await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, deleteEntry.Id, body);
            var token = result.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));
        }
    }
}
