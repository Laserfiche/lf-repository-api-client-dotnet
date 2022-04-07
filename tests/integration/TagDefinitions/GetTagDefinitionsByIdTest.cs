using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TagDefinitions
{
    [TestClass]
    public class GetTagDefinitionsByIdTest : BaseTest_V1
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
        public async Task GetTagDefinitionsById_ReturnTag()
        {
            var allTagDefinitionsResponse = await client.GetTagDefinitionsAsync(TestConfig.RepositoryId);
            var firstTagDefinition = allTagDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTagDefinition, "No tag definitions exist in the repository.");

            var response = await client.GetTagDefinitionByIdAsync(TestConfig.RepositoryId, firstTagDefinition.Id);
            var tagDefinition = response.Result;

            Assert.IsNotNull(tagDefinition);
            Assert.AreEqual(firstTagDefinition.Id, tagDefinition.Id);
        }
    }
}
