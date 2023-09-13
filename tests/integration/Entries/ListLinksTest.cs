using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ListLinksTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetEntryLinks_ReturnLinks()
        {
            int entryId = 1;
            var linkCollectionResponse = await client.EntriesClient.ListLinksAsync(RepositoryId, entryId).ConfigureAwait(false);
            
            Assert.IsNotNull(linkCollectionResponse.Value);
        }

        [TestMethod]
        public async Task GetEntryLinks_ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;

            Task<bool> PagingCallback(LinkCollectionResponse data)
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

            await client.EntriesClient.ListLinksForEachAsync(PagingCallback, RepositoryId, entryId, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetEntryLinks_SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;

            // Initial request
            var linkCollectionResponse = await client.EntriesClient.ListLinksAsync(RepositoryId, entryId, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            
            Assert.IsNotNull(linkCollectionResponse);

            if (linkCollectionResponse.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = linkCollectionResponse.OdataNextLink;
            
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(linkCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            linkCollectionResponse = await client.EntriesClient.ListLinksNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(linkCollectionResponse);
            Assert.IsTrue(linkCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
