// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ListLinksTest : BaseTest
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
                    await DeleteEntry(entry.Id).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;

            Task<bool> PagingCallback(LinkCollectionResponse data)
            {
                if (data.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Value.Count);
                    Assert.IsTrue(data.Value.Count <= maxPageSize);
                    
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }

            await client.EntriesClient.ListLinksForEachAsync(PagingCallback, new ListLinksParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entryId
            }, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SimplePaging()
        {
            int maxPageSize = 1;

            // Arrange: create a source entry with two links so a second page is guaranteed,
            // instead of paging the shared root entry — whose link count this suite doesn't
            // control (exactly one leftover link there makes nextLink null and fails the test).
            var sourceEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net ListLinks SimplePaging Source").ConfigureAwait(false);
            createdEntries.Add(sourceEntry);
            var targetEntry1 = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net ListLinks SimplePaging Target1").ConfigureAwait(false);
            createdEntries.Add(targetEntry1);
            var targetEntry2 = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net ListLinks SimplePaging Target2").ConfigureAwait(false);
            createdEntries.Add(targetEntry2);

            await client.EntriesClient.SetLinksAsync(new SetLinksParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = sourceEntry.Id,
                Request = new SetLinksRequest()
                {
                    Links = new List<LinkToUpdate>
                    {
                        new LinkToUpdate { LinkDefinitionId = 1, OtherEntryId = targetEntry1.Id },
                        new LinkToUpdate { LinkDefinitionId = 1, OtherEntryId = targetEntry2.Id }
                    }
                }
            }).ConfigureAwait(false);

            // Initial request
            var linkCollectionResponse = await client.EntriesClient.ListLinksAsync(new ListLinksParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = sourceEntry.Id,
                Prefer = $"maxpagesize={maxPageSize}"
            }).ConfigureAwait(false);

            Assert.IsNotNull(linkCollectionResponse);
            Assert.AreNotEqual(0, linkCollectionResponse.Value.Count);

            var nextLink = linkCollectionResponse.OdataNextLink;

            Assert.IsNotNull(nextLink);
            Assert.IsTrue(linkCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            linkCollectionResponse = await client.EntriesClient.ListLinksNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);

            Assert.IsNotNull(linkCollectionResponse);
            Assert.AreNotEqual(0, linkCollectionResponse.Value.Count);
            Assert.IsTrue(linkCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
