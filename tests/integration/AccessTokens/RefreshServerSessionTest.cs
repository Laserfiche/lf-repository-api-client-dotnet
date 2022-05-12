using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AccessTokens
{
    [TestClass]
    public class RefreshServerSessionTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task RefreshServerSession_RefreshSuccessful()
        {
            var currentTime = DateTime.UtcNow;
            var refreshResponse = await client.ServerSessionClient.RefreshServerSessionAsync(RepositoryId);
            var expireTime = refreshResponse.Result?.Value;
            Assert.IsNotNull(expireTime);
            Assert.IsTrue(currentTime < expireTime.Value.UtcDateTime);
        }
    }
}
