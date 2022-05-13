﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class MoveEntryTest : BaseTest
    {
        IRepositoryApiClient client = null;
        IList<Entry> createdEntries;

        [TestInitialize]
        public void Initialize()
        {
            createdEntries = new List<Entry>();
            client = CreateClient();
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

            var response = await client.EntriesClient.MoveOrRenameDocumentAsync(RepositoryId, childFolder.Id, request, autoRename: true);

            var movedEntry = response.Result;
            Assert.IsNotNull(movedEntry);
            Assert.AreEqual(childFolder.Id, movedEntry.Id);
            Assert.AreEqual(parentFolder.Id, movedEntry.ParentId);
            Assert.AreEqual(request.Name, movedEntry.Name);
            Assert.AreEqual(typeof(Folder), movedEntry.GetType());
        }
    }
}
