// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class RotateImagePageTest : BaseTest
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
        public async Task RotateImagePage()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net RotateImagePage";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Append a 1x1 white PNG as the image page
            var pngBytes = new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            var appendResult = await client.EntriesClient.AppendImagePageAsync(new AppendImagePageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ImageFile = new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png")
            }).ConfigureAwait(false);

            Assert.IsNotNull(appendResult);
            Assert.AreEqual(1, ((Document)appendResult).PageCount);

            // Verify initial rotation is zero
            var pageInfoBeforeList = await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageRange = "1"
            }).ConfigureAwait(false);
            var pageInfoBefore = pageInfoBeforeList[0];

            Assert.IsNotNull(pageInfoBefore);
            Assert.AreEqual(0, pageInfoBefore.ImageRotation, "Initial image rotation should be zero");

            // Rotate the page 90 degrees
            var result = await client.EntriesClient.RotateImagePageAsync(new RotateImagePageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1,
                Request = new RotateImagePageRequest() { RotationAngle = 90 }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(1, ((Document)result).PageCount);

            // Verify page still exists after rotation
            var pageInfoAfterList = await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageRange = "1"
            }).ConfigureAwait(false);
            var pageInfoAfter = pageInfoAfterList[0];

            Assert.IsNotNull(pageInfoAfter);
            Assert.IsTrue(pageInfoAfter.HasImage, "Page should still have image content after rotation");
        }
    }
}
