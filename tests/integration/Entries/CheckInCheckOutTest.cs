// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [Ignore("Temporarily ignored until lf-repository-api-client-dotnet preview is published to Nuget.org after server deploys")]
    [TestClass]
    public class CheckInCheckOutTest : BaseTest
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
        public async Task PutUnderVersionControl_ThenCheckOut_ThenCheckIn()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CICO";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Put under version control
            var vcResult = await client.EntriesClient.PutUnderVersionControlAsync(new PutUnderVersionControlParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(vcResult);
            Assert.AreEqual(createdEntryId, vcResult.Id);

            // Check out with lock
            var checkOutResult = await client.EntriesClient.CheckOutDocumentAsync(new CheckOutDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CheckOutDocumentRequest() { Lock = true, Comment = "client test checkout" }
            }).ConfigureAwait(false);

            Assert.IsNotNull(checkOutResult);
            Assert.AreEqual(createdEntryId, checkOutResult.Id);

            // Verify new fields while checked out
            var whileCheckedOut = (Document)await client.EntriesClient.GetEntryAsync(new GetEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);
            Assert.IsTrue(whileCheckedOut.IsCheckedOut);
            Assert.IsNotNull(whileCheckedOut.CheckedOutBy, "CheckedOutBy should be populated");
            Assert.IsFalse(whileCheckedOut.CheckedOutBy.StartsWith("S-1-"), "CheckedOutBy should be a display name, not a SID");
            Assert.IsFalse(whileCheckedOut.IsCheckedOutByAnotherUser, "IsCheckedOutByAnotherUser should be false (same user)");
            Assert.IsTrue(whileCheckedOut.IsLocked);
            Assert.IsNotNull(whileCheckedOut.LockedBy, "LockedBy should be populated");
            Assert.IsFalse(whileCheckedOut.LockedBy.StartsWith("S-1-"), "LockedBy should be a display name, not a SID");
            Assert.IsFalse(whileCheckedOut.IsLockedByAnotherUser, "IsLockedByAnotherUser should be false (same user)");

            // Check in
            var checkInResult = await client.EntriesClient.CheckInDocumentAsync(new CheckInDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(checkInResult);
            Assert.AreEqual(createdEntryId, checkInResult.Id);

            // Verify lock released
            var lockInfo = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsFalse(lockInfo.IsActive);
        }

        [TestMethod]
        public async Task CheckOut_WithLock_ThenUndoCheckOut_ReleasesLock()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net UndoCheckOut";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            await client.EntriesClient.PutUnderVersionControlAsync(new PutUnderVersionControlParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            await client.EntriesClient.CheckOutDocumentAsync(new CheckOutDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CheckOutDocumentRequest() { Lock = true }
            }).ConfigureAwait(false);

            // Verify locked
            var lockInfo = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);
            Assert.IsTrue(lockInfo.IsActive);

            // Undo checkout
            var undoResult = await client.EntriesClient.UndoCheckOutAsync(new UndoCheckOutParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(undoResult);

            // Verify unlocked
            var afterUndo = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);
            Assert.IsFalse(afterUndo.IsActive);
        }

        [TestMethod]
        public async Task CheckOut_WithoutLock()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CheckOutNoLock";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            await client.EntriesClient.PutUnderVersionControlAsync(new PutUnderVersionControlParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            await client.EntriesClient.CheckOutDocumentAsync(new CheckOutDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CheckOutDocumentRequest() { Lock = false }
            }).ConfigureAwait(false);

            // Verify not locked
            var lockInfo = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);
            Assert.IsFalse(lockInfo.IsActive);

            // Cleanup
            await client.EntriesClient.UndoCheckOutAsync(new UndoCheckOutParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task PutUnderVersionControl_AlreadyUnderVC_NoOp()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net VCNoOp";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            await client.EntriesClient.PutUnderVersionControlAsync(new PutUnderVersionControlParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            // Call again — should succeed without error
            var result = await client.EntriesClient.PutUnderVersionControlAsync(new PutUnderVersionControlParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
        }

        [TestMethod]
        public async Task CheckIn_WithUnlockFalse_KeepsLock()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CheckInKeepLock";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            await client.EntriesClient.PutUnderVersionControlAsync(new PutUnderVersionControlParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            await client.EntriesClient.CheckOutDocumentAsync(new CheckOutDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CheckOutDocumentRequest() { Lock = true }
            }).ConfigureAwait(false);

            // Check in with unlock=false
            var checkInResult = await client.EntriesClient.CheckInDocumentAsync(new CheckInDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CheckInDocumentRequest() { Unlock = false }
            }).ConfigureAwait(false);

            Assert.IsNotNull(checkInResult);
            Assert.AreEqual(createdEntryId, checkInResult.Id);

            // Verify lock is still active
            var lockInfo = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);
            Assert.IsTrue(lockInfo.IsActive, "Persistent lock should remain active when unlock=false");

            // Cleanup — unlock manually
            await client.EntriesClient.UnlockDocumentAsync(new UnlockDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CheckIn_DefaultUnlock_ReleasesLock()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CheckInDefaultUnlock";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            await client.EntriesClient.PutUnderVersionControlAsync(new PutUnderVersionControlParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            await client.EntriesClient.CheckOutDocumentAsync(new CheckOutDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CheckOutDocumentRequest() { Lock = true }
            }).ConfigureAwait(false);

            // Check in with no request body (default behavior)
            var checkInResult = await client.EntriesClient.CheckInDocumentAsync(new CheckInDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(checkInResult);

            // Verify lock is released (backward-compatible default)
            var lockInfo = await client.EntriesClient.GetDocumentLockInfoAsync(new GetDocumentLockInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);
            Assert.IsFalse(lockInfo.IsActive, "Persistent lock should be released with default check-in");
        }
    }
}
