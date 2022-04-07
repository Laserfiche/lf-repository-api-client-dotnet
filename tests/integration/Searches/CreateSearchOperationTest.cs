using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class CreateSearchOperationTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;
        string token;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
            token = "";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (!string.IsNullOrEmpty(token))
            {
                await client.CancelOrCloseSearchAsync(TestConfig.RepositoryId, token);
                Thread.Sleep(5000);
            }
            await Logout(client);
        }

        [TestMethod]
        public async Task CreateSearchOperation_ReturnToken()
        {
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var response = await client.CreateSearchOperationAsync(TestConfig.RepositoryId, request);
            token = response.Result?.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));
            var redirectUrl = response.Headers["Location"].ToList()[0];
            Assert.IsNotNull(redirectUrl);
            Assert.IsTrue(redirectUrl.Contains(token));
        }
    }
}
