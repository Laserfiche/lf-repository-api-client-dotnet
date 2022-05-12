using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionFieldsTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_ReturnTemplateFields()
        {
            var allTemplateDefinitionsResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var response = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsAsync(RepositoryId, firstTemplateDefinition.Id);
            var templateFieldDefinitions = response.Result?.Value;

            Assert.IsNotNull(templateFieldDefinitions);
            Assert.AreEqual(firstTemplateDefinition.FieldCount, templateFieldDefinitions.Count);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_ForEachPaging()
        {
            int maxPageSize = 10;

            var allTemplateDefinitionsResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            Task<bool> PagingCallback(SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo> data)
            {
                Assert.AreNotEqual(0, data.Result.Value.Count);
                Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                return Task.FromResult(true);
            }

            await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsForEachAsync(PagingCallback, RepositoryId, firstTemplateDefinition.Id, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var response = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.TemplateDefinitionsClient.GetTemplateDefinitionsNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
