// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    /// <summary>
    /// REQ-DOC-002 opt-in introspection on Get Entry: the folder childInfo facet and the
    /// document totalDocumentSize. Both are single-GET-only and must be omitted unless requested.
    /// </summary>
    [TestClass]
    public class EntryIntrospectionTest : BaseTest
    {
        private const int RootFolderId = 1;
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
        public async Task GetEntry_IncludeChildInfo_RootFolder_ReportsTotalAndPerTypeCounts()
        {
            var entry = await client.EntriesClient.GetEntryAsync(new GetEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = RootFolderId,
                IncludeChildInfo = true
            }).ConfigureAwait(false);

            var folder = entry as Folder;
            Assert.IsNotNull(folder, "Root entry should be a folder");
            Assert.IsNotNull(folder.ChildInfo, "childInfo should be populated when requested");
            // Single object: a total plus the per-type breakdown.
            Assert.AreEqual(
                (long)folder.ChildInfo.FolderCount + folder.ChildInfo.DocumentCount + folder.ChildInfo.ShortcutCount,
                folder.ChildInfo.ChildCount,
                "childCount should equal the sum of the per-type counts");
            Assert.AreEqual(folder.ChildInfo.ChildCount > 0, folder.ChildInfo.HasChildren,
                "hasChildren must be consistent with childCount");
            // The repository root reliably contains at least one child in our test environments.
            Assert.IsTrue(folder.ChildInfo.ChildCount > 0, "Root folder should report a positive child count");
        }

        [TestMethod]
        public async Task GetEntry_NoChildInfo_OmitsChildInfo()
        {
            var entry = await client.EntriesClient.GetEntryAsync(new GetEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = RootFolderId
            }).ConfigureAwait(false);

            var folder = entry as Folder;
            Assert.IsNotNull(folder);
            Assert.IsNull(folder.ChildInfo, "childInfo must be omitted (not null) when not requested");
        }

        [TestMethod]
        public async Task GetEntry_IncludeTotalSize_Document_ReturnsTotalAtLeastEdocSize()
        {
            importedEntry = await ImportTestDocument().ConfigureAwait(false);

            var entry = await client.EntriesClient.GetEntryAsync(new GetEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = importedEntry.Id,
                IncludeTotalSize = true
            }).ConfigureAwait(false);

            var document = entry as Document;
            Assert.IsNotNull(document, "Imported entry should be a document");
            Assert.IsNotNull(document.TotalDocumentSize, "totalDocumentSize should be populated when requested");
            // Total stored size = electronic document + page (image/text/locations/thumbnail) data +
            // attachments. Pages render asynchronously after import (see server async page/OCR note),
            // so for a freshly imported edoc-only document the total may equal the edoc size; the
            // >= assertion is robust either way.
            Assert.IsTrue(document.TotalDocumentSize.Value >= document.ElectronicDocumentSize,
                $"totalDocumentSize ({document.TotalDocumentSize}) should be >= electronicDocumentSize ({document.ElectronicDocumentSize})");
        }

        [TestMethod]
        public async Task GetEntry_Document_NoIncludeTotalSize_OmitsTotalSize()
        {
            importedEntry = await ImportTestDocument().ConfigureAwait(false);

            var entry = await client.EntriesClient.GetEntryAsync(new GetEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = importedEntry.Id
            }).ConfigureAwait(false);

            var document = entry as Document;
            Assert.IsNotNull(document);
            Assert.IsNull(document.TotalDocumentSize, "totalDocumentSize must be omitted when not requested");
        }

        private async Task<Entry> ImportTestDocument()
        {
            string fileLocation = TempPath + "test.pdf";
            fileStream = File.OpenRead(fileLocation);
            var file = new FileParameter(fileStream, "test.pdf", "application/pdf");
            return await client.EntriesClient.ImportEntryAsync(new ImportEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = RootFolderId,
                File = file,
                Request = new ImportEntryRequest() { Name = "IntegrationTest EntryIntrospection", AutoRename = true }
            }).ConfigureAwait(false);
        }
    }
}
