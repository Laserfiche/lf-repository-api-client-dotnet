using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AuditReasons
{
    [TestClass]
    public class ListAuditReasonsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ListAuditReasons_ReturnAuditReasons()
        {
            var response = await client.AuditReasonsClient.ListAuditReasonsAsync(RepositoryId).ConfigureAwait(false);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Value);
            Assert.IsNotNull(response.Value.Count > 0);
            Assert.IsNotNull(response.Value[0]);
            Assert.IsNotNull(response.Value[0].AuditEventType);
        }
    }
}
