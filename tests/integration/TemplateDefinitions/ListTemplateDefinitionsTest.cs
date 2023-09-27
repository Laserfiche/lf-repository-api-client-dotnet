using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
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
        public async Task ReturnAllTemplates()
        {
            var templateDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(new ListTemplateDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
            }).ConfigureAwait(false);
            
            AssertCollectionResponse(templateDefinitionCollectionResponse);
        }

        [TestMethod]
        public async Task TemplateNameQueryParameter_ReturnSingleTemplate()
        {
            var allTemplateDefinitions = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(new ListTemplateDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
            }).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitions.Value?.FirstOrDefault();
            
            Assert.IsNotNull(firstTemplateDefinition);

            var templateDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(new ListTemplateDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
                TemplateName = firstTemplateDefinition.Name
            }).ConfigureAwait(false);

            AssertCollectionResponse(templateDefinitionCollectionResponse);
            Assert.AreEqual(1, templateDefinitionCollectionResponse.Value.Count);
            Assert.AreEqual(firstTemplateDefinition.Id, templateDefinitionCollectionResponse.Value.FirstOrDefault().Id);
        }

        [TestMethod]
        public async Task ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(TemplateDefinitionCollectionResponse data)
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

            await client.TemplateDefinitionsClient.ListTemplateDefinitionsForEachAsync(PagingCallback, new ListTemplateDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
            }, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var templateDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(new ListTemplateDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
                Prefer = $"maxpagesize={maxPageSize}"
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(templateDefinitionCollectionResponse);

            if (templateDefinitionCollectionResponse.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = templateDefinitionCollectionResponse.OdataNextLink;
            
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(templateDefinitionCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            templateDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateDefinitionsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(templateDefinitionCollectionResponse);
            Assert.IsTrue(templateDefinitionCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
