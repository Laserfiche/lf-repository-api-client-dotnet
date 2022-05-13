using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class SetFieldsTest : BaseTest
    {
        IRepositoryApiClient client = null;
        Entry entry;

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
            entry = null;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (entry != null)
            {
                DeleteEntryWithAuditReason body = new DeleteEntryWithAuditReason();
                await client.EntriesClient.DeleteEntryInfoAsync(RepositoryId, entry.Id, body);
            }
        }

        [TestMethod]
        public async Task SetFields_ReturnFields()
        {
            // Find a string field
            WFieldInfo field = null;
            string fieldValue = "a";
            var fieldDefinitionsResponse = await client.FieldDefinitionsClient.GetFieldDefinitionsAsync(RepositoryId);
            var fieldDefinitions = fieldDefinitionsResponse.Result?.Value;
            Assert.IsNotNull(fieldDefinitions);
            foreach(var fieldDefinition in fieldDefinitions)
            {
                if (fieldDefinition.FieldType == WFieldType.String && string.IsNullOrEmpty(fieldDefinition.Constraint) && fieldDefinition.Length >= 1)
                {
                    field = fieldDefinition;
                    break;
                }
            }
            Assert.IsNotNull(field, "Could not find a string field to set.");

            var requestBody = new Dictionary<string, FieldToUpdate>()
            {
                [field.Name] = new FieldToUpdate()
                {
                    Values = new List<ValueToUpdate>()
                    {
                        new ValueToUpdate() { Value = fieldValue, Position = 1 }
                    }
                }
            };
            entry = await CreateEntry(client, "APIServerClientIntegrationTest SetFields");

            var response = await client.EntriesClient.AssignFieldValuesAsync(RepositoryId, entry.Id, requestBody);
            var fields = response.Result?.Value;
            Assert.IsNotNull(fields);
            Assert.AreEqual(1, fields.Count);
            Assert.AreEqual(field.Name, fields.FirstOrDefault()?.FieldName);
            Assert.AreEqual(fieldValue, fields.FirstOrDefault()?.Values.FirstOrDefault()["value"]);
        }
    }
}
