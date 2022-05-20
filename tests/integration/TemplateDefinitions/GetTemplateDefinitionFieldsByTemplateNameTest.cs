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
            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var result = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsByTemplateNameAsync(RepositoryId, firstTemplateDefinition.Name);
            var templateFieldDefinitions = result.Value;

            Assert.IsNotNull(templateFieldDefinitions);
            Assert.AreEqual(firstTemplateDefinition.FieldCount, templateFieldDefinitions.Count);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_ForEachPaging()
        {
            int maxPageSize = 10;

            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            Task<bool> PagingCallback(ODataValueContextOfIListOfTemplateFieldInfo data)
            {
                if (data.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Value.Count);
                    Assert.IsTrue(data.Value.Count <= maxPageSize);
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
            var result = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(result);

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.TemplateDefinitionsClient.GetTemplateDefinitionsNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
