using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionTest : BaseTest
    {
        IRepositoryApiClient client = null;

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
            var response = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            Assert.IsNotNull(response.Result?.Value);
        }

        [TestMethod]
        public async Task GetTemplateDefinition_TemplateNameQueryParameter_ReturnSingleTemplate()
        {
            var allTemplateDefinitionsResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var response = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(TestConfig.RepositoryId, templateName: firstTemplateDefinition.Name);
            Assert.IsNotNull(response.Result?.Value);
            Assert.AreEqual(1, response.Result?.Value.Count);
            Assert.AreEqual(firstTemplateDefinition.Id, response.Result?.Value.FirstOrDefault().Id);
        }

        [TestMethod]
        public async Task GetTemplateDefinition_ForEachPaging()
        {
            int maxPageSize = 10;

            bool PagingCallback(SwaggerResponse<ODataValueContextOfIListOfWTemplateInfo> data)
            {
                if (data.Result.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Result.Value.Count);
                    Assert.IsTrue(data.Result.Value.Count <= maxPageSize);
                    return true; // If data aren't exhusted, keep asking.
                }
                else
                {
                    return false;
                }
            }

            await client.TemplateDefinitionsClient.GetTemplateDefinitionsForEachAsync(PagingCallback, TestConfig.RepositoryId, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetTemplateDefinition_SimplePaging()
        {
            int maxPageSize = 1;

            // Initial request
            var response = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(TestConfig.RepositoryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

            if (response.Result.Value.Count == 0)
            {
                return; // There's no point testing if we don't have any such item.
            }

            var nextLink = response.Result.OdataNextLink;
            Assert.IsNotNull(nextLink);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);

            // Paging request
            response = await client.TemplateDefinitionsClient.GetTemplateDefinitionsNextLinkAsync(nextLink, maxPageSize);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Result.Value.Count <= maxPageSize);
        }
    }
}
