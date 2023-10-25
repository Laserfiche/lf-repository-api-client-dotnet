// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
                await DeleteEntry(entry.Id).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task SetAndReturnFields()
        {
            // Find a string field
            FieldDefinition field = null;
            string fieldValue = "a";
            var fieldDefinitionCollectionResponse = await client.FieldDefinitionsClient.ListFieldDefinitionsAsync(new ListFieldDefinitionsParameters()
            {
                RepositoryId = RepositoryId
            }).ConfigureAwait(false);
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
                        Name = field.Name,
                        Values = new List<string> { fieldValue }
                    }
                }
            };
            entry = await CreateEntry(client, "RepositoryApiClientIntegrationTest .Net SetFields").ConfigureAwait(false);

            var fieldCollectionResponse = await client.EntriesClient.SetFieldsAsync(new SetFieldsParameters()
            {
                RepositoryId = RepositoryId,
                EntryId = entry.Id,
                Request = request
            }).ConfigureAwait(false);
            var fields = fieldCollectionResponse.Value;
            
            Assert.IsNotNull(fields);
            Assert.AreEqual(1, fields.Count);
            Assert.AreEqual(field.Name, fields.FirstOrDefault()?.Name);
        }
    }
}
