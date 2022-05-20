using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.SimpleSearches
{
    [TestClass]
    public class SimpleSearchTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task SimpleSearch_ReturnSearchResults()
        {
            var request = new SimpleSearchRequest() { SearchCommand = "({LF:Basic ~= \"search text\", option=\"DFANLT\"})" };
            var result = await client.SimpleSearchesClient.CreateSimpleSearchOperationAsync(RepositoryId, request: request);
            Assert.IsNotNull(result.Value);
        }
    }
}
