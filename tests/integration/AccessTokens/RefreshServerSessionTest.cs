using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class RefreshServerSessionTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task RefreshServerSession_RefreshSuccessful()
        {
            if (AuthorizationType == AuthorizationType.APIServerUsernamePassword)
            {
                return;
            }

            var currentTime = DateTime.UtcNow;
            var refreshResult = await client.ServerSessionClient.RefreshServerSessionAsync(RepositoryId);
            var expireTime = refreshResult?.Value;
            Assert.IsNotNull(expireTime);
            Assert.IsTrue(currentTime < expireTime.Value.UtcDateTime);
        }
    }
}
