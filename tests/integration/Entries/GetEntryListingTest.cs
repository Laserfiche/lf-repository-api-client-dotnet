using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryListingTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await Logout(client);
        }

        [TestMethod]
        public async Task GetEntryListing_ReturnEntries()
        {
            int entryId = 1;
            var response = await client.EntriesClient.GetEntryListingAsync(TestConfig.RepositoryId, entryId);
            var entries = response.Result?.Value;
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
            
            bool PagingCallback(SwaggerResponse<ODataValueContextOfIListOfODataGetEntryChildren> data)
            {
                if (data.Result.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Result.Value.Count);
                    Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            await client.EntriesClient.GetEntryListingForEachAsync(PagingCallback, TestConfig.RepositoryId, entryId, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetEntryListing_SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;
            
            // Initial request
            var response = await client.EntriesClient.GetEntryListingAsync(TestConfig.RepositoryId, entryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.EntriesClient.GetEntryListingNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
