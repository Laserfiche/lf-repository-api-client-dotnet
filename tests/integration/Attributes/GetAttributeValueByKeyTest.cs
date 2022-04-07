using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Attributes
{
    [TestClass]
    public class GetAttributeValueByKeyTest : BaseTest_V1
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
        public async Task GetAttributeByKey_ReturnAttribute()
        {
            var attributeKeysResponse = await client.GetTrusteeAttributeKeyValuePairsAsync(TestConfig.RepositoryId);
            var attributeKeys = attributeKeysResponse.Result?.Value;
            Assert.IsNotNull(attributeKeys);
            Assert.IsTrue(attributeKeys.Count > 0, "No attribute keys exist on the user.");

            var attributeValueResponse = await client.GetTrusteeAttributeValueByKeyAsync(TestConfig.RepositoryId, attributeKeys.First().Key);
            Assert.IsTrue(!string.IsNullOrEmpty(attributeValueResponse.Result?.Value));
        }
    }
}
