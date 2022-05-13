using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionByIdTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTemplateDefinitionById_ReturnTemplate()
        {
            var allTemplateDefinitionsResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition, "No template definitions exist in the repository.");

            var response = await client.TemplateDefinitionsClient.GetTemplateDefinitionByIdAsync(RepositoryId, firstTemplateDefinition.Id);

            Assert.IsNotNull(response.Result);
            Assert.AreEqual(firstTemplateDefinition.Id, response.Result.Id);
        }
    }
}
