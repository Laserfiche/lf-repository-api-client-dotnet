using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class RemoveTemplateFromEntryTest : BaseTest
    {
        IRepositoryApiClient client = null;
        Entry entry;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            entry = null;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (entry != null)
            {
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, entry.Id, body);
            }
        }

        [TestMethod]
        public async Task RemoveTemplateFromEntry_ReturnEntry()
        {
            // Find a template definition with no required fields
            WTemplateInfo template = null;
            var templateDefinitionResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var templateDefinitions = templateDefinitionResponse.Result?.Value;
            Assert.IsNotNull(templateDefinitions);
            Assert.IsTrue(templateDefinitions.Count > 0, "No template definitions exist in the repository.");
            foreach (var templateDefinition in templateDefinitions)
            {
                var templateDefinitionFieldsResponse = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsAsync(RepositoryId, templateDefinition.Id);
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
            var setTemplateEntryResponse = await client.EntriesClient.WriteTemplateValueToEntryAsync(RepositoryId, entry.Id, request);
            Assert.IsNotNull(setTemplateEntryResponse.Result);
            Assert.AreEqual(template.Name, setTemplateEntryResponse.Result.TemplateName);

            // Delete the template on the entry
            var deleteTemplateResponse = await client.EntriesClient.DeleteAssignedTemplateAsync(RepositoryId, entry.Id);
            var returnedEntry = deleteTemplateResponse.Result;
            Assert.IsNotNull(returnedEntry);
            Assert.AreEqual(entry.Id, returnedEntry.Id);
            Assert.AreEqual(0, returnedEntry.TemplateId);
            Assert.IsTrue(string.IsNullOrEmpty(returnedEntry.TemplateName));
        }
    }
}
