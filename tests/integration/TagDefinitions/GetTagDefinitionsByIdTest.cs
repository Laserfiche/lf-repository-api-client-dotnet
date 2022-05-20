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
            var allTagDefinitionsResult = await client.TagDefinitionsClient.GetTagDefinitionsAsync(RepositoryId);
            var firstTagDefinition = allTagDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTagDefinition, "No tag definitions exist in the repository.");

            var tagDefinition = await client.TagDefinitionsClient.GetTagDefinitionByIdAsync(RepositoryId, firstTagDefinition.Id);

            Assert.IsNotNull(tagDefinition);
            Assert.AreEqual(firstTagDefinition.Id, tagDefinition.Id);
        }
    }
}
