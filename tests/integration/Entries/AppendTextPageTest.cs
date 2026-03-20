// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class AppendTextPageTest : BaseTest
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
        public async Task AppendTextPage()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net AppendTextPage";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            var originalPageCount = ((Document)createdEntry).PageCount;
            Assert.IsTrue(originalPageCount > 0);

            var result = await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = "Integration test appended text page content." }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(originalPageCount + 1, ((Document)result).PageCount);

            // Verify the appended page via GetPageInfo
            var pageInfo = await client.EntriesClient.GetPageInfoAsync(new GetPageInfoParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = originalPageCount + 1
            }).ConfigureAwait(false);

            Assert.IsNotNull(pageInfo);
            Assert.AreEqual(originalPageCount + 1, pageInfo.PageNumber);
            Assert.AreEqual(createdEntryId, pageInfo.EntryId);
            Assert.IsTrue(pageInfo.HasText, "Appended page should have text content");
            Assert.IsTrue(pageInfo.TextDataSize > 0, "Appended text page should have text data");
        }
    }
}
