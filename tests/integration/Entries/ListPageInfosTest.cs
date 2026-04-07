// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ListPageInfosTest : BaseTest
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
        public async Task ListPageInfos()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net ListPageInfos";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Add 2 text pages
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = "Page 1 content" }
            }).ConfigureAwait(false);
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = "Page 2 content" }
            }).ConfigureAwait(false);

            var pages = await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(pages);
            Assert.AreEqual(2, pages.Count);

            for (int i = 0; i < pages.Count; i++)
            {
                Assert.AreEqual(i + 1, pages[i].PageNumber);
                Assert.AreEqual(createdEntryId, pages[i].EntryId);
                Assert.IsTrue(pages[i].PageId > 0);
            }
        }

        [TestMethod]
        public async Task ListPageInfos_WithPageRange_ReturnsFilteredPages()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net ListPageInfos PageRange";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Add 3 text pages
            for (int i = 1; i <= 3; i++)
            {
                await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
                {
                    RepositoryId = RepositoryId,
                    EntryId = createdEntryId,
                    Request = new AppendTextPageRequest() { Text = $"Page {i} content" }
                }).ConfigureAwait(false);
            }

            // Request only pages 1-2
            var pages = await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageRange = "1-2"
            }).ConfigureAwait(false);

            Assert.IsNotNull(pages);
            Assert.AreEqual(2, pages.Count);
            Assert.AreEqual(1, pages[0].PageNumber);
            Assert.AreEqual(2, pages[1].PageNumber);
        }

        [TestMethod]
        public async Task ListPageInfos_WithSinglePageRange_ReturnsOnePage()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net ListPageInfos SinglePage";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Add 2 text pages
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = "Page 1 content" }
            }).ConfigureAwait(false);
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = "Page 2 content" }
            }).ConfigureAwait(false);

            // Request only page 2
            var pages = await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageRange = "2"
            }).ConfigureAwait(false);

            Assert.IsNotNull(pages);
            Assert.AreEqual(1, pages.Count);
            Assert.AreEqual(2, pages[0].PageNumber);
            Assert.AreEqual(createdEntryId, pages[0].EntryId);
        }

        [TestMethod]
        public async Task ListPageInfos_EntryNotFound_Returns404()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
                {
                    RepositoryId = RepositoryId,
                    EntryId = 999999999
                }).ConfigureAwait(false));

            Assert.AreEqual(404, ex.StatusCode);
        }
    }
}
