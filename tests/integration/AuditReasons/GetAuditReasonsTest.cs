using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AuditReasons
{
    [TestClass]
    public class GetAuditReasonsTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetAuditReasons_ReturnAuditReasons()
        {
            var auditReasonsResponse = await client.AuditReasonsClient.GetAuditReasonsAsync(RepositoryId);
            var auditReasons = auditReasonsResponse.Result;

            Assert.IsNotNull(auditReasons);
            Assert.IsNotNull(auditReasons.DeleteEntry);
            Assert.IsNotNull(auditReasons.ExportDocument);
        }
    }
}
