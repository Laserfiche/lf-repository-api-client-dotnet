using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ImportDocumentTest : BaseTest
    {
        IRepositoryApiClient client = null;
        int createdEntryId;
        Stream fileStream;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            createdEntryId = 0;
            fileStream = null;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            fileStream?.Dispose();
            if (createdEntryId != 0)
            {
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, createdEntryId, body);
                Thread.Sleep(5000);
            }
        }

        private FileParameter GetFileParameter()
        {
            string fileLocation = TempPath + "test.pdf";
            fileStream = File.OpenRead(fileLocation);
            return new FileParameter(fileStream, "test", "application/pdf");
        }

        [TestMethod]
        public async Task ImportDocument_DocumentCreated()
        {
            int parentEntryId = 1;
            string fileName = "APIServerClientIntegrationTest ImportDocument";
            var electronicDocument = GetFileParameter();
            var request = new PostEntryWithEdocMetadataRequest();

            var result = await client.EntriesClient.ImportDocumentAsync(RepositoryId, parentEntryId, fileName, autoRename: true, electronicDocument: electronicDocument, request: request);

            var operations = result.Operations;
            Assert.IsNotNull(operations);
            Assert.AreEqual(0, operations.EntryCreate.Exceptions.Count);
            Assert.AreNotEqual(0, operations.EntryCreate.EntryId);
            Assert.AreEqual(0, operations.SetEdoc.Exceptions.Count);
            Assert.IsTrue(!string.IsNullOrEmpty(result.DocumentLink));
            createdEntryId = operations.EntryCreate.EntryId;
        }

        [TestMethod]
        public async Task ImportDocument_DocumentCreatedWithTemplate()
        {
            // Find a template definition with no required fields
            WTemplateInfo template = null;
            var templateDefinitionResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var templateDefinitions = templateDefinitionResult.Value;
            Assert.IsNotNull(templateDefinitions);
            Assert.IsTrue(templateDefinitions.Count > 0, "No template definitions exist in the repository.");
            foreach (var templateDefinition in templateDefinitions)
            {
                var templateDefinitionFieldsResult = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsAsync(RepositoryId, templateDefinition.Id);
                if (templateDefinitionFieldsResult.Value != null && templateDefinitionFieldsResult.Value.All(f => !f.IsRequired))
                {
                    template = templateDefinition;
                    break;
                }
            }
            Assert.IsNotNull(template, "Could not find a good template definition to assign to the entry");

            int parentEntryId = 1;
            string fileName = "APIServerClientIntegrationTest ImportDocument";
            var electronicDocument = GetFileParameter();
            var request = new PostEntryWithEdocMetadataRequest()
            {
                Template = template.Name
            };

            var result = await client.EntriesClient.ImportDocumentAsync(RepositoryId, parentEntryId, fileName, autoRename: true, electronicDocument: electronicDocument, request: request);

            var operations = result.Operations;
            Assert.IsNotNull(operations);
            Assert.AreEqual(0, operations.EntryCreate.Exceptions.Count);
            Assert.AreNotEqual(0, operations.EntryCreate.EntryId);
            Assert.AreEqual(0, operations.SetEdoc.Exceptions.Count);
            Assert.IsTrue(!string.IsNullOrEmpty(result.DocumentLink));
            Assert.AreEqual(0, operations.SetTemplate.Exceptions.Count);
            Assert.AreEqual(template.Name, operations.SetTemplate.Template);
            createdEntryId = operations.EntryCreate.EntryId;
        }
    }
}
