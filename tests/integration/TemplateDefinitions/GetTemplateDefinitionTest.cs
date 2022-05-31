using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionTest : BaseTest
    {
        IRepositoryApiClient client = null;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTemplateDefinition_ReturnAllTemplates()
        {
            var result = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public async Task GetTemplateDefinition_TemplateNameQueryParameter_ReturnSingleTemplate()
        {
            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var result = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId, templateName: firstTemplateDefinition.Name);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Count);
            Assert.AreEqual(firstTemplateDefinition.Id, result.Value.FirstOrDefault().Id);
        }

        [TestMethod]
        public async Task GetTemplateDefinition_ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(ODataValueContextOfIListOfWTemplateInfo data)
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

            await client.TemplateDefinitionsClient.GetTemplateDefinitionsForEachAsync(PagingCallback, RepositoryId, maxPageSize: maxPageSize);
            await Task.Delay(5000);
        }

        [TestMethod]
        public async Task GetTemplateDefinition_SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var result = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(result);

            if (result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

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
