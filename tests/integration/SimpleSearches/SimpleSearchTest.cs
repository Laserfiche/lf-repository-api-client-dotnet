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

        [TestMethod]
        public async Task SimpleSearch_ReturnSearchResults()
        {
            var request = new SimpleSearchRequest() { SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})" };
            var result = await client.SimpleSearchesClient.CreateSimpleSearchOperationAsync(RepositoryId, request: request).ConfigureAwait(false);
            Assert.IsNotNull(result.Value);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await DeleteEntry(client, _createdEntry.Id).ConfigureAwait(false);
        }
    }
}
