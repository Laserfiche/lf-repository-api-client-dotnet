﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TagDefinitions
{
    [TestClass]
    public class GetTagDefinitionTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTagDefinition_ReturnTag()
        {
            var tagDefinitionCollectionResponse = await client.TagDefinitionsClient.ListTagDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTagDefinition = tagDefinitionCollectionResponse.Value?.FirstOrDefault();
            
            Assert.IsNotNull(firstTagDefinition, "No tag definitions exist in the repository.");

            var tagDefinition = await client.TagDefinitionsClient.GetTagDefinitionAsync(RepositoryId, firstTagDefinition.Id).ConfigureAwait(false);

            Assert.IsNotNull(tagDefinition);
            Assert.AreEqual(firstTagDefinition.Id, tagDefinition.Id);
        }
    }
}