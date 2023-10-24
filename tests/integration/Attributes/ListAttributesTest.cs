// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Attributes
{
    [TestClass]
    public class ListAttributesTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ReturnAttributes()
        {
            var response = await client.AttributesClient.ListAttributesAsync(new ListAttributesParameters()
            {
                RepositoryId = RepositoryId
            }).ConfigureAwait(false);

            Assert.IsNotNull(response.Value);
        }

        [TestMethod]
        public async Task ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(AttributeCollectionResponse data)
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

            await client.AttributesClient.ListAttributesForEachAsync(PagingCallback, new ListAttributesParameters()
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
            var result = await client.AttributesClient.ListAttributesAsync(new ListAttributesParameters()
            {
                RepositoryId = RepositoryId,
                Prefer = $"maxpagesize={maxPageSize}"
            }).ConfigureAwait(false);
            Assert.IsNotNull(result);

            if (result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(result.Value.Count <= maxPageSize);

            // Paging request
            result = await client.AttributesClient.ListAttributesNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Value.Count <= maxPageSize);
        }
    }
}
