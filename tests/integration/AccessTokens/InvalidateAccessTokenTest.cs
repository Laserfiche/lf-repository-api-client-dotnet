using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class InvalidateAccessTokenTest : BaseTest_V1_Alpha
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
            if (needLogout)
                await Logout(client);
        }

        [TestMethod]
        public async Task InvalidateAccessToken_LogoutSuccessful()
        {
            var response = await client.InvalidateAccessTokenAsync(TestConfig.RepositoryId);
            Assert.AreEqual(true, response.Result?.Value);

            needLogout = false;
            try
            {
                await client.GetFieldDefinitionsAsync(TestConfig.RepositoryId);
                Assert.Fail("Should not be able to make a successful api call after access token is invalidated.");
            }
            catch (ApiException) { }
        }
    }
}
