// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ImportEmptyDocTest : BaseTest
    {
        Entry importedEntry;
        Stream fileStream;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            importedEntry = null;
            fileStream = null;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            fileStream?.Dispose();
            if (importedEntry != null)
            {
                await DeleteEntry(importedEntry.Id).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ImportEmptyDoc()
        {
            int parentEntryId = 1;
            string fileName = "RepositoryApiClientIntegrationTest .Net ImportEmptyDoc";
            fileStream = new MemoryStream();
            var electronicDocument = new FileParameter(fileStream, "empty", "application/octet-stream");

            var request = new ImportEntryRequest()
            {
                Name = fileName,
                AutoRename = true
            };

            importedEntry = await client.EntriesClient.ImportEntryAsync(new ImportEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = parentEntryId,
                File = electronicDocument,
                Request = request
            }).ConfigureAwait(false);

            Assert.IsNotNull(importedEntry);
            Assert.IsNotNull(importedEntry.Id);
            Assert.AreEqual(EntryType.Document, importedEntry.EntryType);
            Assert.AreEqual(0, ((Document)importedEntry).PageCount);
        }
    }
}
