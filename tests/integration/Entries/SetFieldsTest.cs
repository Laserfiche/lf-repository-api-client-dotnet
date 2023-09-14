using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Entries
{
    [TestClass]
    public class SetFieldsTest : BaseTest
    {
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
                StartDeleteEntryRequest body = new();
                await client.EntriesClient.StartDeleteEntryAsync(RepositoryId, entry.Id, body).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task SetAndReturnFields()
        {
            // Find a string field
            FieldDefinition field = null;
            string fieldValue = "a";
            var fieldDefinitionCollectionResponse = await client.FieldDefinitionsClient.ListFieldDefinitionsAsync(RepositoryId).ConfigureAwait(false);
            var fieldDefinitions = fieldDefinitionCollectionResponse.Value;
            
            Assert.IsNotNull(fieldDefinitions);
            
            foreach(var fieldDefinition in fieldDefinitions)
            {
                if (fieldDefinition.FieldType == FieldType.String && string.IsNullOrEmpty(fieldDefinition.Constraint) && fieldDefinition.Length >= 1)
                {
                    field = fieldDefinition;
                    break;
                }
            }
            
            Assert.IsNotNull(field, "Could not find a string field to set.");

            var request = new SetFieldsRequest()
            {
                Fields = new List<FieldToUpdate>
                {
                    new FieldToUpdate()
                    {
                        Values = new List<string> { fieldValue }
                    }
                }
            };
            entry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net SetFields").ConfigureAwait(false);

            var fieldCollectionResponse = await client.EntriesClient.SetFieldsAsync(RepositoryId, entry.Id, request).ConfigureAwait(false);
            var fields = fieldCollectionResponse.Value;
            
            Assert.IsNotNull(fields);
            Assert.AreEqual(1, fields.Count);
            Assert.AreEqual(field.Name, fields.FirstOrDefault()?.Name);
        }
    }
}
