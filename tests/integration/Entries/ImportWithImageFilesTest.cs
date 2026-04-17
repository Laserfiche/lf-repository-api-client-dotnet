// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [Ignore("Temporarily ignored: cloud test server not yet updated with V2 endpoints")]
    [TestClass]
    public class ImportWithImageFilesTest : BaseTest
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

        // Minimal valid 1x1 PNG
        private static byte[] CreateMinimalPng()
        {
            return new byte[] {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 0xDE, 0x00, 0x00, 0x00,
                0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
                0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC, 0x33, 0x00, 0x00, 0x00,
                0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };
        }

        [TestMethod]
        public async Task Import_WithImageFiles_CreatesDocumentWithPages()
        {
            var pngBytes = CreateMinimalPng();
            var imageFiles = new List<FileParameter>
            {
                new FileParameter(new MemoryStream(pngBytes), "page1.png", "image/png"),
                new FileParameter(new MemoryStream(pngBytes), "page2.png", "image/png"),
                new FileParameter(new MemoryStream(pngBytes), "page3.png", "image/png")
            };

            var result = (Document)await client.EntriesClient.ImportEntryAsync(new ImportEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = 1,
                File = new FileParameter(new MemoryStream(new byte[0]), "empty.txt", "text/plain"),
                ImageFiles = imageFiles,
                Request = new ImportEntryRequest()
                {
                    Name = "RepositoryApiClientIntegrationTest .Net Import ImageFiles",
                    AutoRename = true
                }
            }).ConfigureAwait(false);

            createdEntryId = result.Id;
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.PageCount);
        }

        [TestMethod]
        public async Task Import_WithEdocAndImageFiles_BothApplied()
        {
            var pngBytes = CreateMinimalPng();
            var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // minimal PDF header
            var imageFiles = new List<FileParameter>
            {
                new FileParameter(new MemoryStream(pngBytes), "page1.png", "image/png"),
                new FileParameter(new MemoryStream(pngBytes), "page2.png", "image/png")
            };

            var result = (Document)await client.EntriesClient.ImportEntryAsync(new ImportEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = 1,
                File = new FileParameter(new MemoryStream(pdfBytes), "test.pdf", "application/pdf"),
                ImageFiles = imageFiles,
                Request = new ImportEntryRequest()
                {
                    Name = "RepositoryApiClientIntegrationTest .Net Import Edoc+ImageFiles",
                    AutoRename = true,
                    ImportAsElectronicDocument = true
                }
            }).ConfigureAwait(false);

            createdEntryId = result.Id;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsElectronicDocument, "Document should have edoc");
            Assert.AreEqual(2, result.PageCount, "Document should have 2 image pages");
        }

        [TestMethod]
        public async Task Import_WithPdfAndImageFiles_ImportAsEdocFalse_Succeeds()
        {
            // Non-image files (PDF, Word, etc.) always become the electronic document at the
            // LFS layer, regardless of ImportAsElectronicDocument. Previously the controller
            // rejected this combination with 400 based on the flag alone; now the validation
            // checks the file extension so a PDF + imageFiles can coexist.
            var pngBytes = CreateMinimalPng();
            var pdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // minimal PDF header
            var imageFiles = new List<FileParameter>
            {
                new FileParameter(new MemoryStream(pngBytes), "page1.png", "image/png")
            };

            var result = (Document)await client.EntriesClient.ImportEntryAsync(new ImportEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = 1,
                File = new FileParameter(new MemoryStream(pdfBytes), "test.pdf", "application/pdf"),
                ImageFiles = imageFiles,
                Request = new ImportEntryRequest()
                {
                    Name = "RepositoryApiClientIntegrationTest .Net Import Pdf+ImageFiles NoFlag",
                    AutoRename = true,
                    ImportAsElectronicDocument = false
                }
            }).ConfigureAwait(false);

            createdEntryId = result.Id;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsElectronicDocument, "PDF should be imported as the edoc regardless of ImportAsElectronicDocument");
            Assert.AreEqual(1, result.PageCount, "imageFiles should produce 1 image page");
        }

        [TestMethod]
        public async Task Import_WithFileAndImageFiles_ImportAsEdocFalse_Returns400()
        {
            var pngBytes = CreateMinimalPng();
            var imageFiles = new List<FileParameter>
            {
                new FileParameter(new MemoryStream(pngBytes), "page1.png", "image/png")
            };

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.EntriesClient.ImportEntryAsync(new ImportEntryParameters()
                {
                    RepositoryId = RepositoryId,
                    EntryId = 1,
                    File = new FileParameter(new MemoryStream(pngBytes), "test.png", "image/png"),
                    ImageFiles = imageFiles,
                    Request = new ImportEntryRequest()
                    {
                        Name = "RepositoryApiClientIntegrationTest .Net Import Conflict",
                        AutoRename = true,
                        ImportAsElectronicDocument = false
                    }
                }).ConfigureAwait(false));

            Assert.AreEqual(400, ex.StatusCode);
        }

        [TestMethod]
        public async Task Import_WithImageFiles_ExceedsMaxCount_Returns400()
        {
            var pngBytes = CreateMinimalPng();
            var imageFiles = new List<FileParameter>();
            for (int i = 0; i < 11; i++)
            {
                imageFiles.Add(new FileParameter(new MemoryStream(pngBytes), $"page{i}.png", "image/png"));
            }

            var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.EntriesClient.ImportEntryAsync(new ImportEntryParameters()
                {
                    RepositoryId = RepositoryId,
                    EntryId = 1,
                    File = new FileParameter(new MemoryStream(new byte[0]), "empty.txt", "text/plain"),
                    ImageFiles = imageFiles,
                    Request = new ImportEntryRequest()
                    {
                        Name = "RepositoryApiClientIntegrationTest .Net Import TooMany",
                        AutoRename = true
                    }
                }).ConfigureAwait(false));

            Assert.AreEqual(400, ex.StatusCode);
        }
    }
}
