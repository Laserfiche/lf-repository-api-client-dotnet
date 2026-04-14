// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class CopyPagesTest : BaseTest
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
        public async Task CopyPages()
        {
            var sourceEntry = await CreateEmptyDocument("RepositoryApiClientIntegrationTest .Net CopyPages Source").ConfigureAwait(false);
            sourceEntryId = sourceEntry.Id;

            // Add 2 text pages to source
            await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = sourceEntryId,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "Source page 1" } }
            }).ConfigureAwait(false);
            await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = sourceEntryId,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "Source page 2" } }
            }).ConfigureAwait(false);

            var destEntry = await CreateEmptyDocument("RepositoryApiClientIntegrationTest .Net CopyPages Dest").ConfigureAwait(false);
            destEntryId = destEntry.Id;

            var result = await client.EntriesClient.CopyPagesAsync(new CopyPagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = sourceEntryId,
                Request = new CopyPagesRequest()
                {
                    PageRange = "1",
                    DestinationEntryId = destEntryId,
                    DestinationPageNumber = 1
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(sourceEntryId, result.Id);
            // Source retains all pages (copy, not move)
            Assert.AreEqual(2, ((Document)result).PageCount);
        }
    }
}
