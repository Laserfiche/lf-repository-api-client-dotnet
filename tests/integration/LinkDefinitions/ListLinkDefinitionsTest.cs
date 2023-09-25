using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.LinkDefinitions
{
    [TestClass]
    public class ListLinkDefinitionsTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ReturnAllLinks()
        {
            var linkedDefinitionCollectionResponse = await client.LinkDefinitionsClient.ListLinkDefinitionsAsync(new ListLinkDefinitionsParameters()
            {
                RepositoryId = RepositoryId
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(linkedDefinitionCollectionResponse.Value);
        }

        [TestMethod]
        public async Task ForEachPaging()
        {
            int maxPageSize = 10;

            Task<bool> PagingCallback(LinkDefinitionCollectionResponse data)
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

            await client.LinkDefinitionsClient.ListLinkDefinitionsForEachAsync(PagingCallback, new ListLinkDefinitionsParameters()
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
            var linkDefinitionCollectionResponse = await client.LinkDefinitionsClient.ListLinkDefinitionsAsync(new ListLinkDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
                Prefer = $"maxpagesize={maxPageSize}"
            }).ConfigureAwait(false);
            
            Assert.IsNotNull(linkDefinitionCollectionResponse);

            if (linkDefinitionCollectionResponse.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = linkDefinitionCollectionResponse.OdataNextLink;
            
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(linkDefinitionCollectionResponse.Value.Count <= maxPageSize);

            // Paging request
            linkDefinitionCollectionResponse = await client.LinkDefinitionsClient.ListLinkDefinitionsNextLinkAsync(nextLink, maxPageSize).ConfigureAwait(false);
            
            Assert.IsNotNull(linkDefinitionCollectionResponse);
            Assert.IsTrue(linkDefinitionCollectionResponse.Value.Count <= maxPageSize);
        }
    }
}
