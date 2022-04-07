using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class RefreshAccessTokenTest : BaseTest_V1_Alpha
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
        public async Task RefreshAccessToken_RefreshSuccessful()
        {
            var request = new CreateConnectionRequest()
            {
                Username = TestConfig.Username,
                Password = TestConfig.Password
            };
            var tokenResponse = await client.CreateAccessTokenAsync(TestConfig.RepositoryId, request, null, TestConfig.AccountId);
            var tokenInfo = tokenResponse.Result;
            Assert.IsTrue(!string.IsNullOrEmpty(tokenInfo.AuthToken));
            client.AccessToken = tokenInfo.AuthToken;

            Thread.Sleep(5000);

            var refreshResponse = await client.RefreshAccessTokenAsync(TestConfig.RepositoryId);
            var expireTime = refreshResponse.Result?.Value;
            Assert.IsNotNull(expireTime);
            Assert.IsTrue(tokenInfo.ExpireTime < expireTime);
        }
    }
}
