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
            var sourceEntry = await CreateEmptyDocument("RepositoryApiClientIntegrationTest .Net TransferPages Source").ConfigureAwait(false);
            sourceEntryId = sourceEntry.Id;

            // Add 2 text pages to source
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = sourceEntryId,
                Request = new AppendTextPageRequest() { Text = "Source page 1" }
            }).ConfigureAwait(false);
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = sourceEntryId,
                Request = new AppendTextPageRequest() { Text = "Source page 2" }
            }).ConfigureAwait(false);

            var destEntry = await CreateEmptyDocument("RepositoryApiClientIntegrationTest .Net TransferPages Dest").ConfigureAwait(false);
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
            Assert.AreEqual(1, ((Document)result).PageCount);
        }
    }
}
