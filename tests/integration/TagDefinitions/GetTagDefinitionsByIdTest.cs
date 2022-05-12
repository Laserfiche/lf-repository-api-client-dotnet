using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TagDefinitions
{
    [TestClass]
    public class GetTagDefinitionsByIdTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTagDefinitionsById_ReturnTag()
        {
            var allTagDefinitionsResponse = await client.TagDefinitionsClient.GetTagDefinitionsAsync(RepositoryId);
            var firstTagDefinition = allTagDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTagDefinition, "No tag definitions exist in the repository.");

            var response = await client.TagDefinitionsClient.GetTagDefinitionByIdAsync(RepositoryId, firstTagDefinition.Id);
            var tagDefinition = response.Result;

            Assert.IsNotNull(tagDefinition);
            Assert.AreEqual(firstTagDefinition.Id, tagDefinition.Id);
        }
    }
}
