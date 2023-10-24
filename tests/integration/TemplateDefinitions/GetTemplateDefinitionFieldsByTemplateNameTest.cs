// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionFieldsByTemplateNameTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_ReturnTemplateFields()
        {
            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var result = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsByTemplateNameAsync(RepositoryId, firstTemplateDefinition.Name).ConfigureAwait(false);
            var templateFieldDefinitions = result.Value;

            Assert.IsNotNull(templateFieldDefinitions);
            Assert.AreEqual(firstTemplateDefinition.FieldCount, templateFieldDefinitions.Count);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_ForEachPaging()
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

            await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsByTemplateNameForEachAsync(PagingCallback, RepositoryId, firstTemplateDefinition.Name, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_SimplePaging()
        {
            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            int maxPageSize = 1;

            // Initial request
            var result = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsByTemplateNameAsync(RepositoryId, firstTemplateDefinition.Name, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            Assert.IsNotNull(result);

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsByTemplateNameNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
