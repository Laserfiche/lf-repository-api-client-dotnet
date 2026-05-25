// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.FieldDefinitions
{
    /// <summary>
    /// Integration tests for the field-definition admin endpoints introduced by PRD 6.3.A:
    /// Create / Update / Delete + GetListValues / ReplaceListValues + GetContainingTemplates + GetAssignedEntryCount.
    /// </summary>
    [TestClass]
    public class FieldDefinitionAdminTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        private static string UniqueName(string prefix) =>
            $"{prefix}_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}_{Guid.NewGuid().ToString("N").Substring(0, 6)}";

        [TestMethod]
        public async Task CreateUpdateDelete_StringField_Lifecycle()
        {
            string fieldName = UniqueName("client_test_field");
            int createdId = 0;
            try
            {
                // Create
                var created = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = fieldName,
                        FieldType = FieldType.String,
                        Length = 50,
                        Description = "Created from .NET client integration test",
                        IsIndexed = true,
                        WarnIfBlank = true,
                    }
                }).ConfigureAwait(false);
                Assert.IsNotNull(created);
                Assert.IsTrue(created.Id > 0);
                Assert.AreEqual(fieldName, created.Name);
                Assert.AreEqual(FieldType.String, created.FieldType);
                Assert.IsTrue(created.IsIndexed);
                Assert.IsTrue(created.WarnIfBlank);
                createdId = created.Id;

                // Update — patch description and IsRequired only
                var updated = await client.FieldDefinitionsClient.UpdateFieldDefinitionAsync(new UpdateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                    Request = new UpdateFieldDefinitionRequest
                    {
                        Description = "Updated description",
                        IsRequired = true,
                    }
                }).ConfigureAwait(false);
                Assert.AreEqual("Updated description", updated.Description);
                Assert.IsTrue(updated.IsRequired);
                Assert.AreEqual(fieldName, updated.Name); // Name preserved (not in PATCH)
                Assert.IsTrue(updated.IsIndexed);          // Flag preserved (not in PATCH)
            }
            finally
            {
                if (createdId > 0)
                {
                    await client.FieldDefinitionsClient.DeleteFieldDefinitionAsync(new DeleteFieldDefinitionParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = createdId,
                    }).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task ListField_ReplaceListValues_RoundTrip()
        {
            string fieldName = UniqueName("client_test_list_field");
            int createdId = 0;
            try
            {
                var initial = new List<string> { "Alpha", "Beta", "Gamma" };
                var created = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = fieldName,
                        FieldType = FieldType.List,
                        ListValues = initial,
                    }
                }).ConfigureAwait(false);
                createdId = created.Id;

                var listed = await client.FieldDefinitionsClient.GetFieldListValuesAsync(new GetFieldListValuesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                }).ConfigureAwait(false);
                CollectionAssert.AreEqual(initial, (List<string>)listed.Values);

                var replacement = new List<string> { "One", "Two", "Three", "Four" };
                var afterReplace = await client.FieldDefinitionsClient.ReplaceFieldListValuesAsync(new ReplaceFieldListValuesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                    Request = new ReplaceListValuesRequest { Values = replacement },
                }).ConfigureAwait(false);
                CollectionAssert.AreEqual(replacement, (List<string>)afterReplace.Values);

                // Independent GET — proves the PUT persisted; same-request reads can mask a missing Save() (Trap 4).
                var afterReplaceReread = await client.FieldDefinitionsClient.GetFieldListValuesAsync(new GetFieldListValuesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                }).ConfigureAwait(false);
                CollectionAssert.AreEqual(replacement, (List<string>)afterReplaceReread.Values);

                // Clear via empty array
                var afterClear = await client.FieldDefinitionsClient.ReplaceFieldListValuesAsync(new ReplaceFieldListValuesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                    Request = new ReplaceListValuesRequest { Values = new List<string>() },
                }).ConfigureAwait(false);
                Assert.AreEqual(0, afterClear.Values.Count);

                var afterClearReread = await client.FieldDefinitionsClient.GetFieldListValuesAsync(new GetFieldListValuesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                }).ConfigureAwait(false);
                Assert.AreEqual(0, afterClearReread.Values.Count);
            }
            finally
            {
                if (createdId > 0)
                {
                    await client.FieldDefinitionsClient.DeleteFieldDefinitionAsync(new DeleteFieldDefinitionParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = createdId,
                    }).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task GetContainingTemplates_FieldNotInAnyTemplate_ReturnsEmpty()
        {
            string fieldName = UniqueName("client_test_orphan_field");
            int createdId = 0;
            try
            {
                var created = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest { Name = fieldName, FieldType = FieldType.String, Length = 10 }
                }).ConfigureAwait(false);
                createdId = created.Id;

                var containing = await client.FieldDefinitionsClient.GetFieldContainingTemplatesAsync(new GetFieldContainingTemplatesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                }).ConfigureAwait(false);

                Assert.IsNotNull(containing);
                Assert.AreEqual(0, containing.Count);
            }
            finally
            {
                if (createdId > 0)
                {
                    await client.FieldDefinitionsClient.DeleteFieldDefinitionAsync(new DeleteFieldDefinitionParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = createdId,
                    }).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task ExtendedProperties_CreateGetUpdate_RoundTrip()
        {
            string fieldName = UniqueName("client_test_props_field");
            int createdId = 0;
            try
            {
                // Create with an initial property set — atomic with the create call.
                var initialProps = new Dictionary<string, string>
                {
                    { "lf-cli-test-key1", "alpha" },
                    { "lf-cli-test-key2", "beta" },
                };
                var created = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = fieldName,
                        FieldType = FieldType.String,
                        Length = 25,
                        Properties = initialProps,
                    }
                }).ConfigureAwait(false);
                createdId = created.Id;

                // GET — bag should include the two keys we set on Create (server may also include
                // RA-internal properties; we only assert ours are present, not exclusivity).
                var bag = await client.FieldDefinitionsClient.GetFieldPropertiesAsync(new GetFieldPropertiesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                }).ConfigureAwait(false);
                Assert.IsTrue(bag.Properties.ContainsKey("lf-cli-test-key1"));
                Assert.AreEqual("alpha", bag.Properties["lf-cli-test-key1"]);
                Assert.AreEqual("beta", bag.Properties["lf-cli-test-key2"]);

                // PATCH — set one new key, remove one of the existing keys.
                var afterUpdate = await client.FieldDefinitionsClient.UpdateFieldPropertiesAsync(new UpdateFieldPropertiesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                    Request = new UpdateFieldPropertiesRequest
                    {
                        Set = new Dictionary<string, string> { { "lf-cli-test-key3", "gamma" } },
                        Remove = new List<string> { "lf-cli-test-key1" },
                    },
                }).ConfigureAwait(false);
                Assert.IsFalse(afterUpdate.Properties.ContainsKey("lf-cli-test-key1"), "key1 should be removed");
                Assert.AreEqual("beta", afterUpdate.Properties["lf-cli-test-key2"], "key2 should be unchanged");
                Assert.AreEqual("gamma", afterUpdate.Properties["lf-cli-test-key3"], "key3 should be added");

                // Independent GET — verify the PATCH persisted (defense against same-request masking).
                var afterUpdateReread = await client.FieldDefinitionsClient.GetFieldPropertiesAsync(new GetFieldPropertiesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                }).ConfigureAwait(false);
                Assert.IsFalse(afterUpdateReread.Properties.ContainsKey("lf-cli-test-key1"));
                Assert.AreEqual("gamma", afterUpdateReread.Properties["lf-cli-test-key3"]);
            }
            finally
            {
                if (createdId > 0)
                {
                    await client.FieldDefinitionsClient.DeleteFieldDefinitionAsync(new DeleteFieldDefinitionParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = createdId,
                    }).ConfigureAwait(false);
                }
            }
        }

        [TestMethod]
        public async Task GetAssignedEntryCount_BrandNewField_ReturnsZero()
        {
            string fieldName = UniqueName("client_test_unassigned_field");
            int createdId = 0;
            try
            {
                var created = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest { Name = fieldName, FieldType = FieldType.String, Length = 10 }
                }).ConfigureAwait(false);
                createdId = created.Id;

                var count = await client.FieldDefinitionsClient.GetFieldAssignedEntryCountAsync(new GetFieldAssignedEntryCountParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                }).ConfigureAwait(false);

                Assert.IsNotNull(count);
                Assert.AreEqual(0, count.Count);
            }
            finally
            {
                if (createdId > 0)
                {
                    await client.FieldDefinitionsClient.DeleteFieldDefinitionAsync(new DeleteFieldDefinitionParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = createdId,
                    }).ConfigureAwait(false);
                }
            }
        }
    }
}
