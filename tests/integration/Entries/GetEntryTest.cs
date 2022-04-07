using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;
        int createdEntryId;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
            createdEntryId = 0;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (createdEntryId != 0)
            {
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.DeleteEntryInfoAsync(TestConfig.RepositoryId, createdEntryId, body);
                Thread.Sleep(10000);
            }
            await Logout(client);
        }

        [TestMethod]
        public async Task GetEntry_ReturnRootFolder()
        {
            int entryId = 1;
            var response = await client.GetEntryAsync(TestConfig.RepositoryId, entryId);
            var entry = response.Result;
            Assert.IsNotNull(entry);
            Assert.AreEqual(typeof(Folder), entry.GetType());
            Assert.AreEqual(entryId, entry.Id);
            Assert.AreEqual(EntryType.Folder, entry.EntryType);
        }

        private async Task<int> CreateDocument()
        {
            int parentEntryId = 1;
            string fileName = "APIServerClientIntegrationTest GetEntry";
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
        public async Task GetEntry_ReturnDocument()
        {
            createdEntryId = await CreateDocument();
            var response = await client.GetEntryAsync(TestConfig.RepositoryId, createdEntryId);
            var entry = response.Result;
            Assert.IsNotNull(entry);
            Assert.AreEqual(typeof(Document), entry.GetType());
            Assert.AreEqual(createdEntryId, entry.Id);
            Assert.AreEqual(EntryType.Document, entry.EntryType);
        }
    }
}
