using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class ListTemplateDefinitionFieldsByTemplateNameTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_ReturnTemplateFields()
        {
            var allTemplateDefinitions = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitions.Value?.FirstOrDefault();
            
            Assert.IsNotNull(firstTemplateDefinition);

            var templateDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateNameAsync(RepositoryId, firstTemplateDefinition.Name).ConfigureAwait(false);
            var templateFieldDefinitions = templateDefinitionCollectionResponse.Value;

            Assert.IsNotNull(templateFieldDefinitions);
            Assert.AreEqual(firstTemplateDefinition.FieldCount, templateFieldDefinitions.Count);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_ForEachPaging()
        {
            int maxPageSize = 10;

            var allTemplateDefinitions = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitions.Value?.FirstOrDefault();

            AssertCollectionResponse(allTemplateDefinitions);
            Assert.IsNotNull(firstTemplateDefinition);

            Task<bool> PagingCallback(TemplateFieldDefinitionCollectionResponse data)
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

            await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateNameForEachAsync(PagingCallback, RepositoryId, firstTemplateDefinition.Name, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_SimplePaging()
        {
            var allTemplateDefinitions = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitions.Value?.FirstOrDefault();
            
            Assert.IsNotNull(firstTemplateDefinition);

            int maxPageSize = 1;

            // Initial request
            var templateDeifnitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateNameAsync(RepositoryId, firstTemplateDefinition.Name, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            
            Assert.IsNotNull(templateDeifnitionCollectionResponse);
            
            var nextLink = templateDeifnitionCollectionResponse.OdataNextLink;
            
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(templateDeifnitionCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            templateDeifnitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateNameNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(templateDeifnitionCollectionResponse);
            Assert.IsTrue(templateDeifnitionCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
