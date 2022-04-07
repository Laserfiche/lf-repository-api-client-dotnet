using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class CreateAccessTokenTest : BaseTest_V1_Alpha
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
        public async Task CreateAccessToken_UsernamePassword()
        {
            string authTokenBeforeLogin = client.AccessToken;

            await Login(client);
            string authTokenAfterLogin = client.AccessToken;

            Assert.IsNull(authTokenBeforeLogin);
            Assert.IsNotNull(authTokenAfterLogin);
        }

        [TestMethod]
        [ExpectedException(typeof(ApiException<APIServerException>))]
        public async Task CreateAccessToken_Fail()
        {
            await Login(client, password: "fakepassword");
        }
    }
}
