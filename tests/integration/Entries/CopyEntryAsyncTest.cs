using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class CopyEntryAsyncTest : BaseTest
    {
        IRepositoryApiClient client = null;
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
                    await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, entry.Id, body);
                }
            }
        }

        [TestMethod]
        public async Task CreateCopyEntry_CopyEntry()
        {
            // Create a new folder that contains the created entry
            var testFolderName = "CreateCopyEntry_CopyEntry_test_folder";
            var testFolder = await CreateEntry(client, testFolderName);
            createdEntries.Add(testFolder);

            // Create new entry
            string newEntryName = "APIServerClientIntegrationTest CreateFolder";
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = newEntryName
            };
            var targetEntry = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, testFolder.Id, request, autoRename: true);
            Assert.IsNotNull(targetEntry);
            Assert.AreEqual(testFolder.Id, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Copy entry
            var copyRequest = new CopyAsyncRequest()
            {
                Name = "CopiedEntry",
                SourceId = targetEntry.Id
            };
            var copyResult = await client.EntriesClient.CopyEntryAsync(RepositoryId, testFolder.Id, copyRequest, autoRename: true);
            var opToken = copyResult.Token;

            // Wait for the copy operation to finish
            await Task.Delay(5000);
            var opResponse = await client.TasksClient.GetOperationStatusAndProgressAsync(RepositoryId, opToken);
            Assert.AreEqual(OperationStatus.Completed, opResponse.Status);
        }
    }
}
