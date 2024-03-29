// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task ReturnTemplate()
        {
            var templateDefinitionCollectionResponse = await client.TemplateDefinitionsClient.ListTemplateDefinitionsAsync(new ListTemplateDefinitionsParameters()
            {
                RepositoryId = RepositoryId,
            }).ConfigureAwait(false);

            AssertCollectionResponse(templateDefinitionCollectionResponse);
            
            var firstTemplateDefinition = templateDefinitionCollectionResponse.Value?.FirstOrDefault();
            
            Assert.IsNotNull(firstTemplateDefinition, "No template definitions exist in the repository.");

            var templateDefinition = await client.TemplateDefinitionsClient.GetTemplateDefinitionAsync(new GetTemplateDefinitionParameters()
            {
                RepositoryId = RepositoryId,
                TemplateId = firstTemplateDefinition.Id
            }).ConfigureAwait(false);

            Assert.IsNotNull(templateDefinition);
            Assert.AreEqual(firstTemplateDefinition.Id, templateDefinition.Id);
        }
    }
}
