using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryLinksTest : BaseTest_V1
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
        public async Task GetEntryLinks_ReturnLinks()
        {
            int entryId = 1;
            var response = await client.GetLinkValuesFromEntryAsync(TestConfig.RepositoryId, entryId);
            Assert.IsNotNull(response.Result?.Value);
        }
    }
}
