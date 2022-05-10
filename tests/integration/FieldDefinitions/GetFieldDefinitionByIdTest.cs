using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.FieldDefinitions
{
    [TestClass]
    public class GetFieldDefinitionByIdTest : BaseTest
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
        public async Task GetFieldDefinitionById_ReturnField()
        {
            var allFieldDefinitionsResponse = await client.FieldDefinitionsClient.GetFieldDefinitionsAsync(TestConfig.RepositoryId);
            var firstFieldDefinition = allFieldDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstFieldDefinition, "No field definitions exist in the repository.");

            var response = await client.FieldDefinitionsClient.GetFieldDefinitionByIdAsync(TestConfig.RepositoryId, firstFieldDefinition.Id);
            var fieldDefinition = response.Result;

            Assert.IsNotNull(fieldDefinition);
            Assert.AreEqual(firstFieldDefinition.Id, fieldDefinition.Id);
        }
    }
}
