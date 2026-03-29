// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetPageInfoTest : BaseTest
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
        public async Task GetPageInfo()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetPageInfo";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Add a text page
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = "Page info test content" }
            }).ConfigureAwait(false);

            var pageInfo = await client.EntriesClient.GetPageInfoAsync(new GetPageInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1
            }).ConfigureAwait(false);

            Assert.IsNotNull(pageInfo);
            Assert.AreEqual(1, pageInfo.PageNumber);
            Assert.AreEqual(createdEntryId, pageInfo.EntryId);
            Assert.IsTrue(pageInfo.PageId > 0);
            Assert.IsTrue(pageInfo.HasImage || pageInfo.HasText, "Page should have image or text content");
        }

        [TestMethod]
        public async Task GetPageInfo_ImagePage_HasImageProperties()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetPageInfo Image";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Add an image page
            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };
            await client.EntriesClient.AppendImagePageAsync(new AppendImagePageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFile = new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png")
            }).ConfigureAwait(false);

            var pageInfo = await client.EntriesClient.GetPageInfoAsync(new GetPageInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1
            }).ConfigureAwait(false);

            Assert.IsNotNull(pageInfo);
            Assert.IsTrue(pageInfo.HasImage, "Page should have image content");
            Assert.IsTrue(pageInfo.ImageWidth > 0, "Image page should have width > 0");
            Assert.IsTrue(pageInfo.ImageHeight > 0, "Image page should have height > 0");
            Assert.IsTrue(pageInfo.ImageDataSize > 0, "Image page should have data size > 0");
        }

        [TestMethod]
        public async Task GetPageInfo_InvalidPageNumber_Returns404()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetPageInfo NotFound";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.EntriesClient.GetPageInfoAsync(new GetPageInfoParameters()
                {
                    RepositoryId = RepositoryId,
                    EntryId = createdEntryId,
                    PageNumber = 9999
                }).ConfigureAwait(false));

            Assert.IsTrue(ex.StatusCode == 400 || ex.StatusCode == 404);
        }

        [TestMethod]
        public async Task GetPageInfo_EntryNotFound_Returns404()
        {
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.EntriesClient.GetPageInfoAsync(new GetPageInfoParameters()
                {
                    RepositoryId = RepositoryId,
                    EntryId = 999999999,
                    PageNumber = 1
                }).ConfigureAwait(false));

            Assert.AreEqual(404, ex.StatusCode);
        }
    }

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
        public async Task ListPageInfos_AllPagesHaveContent()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net ListPageInfos Content";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Add a text page and an image page
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = "Text page content" }
            }).ConfigureAwait(false);

            var pages = await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(pages);
            Assert.IsTrue(pages.Count > 0);
            foreach (var page in pages)
            {
                Assert.IsTrue(page.HasImage || page.HasText, $"Page {page.PageNumber} should have image or text content");
            }
        }

        [TestMethod]
        public async Task ListPageInfos_ConsistentWithGetPageInfo()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net ListPageInfos Consistent";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Add a text page
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = "Consistency test content" }
            }).ConfigureAwait(false);

            var pages = await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsTrue(pages.Count > 0);

            // Verify first page matches GetPageInfo result
            var singlePageInfo = await client.EntriesClient.GetPageInfoAsync(new GetPageInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1
            }).ConfigureAwait(false);

            var firstPage = pages.First(p => p.PageNumber == 1);
            Assert.AreEqual(singlePageInfo.PageId, firstPage.PageId);
            Assert.AreEqual(singlePageInfo.HasImage, firstPage.HasImage);
            Assert.AreEqual(singlePageInfo.HasText, firstPage.HasText);
            Assert.AreEqual(singlePageInfo.ImageRotation, firstPage.ImageRotation);
            Assert.AreEqual(singlePageInfo.ImageWidth, firstPage.ImageWidth);
            Assert.AreEqual(singlePageInfo.ImageHeight, firstPage.ImageHeight);
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
