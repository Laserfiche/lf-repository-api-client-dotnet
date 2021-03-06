using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class RemoveTemplateFromEntryTest : BaseTest
    {
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

            // Set the template on an entry
            var request = new PutTemplateRequest()
            {
                TemplateName = template.Name
            };
            entry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net RemoveTemplateFromEntry");
            var setTemplateEntryResult = await client.EntriesClient.WriteTemplateValueToEntryAsync(RepositoryId, entry.Id, request);
            Assert.IsNotNull(setTemplateEntryResult);
            Assert.AreEqual(template.Name, setTemplateEntryResult.TemplateName);

            // Delete the template on the entry
            var deleteTemplateResponse = await client.EntriesClient.DeleteAssignedTemplateAsync(RepositoryId, entry.Id);
            var returnedEntry = deleteTemplateResponse;
            Assert.IsNotNull(returnedEntry);
            Assert.AreEqual(entry.Id, returnedEntry.Id);
            Assert.AreEqual(0, returnedEntry.TemplateId);
            Assert.IsTrue(string.IsNullOrEmpty(returnedEntry.TemplateName));
        }
    }
}
