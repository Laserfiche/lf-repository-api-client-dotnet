using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryByPathTest : BaseTest
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
        public async Task ReturnRootFolder()
        {
            int entryId = 1;
            string fullPath = "\\";
            var result = await client.EntriesClient.GetEntryByPathAsync(new GetEntryByPathParameters()
            {
                RepositoryId = RepositoryId,
                FullPath = fullPath
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(result.Entry);
            Assert.IsNull(result.AncestorEntry);
            Assert.AreEqual(typeof(Folder), result.Entry.GetType());
            Assert.AreEqual(entryId, result.Entry.Id);
            Assert.AreEqual(fullPath, result.Entry.FullPath);
            Assert.AreEqual(EntryType.Folder, result.Entry.EntryType);
        }
    }
}
