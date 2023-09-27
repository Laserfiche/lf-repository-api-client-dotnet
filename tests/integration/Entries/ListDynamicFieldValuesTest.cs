using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ListDynamicFieldValuesTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ReturnDynamicFields()
        {
            // Get a template definition id
            var templateDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(new ListTemplateDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
            }).ConfigureAwait(false);
            AssertCollectionResponse(templateDefinitionCollectionResponse);

            int entryId = 1;
            var request = new ListDynamicFieldValuesRequest() { TemplateId = templateDefinitionCollectionResponse.Value.First().Id };
            var dynamicFieldValues = await client.EntriesClient.ListDynamicFieldValuesAsync(new ListDynamicFieldValuesParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entryId,
                Request = request
            }).ConfigureAwait(false);

            Assert.IsNotNull(dynamicFieldValues);
        }
    }
}
