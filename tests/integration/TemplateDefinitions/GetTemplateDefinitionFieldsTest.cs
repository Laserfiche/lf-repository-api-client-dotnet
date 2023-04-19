using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionFieldsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_ReturnTemplateFields()
        {
            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var result = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsAsync(RepositoryId, firstTemplateDefinition.Id).ConfigureAwait(false);
            var templateFieldDefinitions = result.Value;

            Assert.IsNotNull(templateFieldDefinitions);
            Assert.AreEqual(firstTemplateDefinition.FieldCount, templateFieldDefinitions.Count);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_ForEachPaging()
        {
            int maxPageSize = 10;

            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
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

            await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsForEachAsync(PagingCallback, RepositoryId, firstTemplateDefinition.Id, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_SimplePaging()
        {
            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            int maxPageSize = 1;

            // Initial request
            var result = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsAsync(RepositoryId, firstTemplateDefinition.Id, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            Assert.IsNotNull(result);

            if (result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
