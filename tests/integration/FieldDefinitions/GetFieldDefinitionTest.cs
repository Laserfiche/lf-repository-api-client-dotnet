using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.FieldDefinitions
{
    [TestClass]
    public class GetFieldDefinitionTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        [TestMethod]
        public async Task GetFieldDefinitionById_ReturnField()
        {
            var allFieldDefinitionsResult = await client.FieldDefinitionsClient.ListFieldDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var firstFieldDefinition = allFieldDefinitionsResult.Value?.FirstOrDefault();
            
            Assert.IsNotNull(firstFieldDefinition, "No field definitions exist in the repository.");

            var fieldDefinition = await client.FieldDefinitionsClient.GetFieldDefinitionAsync(RepositoryId, firstFieldDefinition.Id).ConfigureAwait(false);

            Assert.IsNotNull(fieldDefinition);
            Assert.AreEqual(firstFieldDefinition.Id, fieldDefinition.Id);
        }
    }
}
