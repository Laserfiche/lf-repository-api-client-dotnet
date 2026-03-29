// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetPageTextTest : BaseTest
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
        public async Task GetPageText_ReturnsTextContent()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetPageText";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            string expectedText = "Hello world integration test";
            await client.EntriesClient.AppendTextPageAsync(new AppendTextPageParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                Request = new AppendTextPageRequest() { Text = expectedText }
            }).ConfigureAwait(false);

            var result = await client.EntriesClient.GetPageTextAsync(new GetPageTextParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId,
                PageNumber = 1
            }).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedText, result.Text);
        }

        [TestMethod]
        public async Task GetPageText_PageWithNoText_ThrowsApiException()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net GetPageTextNoText";
            var createdEntry = await CreateEmptyDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.EntriesClient.GetPageTextAsync(new GetPageTextParameters()
                {
                    RepositoryId = RepositoryId,
                    EntryId = createdEntryId,
                    PageNumber = 1
                }).ConfigureAwait(false));
        }
    }
}
