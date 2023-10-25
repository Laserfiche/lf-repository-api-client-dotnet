// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.SimpleSearches
{
    [TestClass]
    public class SimpleSearchTest : BaseTest
    {
        private const string _entryToCreate = "RepositoryClientIntegrationTest - SimpleSearchTest - search text";
        private Entry _createdEntry;

        [TestInitialize]
        public async Task Initialize()
        {
            client = CreateClient();
            _createdEntry = await CreateEntry(client, _entryToCreate).ConfigureAwait(false);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await DeleteEntry(_createdEntry.Id).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SearchEntry()
        {
            var request = new SearchEntryRequest() { SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})" };
            var entryCollectionResponse = await client.SimpleSearchesClient.SearchEntryAsync(new SearchEntryParameters()
            {
                RepositoryId = RepositoryId,
                Request = request
            }).ConfigureAwait(false);

            AssertCollectionResponse(entryCollectionResponse);
        }
    }
}
