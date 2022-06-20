using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TagDefinitions
{
    [TestClass]
    public class GetTagDefinitionsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTagDefinitions_ReturnAllTags()
        {
            var result = await client.TagDefinitionsClient.GetTagDefinitionsAsync(RepositoryId);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public async Task GetTagDefinitions_ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(ODataValueContextOfIListOfWTagInfo data)
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

            await client.TagDefinitionsClient.GetTagDefinitionsForEachAsync(PagingCallback, RepositoryId, maxPageSize: maxPageSize);
            await Task.Delay(5000);
        }

        [TestMethod]
        public async Task GetTagDefinitions_SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var result = await client.TagDefinitionsClient.GetTagDefinitionsAsync(RepositoryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(result);

            if (result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.TagDefinitionsClient.GetTagDefinitionsNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
