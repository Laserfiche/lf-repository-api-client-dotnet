using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class SetTemplateTest : BaseTest
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
                await DeleteEntry(entry.Id).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ReturnEntryWithTemplate()
        {
            // Find a template definition with no required fields
            TemplateDefinition template = null;
            var templateDefinitionResult = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(new ListTemplateDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
            }).ConfigureAwait(false);
            var templateDefinitions = templateDefinitionResult.Value;
            Assert.IsNotNull(templateDefinitions);
            Assert.IsTrue(templateDefinitions.Count > 0, "No template definitions exist in the repository.");
            foreach (var templateDefinition in templateDefinitions)
            {
                var templateDefinitionFields = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(new ListTemplateFieldDefinitionsByTemplateIdParameters()
                {
                    RepositoryId = RepositoryId,
                    TemplateId = templateDefinition.Id,
                }).ConfigureAwait(false);
                if (templateDefinitionFields.Value != null && templateDefinitionFields.Value.All(f => !f.IsRequired))
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
            entry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net SetTemplate").ConfigureAwait(false);
            var setTemplateResult = await client.EntriesClient.SetTemplateAsync(new SetTemplateParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entry.Id,
                Request = request
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(setTemplateResult);
            Assert.AreEqual(template.Name, setTemplateResult.TemplateName);
        }
    }
}
