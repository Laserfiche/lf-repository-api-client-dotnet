using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionByIdTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task Integrationtestthatfails()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task GetTemplateDefinitionById_ReturnTemplate()
        {
            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition, "No template definitions exist in the repository.");

            var result = await client.TemplateDefinitionsClient.GetTemplateDefinitionByIdAsync(RepositoryId, firstTemplateDefinition.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(firstTemplateDefinition.Id, result.Id);
        }
    }
}
