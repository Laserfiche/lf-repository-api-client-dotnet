using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class RefreshServerSessionTest : BaseTest_V1
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
        public async Task RefreshServerSession_RefreshSuccessful()
        {
            var currentTime = DateTime.UtcNow;
            var refreshResponse = await client.RefreshServerSessionAsync(TestConfig.RepositoryId);
            var expireTime = refreshResponse.Result?.Value;
            Assert.IsNotNull(expireTime);
            Assert.IsTrue(currentTime < expireTime.Value.UtcDateTime);
        }
    }
}
