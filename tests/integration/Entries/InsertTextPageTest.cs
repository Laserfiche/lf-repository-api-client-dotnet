// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class InsertTextPageTest : BaseTest
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
        public async Task InsertTextPage()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net InsertTextPage";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            var originalPageCount = ((Document)createdEntry).PageCount;
            Assert.IsTrue(originalPageCount > 0);

            var result = await client.EntriesClient.InsertTextPageAsync(new InsertTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1,
                Request = new InsertTextPageRequest() { Text = "Integration test inserted text page content." }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(originalPageCount + 1, ((Document)result).PageCount);

            // Verify the inserted page via GetPageInfo
            var pageInfo = await client.EntriesClient.GetPageInfoAsync(new GetPageInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1
            }).ConfigureAwait(false);

            Assert.IsNotNull(pageInfo);
            Assert.AreEqual(1, pageInfo.PageNumber);
            Assert.AreEqual(createdEntryId, pageInfo.EntryId);
            Assert.IsTrue(pageInfo.HasText, "Inserted page should have text content");
            Assert.IsTrue(pageInfo.TextDataSize > 0, "Inserted text page should have text data");
        }
    }
}
