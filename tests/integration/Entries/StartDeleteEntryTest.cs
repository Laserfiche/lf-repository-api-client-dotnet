// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class StartDeleteEntryTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ReturnOperationToken()
        {
            var deleteEntry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net DeleteFolder").ConfigureAwait(false);
            
            StartDeleteEntryRequest body = new();
            var response = await client.EntriesClient.StartDeleteEntryAsync(new StartDeleteEntryParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = deleteEntry.Id,
                Request = body
            }).ConfigureAwait(false);
            
            AssertIsNotNullOrEmpty(response.TaskId);
        }
    }
}
