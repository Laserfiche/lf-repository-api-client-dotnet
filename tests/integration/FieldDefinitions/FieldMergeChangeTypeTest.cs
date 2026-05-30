// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.FieldDefinitions
{
    /// <summary>
    /// Integration tests for the field destructive-operation endpoints introduced by PRD 6.3.B:
    /// POST /FieldDefinitions/Merge and POST /FieldDefinitions/{fieldId}/ChangeType.
    /// Exercises both happy paths and the allowDataLoss gating.
    /// </summary>
    [TestClass]
    public class FieldMergeChangeTypeTest : BaseTest
    {
        [TestInitialize]
        public void Initialize()
        {
            client = CreateClient();
        }

        private static string UniqueName(string prefix) =>
            $"{prefix}_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}_{Guid.NewGuid().ToString("N").Substring(0, 6)}";

        private async Task TryDeleteField(int fieldId)
        {
            if (fieldId <= 0) return;
            try
            {
                await client.FieldDefinitionsClient.DeleteFieldDefinitionAsync(new DeleteFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = fieldId,
                }).ConfigureAwait(false);
            }
            catch (ApiException ex) when (ex.StatusCode == 404)
            {
                // A parallel test or a manual repo edit could have removed the field already.
            }
        }

        // Reads a field directly to assert that MergeFields preserved it. Any failure (including 404)
        // surfaces as a test failure — MergeFields does not consume sources, so the GET must succeed.
        private async Task AssertFieldExists(int fieldId, string expectedName)
        {
            var fetched = await client.FieldDefinitionsClient.GetFieldDefinitionAsync(new GetFieldDefinitionParameters
            {
                RepositoryId = RepositoryId,
                FieldId = fieldId,
            }).ConfigureAwait(false);
            Assert.AreEqual(expectedName, fetched.Name);
        }

        private async Task<FieldDefinition> CreateStringField(string namePrefix, int length = 25)
        {
            return await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
            {
                RepositoryId = RepositoryId,
                Request = new CreateFieldDefinitionRequest
                {
                    Name = UniqueName(namePrefix),
                    FieldType = FieldType.String,
                    Length = length,
                }
            }).ConfigureAwait(false);
        }

        // ---------------- MergeFields ----------------

        [TestMethod]
        public async Task MergeFields_TwoStringFields_FailStrategy_HappyPath()
        {
            FieldDefinition src1 = null, src2 = null;
            int mergedId = 0;
            try
            {
                src1 = await CreateStringField("client_test_merge_src1");
                src2 = await CreateStringField("client_test_merge_src2");

                string newName = UniqueName("client_test_merge_dest");
                var merged = await client.FieldDefinitionsClient.MergeFieldsAsync(new MergeFieldsParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new MergeFieldsRequest
                    {
                        SourceFieldIds = new List<int> { src1.Id, src2.Id },
                        NewFieldName = newName,
                        OnConflict = FieldMergeConflictStrategy.Fail,
                    },
                }).ConfigureAwait(false);

                Assert.IsNotNull(merged);
                Assert.IsTrue(merged.Id > 0);
                Assert.AreEqual(newName, merged.Name);
                mergedId = merged.Id;

                // Independent GET — confirm the new field is discoverable post-merge (defense against same-request masking).
                var fetched = await client.FieldDefinitionsClient.GetFieldDefinitionAsync(new GetFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = mergedId,
                }).ConfigureAwait(false);
                Assert.AreEqual(newName, fetched.Name);

                // Source field definitions are preserved by MergeFields — the operation creates a new field,
                // it does not delete the originals. Confirms the documented lifecycle contract.
                await AssertFieldExists(src1.Id, src1.Name);
                await AssertFieldExists(src2.Id, src2.Name);
            }
            finally
            {
                await TryDeleteField(mergedId);
                if (src1 != null) await TryDeleteField(src1.Id);
                if (src2 != null) await TryDeleteField(src2.Id);
            }
        }

        [TestMethod]
        public async Task MergeFields_MakeMultivalueStrategy_HappyPath()
        {
            FieldDefinition src1 = null, src2 = null;
            int mergedId = 0;
            try
            {
                src1 = await CreateStringField("client_test_merge_mv_src1");
                src2 = await CreateStringField("client_test_merge_mv_src2");

                var merged = await client.FieldDefinitionsClient.MergeFieldsAsync(new MergeFieldsParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new MergeFieldsRequest
                    {
                        SourceFieldIds = new List<int> { src1.Id, src2.Id },
                        NewFieldName = UniqueName("client_test_merge_mv_dest"),
                        OnConflict = FieldMergeConflictStrategy.MakeMultivalue,
                    },
                }).ConfigureAwait(false);

                Assert.IsNotNull(merged);
                Assert.IsTrue(merged.Id > 0);
                mergedId = merged.Id;

                await AssertFieldExists(src1.Id, src1.Name);
                await AssertFieldExists(src2.Id, src2.Name);
            }
            finally
            {
                await TryDeleteField(mergedId);
                if (src1 != null) await TryDeleteField(src1.Id);
                if (src2 != null) await TryDeleteField(src2.Id);
            }
        }

        [TestMethod]
        public async Task MergeFields_OneSource_Throws400()
        {
            FieldDefinition src1 = null;
            try
            {
                src1 = await CreateStringField("client_test_merge_lone_src");

                var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                    await client.FieldDefinitionsClient.MergeFieldsAsync(new MergeFieldsParameters
                    {
                        RepositoryId = RepositoryId,
                        Request = new MergeFieldsRequest
                        {
                            SourceFieldIds = new List<int> { src1.Id },
                            NewFieldName = UniqueName("client_test_merge_lone_dest"),
                            OnConflict = FieldMergeConflictStrategy.Fail,
                        },
                    }).ConfigureAwait(false)).ConfigureAwait(false);

                Assert.AreEqual(400, ex.StatusCode);
            }
            finally
            {
                if (src1 != null) await TryDeleteField(src1.Id);
            }
        }

        [TestMethod]
        public async Task MergeFields_DuplicateSourceIds_Throws400()
        {
            FieldDefinition src1 = null;
            try
            {
                src1 = await CreateStringField("client_test_merge_dup_src");

                var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                    await client.FieldDefinitionsClient.MergeFieldsAsync(new MergeFieldsParameters
                    {
                        RepositoryId = RepositoryId,
                        Request = new MergeFieldsRequest
                        {
                            SourceFieldIds = new List<int> { src1.Id, src1.Id },
                            NewFieldName = UniqueName("client_test_merge_dup_dest"),
                            OnConflict = FieldMergeConflictStrategy.Fail,
                        },
                    }).ConfigureAwait(false)).ConfigureAwait(false);

                Assert.AreEqual(400, ex.StatusCode);
            }
            finally
            {
                if (src1 != null) await TryDeleteField(src1.Id);
            }
        }

        [TestMethod]
        public async Task MergeFields_UseFirstFieldWithoutAllowDataLoss_Throws400()
        {
            FieldDefinition src1 = null, src2 = null;
            try
            {
                src1 = await CreateStringField("client_test_merge_uff_src1");
                src2 = await CreateStringField("client_test_merge_uff_src2");

                var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                    await client.FieldDefinitionsClient.MergeFieldsAsync(new MergeFieldsParameters
                    {
                        RepositoryId = RepositoryId,
                        Request = new MergeFieldsRequest
                        {
                            SourceFieldIds = new List<int> { src1.Id, src2.Id },
                            NewFieldName = UniqueName("client_test_merge_uff_dest"),
                            OnConflict = FieldMergeConflictStrategy.UseFirstField,
                            AllowDataLoss = false,
                        },
                    }).ConfigureAwait(false)).ConfigureAwait(false);

                Assert.AreEqual(400, ex.StatusCode);
            }
            finally
            {
                if (src1 != null) await TryDeleteField(src1.Id);
                if (src2 != null) await TryDeleteField(src2.Id);
            }
        }

        [TestMethod]
        public async Task MergeFields_UseFirstFieldWithAllowDataLoss_Succeeds()
        {
            FieldDefinition src1 = null, src2 = null;
            int mergedId = 0;
            try
            {
                src1 = await CreateStringField("client_test_merge_uffok_src1");
                src2 = await CreateStringField("client_test_merge_uffok_src2");

                var merged = await client.FieldDefinitionsClient.MergeFieldsAsync(new MergeFieldsParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new MergeFieldsRequest
                    {
                        SourceFieldIds = new List<int> { src1.Id, src2.Id },
                        NewFieldName = UniqueName("client_test_merge_uffok_dest"),
                        OnConflict = FieldMergeConflictStrategy.UseFirstField,
                        AllowDataLoss = true,
                    },
                }).ConfigureAwait(false);

                Assert.IsNotNull(merged);
                Assert.IsTrue(merged.Id > 0);
                mergedId = merged.Id;

                await AssertFieldExists(src1.Id, src1.Name);
                await AssertFieldExists(src2.Id, src2.Name);
            }
            finally
            {
                await TryDeleteField(mergedId);
                if (src1 != null) await TryDeleteField(src1.Id);
                if (src2 != null) await TryDeleteField(src2.Id);
            }
        }

        [TestMethod]
        public async Task MergeFields_RemoveFromTemplates_PassThroughSucceeds()
        {
            // V2 has no template CRUD, so the side effect on templates can't be observed end-to-end here.
            // This test exercises the option pass-through: the request deserializes, the controller forwards
            // removeFromTemplates=true to the connector, and the RA call (with no template references to clean
            // up) returns success. Behavioral effect is covered by RA's MergeFieldsNoRemoveFromTemplate /
            // MergeBasicWithMultivalue tests in RepositoryAccess.
            FieldDefinition src1 = null, src2 = null;
            int mergedId = 0;
            try
            {
                src1 = await CreateStringField("client_test_merge_rft_src1");
                src2 = await CreateStringField("client_test_merge_rft_src2");

                var merged = await client.FieldDefinitionsClient.MergeFieldsAsync(new MergeFieldsParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new MergeFieldsRequest
                    {
                        SourceFieldIds = new List<int> { src1.Id, src2.Id },
                        NewFieldName = UniqueName("client_test_merge_rft_dest"),
                        OnConflict = FieldMergeConflictStrategy.Fail,
                        RemoveFromTemplates = true,
                    },
                }).ConfigureAwait(false);

                Assert.IsTrue(merged.Id > 0);
                mergedId = merged.Id;

                // The option doesn't affect source-field lifecycle — sources remain even when
                // removeFromTemplates=true (which only affects template references).
                await AssertFieldExists(src1.Id, src1.Name);
                await AssertFieldExists(src2.Id, src2.Name);
            }
            finally
            {
                await TryDeleteField(mergedId);
                if (src1 != null) await TryDeleteField(src1.Id);
                if (src2 != null) await TryDeleteField(src2.Id);
            }
        }

        [TestMethod]
        public async Task MergeFields_AutoRename_PassThroughSucceeds()
        {
            // Pass-through coverage for the autoRename option: confirms the parameter deserializes and
            // is forwarded through controller → connector → RA without faulting. The visible effect on
            // collision is repository-version-dependent (LFS's merge endpoint does not always honor
            // X-LF-Autorename the way Field.Create does), so this test exercises only the no-collision
            // happy path. Tests that depend on the rename behavior live with RepositoryAccess.
            FieldDefinition src1 = null, src2 = null;
            int mergedId = 0;
            try
            {
                src1 = await CreateStringField("client_test_merge_autorename_src1");
                src2 = await CreateStringField("client_test_merge_autorename_src2");

                string newName = UniqueName("client_test_merge_autorename_dest");
                var merged = await client.FieldDefinitionsClient.MergeFieldsAsync(new MergeFieldsParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new MergeFieldsRequest
                    {
                        SourceFieldIds = new List<int> { src1.Id, src2.Id },
                        NewFieldName = newName,
                        OnConflict = FieldMergeConflictStrategy.Fail,
                        AutoRename = true,
                    },
                }).ConfigureAwait(false);

                Assert.IsTrue(merged.Id > 0);
                mergedId = merged.Id;
                // Without a collision, AutoRename has nothing to do — the requested name is honored.
                Assert.AreEqual(newName, merged.Name);
            }
            finally
            {
                await TryDeleteField(mergedId);
                if (src1 != null) await TryDeleteField(src1.Id);
                if (src2 != null) await TryDeleteField(src2.Id);
            }
        }

        // ---------------- ChangeFieldType ----------------

        [TestMethod]
        public async Task ChangeFieldType_LosslessStringToList_HappyPath()
        {
            // Brand-new String field with no constraint/default/entries → safe to widen the type domain.
            FieldDefinition fld = null;
            try
            {
                fld = await CreateStringField("client_test_change_lossless");

                var changed = await client.FieldDefinitionsClient.ChangeFieldTypeAsync(new ChangeFieldTypeParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = fld.Id,
                    Request = new ChangeFieldTypeRequest
                    {
                        NewFieldType = FieldType.List,
                        AllowDataLoss = false,
                    },
                }).ConfigureAwait(false);

                Assert.IsNotNull(changed);
                Assert.AreEqual(FieldType.List, changed.FieldType);

                // Independent GET — confirms the ChangeType+Save reached storage (defense against missed Save).
                var fetched = await client.FieldDefinitionsClient.GetFieldDefinitionAsync(new GetFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = fld.Id,
                }).ConfigureAwait(false);
                Assert.AreEqual(FieldType.List, fetched.FieldType);
            }
            finally
            {
                if (fld != null) await TryDeleteField(fld.Id);
            }
        }

        [TestMethod]
        public async Task ChangeFieldType_SafeWidening_ShortToLong_Succeeds()
        {
            // ShortInteger→LongInteger is a same-family widening — all existing values fit, so it's lossless
            // even with assigned entries. This test runs without assigned entries; the predicate path proven
            // out is "no constraint, no default, no entries" — but proving the API accepts the widening here
            // also documents the supported transition for callers.
            FieldDefinition fld = null;
            try
            {
                fld = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = UniqueName("client_test_change_widen"),
                        FieldType = FieldType.ShortInteger,
                    }
                }).ConfigureAwait(false);

                var changed = await client.FieldDefinitionsClient.ChangeFieldTypeAsync(new ChangeFieldTypeParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = fld.Id,
                    Request = new ChangeFieldTypeRequest
                    {
                        NewFieldType = FieldType.LongInteger,
                        AllowDataLoss = false,
                    },
                }).ConfigureAwait(false);

                Assert.AreEqual(FieldType.LongInteger, changed.FieldType);
            }
            finally
            {
                if (fld != null) await TryDeleteField(fld.Id);
            }
        }

        [TestMethod]
        public async Task ChangeFieldType_LeavingListWithoutFlag_Throws400()
        {
            // Leaving the List type clears list items unconditionally → always lossy.
            FieldDefinition fld = null;
            try
            {
                fld = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = UniqueName("client_test_change_leave_list"),
                        FieldType = FieldType.List,
                        Length = 25,
                        ListValues = new List<string> { "Red", "Blue" },
                    }
                }).ConfigureAwait(false);

                var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                    await client.FieldDefinitionsClient.ChangeFieldTypeAsync(new ChangeFieldTypeParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = fld.Id,
                        Request = new ChangeFieldTypeRequest
                        {
                            NewFieldType = FieldType.String,
                            AllowDataLoss = false,
                        },
                    }).ConfigureAwait(false)).ConfigureAwait(false);

                Assert.AreEqual(400, ex.StatusCode);
            }
            finally
            {
                if (fld != null) await TryDeleteField(fld.Id);
            }
        }

        [TestMethod]
        public async Task ChangeFieldType_LeavingListWithFlag_Succeeds()
        {
            FieldDefinition fld = null;
            try
            {
                fld = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = UniqueName("client_test_change_leave_list_ok"),
                        FieldType = FieldType.List,
                        Length = 25,
                        ListValues = new List<string> { "Red", "Blue" },
                    }
                }).ConfigureAwait(false);

                var changed = await client.FieldDefinitionsClient.ChangeFieldTypeAsync(new ChangeFieldTypeParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = fld.Id,
                    Request = new ChangeFieldTypeRequest
                    {
                        NewFieldType = FieldType.String,
                        AllowDataLoss = true,
                    },
                }).ConfigureAwait(false);

                Assert.AreEqual(FieldType.String, changed.FieldType);

                // Independent GET — confirm the type change persisted to storage.
                var fetched = await client.FieldDefinitionsClient.GetFieldDefinitionAsync(new GetFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = fld.Id,
                }).ConfigureAwait(false);
                Assert.AreEqual(FieldType.String, fetched.FieldType);
            }
            finally
            {
                if (fld != null) await TryDeleteField(fld.Id);
            }
        }

        [TestMethod]
        public async Task ChangeFieldType_ConstraintCrossFamilyWithoutFlag_Throws400()
        {
            // String field with a regex constraint converted to Number: the constraint can't survive a
            // cross-family change → IsLossyTypeConversion flags it; server returns 400.
            FieldDefinition fld = null;
            try
            {
                fld = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = UniqueName("client_test_change_constraint"),
                        FieldType = FieldType.String,
                        Length = 25,
                        Constraint = "^[A-Z]+$",
                    }
                }).ConfigureAwait(false);

                var ex = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                    await client.FieldDefinitionsClient.ChangeFieldTypeAsync(new ChangeFieldTypeParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = fld.Id,
                        Request = new ChangeFieldTypeRequest
                        {
                            NewFieldType = FieldType.Number,
                            AllowDataLoss = false,
                        },
                    }).ConfigureAwait(false)).ConfigureAwait(false);

                Assert.AreEqual(400, ex.StatusCode);
            }
            finally
            {
                if (fld != null) await TryDeleteField(fld.Id);
            }
        }
    }
}
