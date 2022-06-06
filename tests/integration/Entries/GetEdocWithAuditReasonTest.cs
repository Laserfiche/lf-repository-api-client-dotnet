using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEdocWithAuditReasonTest : BaseTest
    {
        IRepositoryApiClient client = null;
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

        private async Task<int> CreateDocument()
        {
            int parentEntryId = 1;
            string fileName = "APIServerClientIntegrationTest GetDocumentContent AuditReason";
            string fileLocation = TempPath + "test.pdf";
            var request = new PostEntryWithEdocMetadataRequest();
            using (var fileStream = File.OpenRead(fileLocation))
            {
                var electronicDocument = new FileParameter(fileStream, "test", "application/pdf");
                var result = await client.EntriesClient.ImportDocumentAsync(RepositoryId, parentEntryId, fileName, autoRename: true, electronicDocument: electronicDocument, request: request);

                var operations = result.Operations;
                Assert.IsNotNull(operations?.EntryCreate);
                Assert.AreEqual(0, operations.EntryCreate.Exceptions.Count);
                Assert.AreNotEqual(0, operations.EntryCreate.EntryId);
                Assert.AreEqual(0, operations.SetEdoc.Exceptions.Count);
                Assert.IsFalse(string.IsNullOrEmpty(result.DocumentLink));
                return operations.EntryCreate.EntryId;
            }
        }

        [TestMethod]
        public async Task Export_document_with_audit_reasonAsync_ReturnDocument()
        {
            createdEntryId = await CreateDocument();
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
