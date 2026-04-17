// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [Ignore("Temporarily ignored: cloud test server not yet updated with V2 endpoints")]
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
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Add 2 text pages
            await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "Page 1 content" } }
            }).ConfigureAwait(false);
            await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "Page 2 content" } }
            }).ConfigureAwait(false);

            var result = await client.EntriesClient.MovePagesAsync(new MovePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new MovePagesRequest()
                {
                    PageRange = "1",
                    DestinationPageNumber = 2
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(2, ((Document)result).PageCount);
        }
    }
}
