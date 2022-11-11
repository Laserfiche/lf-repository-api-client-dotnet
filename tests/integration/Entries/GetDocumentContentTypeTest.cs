using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetDocumentContentTypeTest : BaseTest
    {
        int createdEntryId;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntryId = 0;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (createdEntryId != 0)
            {
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, createdEntryId, body);
            }
        }

        [TestMethod]
        public async Task GetDocumentContentTypeAsync_ReturnHeaders()
        {
            createdEntryId = await CreateDocument("RepositoryApiClientIntegrationTest .Net GetDocumentContentTypeAsync");

            var response = await client.EntriesClient.GetDocumentContentTypeAsync(RepositoryId, createdEntryId);

            Assert.AreEqual(200, response.StatusCode);
            Assert.IsTrue(response.Headers.ContainsKey("Content-Type"));
            Assert.IsTrue(response.Headers.ContainsKey("Content-Length"));
        }
    }
}
