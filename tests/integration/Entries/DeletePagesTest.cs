using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class DeletePagesTest : BaseTest
    {
        int createdEntryId;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntryId = 0;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (createdEntryId != 0)
            {
                await DeleteEntry(createdEntryId).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task DeletePages()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net DeletePages";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            Assert.IsTrue(((Document)createdEntry).PageCount > 0);

            var result = await client.EntriesClient.DeletePagesAsync(new DeletePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.AreEqual(createdEntry.Id, result.Id);
            Assert.IsTrue(((Document)result).PageCount == 0);
        }
    }
}
