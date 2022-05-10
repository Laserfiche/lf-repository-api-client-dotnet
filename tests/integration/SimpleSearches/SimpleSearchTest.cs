using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.SimpleSearches
{
    [TestClass]
    public class SimpleSearchTest : BaseTest
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
        public async Task SimpleSearch_ReturnSearchResults()
        {
            var request = new SimpleSearchRequest() { SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})" };
            var response = await client.SimpleSearchesClient.CreateSimpleSearchOperationAsync(TestConfig.RepositoryId, request: request);
            Assert.IsNotNull(response.Result?.Value);
        }
    }
}
