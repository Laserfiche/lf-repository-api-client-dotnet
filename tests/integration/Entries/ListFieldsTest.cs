﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class ListFieldsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetEntryFields_ReturnFields()
        {
            int entryId = 1;
            var fieldCollectionResponse = await client.EntriesClient.ListFieldsAsync(RepositoryId, entryId).ConfigureAwait(false);
            
            Assert.IsNotNull(fieldCollectionResponse.Value);
        }

        [TestMethod]
        public async Task GetEntryFields_ForEachPaging()
        {
            int entryId = 1;
            int maxPageSize = 10;

            Task<bool> PagingCallback(FieldCollectionResponse data)
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

            await client.EntriesClient.ListFieldsForEachAsync(PagingCallback, RepositoryId, entryId, maxPageSize: maxPageSize).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetEntryFields_SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;

            // Initial request
            var fieldCollectionResponse = await client.EntriesClient.ListFieldsAsync(RepositoryId, entryId, prefer: $"maxpagesize={maxPageSize}").ConfigureAwait(false);
            
            Assert.IsNotNull(fieldCollectionResponse);

            if (fieldCollectionResponse.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = fieldCollectionResponse.OdataNextLink;
            
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(fieldCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            fieldCollectionResponse = await client.EntriesClient.ListFieldsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(fieldCollectionResponse);
            Assert.IsTrue(fieldCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
