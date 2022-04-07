using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.FieldDefinitions
{
    [TestClass]
    public class GetFieldDefinitionsTest : BaseTest_V1
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
        public async Task GetFieldDefinitions_ReturnAllFields()
        {
            var response = await client.GetFieldDefinitionsAsync(TestConfig.RepositoryId);
            Assert.IsNotNull(response.Result?.Value);
        }
    }
}
