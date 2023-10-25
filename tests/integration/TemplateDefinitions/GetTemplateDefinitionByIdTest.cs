// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionByIdTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetTemplateDefinitionById_ReturnTemplate()
        {
            var allTemplateDefinitionsResult = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstTemplateDefinition = allTemplateDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition, "No template definitions exist in the repository.");

            var result = await client.TemplateDefinitionsClient.GetTemplateDefinitionByIdAsync(RepositoryId, firstTemplateDefinition.Id).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(firstTemplateDefinition.Id, result.Id);
        }
    }
}
