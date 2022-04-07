using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TagDefinitions
{
    [TestClass]
    public class GetTagDefinitionsTest : BaseTest_V1
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
        public async Task GetTagDefinitions_ReturnAllTags()
        {
            var response = await client.GetTagDefinitionsAsync(TestConfig.RepositoryId);
            Assert.IsNotNull(response.Result?.Value);
        }
    }
}
