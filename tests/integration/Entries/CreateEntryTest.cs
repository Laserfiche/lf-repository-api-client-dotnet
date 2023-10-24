// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class CreateEntryTest : BaseTest
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
                    await client.EntriesClient.StartDeleteEntryAsync(new StartDeleteEntryParameters()
                    {
                        RepositoryId = RepositoryId,
                        EntryId = entry.Id,
                        Request = body
                    }).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task CreateFolder()
        {
            string newEntryName = "RepositoryApiClientIntegrationTest .Net CreateFolder";
            int parentEntryId = 1;
            var request = new CreateEntryRequest()
            {
                EntryType = CreateEntryRequestEntryType.Folder,
                Name = newEntryName,
                AutoRename = true
            };

            var entry = await client.EntriesClient.CreateEntryAsync(new CreateEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                Request = request
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(entry);
            createdEntries.Add(entry);
            Assert.AreEqual(parentEntryId, entry.ParentId);
            Assert.AreEqual(EntryType.Folder, entry.EntryType);
            Assert.AreEqual(typeof(Folder), entry.GetType());
        }

        [TestMethod]
        public async Task CreateShortcut()
        {
            // Create new entry
            string newEntryName = "RepositoryApiClientIntegrationTest .Net CreateFolder";
            int parentEntryId = 1;
            var request = new CreateEntryRequest()
            {
                EntryType = CreateEntryRequestEntryType.Folder,
                Name = newEntryName,
                AutoRename = true
            };
            var targetEntry = await client.EntriesClient.CreateEntryAsync(new CreateEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                Request = request
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Create shortcut to the new entry
            newEntryName = "RepositoryApiClientIntegrationTest .Net CreateShortcut";
            request = new CreateEntryRequest()
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
                Request = request
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(shortcut);
            createdEntries.Add(shortcut);
            Assert.AreEqual(parentEntryId, shortcut.ParentId);
            Assert.AreEqual(EntryType.Shortcut, shortcut.EntryType);
            Assert.AreEqual(typeof(Shortcut), shortcut.GetType());
            Assert.AreEqual(targetEntry.EntryType, ((Shortcut)shortcut).TargetType);
            Assert.AreEqual(targetEntry.Id, ((Shortcut)shortcut).TargetId);
        }
    }
}
