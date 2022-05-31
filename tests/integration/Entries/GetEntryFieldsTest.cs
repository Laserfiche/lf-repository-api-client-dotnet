using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryFieldsTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetEntryFields_ReturnFields()
        {
            int entryId = 1;
            var result = await client.EntriesClient.GetFieldValuesAsync(RepositoryId, entryId);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public async Task GetEntryFields_ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;

            Task<bool> PagingCallback(ODataValueContextOfIListOfFieldValue data)
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

            await client.EntriesClient.GetFieldValuesForEachAsync(PagingCallback, RepositoryId, entryId, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetEntryFields_SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;

            // Initial request
            var result = await client.EntriesClient.GetFieldValuesAsync(RepositoryId, entryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(result);

            if (result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.EntriesClient.GetFieldValuesNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
