using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class MoveEntryTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;
        IList<Entry> createdEntries;

        [TestInitialize]
        public async Task Initialize()
        {
            createdEntries = new List<Entry>();
            client = await CreateClientAndLogin();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            foreach (var entry in createdEntries)
            {
                if (entry != null)
                {
                    DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                    await client.DeleteEntryInfoAsync(TestConfig.RepositoryId, entry.Id, body);
                    Thread.Sleep(5000);
                }
            }
            await Logout(client);
        }

        [TestMethod]
        public async Task MoveAndRenameEntry_ReturnEntry()
        {
            var parentFolder = await CreateEntry(client, "APIServerClientIntegrationTest ParentFolder");
            createdEntries.Add(parentFolder);
            var childFolder = await CreateEntry(client, "APIServerClientIntegrationTest ChildFolder");
            createdEntries.Add(childFolder);
            var request = new PatchEntryRequest()
            {
                ParentId = parentFolder.Id,
                Name = "APIServerClientIntegrationTest MovedFolder"
            };

            var response = await client.MoveOrRenameDocumentAsync(TestConfig.RepositoryId, childFolder.Id, request, autoRename: true);

            var movedEntry = response.Result;
            Assert.IsNotNull(movedEntry);
            Assert.AreEqual(childFolder.Id, movedEntry.Id);
            Assert.AreEqual(parentFolder.Id, movedEntry.ParentId);
            Assert.AreEqual(request.Name, movedEntry.Name);
            Assert.AreEqual(typeof(Folder), movedEntry.GetType());
        }
    }
}
