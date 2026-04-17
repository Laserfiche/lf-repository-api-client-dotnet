// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [Ignore("Temporarily ignored: cloud test server not yet updated with V2 endpoints")]
    [TestClass]
    public class LockDocumentTest : BaseTest
    {
        int createdEntryId;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntryId = 0;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (createdEntryId != 0)
            {
                await DeleteEntry(createdEntryId).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task LockDocument_ThenGetLockInfo_ThenUnlock()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net LockDocument";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Lock
            var lockResult = await client.EntriesClient.LockDocumentAsync(new LockDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new LockDocumentRequest() { Comment = "client test lock", Extent = "All" }
            }).ConfigureAwait(false);

            Assert.IsNotNull(lockResult);
            Assert.IsTrue(lockResult.IsActive);
            Assert.IsNotNull(lockResult.LockToken);
            Assert.AreEqual("client test lock", lockResult.Comment);
            Assert.AreEqual(createdEntryId, lockResult.EntryId);

            // Get lock info
            var lockInfo = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(lockInfo);
            Assert.IsTrue(lockInfo.IsActive);
            Assert.AreEqual(lockResult.LockToken, lockInfo.LockToken);

            // Unlock
            await client.EntriesClient.UnlockDocumentAsync(new UnlockDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            // Verify unlocked
            var afterUnlock = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsFalse(afterUnlock.IsActive);
        }

        [TestMethod]
        public async Task UnlockDocument_ByToken()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net UnlockByToken";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var lockResult = await client.EntriesClient.LockDocumentAsync(new LockDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new LockDocumentRequest()
            }).ConfigureAwait(false);

            // Unlock by token
            await client.EntriesClient.UnlockDocumentAsync(new UnlockDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                LockToken = lockResult.LockToken
            }).ConfigureAwait(false);

            var afterUnlock = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsFalse(afterUnlock.IsActive);
        }

        [TestMethod]
        public async Task GetDocumentLockInfo_NotLocked_ReturnsInactive()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetLockNotLocked";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var lockInfo = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(lockInfo);
            Assert.IsFalse(lockInfo.IsActive);
        }
    }
}
