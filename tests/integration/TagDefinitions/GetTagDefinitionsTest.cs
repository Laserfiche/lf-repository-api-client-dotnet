using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TagDefinitions
{
    [TestClass]
    public class GetTagDefinitionsTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;

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
        public async Task GetTagDefinitions_ReturnAllTags()
        {
            var response = await client.GetTagDefinitionsAsync(TestConfig.RepositoryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetTagDefinitions_ForEachPaging()
        {
            int maxPageSize = 10;

            bool PagingCallback(SwaggerResponse<ODataValueContextOfIListOfWTagInfo> data)
            {
                if (data.Result.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Result.Value.Count);
                    Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                    return true; // If data aren't exhusted, keep asking.
                }
                else
                {
                    return false;
                }
            }

            await client.GetTagDefinitionsForEachAsync(PagingCallback, TestConfig.RepositoryId, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetTagDefinitions_SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var response = await client.GetTagDefinitionsAsync(TestConfig.RepositoryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.GetTagDefinitionsNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
