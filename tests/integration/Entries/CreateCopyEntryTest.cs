// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class CreateCopyEntryTest : BaseTest
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
        public async Task CreateCopyEntry_CreateFolder()
        {
            string newEntryName = "RepositoryApiClientIntegrationTest .Net CreateFolder";
            int parentEntryId = 1;
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = newEntryName
            };

            var entry = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: true).ConfigureAwait(false);
            Assert.IsNotNull(entry);
            createdEntries.Add(entry);
            Assert.AreEqual(parentEntryId, entry.ParentId);
            Assert.AreEqual(EntryType.Folder, entry.EntryType);
            Assert.AreEqual(typeof(Folder), entry.GetType());
        }

        [TestMethod]
        public async Task CreateCopyEntry_CreateShortcut()
        {
            // Create new entry
            string newEntryName = "RepositoryApiClientIntegrationTest .Net CreateFolder";
            int parentEntryId = 1;
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = newEntryName
            };
            var targetEntry = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: true).ConfigureAwait(false);
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Create shortcut to the new entry
            newEntryName = "RepositoryApiClientIntegrationTest .Net CreateShortcut";
            request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Shortcut,
                Name = newEntryName,
                TargetId = targetEntry.Id
            };
            var shortcut = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: true).ConfigureAwait(false);
            Assert.IsNotNull(shortcut);
            createdEntries.Add(shortcut);
            Assert.AreEqual(parentEntryId, shortcut.ParentId);
            Assert.AreEqual(EntryType.Shortcut, shortcut.EntryType);
            Assert.AreEqual(typeof(Shortcut), shortcut.GetType());
            Assert.AreEqual(targetEntry.EntryType, ((Shortcut)shortcut).TargetType);
            Assert.AreEqual(targetEntry.Id, ((Shortcut)shortcut).TargetId);
        }


        [TestMethod]
        public async Task CreateCopyEntry_CopyShortcut()
        {
            // Create new entry
            string newEntryName = "RepositoryApiClientIntegrationTest .Net CreateFolder";
            int parentEntryId = 1;
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = newEntryName
            };
            var targetEntry = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: true).ConfigureAwait(false);
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);
            
            // Create shortcut to the new entry
            newEntryName = "RepositoryApiClientIntegrationTest .Net CreateShortcut";
            request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Shortcut,
                Name = newEntryName,
                TargetId = targetEntry.Id
            };
            var shortcut = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: true).ConfigureAwait(false);

            Assert.IsNotNull(shortcut);
            createdEntries.Add(shortcut);
            Assert.AreEqual(parentEntryId, shortcut.ParentId);
            Assert.AreEqual(EntryType.Shortcut, shortcut.EntryType);

            // Copy entry
            request = new PostEntryChildrenRequest()
            {
                Name = "RepositoryApiClientIntegrationTest .Net CopiedEntry",
                SourceId = shortcut.Id
            };
            var newEntry = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: true).ConfigureAwait(false);
            
            createdEntries.Add(newEntry);
            Assert.IsTrue(newEntry.Name.StartsWith(request.Name));
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
            var request = new PostEntryChildrenRequest()
            {
                EntryType = PostEntryChildrenEntryType.Folder,
                Name = newEntryName
            };
            var targetEntry = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: true).ConfigureAwait(false);
            Assert.IsNotNull(targetEntry);
            createdEntries.Add(targetEntry);
            Assert.AreEqual(parentEntryId, targetEntry.ParentId);
            Assert.AreEqual(EntryType.Folder, targetEntry.EntryType);

            // Copy entry
            request = new PostEntryChildrenRequest()
            {
                Name = "RepositoryApiClientIntegrationTest .Net CopiedEntry",
                SourceId = targetEntry.Id
            };
            _ = await client.EntriesClient.CreateOrCopyEntryAsync(RepositoryId, parentEntryId, request, autoRename: true).ConfigureAwait(false);
        }
    }
}
