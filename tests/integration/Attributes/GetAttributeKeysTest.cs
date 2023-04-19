﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Attributes
{
    [TestClass]
    public class GetAttributesTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetAttributes_ReturnAttributes()
        {
            var response = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsAsync(RepositoryId).ConfigureAwait(false);
            Assert.IsNotNull(response.Value);
        }

        [TestMethod]
        public async Task GetAttributes_ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(ODataValueContextOfListOfAttribute data)
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

            await client.AttributesClient.GetTrusteeAttributeKeyValuePairsForEachAsync(PagingCallback, RepositoryId, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAttributes_SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var result = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsAsync(RepositoryId, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            Assert.IsNotNull(result);

            if (result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.AttributesClient.GetTrusteeAttributeKeyValuePairsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
