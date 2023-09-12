using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ListTemplateDefinitionsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetDynamicFields_ReturnDynamicFields()
        {
            // Get a template definition id
            var collectionResponse = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            AssertCollectionResponse(collectionResponse);

            int entryId = 1;
            var request = new ListDynamicFieldValuesRequest() { TemplateId = collectionResponse.Value.First().Id };
            var result = await client.EntriesClient.ListDynamicFieldValuesAsync(RepositoryId, entryId, request).ConfigureAwait(false);

            Assert.IsNotNull(result);
        }
    }
}
