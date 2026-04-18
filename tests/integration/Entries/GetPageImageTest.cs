// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetPageImageTest : BaseTest
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
        public async Task GetPageImage_ReturnsImageStream()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetPageImage";
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

            await client.EntriesClient.CreatePagesAsync(new CreatePagesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFiles = new System.Collections.Generic.List<FileParameter>
                {
                    new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png")
                }
            }).ConfigureAwait(false);

            using var imageStream = await client.EntriesClient.GetPageImageAsync(new GetPageImageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1
            }).ConfigureAwait(false);

            Assert.IsNotNull(imageStream);
            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms).ConfigureAwait(false);
            Assert.IsTrue(ms.Length > 0, "Image stream should not be empty");
        }

        [TestMethod]
        public async Task GetPageImage_NonExistentPage_ThrowsApiException()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetPageImage404";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.EntriesClient.GetPageImageAsync(new GetPageImageParameters()
                {
                    RepositoryId = RepositoryId,
                    EntryId = createdEntryId,
                    PageNumber = 999
                }).ConfigureAwait(false));
        }
    }
}
