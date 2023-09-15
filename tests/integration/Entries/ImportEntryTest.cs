﻿using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ImportEntryTest : BaseTest
    {
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
                StartDeleteEntryRequest body = new();
                await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, createdEntryId, body).ConfigureAwait(false);
            }
        }

        private FileParameter GetFileParameter()
        {
            string fileLocation = TempPath + "test.pdf";
            fileStream = File.OpenRead(fileLocation);
            return new FileParameter(fileStream, "test", "application/pdf");
        }

        [TestMethod]
        public async Task DocumentCreated()
        {
            int parentEntryId = 1;
            string fileName = "RepositoryApiClientIntegrationTest .Net ImportDocument";
            var electronicDocument = GetFileParameter();
            var request = new ImportEntryRequest()
            {
                Name = fileName,
                AutoRename = true
            };

            var importedEntry = await client.EntriesClient.ImportEntryAsync(RepositoryId, parentEntryId, fileName, file: electronicDocument, request: request).ConfigureAwait(false);

            Assert.IsNotNull(importedEntry);
        }

        [TestMethod]
        public async Task DocumentCreatedWithTemplate()
        {
            // Find a template definition with no required fields
            TemplateDefinition template = null;
            var templateDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var templateDefinitions = templateDefinitionCollectionResponse.Value;
            
            Assert.IsNotNull(templateDefinitions);
            Assert.IsTrue(templateDefinitions.Count > 0, "No template definitions exist in the repository.");
            
            foreach (var templateDefinition in templateDefinitions)
            {
                var templateFieldDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(RepositoryId, templateDefinition.Id).ConfigureAwait(false);
                if (templateFieldDefinitionCollectionResponse.Value != null && templateFieldDefinitionCollectionResponse.Value.All(f => !f.IsRequired))
                {
                    template = templateDefinition;
                    break;
                }
            }
            Assert.IsNotNull(template, "Could not find a good template definition to assign to the entry");

            int parentEntryId = 1;
            string fileName = "RepositoryApiClientIntegrationTest .Net ImportDocument";
            var electronicDocument = GetFileParameter();
            var request = new ImportEntryRequest()
            {
                Name = fileName,
                AutoRename = true,
                Metadata = new ImportEntryRequestMetadata()
                {
                    TemplateName = template.Name
                }
            };

            var importedEntry = await client.EntriesClient.ImportEntryAsync(RepositoryId, parentEntryId, fileName, file: electronicDocument, request: request).ConfigureAwait(false);

            Assert.IsNotNull(importedEntry);
            Assert.IsNotNull(importedEntry.TemplateId);
            Assert.IsNotNull(importedEntry.TemplateName);
            Assert.AreEqual(template.Name, importedEntry.TemplateName);
        }

        [TestMethod]
        public async Task ThrowExceptionWithCreateEntryResult()
        {
            int parentEntryId = 1;
            string fileName = "RepositoryApiClientIntegrationTest .Net ImportDocument";
            var electronicDocument = GetFileParameter();
            var request = new ImportEntryRequest()
            {
                Name = fileName,
                AutoRename = true,
                Metadata = new ImportEntryRequestMetadata()
                {
                    TemplateName = "faketemplate123"
                }
            };

            try
            {
                _ = await client.EntriesClient.ImportEntryAsync(RepositoryId, parentEntryId, fileName, file: electronicDocument, request: request).ConfigureAwait(false);
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
                Assert.AreEqual(204, e.ProblemDetails.ErrorCode);
                Assert.IsNotNull(e.ProblemDetails.TraceId);
                Assert.AreEqual(0, e.ProblemDetails.Extensions.Count);
            }
        }
    }
}
