using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class StartDeleteEntryTest : BaseTest
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
            StartDeleteEntryRequest body = new StartDeleteEntryRequest();
            var response = await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, deleteEntry.Id, body).ConfigureAwait(false);
            Assert.IsTrue(!string.IsNullOrEmpty(response.TaskId));
        }
    }
}
