// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class MovePagesTest : BaseTest
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
        public async Task MovePages()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net MovePages";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            var originalPageCount = ((Document)createdEntry).PageCount;
            Assert.IsTrue(originalPageCount >= 2, "Document must have at least 2 pages for MovePages test.");

            var result = await client.EntriesClient.MovePagesAsync(new MovePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new MovePagesRequest()
                {
                    PageRange = "1",
                    DestinationPageNumber = originalPageCount
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(originalPageCount, ((Document)result).PageCount);
        }
    }
}
