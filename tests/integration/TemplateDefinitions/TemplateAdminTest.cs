// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.TemplateDefinitions
{
    /// <summary>
    /// Integration tests for the template-definition admin endpoints introduced by PRD 6.3.C:
    /// Create / Update / Delete + GetAssignedEntryCount + Get/UpdateProperties +
    /// AddField / UpdateFieldProperties / RemoveField / MoveField.
    /// </summary>
    [TestClass]
    public class TemplateAdminTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        private static string UniqueName(string prefix) =>
            $"{prefix}_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}_{Guid.NewGuid().ToString("N").Substring(0, 6)}";

        // The admin tests reuse existing repository field definitions rather than creating
        // their own. 6.3.C is independent of 6.3.A so CreateFieldDefinition isn't on the
        // base swagger; we pick from what dev-CA already has.
        private async Task<string[]> PickExistingFieldNamesAsync(int count)
        {
            var fields = await client.FieldDefinitionsClient.ListFieldDefinitionsAsync(new ListFieldDefinitionsParameters
            {
                RepositoryId = RepositoryId,
            }).ConfigureAwait(false);
            var names = fields.Value?
                .Where(f => !string.IsNullOrEmpty(f.Name))
                .Select(f => f.Name)
                .Take(count)
                .ToArray() ?? Array.Empty<string>();
            Assert.IsTrue(names.Length >= count, $"Need at least {count} existing field definition(s) in dev-CA repository; found {names.Length}.");
            return names;
        }

        private async Task SafeDeleteTemplateAsync(int templateId)
        {
            if (templateId <= 0) return;
            try
            {
                await client.TemplateDefinitionsClient.DeleteTemplateAsync(new DeleteTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = templateId,
                }).ConfigureAwait(false);
            }
            catch
            {
                // Swallow cleanup failures so an earlier assertion isn't masked.
            }
        }

        [TestMethod]
        public async Task CreateUpdateDelete_Template_Lifecycle()
        {
            string templateName = UniqueName("client_test_tmpl");
            int createdId = 0;
            try
            {
                // Create
                var created = await client.TemplateDefinitionsClient.CreateTemplateAsync(new CreateTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateTemplateRequest
                    {
                        Name = templateName,
                        Description = "Created from .NET client integration test",
                        IsAutoAssignable = false,
                        Color = new LFColor { A = 255, R = 100, G = 50, B = 25 },
                    }
                }).ConfigureAwait(false);
                Assert.IsNotNull(created);
                Assert.IsTrue(created.Id > 0);
                Assert.AreEqual(templateName, created.Name);
                Assert.AreEqual("Created from .NET client integration test", created.Description);
                Assert.IsFalse(created.IsAutoAssignable);
                Assert.IsNotNull(created.Color);
                Assert.AreEqual((byte)100, created.Color.R);
                createdId = created.Id;

                // Update — rename + flip IsAutoAssignable
                string renamed = UniqueName("client_test_tmpl_renamed");
                var updated = await client.TemplateDefinitionsClient.UpdateTemplateAsync(new UpdateTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                    Request = new UpdateTemplateRequest
                    {
                        Name = renamed,
                        IsAutoAssignable = true,
                    }
                }).ConfigureAwait(false);
                Assert.AreEqual(renamed, updated.Name);
                Assert.IsTrue(updated.IsAutoAssignable);

                // Independent GET to confirm persistence
                var readBack = await client.TemplateDefinitionsClient.GetTemplateDefinitionAsync(new GetTemplateDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                Assert.AreEqual(renamed, readBack.Name);
                Assert.IsTrue(readBack.IsAutoAssignable);

                // Delete
                int deletedId = createdId;
                await client.TemplateDefinitionsClient.DeleteTemplateAsync(new DeleteTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                createdId = 0;

                // 404 after delete — verify the just-deleted template is actually gone.
                var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                {
                    await client.TemplateDefinitionsClient.GetTemplateDefinitionAsync(new GetTemplateDefinitionParameters
                    {
                        RepositoryId = RepositoryId,
                        TemplateId = deletedId,
                    }).ConfigureAwait(false);
                });
                Assert.AreEqual(404, ex.StatusCode, $"Expected 404 from GET on deleted template id={deletedId}, got {ex.StatusCode}");
            }
            finally
            {
                await SafeDeleteTemplateAsync(createdId);
            }
        }

        [TestMethod]
        public async Task UpdateTemplate_ClearColor_SetsColorToNull()
        {
            string templateName = UniqueName("client_test_tmpl");
            int createdId = 0;
            try
            {
                var created = await client.TemplateDefinitionsClient.CreateTemplateAsync(new CreateTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateTemplateRequest
                    {
                        Name = templateName,
                        Color = new LFColor { A = 255, R = 30, G = 60, B = 90 },
                    }
                }).ConfigureAwait(false);
                createdId = created.Id;
                Assert.IsNotNull(created.Color);

                await client.TemplateDefinitionsClient.UpdateTemplateAsync(new UpdateTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                    Request = new UpdateTemplateRequest { ClearColor = true }
                }).ConfigureAwait(false);

                var readBack = await client.TemplateDefinitionsClient.GetTemplateDefinitionAsync(new GetTemplateDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                Assert.IsNull(readBack.Color, "Color should be cleared after ClearColor=true");
            }
            finally
            {
                await SafeDeleteTemplateAsync(createdId);
            }
        }

        [TestMethod]
        public async Task AddRemoveMoveField_Lifecycle()
        {
            var fieldNames = await PickExistingFieldNamesAsync(3);
            string templateName = UniqueName("client_test_tmpl");
            int createdId = 0;
            try
            {
                var created = await client.TemplateDefinitionsClient.CreateTemplateAsync(new CreateTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateTemplateRequest { Name = templateName }
                }).ConfigureAwait(false);
                createdId = created.Id;
                Assert.AreEqual(0, created.FieldCount);

                // Add three fields
                foreach (var name in fieldNames)
                {
                    await client.TemplateDefinitionsClient.AddTemplateFieldAsync(new AddTemplateFieldParameters
                    {
                        RepositoryId = RepositoryId,
                        TemplateId = createdId,
                        Request = new AddTemplateFieldRequest { FieldName = name }
                    }).ConfigureAwait(false);
                }

                // Read back: 3 fields, in addition order
                var fieldsAfterAdd = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(new ListTemplateFieldDefinitionsByTemplateIdParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                Assert.IsNotNull(fieldsAfterAdd.Value);
                Assert.AreEqual(3, fieldsAfterAdd.Value.Count);
                CollectionAssert.AreEqual(fieldNames, fieldsAfterAdd.Value.Select(f => f.Name).ToArray());

                // Move first field to position 3
                await client.TemplateDefinitionsClient.MoveTemplateFieldAsync(new MoveTemplateFieldParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                    Request = new MoveTemplateFieldRequest { FieldName = fieldNames[0], NewPosition = 3 }
                }).ConfigureAwait(false);

                var fieldsAfterMove = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(new ListTemplateFieldDefinitionsByTemplateIdParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                Assert.AreEqual(3, fieldsAfterMove.Value.Count);
                Assert.AreEqual(fieldNames[0], fieldsAfterMove.Value[2].Name, "Moved field should now be at position 3");

                // Remove middle field
                await client.TemplateDefinitionsClient.RemoveTemplateFieldAsync(new RemoveTemplateFieldParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                    FieldName = fieldNames[1],
                }).ConfigureAwait(false);

                var fieldsAfterRemove = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(new ListTemplateFieldDefinitionsByTemplateIdParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                Assert.AreEqual(2, fieldsAfterRemove.Value.Count);
                Assert.IsFalse(fieldsAfterRemove.Value.Any(f => f.Name == fieldNames[1]));
            }
            finally
            {
                await SafeDeleteTemplateAsync(createdId);
            }
        }

        [TestMethod]
        public async Task UpdateTemplateFieldProperties_IsRequired_RoundTrip()
        {
            // Note: LocalDescription is intentionally not exercised here. The
            // underlying RA `SetFieldLocalDescription` requires LFS ≥ 12.0.2,
            // which dev-CA runs older than at the time this test was authored.
            // The contract still covers it on the API surface (controller test +
            // server-side unit tests verify the LocalDescription branch).
            var fieldNames = await PickExistingFieldNamesAsync(1);
            string templateName = UniqueName("client_test_tmpl");
            int createdId = 0;
            try
            {
                var created = await client.TemplateDefinitionsClient.CreateTemplateAsync(new CreateTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateTemplateRequest
                    {
                        Name = templateName,
                        Fields = new List<TemplateFieldAssignment>
                        {
                            new TemplateFieldAssignment { FieldName = fieldNames[0], IsRequired = false }
                        }
                    }
                }).ConfigureAwait(false);
                createdId = created.Id;

                var before = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(new ListTemplateFieldDefinitionsByTemplateIdParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                var beforeField = before.Value.Single();
                Assert.IsFalse(beforeField.IsRequired);

                await client.TemplateDefinitionsClient.UpdateTemplateFieldPropertiesAsync(new UpdateTemplateFieldPropertiesParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                    FieldName = fieldNames[0],
                    Request = new UpdateTemplateFieldPropertiesRequest { IsRequired = true }
                }).ConfigureAwait(false);

                var after = await client.TemplateDefinitionsClient.ListTemplateFieldDefinitionsByTemplateIdAsync(new ListTemplateFieldDefinitionsByTemplateIdParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                var afterField = after.Value.Single();
                Assert.IsTrue(afterField.IsRequired, "IsRequired should now be true");
            }
            finally
            {
                await SafeDeleteTemplateAsync(createdId);
            }
        }

        [TestMethod]
        public async Task Properties_RoundTrip()
        {
            string templateName = UniqueName("client_test_tmpl");
            int createdId = 0;
            try
            {
                var created = await client.TemplateDefinitionsClient.CreateTemplateAsync(new CreateTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateTemplateRequest { Name = templateName }
                }).ConfigureAwait(false);
                createdId = created.Id;

                // Set initial keys
                var first = await client.TemplateDefinitionsClient.UpdateTemplatePropertiesAsync(new UpdateTemplatePropertiesParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                    Request = new UpdateTemplatePropertiesRequest
                    {
                        Set = new Dictionary<string, string>
                        {
                            { "test.prop.one", "value-one" },
                            { "test.prop.two", "value-two" },
                        }
                    }
                }).ConfigureAwait(false);
                Assert.IsTrue(first.Properties.ContainsKey("test.prop.one"));
                Assert.AreEqual("value-one", first.Properties["test.prop.one"]);
                Assert.AreEqual("value-two", first.Properties["test.prop.two"]);

                // Independent GET
                var readBack = await client.TemplateDefinitionsClient.GetTemplatePropertiesAsync(new GetTemplatePropertiesParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                Assert.AreEqual("value-one", readBack.Properties["test.prop.one"]);

                // Remove one, overwrite the other
                var afterPatch = await client.TemplateDefinitionsClient.UpdateTemplatePropertiesAsync(new UpdateTemplatePropertiesParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                    Request = new UpdateTemplatePropertiesRequest
                    {
                        Set = new Dictionary<string, string> { { "test.prop.two", "value-two-updated" } },
                        Remove = new List<string> { "test.prop.one" },
                    }
                }).ConfigureAwait(false);
                Assert.IsFalse(afterPatch.Properties.ContainsKey("test.prop.one"), "test.prop.one should be removed");
                Assert.AreEqual("value-two-updated", afterPatch.Properties["test.prop.two"]);
            }
            finally
            {
                await SafeDeleteTemplateAsync(createdId);
            }
        }

        [TestMethod]
        public async Task GetTemplateAssignedEntryCount_NewTemplate_ReturnsZero()
        {
            string templateName = UniqueName("client_test_tmpl");
            int createdId = 0;
            try
            {
                var created = await client.TemplateDefinitionsClient.CreateTemplateAsync(new CreateTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateTemplateRequest { Name = templateName }
                }).ConfigureAwait(false);
                createdId = created.Id;

                var count = await client.TemplateDefinitionsClient.GetTemplateAssignedEntryCountAsync(new GetTemplateAssignedEntryCountParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = createdId,
                }).ConfigureAwait(false);
                Assert.IsNotNull(count);
                Assert.AreEqual(0, count.Count, "A fresh template should have zero assigned entries");
            }
            finally
            {
                await SafeDeleteTemplateAsync(createdId);
            }
        }
    }
}
