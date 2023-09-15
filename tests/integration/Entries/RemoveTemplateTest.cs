using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class RemoveTemplateTest : BaseTest
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
                StartDeleteEntryRequest body = new();
                await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, entry.Id, body).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task RemoveTemplateFromEntry()
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

            // Set the template on an entry
            var request = new SetTemplateRequest()
            {
                TemplateName = template.Name
            };
            entry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net RemoveTemplateFromEntry").ConfigureAwait(false);
            var entryWithTemplate = await client.EntriesClient.SetTemplateAsync(RepositoryId, entry.Id, request).ConfigureAwait(false);
            
            Assert.IsNotNull(entryWithTemplate);
            Assert.AreEqual(template.Name, entryWithTemplate.TemplateName);

            // Delete the template on the entry
            var entryWithoutTemplate = await client.EntriesClient.RemoveTemplateAsync(RepositoryId, entry.Id).ConfigureAwait(false);
            
            Assert.IsNotNull(entryWithoutTemplate);
            Assert.AreEqual(entry.Id, entryWithoutTemplate.Id);
            Assert.AreEqual(0, entryWithoutTemplate.TemplateId);
            Assert.IsTrue(string.IsNullOrEmpty(entryWithoutTemplate.TemplateName));
        }
    }
}
