// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [Ignore("Temporarily ignored until lf-repository-api-client-dotnet preview is published to Nuget.org after server deploys")]
    [TestClass]
    public class ImportEmptyDocTest : BaseTest
    {
        Entry importedEntry;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            importedEntry = null;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (importedEntry != null)
            {
                await DeleteEntry(importedEntry.Id).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ImportEmptyDoc()
        {
            string fileName = "RepositoryApiClientIntegrationTest .Net ImportEmptyDoc";
            importedEntry = await CreateEmptyDocument(fileName).ConfigureAwait(false);

            Assert.AreEqual(EntryType.Document, importedEntry.EntryType);
            Assert.AreEqual(0, ((Document)importedEntry).PageCount);
        }
    }
}
