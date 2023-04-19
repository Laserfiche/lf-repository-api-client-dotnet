using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class DeleteEntryTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task DeleteEntry_ReturnOperationToken()
        {
            var deleteEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net DeleteFolder").ConfigureAwait(false);
            DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
            var result = await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, deleteEntry.Id, body).ConfigureAwait(false);
            var token = result.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));
        }
    }
}
