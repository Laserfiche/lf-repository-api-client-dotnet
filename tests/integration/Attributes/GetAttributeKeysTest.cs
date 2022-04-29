using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Attributes
{
    [TestClass]
    public class GetAttributesTest : BaseTest_V1
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
        public async Task GetAttributes_ReturnAttributes()
        {
            var response = await client.GetTrusteeAttributeKeyValuePairsAsync(TestConfig.RepositoryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetAttributes_ForEachPaging()
        {
            int maxPageSize = 10;

            bool PagingCallback(SwaggerResponse<ODataValueContextOfListOfAttribute> data)
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

            await client.GetTrusteeAttributeKeyValuePairsForEachAsync(PagingCallback, TestConfig.RepositoryId, maxPageSize: maxPageSize);
        }

    }
}
