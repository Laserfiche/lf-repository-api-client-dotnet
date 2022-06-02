using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryListingTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetEntryListing_ReturnEntries()
        {
            int entryId = 1;
            var result = await client.EntriesClient.GetEntryListingAsync(RepositoryId, entryId);
            var entries = result.Value;
            Assert.IsNotNull(entries);
            foreach (var entry in entries)
            {
                Assert.AreEqual(entryId, entry.ParentId);
            }
        }

        [TestMethod]
        public async Task GetEntryListing_ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;
            
            Task<bool> PagingCallback(ODataValueContextOfIListOfEntry data)
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

            await client.EntriesClient.GetEntryListingForEachAsync(PagingCallback, RepositoryId, entryId, maxPageSize: maxPageSize);
            await Task.Delay(5000);
        }

        [TestMethod]
        public async Task GetEntryListing_SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;
            
            // Initial request
            var result = await client.EntriesClient.GetEntryListingAsync(RepositoryId, entryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(result);

            if (result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.EntriesClient.GetEntryListingNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
