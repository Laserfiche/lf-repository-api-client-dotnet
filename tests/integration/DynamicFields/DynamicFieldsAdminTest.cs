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
    /// external-table reads (RA-direct) and template form-logic rules (RWS-reuse).
    /// Self-sufficient: reads the external-table fixture and creates its own throwaway template /
    /// field to bind a form-logic rule, cleaning up afterward. Skips (Inconclusive) when the fixture
    /// is not registered on the target account.
    ///
    /// External tables are READ-ONLY here: on cloud the LFS hard-denies register/update/unregister
    /// (LFCR_E_ACCESS_DENIED / 9013), so external tables are provisioned out-of-band via Process
    /// Automation "data management" and surfaced read-only. The write client methods are gated out
    /// of the cloud build (EXTERNAL_TABLE_WRITE) and are therefore absent from this client.
    ///
    /// Fixture provisioning: import a CSV (columns City, State, Company, Fname, Lname, Email — e.g.
    /// RepositoryAccess src/SharedTest/TestFiles/data.csv) into the account's PA "data management"
    /// as a lookup table named <c>APIServer_DynamicFields_Integration_Tests</c>. It then surfaces
    /// through ListExternalTables automatically.
    /// </summary>
    [TestClass]
    [SkipIfEndpointMissing("ListExternalTables", "GetExternalTable", "ListExternalTableColumns", "GetTemplateFormLogicRules", "SetTemplateFormLogicRules")]
    public class DynamicFieldsAdminTest : BaseTest
    {
        // Lookup table provisioned on the dev account's PA "data management" for these tests
        // (City, State, Company, Fname, Lname, Email — same shape as RA TestFiles/data.csv).
        private const string ExternalTableFixtureName = "APIServer_DynamicFields_Integration_Tests";

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
        public async Task ExternalTable_ListGetColumns_ReadOnly()
        {
            var existing = await FindExistingExternalTableAsync();
            if (existing == null)
            {
                Assert.Inconclusive($"No external table registered on the target account. Provision the shared '{ExternalTableFixtureName}' lookup table (import a CSV into PA 'data management') to exercise external-table reads.");
                return;
            }

            // List surfaces the fixture with the expected lfe schema.
            Assert.IsTrue(existing.Id > 0);
            Assert.AreEqual("lfe", existing.LaserficheSchema);

            // Get by id round-trips the same registration.
            var got = await client.DynamicFieldsClient.GetExternalTableAsync(new GetExternalTableParameters { RepositoryId = RepositoryId, ExternalTableId = existing.Id }).ConfigureAwait(false);
            Assert.AreEqual(existing.Id, got.Id);
            Assert.AreEqual(existing.LaserficheName, got.LaserficheName);

            // Columns hit the backing external data source and return at least one column.
            var columns = await client.DynamicFieldsClient.ListExternalTableColumnsAsync(new ListExternalTableColumnsParameters { RepositoryId = RepositoryId, ExternalTableId = existing.Id }).ConfigureAwait(false);
            Assert.IsNotNull(columns);
            Assert.IsTrue(columns.Count > 0, "Expected the external table to expose at least one column.");
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
