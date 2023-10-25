// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TagDefinitions
{
    [TestClass]
    public class GetTagDefinitionsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTagDefinitions_ReturnAllTags()
        {
            var result = await client.TagDefinitionsClient.GetTagDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public async Task GetTagDefinitions_ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(ODataValueContextOfIListOfWTagInfo data)
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

            await client.TagDefinitionsClient.GetTagDefinitionsForEachAsync(PagingCallback, RepositoryId, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTagDefinitions_SimplePaging()
        {
            // Get total count of tags
            var  result = await client.TagDefinitionsClient.GetTagDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            int tagsCount = result.Value.Count;

            int maxPageSize = 1;

            // Initial request
            result = await client.TagDefinitionsClient.GetTagDefinitionsAsync(RepositoryId, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            if (result.Value.Count == 0 || tagsCount <= maxPageSize)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);

            // Paging request
            result = await client.TagDefinitionsClient.GetTagDefinitionsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
