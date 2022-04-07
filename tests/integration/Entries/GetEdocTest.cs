using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEdocTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;
        int createdEntryId;
        string fileToWriteTo;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
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
                await client.DeleteEntryInfoAsync(TestConfig.RepositoryId, createdEntryId, body);
                Thread.Sleep(10000);
            }
            await Logout(client);
        }

        private async Task<int> CreateDocument()
        {
            int parentEntryId = 1;
            string fileName = "APIServerClientIntegrationTest GetDocumentContent";
            string fileLocation = TempPath + "test.pdf";
            var request = new PostEntryWithEdocMetadataRequest();
            using (var fileStream = File.OpenRead(fileLocation))
            {
                var electronicDocument = new FileParameter(fileStream, "test", "application/pdf");
                var response = await client.ImportDocumentAsync(TestConfig.RepositoryId, parentEntryId, fileName, autoRename: true, electronicDocument, request);

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
        public async Task GetEdoc_ReturnDocument()
        {
            createdEntryId = await CreateDocument();

            using (var response = await client.ExportDocumentAsync(TestConfig.RepositoryId, createdEntryId))
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
