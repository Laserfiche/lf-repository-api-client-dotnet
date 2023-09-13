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
                StartDeleteEntryRequest body = new StartDeleteEntryRequest();
                await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, createdEntryId, body).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task GetEntry_ReturnRootFolder()
        {
            int entryId = 1;
            var entry = await client.EntriesClient.GetEntryAsync(RepositoryId, entryId).ConfigureAwait(false);
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
            var request = new ImportEntryRequest()
            {
                AutoRename = true
            }
            using (var fileStream = File.OpenRead(fileLocation))
            {
                var electronicDocument = new FileParameter(fileStream, "test", "application/pdf");
                var importedEntry = await client.EntriesClient.ImportEntryAsync(RepositoryId, parentEntryId, fileName, file: electronicDocument, request: request).ConfigureAwait(false);

                Assert.IsNotNull(importedEntry);
                Assert.IsNotNull(importedEntry.Id);

                return importedEntry.Id;
            }
        }

        [TestMethod]
        public async Task GetEntry_ReturnDocument()
        {
            createdEntryId = await CreateDocument().ConfigureAwait(false);
            var entry = await client.EntriesClient.GetEntryAsync(RepositoryId, createdEntryId).ConfigureAwait(false);
            Assert.IsNotNull(entry);
            Assert.AreEqual(typeof(Document), entry.GetType());
            Assert.AreEqual(createdEntryId, entry.Id);
            Assert.AreEqual(EntryType.Document, entry.EntryType);
        }

        [TestMethod]
        public async Task GetEntry_ThrowException()
        {
            int entryId = 1;
            string repositoryId = "fakeRepository";
            try
            {
                await client.EntriesClient.GetEntryAsync(repositoryId, entryId).ConfigureAwait(false);
            }
            catch (ApiException e)
            {
                Assert.IsNotNull(e?.ProblemDetails?.Title);
                Assert.AreEqual(e.ProblemDetails.Title, e.Message);
                Assert.AreEqual((int)HttpStatusCode.NotFound, e.StatusCode);
                Assert.AreEqual((int)HttpStatusCode.NotFound, e.ProblemDetails.Status);
                Assert.IsNotNull(e.ProblemDetails.OperationId);
                Assert.IsNotNull(e.ProblemDetails.Type);
                Assert.IsNotNull(e.ProblemDetails.Instance);
                Assert.IsNotNull(e.ProblemDetails.ErrorSource);
                Assert.AreNotEqual(default, e.ProblemDetails.ErrorCode);
                Assert.IsNotNull(e.ProblemDetails.TraceId);
                Assert.AreEqual(0, e.ProblemDetails.Extensions.Count);
            }
        }
    }
}
