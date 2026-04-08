// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class AppendImagePageTest : BaseTest
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
        public async Task AppendImagePage()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net AppendImagePage";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            var originalPageCount = ((Document)createdEntry).PageCount;
            Assert.AreEqual(0, originalPageCount);

            // 1x1 white PNG
            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            var result = await client.EntriesClient.AppendImagePageAsync(new AppendImagePageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFiles = new System.Collections.Generic.List<FileParameter>
                {
                    new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png")
                }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(originalPageCount + 1, ((Document)result).PageCount);
        }

        [TestMethod]
        public async Task AppendImagePage_MultipleFiles()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net AppendImagePageMultiple";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // 1x1 white PNG
            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            var result = await client.EntriesClient.AppendImagePageAsync(new AppendImagePageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFiles = new System.Collections.Generic.List<FileParameter>
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
        public async Task AppendImagePage_WithGenerateText()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net AppendImagePageGenerateText";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // 1x1 white PNG
            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            var result = await client.EntriesClient.AppendImagePageAsync(new AppendImagePageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFiles = new System.Collections.Generic.List<FileParameter>
                {
                    new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png")
                },
                GenerateText = true
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(1, ((Document)result).PageCount);
        }
    }
}
