using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class InvalidateServerSessionTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;
        bool needLogout = true;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if(needLogout)
                await Logout(client);
        }

        [TestMethod]
        public async Task InvalidateServerSession_LogoutSuccessful()
        {
            var response = await client.InvalidateServerSessionAsync(TestConfig.RepositoryId);
            Assert.AreEqual(true, response.Result?.Value);
            needLogout = false;
        }
    }
}
