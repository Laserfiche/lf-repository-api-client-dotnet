// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class TransferPagesTest : BaseTest
    {
        int sourceEntryId;
        int destEntryId;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            sourceEntryId = 0;
            destEntryId = 0;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (sourceEntryId != 0)
            {
                await DeleteEntry(sourceEntryId).ConfigureAwait(false);
            }
            if (destEntryId != 0)
            {
                await DeleteEntry(destEntryId).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task TransferPages()
        {
            var sourceEntry = await CreateDocument("RepositoryApiClientIntegrationTest .Net TransferPages Source").ConfigureAwait(false);
            sourceEntryId = sourceEntry.Id;
            var sourcePageCount = ((Document)sourceEntry).PageCount;
            Assert.IsTrue(sourcePageCount >= 2, "Source document must have at least 2 pages for TransferPages test.");

            var destEntry = await CreateDocument("RepositoryApiClientIntegrationTest .Net TransferPages Dest").ConfigureAwait(false);
            destEntryId = destEntry.Id;

            var result = await client.EntriesClient.TransferPagesAsync(new TransferPagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = sourceEntryId,
                Request = new TransferPagesRequest()
                {
                    PageRange = "1",
                    DestinationEntryId = destEntryId,
                    DestinationPageNumber = 1
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(sourceEntryId, result.Id);
            Assert.AreEqual(sourcePageCount - 1, ((Document)result).PageCount);
        }
    }
}
