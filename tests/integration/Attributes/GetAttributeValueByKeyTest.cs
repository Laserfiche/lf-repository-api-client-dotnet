using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Attributes
{
    [TestClass]
    public class GetAttributeValueByKeyTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetAttributeByKey_ReturnAttribute()
        {
            var attributeKeysResponse = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsAsync(RepositoryId);
            var attributeKeys = attributeKeysResponse.Result?.Value;
            Assert.IsNotNull(attributeKeys);
            Assert.IsTrue(attributeKeys.Count > 0, "No attribute keys exist on the user.");

            var attributeValueResponse = await client.AttributesClient.GetTrusteeAttributeValueByKeyAsync(RepositoryId, attributeKeys.First().Key);
            Assert.IsTrue(!string.IsNullOrEmpty(attributeValueResponse.Result?.Value));
        }
    }
}
