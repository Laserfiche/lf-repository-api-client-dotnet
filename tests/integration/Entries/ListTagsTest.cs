// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ListTagsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;

            Task<bool> PagingCallback(TagCollectionResponse data)
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

            await client.EntriesClient.ListTagsForEachAsync(PagingCallback, new ListTagsParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entryId
            }, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;

            // Initial request
            var tagCollectionResponse = await client.EntriesClient.ListTagsAsync(new ListTagsParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entryId,
                Prefer = $"maxpagesize={maxPageSize}"
            }).ConfigureAwait(false);
            Assert.IsNotNull(tagCollectionResponse);

            if (tagCollectionResponse.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = tagCollectionResponse.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(tagCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            tagCollectionResponse = await client.EntriesClient.ListTagsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            Assert.IsNotNull(tagCollectionResponse);
            Assert.IsTrue(tagCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
