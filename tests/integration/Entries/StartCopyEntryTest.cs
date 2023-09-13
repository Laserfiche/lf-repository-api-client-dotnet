using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class StartCopyEntryTest : BaseTest
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
                    StartDeleteEntryRequest body = new();
                    await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, entry.Id, body).ConfigureAwait(false);
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
            var request = new CreateEntryRequest()
            {
                EntryType = CreateEntryRequestEntryType.Folder,
                Name = newEntryName,
                AutoRename = true
            };
            var targetEntry = await client.EntriesClient.CreateEntryAsync(RepositoryId, testFolder.Id, request).ConfigureAwait(false);
            
            Assert.IsNotNull(targetEntry);
            Assert.AreEqual(testFolder.Id, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Copy entry
            var copyRequest = new StartCopyEntryRequest()
            {
                Name = "RepositoryApiClientIntegrationTest .Net CopiedEntry",
                SourceId = targetEntry.Id,
                AutoRename = true
            };
            var copyResult = await client.EntriesClient.StartCopyEntryAsync(RepositoryId, testFolder.Id, copyRequest).ConfigureAwait(false);

            // Wait for the copy operation to finish
            await Task.Delay(5000).ConfigureAwait(false);
            var taskCollectionResponse = await client.TasksClient.ListTasksAsync(RepositoryId).ConfigureAwait(false);
            AssertCollectionResponse(taskCollectionResponse);
            Assert.IsNotNull(taskCollectionResponse.Value.First(t => t.Id == copyResult.TaskId && t.Status == TaskStatus.Completed));
        }

        [TestMethod]
        public async Task CreateCopyEntry_CopyShortcut()
        {
            // Create new entry
            string newEntryName = "RepositoryApiClientIntegrationTest .Net CreateFolder";
            int parentEntryId = 1;
            var createEntryRequest = new CreateEntryRequest()
            {
                EntryType = CreateEntryRequestEntryType.Folder,
                Name = newEntryName,
                AutoRename = true
            };
            var targetEntry = await client.EntriesClient.CreateEntryAsync(RepositoryId, parentEntryId, createEntryRequest).ConfigureAwait(false);
            
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Create shortcut to the new entry
            newEntryName = "RepositoryApiClientIntegrationTest .Net CreateShortcut";
            createEntryRequest = new CreateEntryRequest()
            {
                EntryType = CreateEntryRequestEntryType.Shortcut,
                Name = newEntryName,
                TargetId = targetEntry.Id,
                AutoRename = true
            };
            var shortcut = await client.EntriesClient.CreateEntryAsync(RepositoryId, parentEntryId, createEntryRequest).ConfigureAwait(false);

            Assert.IsNotNull(shortcut);
            createdEntries.Add(shortcut);
            Assert.AreEqual(parentEntryId, shortcut.ParentId);
            Assert.AreEqual(EntryType.Shortcut, shortcut.EntryType);

            // Copy entry
            var copyEntryRequest = new CopyEntryRequest()
            {
                Name = "RepositoryApiClientIntegrationTest .Net CopiedEntry",
                SourceId = shortcut.Id,
                AutoRename = true
            };
            var newEntry = await client.EntriesClient.CopyEntryAsync(RepositoryId, parentEntryId, copyEntryRequest).ConfigureAwait(false);

            createdEntries.Add(newEntry);
            Assert.IsTrue(newEntry.Name.StartsWith(createEntryRequest.Name));
            Assert.AreEqual(parentEntryId, newEntry.ParentId);
            Assert.AreEqual(shortcut.EntryType, newEntry.EntryType);
        }


        [ExpectedException(typeof(ApiException))]
        [TestMethod]
        public async Task CreateCopyEntry_CopyFolder()
        {
            // Create new entry
            string newEntryName = "RepositoryApiClientIntegrationTest .Net CreateFolder";
            int parentEntryId = 1;
            var createEntryRequest = new CreateEntryRequest()
            {
                EntryType = CreateEntryRequestEntryType.Folder,
                Name = newEntryName,
                AutoRename = true
            };
            var targetEntry = await client.EntriesClient.CreateEntryAsync(RepositoryId, parentEntryId, createEntryRequest).ConfigureAwait(false);
            
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Copy entry
            var copyEntryRequest = new CopyEntryRequest()
            {
                Name = "RepositoryApiClientIntegrationTest .Net CopiedEntry",
                SourceId = targetEntry.Id,
                AutoRename = true
            };
            _ = await client.EntriesClient.CopyEntryAsync(RepositoryId, parentEntryId, copyEntryRequest).ConfigureAwait(false);
        }
    }
}
