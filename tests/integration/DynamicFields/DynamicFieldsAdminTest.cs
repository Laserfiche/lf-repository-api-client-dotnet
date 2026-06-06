// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Laserfiche.Repository.Api.Client.IntegrationTest.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.DynamicFields
{
    /// <summary>
    /// Integration tests for the Dynamic Fields admin endpoints introduced by PRD REQ-ADMIN-008:
    /// external-table registration (RA-direct) and template form-logic rules (RWS-reuse).
    /// Self-sufficient: reuses the shared <c>PMT_LoadTest_LT</c> external-table fixture's coordinates
    /// and creates its own throwaway alias / template / field, cleaning up afterward. Skips
    /// (Inconclusive) when that fixture is not registered on the target account.
    ///
    /// Fixture provisioning (Option 1, manual / account-level — same fixture the RA cloud test
    /// <c>TemplateTest.FormLogicParentFieldTest</c> uses): import RepositoryAccess
    /// <c>src/SharedTest/TestFiles/data.csv</c> (columns City, State, Company, Fname, Lname, Email)
    /// into the account's Process Automation "data management" as a lookup table named
    /// <c>PMT_LoadTest_LT</c>. It then surfaces through ListExternalTables automatically.
    /// </summary>
    [TestClass]
    [SkipIfEndpointMissing("ListExternalTables", "GetExternalTable", "ListExternalTableColumns", "RegisterExternalTable", "UpdateExternalTable", "UnregisterExternalTable", "GetTemplateFormLogicRules", "SetTemplateFormLogicRules")]
    public class DynamicFieldsAdminTest : BaseTest
    {
        // Shared cross-suite fixture name (RA TemplateTest.FormLogicParentFieldTest, RWS, API Server).
        private const string ExternalTableFixtureName = "PMT_LoadTest_LT";

        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        private static string UniqueName(string prefix) =>
            $"{prefix}_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}_{Guid.NewGuid().ToString("N").Substring(0, 6)}";

        private async Task<ExternalTable> FindExistingExternalTableAsync()
        {
            var list = await client.DynamicFieldsClient.ListExternalTablesAsync(new ListExternalTablesParameters { RepositoryId = RepositoryId }).ConfigureAwait(false);
            // Prefer the shared PMT_LoadTest_LT fixture; fall back to any registration so the suite
            // still exercises the surface on accounts that registered a differently-named table.
            return list?.FirstOrDefault(t => string.Equals(t.LaserficheName, ExternalTableFixtureName, StringComparison.OrdinalIgnoreCase))
                ?? list?.FirstOrDefault();
        }

        [TestMethod]
        public async Task ExternalTable_RegisterListGetColumnsUpdateUnregister_Lifecycle()
        {
            var existing = await FindExistingExternalTableAsync();
            if (existing == null)
            {
                Assert.Inconclusive($"No external table registered on the target account. Provision the shared '{ExternalTableFixtureName}' lookup table (import RA TestFiles/data.csv into PA 'data management') to exercise external-table admin.");
                return;
            }

            string alias = UniqueName("client_test_exttable");
            int newId = 0;
            try
            {
                // Register a new alias pointing at the same underlying (database, schema, table) as the existing fixture.
                var created = await client.DynamicFieldsClient.RegisterExternalTableAsync(new RegisterExternalTableParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new ExternalTableRequest { LaserficheName = alias, Database = existing.Database, Schema = existing.Schema, Table = existing.Table }
                }).ConfigureAwait(false);
                Assert.IsNotNull(created);
                Assert.IsTrue(created.Id > 0);
                Assert.AreEqual(alias, created.LaserficheName);
                Assert.AreEqual("lfe", created.LaserficheSchema);
                newId = created.Id;

                // List contains the new registration.
                var list = await client.DynamicFieldsClient.ListExternalTablesAsync(new ListExternalTablesParameters { RepositoryId = RepositoryId }).ConfigureAwait(false);
                Assert.IsTrue(list.Any(t => t.Id == newId));

                // Get by id.
                var got = await client.DynamicFieldsClient.GetExternalTableAsync(new GetExternalTableParameters { RepositoryId = RepositoryId, ExternalTableId = newId }).ConfigureAwait(false);
                Assert.AreEqual(alias, got.LaserficheName);
                Assert.AreEqual(existing.Table, got.Table);

                // Columns (hits the external data source).
                var columns = await client.DynamicFieldsClient.ListExternalTableColumnsAsync(new ListExternalTableColumnsParameters { RepositoryId = RepositoryId, ExternalTableId = newId }).ConfigureAwait(false);
                Assert.IsNotNull(columns);
                Assert.IsTrue(columns.Count > 0, "Expected the external table to expose at least one column.");

                // Update (re-PUT the same coordinates) returns the updated registration.
                var updated = await client.DynamicFieldsClient.UpdateExternalTableAsync(new UpdateExternalTableParameters
                {
                    RepositoryId = RepositoryId,
                    ExternalTableId = newId,
                    Request = new ExternalTableRequest { LaserficheName = alias, Database = existing.Database, Schema = existing.Schema, Table = existing.Table }
                }).ConfigureAwait(false);
                Assert.AreEqual(newId, updated.Id);
            }
            finally
            {
                if (newId > 0)
                {
                    try
                    {
                        await client.DynamicFieldsClient.UnregisterExternalTableAsync(new UnregisterExternalTableParameters { RepositoryId = RepositoryId, ExternalTableId = newId }).ConfigureAwait(false);
                    }
                    catch { /* best-effort cleanup */ }
                }
            }
        }

        [TestMethod]
        public async Task FormLogic_SetGetClear_Lifecycle()
        {
            var existing = await FindExistingExternalTableAsync();
            if (existing == null)
            {
                Assert.Inconclusive($"No external table registered on the target account. Provision the shared '{ExternalTableFixtureName}' lookup table (import RA TestFiles/data.csv into PA 'data management') to bind a dynamic field to.");
                return;
            }
            var columns = await client.DynamicFieldsClient.ListExternalTableColumnsAsync(new ListExternalTableColumnsParameters { RepositoryId = RepositoryId, ExternalTableId = existing.Id }).ConfigureAwait(false);
            if (columns == null || columns.Count == 0)
            {
                Assert.Inconclusive("External table fixture exposes no columns to bind.");
                return;
            }
            string boundColumn = columns.First();

            string fieldName = UniqueName("client_test_dynfield");
            string templateName = UniqueName("client_test_dyntmpl");
            int fieldId = 0;
            int templateId = 0;
            try
            {
                var field = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest { Name = fieldName, FieldType = FieldType.String, Length = 100 }
                }).ConfigureAwait(false);
                fieldId = field.Id;

                var template = await client.TemplateDefinitionsClient.CreateTemplateAsync(new CreateTemplateParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateTemplateRequest
                    {
                        Name = templateName,
                        Fields = new List<TemplateFieldAssignment> { new TemplateFieldAssignment { FieldName = fieldName, IsRequired = false } }
                    }
                }).ConfigureAwait(false);
                templateId = template.Id;

                // Set a simple dynamic-field rule binding the field to the external column.
                var setResp = await client.DynamicFieldsClient.SetTemplateFormLogicRulesAsync(new SetTemplateFormLogicRulesParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = templateId,
                    Request = new SetFormLogicRulesRequest
                    {
                        Rules = new List<FormLogicRule>
                        {
                            new FormLogicRule { FieldId = fieldId, BoundTableId = existing.Id, BoundColumn = boundColumn, SortColumn = boundColumn, SortDirection = SortDirection.Ascending, Validate = false }
                        }
                    }
                }).ConfigureAwait(false);
                Assert.AreEqual(1, setResp.Count);
                Assert.AreEqual(fieldId, setResp.First().FieldId);

                // Independent GET asserts the rule persisted.
                var getResp = await client.DynamicFieldsClient.GetTemplateFormLogicRulesAsync(new GetTemplateFormLogicRulesParameters { RepositoryId = RepositoryId, TemplateId = templateId }).ConfigureAwait(false);
                Assert.AreEqual(1, getResp.Count);
                Assert.AreEqual(fieldId, getResp.First().FieldId);
                Assert.AreEqual(boundColumn, getResp.First().BoundColumn);
                Assert.AreEqual(existing.Id, getResp.First().BoundTableId);

                // Full-replace with an empty set clears all dynamic fields on the template.
                var clearResp = await client.DynamicFieldsClient.SetTemplateFormLogicRulesAsync(new SetTemplateFormLogicRulesParameters
                {
                    RepositoryId = RepositoryId,
                    TemplateId = templateId,
                    Request = new SetFormLogicRulesRequest { Rules = new List<FormLogicRule>() }
                }).ConfigureAwait(false);
                Assert.AreEqual(0, clearResp.Count);

                var afterClear = await client.DynamicFieldsClient.GetTemplateFormLogicRulesAsync(new GetTemplateFormLogicRulesParameters { RepositoryId = RepositoryId, TemplateId = templateId }).ConfigureAwait(false);
                Assert.AreEqual(0, afterClear.Count);
            }
            finally
            {
                if (templateId > 0)
                {
                    try { await client.TemplateDefinitionsClient.DeleteTemplateAsync(new DeleteTemplateParameters { RepositoryId = RepositoryId, TemplateId = templateId }).ConfigureAwait(false); } catch { }
                }
                if (fieldId > 0)
                {
                    try { await client.FieldDefinitionsClient.DeleteFieldDefinitionAsync(new DeleteFieldDefinitionParameters { RepositoryId = RepositoryId, FieldId = fieldId }).ConfigureAwait(false); } catch { }
                }
            }
        }
    }
}
