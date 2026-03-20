// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            var originalPageCount = ((Document)createdEntry).PageCount;
            Assert.IsTrue(originalPageCount > 0);

            var result = await client.EntriesClient.RotateImagePageAsync(new RotateImagePageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1,
                Request = new RotateImagePageRequest() { RotationAngle = 90 }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(originalPageCount, ((Document)result).PageCount);

            // Verify rotation was applied by reading page info
            var pageInfo = await client.EntriesClient.GetPageInfoAsync(new GetPageInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1
            }).ConfigureAwait(false);

            Assert.IsNotNull(pageInfo);
            Assert.AreNotEqual(0, pageInfo.ImageRotation, "ImageRotation should reflect the applied 90-degree clockwise rotation");
        }
    }
}
