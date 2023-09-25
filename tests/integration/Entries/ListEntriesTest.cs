using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ListEntriesTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ReturnEntries()
        {
            int entryId = 1;
            var entryCollectionResponse = await client.EntriesClient.ListEntriesAsync(new ListEntriesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entryId
            }).ConfigureAwait(false);
            var entries = entryCollectionResponse.Value;
            
            Assert.IsNotNull(entries);
            
            foreach (var entry in entries)
            {
                Assert.AreEqual(entryId, entry.ParentId);
            }
        }

        [TestMethod]
        public async Task ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;
            
            Task<bool> PagingCallback(EntryCollectionResponse data)
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

            await client.EntriesClient.ListEntriesForEachAsync(PagingCallback, new ListEntriesParameters()
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
            var entryCollectionResponse = await client.EntriesClient.ListEntriesAsync(new ListEntriesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entryId,
                Prefer = $"maxpagesize={maxPageSize}"
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(entryCollectionResponse);

            if (entryCollectionResponse.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = entryCollectionResponse.OdataNextLink;
            
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(entryCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            entryCollectionResponse = await client.EntriesClient.ListEntriesNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(entryCollectionResponse);
            Assert.IsTrue(entryCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
