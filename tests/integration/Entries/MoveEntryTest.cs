// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class MoveEntryTest : BaseTest
    {
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
                    await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, entry.Id, body).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task MoveAndRenameEntry_ReturnEntry()
        {
            var parentFolder = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net ParentFolder").ConfigureAwait(false);
            createdEntries.Add(parentFolder);
            var childFolder = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net ChildFolder").ConfigureAwait(false);
            createdEntries.Add(childFolder);
            var request = new PatchEntryRequest()
            {
                ParentId = parentFolder.Id,
                Name = "RepositoryApiClientIntegrationTest .Net MovedFolder"
            };

            var movedEntry = await client.EntriesClient.MoveOrRenameEntryAsync(RepositoryId, childFolder.Id, request, autoRename: true).ConfigureAwait(false);

            Assert.IsNotNull(movedEntry);
            Assert.AreEqual(childFolder.Id, movedEntry.Id);
            Assert.AreEqual(parentFolder.Id, movedEntry.ParentId);
            Assert.AreEqual(request.Name, movedEntry.Name);
            Assert.AreEqual(typeof(Folder), movedEntry.GetType());
        }
    }
}
