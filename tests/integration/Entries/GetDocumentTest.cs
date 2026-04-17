// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [Ignore("Temporarily ignored: cloud test server not yet updated with V2 endpoints")]
    [TestClass]
    public class GetDocumentTest : BaseTest
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
        public async Task GetDocument_ReturnsEdocStream()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetDocument";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            // Write an edoc first
            var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 };
            await client.EntriesClient.UpdateDocumentAsync(new UpdateDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                File = new FileParameter(new MemoryStream(pdfBytes), "test.pdf", "application/pdf")
            }).ConfigureAwait(false);

            // Download the edoc
            using var edocStream = await client.EntriesClient.GetDocumentAsync(new GetDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(edocStream);
            using var ms = new MemoryStream();
            await edocStream.CopyToAsync(ms).ConfigureAwait(false);
            Assert.IsTrue(ms.Length > 0, "Edoc stream should not be empty");
        }

        [TestMethod]
        public async Task GetDocument_NoEdoc_ThrowsApiException()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetDocumentNoEdoc";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.EntriesClient.GetDocumentAsync(new GetDocumentParameters()
                {
                    RepositoryId = RepositoryId,
                    EntryId = createdEntryId
                }).ConfigureAwait(false));
        }
    }
}
