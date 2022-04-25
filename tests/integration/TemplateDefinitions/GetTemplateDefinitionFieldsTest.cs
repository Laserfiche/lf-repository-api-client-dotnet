using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    [TestClass]
    public class GetTemplateDefinitionFieldsTest : BaseTest_V1
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
        public async Task GetTemplateDefinitionFields_ReturnTemplateFields()
        {
            var allTemplateDefinitionsResponse = await client.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            var response = await client.GetTemplateFieldDefinitionsAsync(TestConfig.RepositoryId, firstTemplateDefinition.Id);
            var templateFieldDefinitions = response.Result?.Value;

            Assert.IsNotNull(templateFieldDefinitions);
            Assert.AreEqual(firstTemplateDefinition.FieldCount, templateFieldDefinitions.Count);
        }

        [TestMethod]
        public async Task GetTemplateDefinitionFields_Paging()
        {
            int maxMageSize = 10;

            var allTemplateDefinitionsResponse = await client.GetTemplateDefinitionsAsync(TestConfig.RepositoryId);
            var firstTemplateDefinition = allTemplateDefinitionsResponse.Result?.Value?.FirstOrDefault();
            Assert.IsNotNull(firstTemplateDefinition);

            bool PagingCallback(ODataValueContextOfIListOfTemplateFieldInfo data)
            {
                if (data.OdataNextLink != null)
                {
                    Assert.AreNotEqual(0, data.Value.Count);
                    Assert.IsTrue(data.Value.Count <= maxMageSize);
                    return true; // If data aren't exhusted, keep asking.
                }
                else
                {
                    return false;
                }
            }

            await client.GetTemplateFieldDefinitionsForEachAsync(PagingCallback, TestConfig.RepositoryId, firstTemplateDefinition.Id, string.Format("maxpagesize={0}", maxMageSize));
        }
    }
}
