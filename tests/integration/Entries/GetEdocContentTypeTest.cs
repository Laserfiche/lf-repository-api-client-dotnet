using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEDocContentTypeTest : BaseTest
    {
        IRepositoryApiClient client = null;
        int createdEntryId;
        readonly string contentType = "application/pdf";

        [TestInitialize]
        public void Initialize()
        {
            createdEntryId = 0;
            client = CreateClient();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (createdEntryId != 0)
            {
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, createdEntryId, body);
                Thread.Sleep(10000);
            }
        }

        private async Task<int> CreateDocument()
        {
            int parentEntryId = 1;
            string fileName = "APIServerClientIntegrationTest GetDocumentContentType";
            string fileLocation = TempPath + "test.pdf";
            var request = new PostEntryWithEdocMetadataRequest();
            using (var fileStream = File.OpenRead(fileLocation))
            {
                var electronicDocument = new FileParameter(fileStream, "test", contentType);
                var response = await client.EntriesClient.ImportDocumentAsync(RepositoryId, parentEntryId, fileName, autoRename: true, electronicDocument: electronicDocument, request: request);

                var operations = response.Result?.Operations;
                Assert.IsNotNull(operations?.EntryCreate);
                Assert.AreEqual(0, operations.EntryCreate.Exceptions.Count);
                Assert.AreNotEqual(0, operations.EntryCreate.EntryId);
                Assert.AreEqual(0, operations.SetEdoc.Exceptions.Count);
                Assert.IsFalse(string.IsNullOrEmpty(response.Result.DocumentLink));
                return operations.EntryCreate.EntryId;
            }
        }

        [TestMethod]
        public async Task GetEDocContentType_ReturnDocumentContentType()
        {
            createdEntryId = await CreateDocument();

            var response = await client.EntriesClient.GetDocumentContentTypeAsync(RepositoryId, createdEntryId);

            Assert.AreEqual(contentType, response.Headers["Content-Type"].FirstOrDefault());
            Assert.IsTrue(response.Headers.ContainsKey("Content-Length"));
        }
    }
}
