﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class RemoveTemplateFromEntryTest : BaseTest
    {
        IRepositoryApiClient client = null;
        Entry entry;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
            entry = null;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (entry != null)
            {
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.EntriesClient.DeleteEntryInfoAsync(TestConfig.RepositoryId, entry.Id, body);
                Thread.Sleep(5000);
            }
            await Logout(client);
        }

        [TestMethod]
        public async Task RemoveTemplateFromEntry_ReturnEntry()
        {
            // Find a template definition with no required fields
            WTemplateInfo template = null;
            var templateDefinitionResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            var templateDefinitions = templateDefinitionResponse.Result?.Value;
            Assert.IsNotNull(templateDefinitions);
            Assert.IsTrue(templateDefinitions.Count > 0, "No template definitions exist in the repository.");
            foreach (var templateDefinition in templateDefinitions)
            {
                var templateDefinitionFieldsResponse = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsAsync(TestConfig.RepositoryId, templateDefinition.Id);
                if (templateDefinitionFieldsResponse.Result?.Value != null && templateDefinitionFieldsResponse.Result.Value.All(f => !f.IsRequired))
                {
                    template = templateDefinition;
                    break;
                }
            }
            Assert.IsNotNull(template, "Could not find a good template definition to assign to the entry");

            // Set the template on an entry
            var request = new PutTemplateRequest()
            {
                TemplateName = template.Name
            };
            entry = await CreateEntry(client, "APIServerClientIntegrationTest RemoveTemplateFromEntry");
            var setTemplateEntryResponse = await client.EntriesClient.WriteTemplateValueToEntryAsync(TestConfig.RepositoryId, entry.Id, request);
            Assert.IsNotNull(setTemplateEntryResponse.Result);
            Assert.AreEqual(template.Name, setTemplateEntryResponse.Result.TemplateName);

            // Delete the template on the entry
            var deleteTemplateResponse = await client.EntriesClient.DeleteAssignedTemplateAsync(TestConfig.RepositoryId, entry.Id);
            var returnedEntry = deleteTemplateResponse.Result;
            Assert.IsNotNull(returnedEntry);
            Assert.AreEqual(entry.Id, returnedEntry.Id);
            Assert.AreEqual(0, returnedEntry.TemplateId);
            Assert.IsTrue(string.IsNullOrEmpty(returnedEntry.TemplateName));
        }
    }
}
