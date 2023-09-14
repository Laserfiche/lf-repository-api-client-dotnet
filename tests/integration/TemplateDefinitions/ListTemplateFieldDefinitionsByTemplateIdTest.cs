﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class ListTemplateFieldDefinitionsByTemplateIdTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_ReturnTemplateFields()
        {
            var allTemplateDefinitions = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitions.Value?.FirstOrDefault();
            
            Assert.IsNotNull(firstTemplateDefinition);

            var templateFieldDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(RepositoryId, firstTemplateDefinition.Id).ConfigureAwait(false);
            var templateFieldDefinitions = templateFieldDefinitionCollectionResponse.Value;

            Assert.IsNotNull(templateFieldDefinitions);
            Assert.AreEqual(firstTemplateDefinition.FieldCount, templateFieldDefinitions.Count);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_ForEachPaging()
        {
            int maxPageSize = 10;

            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            
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

            await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdForEachAsync(PagingCallback, RepositoryId, firstTemplateDefinition.Id, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_SimplePaging()
        {
            var allTemplateDefinitions = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitions.Value?.FirstOrDefault();
            
            Assert.IsNotNull(firstTemplateDefinition);

            int maxPageSize = 1;

            // Initial request
            var templateFieldDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(RepositoryId, firstTemplateDefinition.Id, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            
            Assert.IsNotNull(templateFieldDefinitionCollectionResponse);

            if (templateFieldDefinitionCollectionResponse.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = templateFieldDefinitionCollectionResponse.OdataNextLink;
            
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(templateFieldDefinitionCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            templateFieldDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(templateFieldDefinitionCollectionResponse);
            Assert.IsTrue(templateFieldDefinitionCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
