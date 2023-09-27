using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
                    await DeleteEntry(entry.Id).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task CopyShortcut()
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
            var targetEntry = await client.EntriesClient.CreateEntryAsync(new CreateEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                Request = createEntryRequest
            }).ConfigureAwait(false);

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
            var shortcut = await client.EntriesClient.CreateEntryAsync(new CreateEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                Request = createEntryRequest
            }).ConfigureAwait(false);

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
            var newEntry = await client.EntriesClient.CopyEntryAsync(new CopyEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                Request = copyEntryRequest
            }).ConfigureAwait(false);

            createdEntries.Add(newEntry);
            Assert.AreEqual(EntryType.Shortcut, newEntry.EntryType);
            Assert.AreEqual(parentEntryId, newEntry.ParentId);
            Assert.AreEqual(shortcut.EntryType, newEntry.EntryType);
        }

        [ExpectedException(typeof(ApiException))]
        [TestMethod]
        public async Task CopyFolder()
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
            var targetEntry = await client.EntriesClient.CreateEntryAsync(new CreateEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                Request = createEntryRequest
            }).ConfigureAwait(false);

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
            _ = await client.EntriesClient.CopyEntryAsync(new CopyEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                Request = copyEntryRequest
            }).ConfigureAwait(false);
        }
    }
}
