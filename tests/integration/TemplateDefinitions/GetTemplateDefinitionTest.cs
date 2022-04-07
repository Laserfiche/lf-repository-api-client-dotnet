﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionTest : BaseTest_V1
    {
        ILaserficheRepositoryApiClient client = null;

        [TestInitialize]
        public async Task Initialize()
        {
            client = await CreateClientAndLogin();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await Logout(client);
        }

        [TestMethod]
        public async Task GetTemplateDefinition_ReturnAllTemplates()
        {
            var response = await client.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetTemplateDefinition_TemplateNameQueryParameter_ReturnSingleTemplate()
        {
            var allTemplateDefinitionsResponse = await client.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var response = await client.GetTemplateDefinitionsAsync(TestConfig.RepositoryId, templateName: firstTemplateDefinition.Name);
            Assert.IsNotNull(response.Result?.Value);
            Assert.AreEqual(1, response.Result?.Value.Count);
            Assert.AreEqual(firstTemplateDefinition.Id, response.Result?.Value.FirstOrDefault().Id);
        }
    }
}
