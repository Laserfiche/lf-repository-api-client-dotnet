// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class WriteElectronicDocumentTest : BaseTest
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
        public async Task WriteElectronicDocument_Success()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net WriteEdoc";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            Assert.IsFalse(((Document)createdEntry).IsElectronicDocument);

            var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 }; // %PDF-1.4

            var result = await client.EntriesClient.WriteElectronicDocumentAsync(new WriteElectronicDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ElectronicDocument = new FileParameter(new MemoryStream(pdfBytes), "test.pdf", "application/pdf")
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.IsTrue(((Document)result).IsElectronicDocument);
            Assert.IsTrue(((Document)result).ElectronicDocumentSize > 0);
        }

        [TestMethod]
        public async Task WriteElectronicDocument_WithGenerateText()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net WriteEdocGenerateText";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 };

            var result = await client.EntriesClient.WriteElectronicDocumentAsync(new WriteElectronicDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ElectronicDocument = new FileParameter(new MemoryStream(pdfBytes), "test.pdf", "application/pdf"),
                GenerateText = true
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.IsTrue(((Document)result).IsElectronicDocument);
        }

        [TestMethod]
        public async Task WriteElectronicDocument_ReplaceExistingEdoc()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net WriteEdocReplace";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            var pdfBytes1 = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 };

            // Write initial edoc
            await client.EntriesClient.WriteElectronicDocumentAsync(new WriteElectronicDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ElectronicDocument = new FileParameter(new MemoryStream(pdfBytes1), "test.pdf", "application/pdf")
            }).ConfigureAwait(false);

            // Replace with a larger edoc
            var pdfBytes2 = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34, 0x0A, 0x25, 0xE2, 0xE3, 0xCF, 0xD3 };

            var result = await client.EntriesClient.WriteElectronicDocumentAsync(new WriteElectronicDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                ElectronicDocument = new FileParameter(new MemoryStream(pdfBytes2), "test2.pdf", "application/pdf")
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.IsTrue(((Document)result).IsElectronicDocument);
            Assert.IsTrue(((Document)result).ElectronicDocumentSize > 0);
        }
    }
}
