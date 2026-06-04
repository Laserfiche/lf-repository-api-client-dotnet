// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public async Task ForEachPaging()
        {
            int maxPageSize = 10;

            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(new ListTemplateDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
            }).ConfigureAwait(false);
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

            await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdForEachAsync(PagingCallback, new ListTemplateFieldDefinitionsByTemplateIdParameters()
            {
                RepositoryId = RepositoryId,
                TemplateId = firstTemplateDefinition.Id
            }, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SimplePaging()
        {
            int maxPageSize = 1;

            // Self-sufficient: create a dedicated template with two fields so paging at
            // maxpagesize=1 deterministically yields a next link. Previously this paged the first
            // template in the shared repository and assumed it had >= 2 fields, which is brittle to
            // repository state/drift (a 0- or 1-field template sorting first fails the nextLink
            // assertion). See the field-assignment drift incident in WI #671441.
            var fieldNames = await PickExistingFieldNamesAsync(2).ConfigureAwait(false);
            int createdId = 0;
            try
            {
                var created = await client.TemplateDefinitionsClient.CreateTemplateAsync(new CreateTemplateParameters()
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateTemplateRequest()
                    {
                        Name = UniqueName("client_test_paging_tmpl"),
                        Fields = fieldNames.Select(n => new TemplateFieldAssignment() { FieldName = n }).ToList()
                    }
                }).ConfigureAwait(false);
                createdId = created.Id;

                // Initial request
                var templateFieldDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(new ListTemplateFieldDefinitionsByTemplateIdParameters()
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                    Prefer = $"maxpagesize={maxPageSize}"
                }).ConfigureAwait(false);

                Assert.IsNotNull(templateFieldDefinitionCollectionResponse);

                var nextLink = templateFieldDefinitionCollectionResponse.OdataNextLink;

                Assert.IsNotNull(nextLink);
                Assert.IsTrue(templateFieldDefinitionCollectionResponse.Value.Count <= maxPageSize);

                // Paging request
                templateFieldDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);

                Assert.IsNotNull(templateFieldDefinitionCollectionResponse);
                Assert.IsTrue(templateFieldDefinitionCollectionResponse.Value.Count <= maxPageSize);
            }
            finally
            {
                await SafeDeleteTemplateAsync(createdId).ConfigureAwait(false);
            }
        }
    }
}
