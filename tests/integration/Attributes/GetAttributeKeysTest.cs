using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Attributes
{
    [TestClass]
    public class GetAttributesTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetAttributes_ReturnAttributes()
        {
            var response = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsAsync(RepositoryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetAttributes_ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(SwaggerResponse<ODataValueContextOfListOfAttribute> data)
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

            await client.AttributesClient.GetTrusteeAttributeKeyValuePairsForEachAsync(PagingCallback, RepositoryId, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetAttributes_SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var response = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsAsync(RepositoryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
