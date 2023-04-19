using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class CopyEntryAsyncTest : BaseTest
    {
        IList<Entry> createdEntries;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntries = new List<Entry>();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            foreach (var entry in createdEntries)
            {
                if (entry != null)
                {
                    DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                    await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, entry.Id, body).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task CreateCopyEntry_CopyEntry()
        {
            // Create a new folder that contains the created entry
            var testFolderName = "RepositoryApiClientIntegrationTest .Net CreateCopyEntry_CopyEntry_test_folder";
            var testFolder = await CreateEntry(client, testFolderName).ConfigureAwait(false);
            createdEntries.Add(testFolder);

            // Create new entry
            string newEntryName = "RepositoryApiClientIntegrationTest .Net CreateFolder";
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = newEntryName
            };
            var targetEntry = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, testFolder.Id, request, autoRename: true).ConfigureAwait(false);
            Assert.IsNotNull(targetEntry);
            Assert.AreEqual(testFolder.Id, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Copy entry
            var copyRequest = new CopyAsyncRequest()
            {
                Name = "RepositoryApiClientIntegrationTest .Net CopiedEntry",
                SourceId = targetEntry.Id
            };
            var copyResult = await client.EntriesClient.CopyEntryAsync(RepositoryId, testFolder.Id, copyRequest, autoRename: true).ConfigureAwait(false);
            var opToken = copyResult.Token;

            // Wait for the copy operation to finish
            await Task.Delay(5000).ConfigureAwait(false);
            var opResponse = await client.TasksClient.GetOperationStatusAndProgressAsync(RepositoryId, opToken).ConfigureAwait(false);
            Assert.AreEqual(OperationStatus.Completed, opResponse.Status);
        }
    }
}
