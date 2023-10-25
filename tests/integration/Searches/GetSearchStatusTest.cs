// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class GetSearchStatusTest : BaseTest
    {
        string token;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            token = "";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (!string.IsNullOrEmpty(token))
            {
                await client.SearchesClient.CancelOrCloseSearchAsync(RepositoryId, token).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task GetSearchStatus_ReturnSearchStatus()
        {
            // Create search
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var operation = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request).ConfigureAwait(false);
            token = operation.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));

            await Task.Delay(5000).ConfigureAwait(false);

            // Get search status
            var searchStatus = await client.SearchesClient.GetSearchStatusAsync(RepositoryId, token).ConfigureAwait(false);
            Assert.IsNotNull(searchStatus);
            Assert.AreEqual(token, searchStatus.OperationToken);
        }
    }
}
