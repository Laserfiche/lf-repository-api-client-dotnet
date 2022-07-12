using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.LinkDefinitions
{
    [TestClass]
    public class GetLinkDefinitionByIdTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetLinkDefinitionByIdAsync_ReturnLinkDefinition()
        {
            var allLinkDefinitionsResult = await client.LinkDefinitionsClient.GetLinkDefinitionsAsync(RepositoryId);
            var firstLinkDefinition = allLinkDefinitionsResult.Value?.FirstOrDefault();
            Assert.IsNotNull(firstLinkDefinition, "No link definitions exist in the repository.");

            var linkDefinition = await client.LinkDefinitionsClient.GetLinkDefinitionByIdAsync(RepositoryId, firstLinkDefinition.LinkTypeId);

            Assert.IsNotNull(linkDefinition);
            Assert.AreEqual(firstLinkDefinition.LinkTypeId, linkDefinition.LinkTypeId);
        }
    }
}
