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
        public async Task GetDynamicFields_ReturnDynamicFields()
        {
            // Get a template definition id
            var templateDefinitionResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            var templateDefinitions = templateDefinitionResponse.Result?.Value;
            Assert.IsNotNull(templateDefinitions);
            Assert.IsTrue(templateDefinitions.Count > 0, "No template definitions exist in the repository.");

            int entryId = 1;
            var request = new GetDynamicFieldLogicValueRequest() { TemplateId = templateDefinitions.First().Id };
            var response = await client.EntriesClient.GetDynamicFieldValuesAsync(TestConfig.RepositoryId, entryId, request);

            Assert.IsNotNull(response.Result);
        }
    }
}
