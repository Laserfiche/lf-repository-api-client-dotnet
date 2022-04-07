using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.AuditReasons
{
    [TestClass]
    public class GetAuditReasonsTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;

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
        public async Task GetAuditReasons_ReturnAuditReasons()
        {
            var auditReasonsResponse = await client.GetAuditReasonsAsync(TestConfig.RepositoryId);
            var auditReasons = auditReasonsResponse.Result;

            Assert.IsNotNull(auditReasons);
            Assert.IsNotNull(auditReasons.DeleteEntry);
            Assert.IsNotNull(auditReasons.ExportDocument);
        }
    }
}
