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
            var response = await client.EntriesClient.GetFieldValuesAsync(RepositoryId, entryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetEntryFields_ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;

            Task<bool> PagingCallback(SwaggerResponse<ODataValueContextOfIListOfFieldValue> data)
            {
                Assert.AreNotEqual(0, data.Result.Value.Count);
                Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                return Task.FromResult(true);
            }

            await client.EntriesClient.GetFieldValuesForEachAsync(PagingCallback, RepositoryId, entryId, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetEntryFields_SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;

            // Initial request
            var response = await client.EntriesClient.GetFieldValuesAsync(RepositoryId, entryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.EntriesClient.GetFieldValuesNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
