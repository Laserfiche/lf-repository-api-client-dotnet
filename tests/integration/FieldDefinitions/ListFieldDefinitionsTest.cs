// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.FieldDefinitions
{
    [TestClass]
    public class ListFieldDefinitionsTest : BaseTest
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

            Task<bool> PagingCallback(FieldDefinitionCollectionResponse data)
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

            await client.FieldDefinitionsClient.ListFieldDefinitionsForEachAsync(PagingCallback, new ListFieldDefinitionsParameters()
            {
                RepositoryId = RepositoryId
            }, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var fieldDefinitionCollectionResponse = await client.FieldDefinitionsClient.ListFieldDefinitionsAsync(new ListFieldDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
                Prefer = $"maxpagesize={maxPageSize}"
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(fieldDefinitionCollectionResponse);

            if (fieldDefinitionCollectionResponse.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = fieldDefinitionCollectionResponse.OdataNextLink;
            
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(fieldDefinitionCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            fieldDefinitionCollectionResponse = await client.FieldDefinitionsClient.ListFieldDefinitionsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(fieldDefinitionCollectionResponse);
            Assert.IsTrue(fieldDefinitionCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
