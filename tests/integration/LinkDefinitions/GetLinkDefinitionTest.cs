// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.LinkDefinitions
{
    [TestClass]
    public class GetLinkDefinitionTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ReturnLinkDefinition()
        {
            var allLinkDefinitionsResult = await client.LinkDefinitionsClient.ListLinkDefinitionsAsync(new ListLinkDefinitionsParameters()
            {
                RepositoryId = RepositoryId
            }).ConfigureAwait(false);
            var firstLinkDefinition = allLinkDefinitionsResult.Value?.FirstOrDefault();
            
            Assert.IsNotNull(firstLinkDefinition, "No link definitions exist in the repository.");

            var linkDefinition = await client.LinkDefinitionsClient.GetLinkDefinitionAsync(new GetLinkDefinitionParameters()
            {
                RepositoryId = RepositoryId,
                LinkDefinitionId = firstLinkDefinition.Id
            }).ConfigureAwait(false);

            Assert.IsNotNull(linkDefinition);
            Assert.AreEqual(firstLinkDefinition.Id, linkDefinition.Id);
        }
    }
}
