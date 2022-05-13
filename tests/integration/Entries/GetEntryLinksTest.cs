using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryLinksTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetEntryLinks_ReturnLinks()
        {
            int entryId = 1;
            var response = await client.EntriesClient.GetLinkValuesFromEntryAsync(RepositoryId, entryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetEntryLinks_ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;

            Task<bool> PagingCallback(SwaggerResponse<ODataValueContextOfIListOfWEntryLinkInfo> data)
            {
                if (data.Result.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Result.Value.Count);
                    Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }

            await client.EntriesClient.GetLinkValuesFromEntryForEachAsync(PagingCallback, RepositoryId, entryId, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetEntryLinks_SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;

            // Initial request
            var response = await client.EntriesClient.GetLinkValuesFromEntryAsync(RepositoryId, entryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.EntriesClient.GetLinkValuesFromEntryNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
