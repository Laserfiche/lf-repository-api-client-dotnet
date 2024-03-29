// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TagDefinitions
{
    [TestClass]
    public class ListTagDefinitionsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(TagDefinitionCollectionResponse data)
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

            await client.TagDefinitionsClient.ListTagDefinitionsForEachAsync(PagingCallback, new ListTagDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
            }, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SimplePaging()
        {
            // Get total count of tags
            var tagDefinitionCollectionResponse = await client.TagDefinitionsClient.ListTagDefinitionsAsync(new ListTagDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
            }).ConfigureAwait(false);
            AssertCollectionResponse(tagDefinitionCollectionResponse);
            
            int tagCount = tagDefinitionCollectionResponse.Value.Count;
            int maxPageSize = 1;

            // Initial request
            tagDefinitionCollectionResponse = await client.TagDefinitionsClient.ListTagDefinitionsAsync(new ListTagDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
                Prefer = $"maxpagesize={maxPageSize}"
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(tagDefinitionCollectionResponse);
            Assert.IsTrue(tagDefinitionCollectionResponse.Value.Count <= maxPageSize);

            if (tagDefinitionCollectionResponse.Value.Count == 0 || tagCount <= maxPageSize)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = tagDefinitionCollectionResponse.OdataNextLink;
            
            Assert.IsNotNull(nextLink);

            // Paging request
            tagDefinitionCollectionResponse = await client.TagDefinitionsClient.ListTagDefinitionsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(tagDefinitionCollectionResponse);
            Assert.IsTrue(tagDefinitionCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
