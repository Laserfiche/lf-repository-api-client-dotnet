using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class CopyEntryTest : BaseTest
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
                    StartDeleteEntryRequest body = new StartDeleteEntryRequest();
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
            var opToken = copyResult.TaskId;

            // Wait for the copy operation to finish
            await Task.Delay(5000).ConfigureAwait(false);
            var taskCollectionResponse = await client.TasksClient.ListTasksAsync(RepositoryId).ConfigureAwait(false);
            AssertCollectionResponse(taskCollectionResponse);
            Assert.IsNotNull(taskCollectionResponse.Value.First(t => t.Id == opToken && t.Status == TaskStatus.Completed));
        }
    }
}
