using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class InvalidateServerSessionTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task InvalidateServerSession_LogoutSuccessful()
        {
            var response = await client.ServerSessionClient.InvalidateServerSessionAsync(RepositoryId);
            Assert.AreEqual(true, response.Result?.Value);
        }
    }
}
