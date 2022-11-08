using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEdocWithAuditReasonTest : BaseTest
    {
        int createdEntryId;
        string fileToWriteTo;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntryId = 0;
            fileToWriteTo = "";
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (!string.IsNullOrEmpty(fileToWriteTo))
            {
                File.Delete(fileToWriteTo);
            }
            if (createdEntryId != 0)
            {
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, createdEntryId, body);
            }
        }

        [TestMethod]
        public async Task Export_document_with_audit_reasonAsync_ReturnDocument()
        {
            createdEntryId = await CreateDocument("RepositoryApiClientIntegrationTest .Net ExportDocumentWithAuditReasonAsync");
            var request = new GetEdocWithAuditReasonRequest();

            using (var response = await client.EntriesClient.ExportDocumentWithAuditReasonAsync(RepositoryId, createdEntryId, request))
            {
                Assert.AreEqual(200, response.StatusCode);
                Assert.IsTrue(response.Headers.ContainsKey("Content-Type"));
                Assert.IsTrue(response.Headers.ContainsKey("Content-Length"));
                Assert.IsNotNull(response.Stream);

                fileToWriteTo = Path.GetTempFileName();
                using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                {
                    await response.Stream.CopyToAsync(streamToWriteTo);
                }
            }
        }
    }
}
