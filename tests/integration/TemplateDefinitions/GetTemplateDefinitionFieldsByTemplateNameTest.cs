using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionFieldsByTemplateNameTest : BaseTest
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
        public async Task GetTemplateDefinitionFieldsByTemplateName_ReturnTemplateFields()
        {
            var allTemplateDefinitionsResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var response = await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsByTemplateNameAsync(TestConfig.RepositoryId, firstTemplateDefinition.Name);
            var templateFieldDefinitions = response.Result?.Value;

            Assert.IsNotNull(templateFieldDefinitions);
            Assert.AreEqual(firstTemplateDefinition.FieldCount, templateFieldDefinitions.Count);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_ForEachPaging()
        {
            int maxPageSize = 10;

            var allTemplateDefinitionsResponse = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            bool PagingCallback(SwaggerResponse<ODataValueContextOfIListOfTemplateFieldInfo> data)
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

            await client.TemplateDefinitionsClient.GetTemplateFieldDefinitionsByTemplateNameForEachAsync(PagingCallback, TestConfig.RepositoryId, firstTemplateDefinition.Name, maxPageSize: maxPageSize);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFieldsByTemplateName_SimplePaging()
        {
            int entryId = 1;
            int maxPageSize = 1;

            // Initial request
            var response = await client.TemplateDefinitionsClient.GetTemplateDefinitionsAsync(TestConfig.RepositoryId, prefer: $"maxpagesize={maxPageSize}");
            Assert.IsNotNull(response);

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
