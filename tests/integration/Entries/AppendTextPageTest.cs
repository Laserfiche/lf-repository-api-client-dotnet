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
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;
            Assert.AreEqual(0, ((Document)createdEntry).PageCount);

            var result = await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = "Integration test appended text page content." }
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(createdEntryId, result.Id);
            Assert.AreEqual(1, ((Document)result).PageCount);

            // Verify the appended page via ListPageInfos
            var pageInfoList = await client.EntriesClient.ListPageInfosAsync(new ListPageInfosParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageRange = "1"
            }).ConfigureAwait(false);
            var pageInfo = pageInfoList[0];

            Assert.IsNotNull(pageInfo);
            Assert.AreEqual(1, pageInfo.PageNumber);
            Assert.AreEqual(createdEntryId, pageInfo.EntryId);
            Assert.IsTrue(pageInfo.HasText, "Appended page should have text content");
            Assert.IsTrue(pageInfo.TextDataSize > 0, "Appended text page should have text data");
        }
    }
}
