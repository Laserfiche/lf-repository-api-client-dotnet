// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class DeleteElectronicDocumentTest : BaseTest
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
        public async Task DeleteElectronicDocument()
        {
            var entryName = "RepositoryApiClientIntegrationTest .Net DeleteElectronicDocument";
            var createdEntry = await CreateDocument(entryName).ConfigureAwait(false);
            createdEntryId = createdEntry.Id;

            Assert.IsTrue(((Document)createdEntry).ElectronicDocumentSize > 0);

            var result = await client.EntriesClient.DeleteElectronicDocumentAsync(new DeleteElectronicDocumentParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = createdEntryId
            }).ConfigureAwait(false);

            Assert.AreEqual(createdEntry.Id, result.Id);
            Assert.IsTrue(((Document)result).ElectronicDocumentSize == 0);
        }
    }
}
