// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.LinkDefinitions
{
    [TestClass]
    public class GetLinkDefinitionsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetLinkDefinitionsAsync_ReturnAllLinks()
        {
            var result = await client.LinkDefinitionsClient.GetLinkDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public async Task GetLinkDefinitionsAsync_ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(ODataValueContextOfIListOfEntryLinkTypeInfo data)
            {
                if (data.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Value.Count);
                    Assert.IsTrue(data.Value.Count <= maxPageSize);
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }

            await client.LinkDefinitionsClient.GetLinkDefinitionsForEachAsync(PagingCallback, RepositoryId, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetLinkDefinitionsAsync_SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var result = await client.LinkDefinitionsClient.GetLinkDefinitionsAsync(RepositoryId, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            Assert.IsNotNull(result);

            if (result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.LinkDefinitionsClient.GetLinkDefinitionsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
