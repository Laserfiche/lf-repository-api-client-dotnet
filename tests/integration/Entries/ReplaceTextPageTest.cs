// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ReplaceTextPageTest : BaseTest
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
        public async Task ReplaceTextPage()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net ReplaceTextPage";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            var originalPageCount = ((Document)createdEntry).PageCount;
            Assert.IsTrue(originalPageCount > 0);

            var result = await client.EntriesClient.ReplaceTextPageAsync(new ReplaceTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1,
                Request = new ReplaceTextPageRequest() { Text = "Integration test replacement text content." }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(originalPageCount, ((Document)result).PageCount);

            // Verify the replaced page via GetPageInfo
            var pageInfo = await client.EntriesClient.GetPageInfoAsync(new GetPageInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1
            }).ConfigureAwait(false);

            Assert.IsNotNull(pageInfo);
            Assert.AreEqual(1, pageInfo.PageNumber);
            Assert.AreEqual(createdEntryId, pageInfo.EntryId);
            Assert.IsTrue(pageInfo.HasText, "Replaced page should have text content");
            Assert.IsTrue(pageInfo.TextDataSize > 0, "Replaced text page should have text data");
        }
    }
}
