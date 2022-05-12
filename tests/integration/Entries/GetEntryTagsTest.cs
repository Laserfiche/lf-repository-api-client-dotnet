using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryTagsTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetEntryTags_ReturnTags()
        {
            int entryId = 1;
            var response = await client.EntriesClient.GetTagsAssignedToEntryAsync(RepositoryId, entryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetEntryTags_ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;
            bool firstPage = true;

            Task<bool> PagingCallback(SwaggerResponse<ODataValueContextOfIListOfWTagInfo> data)
            {
                if (firstPage && data.Result.Value.Count == 0)
                {
                    Assert.IsNull(data.Result.OdataNextLink);
                    firstPage = false;
                }
                else
                {
                    Assert.AreNotEqual(0, data.Result.Value.Count);
                    Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                }
                return Task.FromResult(true);
            }

            await client.EntriesClient.GetTagsAssignedToEntryForEachAsync(PagingCallback, RepositoryId, entryId, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetEntryTags_SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;

            // Initial request
            var response = await client.EntriesClient.GetTagsAssignedToEntryAsync(RepositoryId, entryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.EntriesClient.GetTagsAssignedToEntryNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
