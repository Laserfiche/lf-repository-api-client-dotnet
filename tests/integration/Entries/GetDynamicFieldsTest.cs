using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetDynamicFieldsTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetDynamicFields_ReturnDynamicFields()
        {
            // Get a template definition id
            var templateDefinitionResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var templateDefinitions = templateDefinitionResult.Value;
            Assert.IsNotNull(templateDefinitions);
            Assert.IsTrue(templateDefinitions.Count > 0, "No template definitions exist in the repository.");

            int entryId = 1;
            var request = new GetDynamicFieldLogicValueRequest() { TemplateId = templateDefinitions.First().Id };
            var result = await client.EntriesClient.GetDynamicFieldValuesAsync(RepositoryId, entryId, request);

            Assert.IsNotNull(result);
        }
    }
}
