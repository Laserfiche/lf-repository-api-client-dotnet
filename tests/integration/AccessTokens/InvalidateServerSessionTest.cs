using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class InvalidateServerSessionTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task InvalidateServerSession_LogoutSuccessful()
        {
            if (AuthorizationType == AuthorizationType.SelfHostedUsernamePassword)
            {
                return;
            }

            var result = await client.ServerSessionClient.InvalidateServerSessionAsync(RepositoryId);
            Assert.AreEqual(true, result.Value);
        }
    }
}
