using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Searches
{
    [TestClass]
    public class CreateSearchOperationTest : BaseTest
    {
        string token;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            token = "";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (!string.IsNullOrEmpty(token))
            {
                await client.SearchesClient.CancelOrCloseSearchAsync(RepositoryId, token).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task CreateSearchOperation_ReturnToken()
        {
            var request = new AdvancedSearchRequest()
            {
                SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})"
            };
            var operation = await client.SearchesClient.CreateSearchOperationAsync(RepositoryId, request).ConfigureAwait(false);
            token = operation.Token;
            Assert.IsTrue(!string.IsNullOrEmpty(token));
        }
    }
}
