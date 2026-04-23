// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [Ignore("Temporarily ignored until lf-repository-api-client-dotnet preview is published to Nuget.org after server deploys")]
    [TestClass]
    public class CreatePagesTest : BaseTest
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
        public async Task CreatePages_ImageFile_AppendsPage()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CreatePages ImageFile";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            Assert.AreEqual(0, ((Document)createdEntry).PageCount);

            // 1x1 white PNG
            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            var result = await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFiles = new List<FileParameter>
                {
                    new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png")
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(1, ((Document)result).PageCount);
        }

        [TestMethod]
        public async Task CreatePages_MultipleImageFiles()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CreatePages MultipleImageFiles";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            Assert.AreEqual(0, ((Document)createdEntry).PageCount);

            // 1x1 white PNG
            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            var result = await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFiles = new List<FileParameter>
                {
                    new FileParameter(new MemoryStream(pngBytes), "test1.png", "image/png"),
                    new FileParameter(new MemoryStream(pngBytes), "test2.png", "image/png"),
                    new FileParameter(new MemoryStream(pngBytes), "test3.png", "image/png")
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(3, ((Document)result).PageCount);
        }

        [TestMethod]
        public async Task CreatePages_TextOnly()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CreatePages TextOnly";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            Assert.AreEqual(0, ((Document)createdEntry).PageCount);

            var result = await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "Integration test text page content." } }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(1, ((Document)result).PageCount);
        }

        [TestMethod]
        public async Task CreatePages_WithPageNumber_Inserts()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CreatePages Insert";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Create an initial text page
            await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "Original page content" } }
            }).ConfigureAwait(false);

            // 1x1 white PNG
            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            // Insert image page at position 1 (before the existing text page)
            var result = await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1,
                ImageFiles = new List<FileParameter>
                {
                    new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png")
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(2, ((Document)result).PageCount);
        }

        [TestMethod]
        public async Task CreatePages_EmptyPage()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CreatePages EmptyPage";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var result = await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "" } }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(1, ((Document)result).PageCount);
        }

        [TestMethod]
        public async Task CreatePages_EmptyPages_Count3()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CreatePages EmptyCount3";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var result = await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "", "", "" } }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(3, ((Document)result).PageCount);
        }

        [TestMethod]
        public async Task CreatePages_TextOnly_WithPageNumber_Inserts()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CreatePages TextInsert";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Create initial page
            await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "Original page" } }
            }).ConfigureAwait(false);

            // Insert text page at position 1
            var result = await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1,
                Request = new CreatePagesRequest() { TextPages = new List<string> { "Inserted page" } }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, ((Document)result).PageCount);

            // Verify inserted page is at position 1
            var pageText = await client.EntriesClient.GetPageTextAsync(new GetPageTextParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1
            }).ConfigureAwait(false);
            Assert.AreEqual("Inserted page", pageText.Text);
        }

        [TestMethod]
        public async Task CreatePages_SingleImageAndText()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CreatePages ImageAndText";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            var result = await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFiles = new List<FileParameter>
                {
                    new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png")
                },
                Request = new CreatePagesRequest() { TextPages = new List<string> { "Page with both image and text" } }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, ((Document)result).PageCount);

            // Verify page has both image and text via ListPageInfos
            var pages = await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageRange = "1"
            }).ConfigureAwait(false);
            Assert.AreEqual(1, pages.Count);
            Assert.IsTrue(pages[0].HasImage, "Page should have image content");
            Assert.IsTrue(pages[0].HasText, "Page should have text content");
        }

        [TestMethod]
        public async Task CreatePages_WithGenerateText()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net CreatePages GenerateText";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            var result = await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFiles = new List<FileParameter>
                {
                    new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png")
                },
                GenerateText = true
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, ((Document)result).PageCount);
        }
    }
}
