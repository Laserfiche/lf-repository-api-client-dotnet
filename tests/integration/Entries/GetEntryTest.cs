using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class GetEntryTest : BaseTest
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
        public async Task GetEntry_ReturnRootFolder()
        {
            int entryId = 1;
            var entry = await client.EntriesClient.GetEntryAsync(RepositoryId, entryId);
            Assert.IsNotNull(entry);
            Assert.AreEqual(typeof(Folder), entry.GetType());
            Assert.AreEqual(entryId, entry.Id);
            Assert.AreEqual(EntryType.Folder, entry.EntryType);
        }

        private async Task<int> CreateDocument()
        {
            int parentEntryId = 1;
            string fileName = "RepositoryApiClientIntegrationTest .Net GetEntry";
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
        public async Task GetEntry_ReturnDocument()
        {
            createdEntryId = await CreateDocument();
            var entry = await client.EntriesClient.GetEntryAsync(RepositoryId, createdEntryId);
            Assert.IsNotNull(entry);
            Assert.AreEqual(typeof(Document), entry.GetType());
            Assert.AreEqual(createdEntryId, entry.Id);
            Assert.AreEqual(EntryType.Document, entry.EntryType);
        }

        //[TestMethod]
        //public async Task GetEntry_ThrowException()
        //{
        //    int entryId = 1;
        //    string repositoryId = "fakeRepository";
        //    try
        //    {
        //        await client.EntriesClient.GetEntryAsync(repositoryId, entryId);
        //    }
        //    catch (ApiException e)
        //    {
        //        Assert.IsNotNull(e?.ProblemDetails?.Title);
        //        Assert.AreEqual(e.ProblemDetails.Title, e.Message);
        //        Assert.AreEqual((int)HttpStatusCode.NotFound, e.StatusCode);
        //        Assert.AreEqual((int)HttpStatusCode.NotFound, e.ProblemDetails.Status);
        //        Assert.IsNotNull(e.ProblemDetails.OperationId);
        //        Assert.IsNotNull(e.ProblemDetails.Type);
        //        Assert.IsNotNull(e.ProblemDetails.Instance);
        //        Assert.IsNotNull(e.ProblemDetails.ErrorSource);
        //        Assert.AreNotEqual(default, e.ProblemDetails.ErrorCode);
        //        Assert.IsNotNull(e.ProblemDetails.TraceId);
        //        Assert.AreEqual(0, e.ProblemDetails.Extensions.Count);
        //    }
        //}
    }
}
