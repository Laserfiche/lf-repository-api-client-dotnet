using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionFieldsByTemplateNameTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_ReturnTemplateFields()
        {
            var allTemplateDefinitionsResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var response = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsByTemplateNameAsync(RepositoryId, firstTemplateDefinition.Name);
            var templateFieldDefinitions = response.Result?.Value;

            Assert.IsNotNull(templateFieldDefinitions);
            Assert.AreEqual(firstTemplateDefinition.FieldCount, templateFieldDefinitions.Count);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_ForEachPaging()
        {
            int maxPageSize = 10;

            var allTemplateDefinitionsResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            Task<bool> PagingCallback(SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo> data)
            {
                if (data.Result.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Result.Value.Count);
                    Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }

            await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsByTemplateNameForEachAsync(PagingCallback, RepositoryId, firstTemplateDefinition.Name, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var response = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

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
