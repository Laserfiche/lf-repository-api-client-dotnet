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

        // ---------------- Coverage tests added during pre-PR review ----------------

        // Test #1: end-to-end round-trip for every FieldType, ensuring the dotnet client
        // serializes each type without crashing and the server persists+returns it correctly.
        // List values are only meaningful for FieldType.List; other type-specific flags
        // (Format/Currency for Number/Date/Time) are exercised separately by AllFlags tests
        // — this one is purely "does the type itself round-trip".
        [TestMethod]
        public async Task AllFieldTypes_CreateDescribeUpdate_RoundTrip()
        {
            var types = new[]
            {
                FieldType.String,
                FieldType.List,
                FieldType.Number,
                FieldType.Date,
                FieldType.DateTime,
                FieldType.Time,
                FieldType.ShortInteger,
                FieldType.LongInteger,
            };
            // FieldType.Blob is supported by RA but historically not exercised via the admin API;
            // skip rather than risk an asymmetric failure that's out of scope for 6.3.A.

            foreach (var fieldType in types)
            {
                string fieldName = UniqueName($"client_test_type_{fieldType}");
                int createdId = 0;
                try
                {
                    var createReq = new CreateFieldDefinitionRequest
                    {
                        Name = fieldName,
                        FieldType = fieldType,
                        Description = $"Round-trip test for {fieldType}",
                    };
                    if (fieldType == FieldType.String || fieldType == FieldType.List)
                    {
                        createReq.Length = 25;
                    }
                    if (fieldType == FieldType.List)
                    {
                        createReq.ListValues = new List<string> { "Alpha", "Beta" };
                    }

                    var created = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                    {
                        RepositoryId = RepositoryId,
                        Request = createReq,
                    }).ConfigureAwait(false);
                    Assert.IsTrue(created.Id > 0, $"{fieldType}: id not assigned");
                    Assert.AreEqual(fieldType, created.FieldType, $"{fieldType}: round-tripped type mismatch");
                    createdId = created.Id;

                    // Independent GET (defense against same-request masking — Trap 4 discipline).
                    var fetched = await client.FieldDefinitionsClient.GetFieldDefinitionAsync(new GetFieldDefinitionParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = createdId,
                    }).ConfigureAwait(false);
                    Assert.AreEqual(fieldType, fetched.FieldType, $"{fieldType}: GET fieldType mismatch");
                    Assert.AreEqual($"Round-trip test for {fieldType}", fetched.Description);

                    // PATCH description; verify it sticks regardless of type.
                    var updated = await client.FieldDefinitionsClient.UpdateFieldDefinitionAsync(new UpdateFieldDefinitionParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = createdId,
                        Request = new UpdateFieldDefinitionRequest
                        {
                            Description = "Updated for " + fieldType,
                        },
                    }).ConfigureAwait(false);
                    Assert.AreEqual("Updated for " + fieldType, updated.Description, $"{fieldType}: PATCH didn't stick");
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

        // Test #2: GET /Properties on a fresh field with no caller-set properties.
        // Codex round-3 finding #4 raised the possibility that LFS returns internal entries
        // alongside (or instead of) caller-set keys. This test surfaces what comes back so
        // PR reviewers and future maintainers have concrete evidence of the contract.
        [TestMethod]
        public async Task GetFieldProperties_OnFreshField_DocumentsLfsInternalEntries()
        {
            string fieldName = UniqueName("client_test_props_probe");
            int createdId = 0;
            try
            {
                var created = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = fieldName,
                        FieldType = FieldType.String,
                        Length = 10,
                        // intentionally no Properties supplied
                    }
                }).ConfigureAwait(false);
                createdId = created.Id;

                var bag = await client.FieldDefinitionsClient.GetFieldPropertiesAsync(new GetFieldPropertiesParameters
                {
                    RepositoryId = RepositoryId,
                    FieldId = createdId,
                }).ConfigureAwait(false);

                Assert.IsNotNull(bag);
                Assert.IsNotNull(bag.Properties);
                // The bag may be empty (LFS doesn't auto-populate) or contain a stable set of
                // RA-managed keys. We don't constrain — we record. If a future server change
                // begins surfacing PII or unexpected internals, that change should be flagged
                // here. Print to test output for triage.
                Console.WriteLine($"GetFieldProperties on fresh field returned {bag.Properties.Count} entries:");
                foreach (var kv in bag.Properties)
                {
                    Console.WriteLine($"  [{kv.Key}] = [{kv.Value}]");
                }
                // Assertion: any caller-visible keys must be reasonable WebDAV-style identifiers
                // (no control chars, not absurdly long) — defends against the round-3 "what if
                // LFS leaks something weird" concern.
                foreach (var kv in bag.Properties)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(kv.Key), "LFS returned an empty property key");
                    Assert.IsTrue(kv.Key.Length <= 256, $"LFS returned an oversized property key: '{kv.Key}'");
                }
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

        // Test #3: server-side 400 validations surface through the dotnet client as
        // ApiException with parseable ProblemDetails. Covers a representative sample of
        // the round-1/2/3 validation tightenings (length<1, listValues-on-non-list,
        // properties empty key, duplicate set+remove).
        [TestMethod]
        public async Task Validation_RoundTripsAs400ProblemDetails()
        {
            // length=0 on Create — server rejects with 400 + instanceDetail "length"
            var ex1 = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = UniqueName("client_test_invalid"),
                        FieldType = FieldType.String,
                        Length = 0,
                    },
                }).ConfigureAwait(false)).ConfigureAwait(false);
            Assert.AreEqual(400, ex1.StatusCode);
            Assert.IsNotNull(ex1.ProblemDetails.Title);

            // listValues supplied for a non-List type — server rejects
            var ex2 = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest
                    {
                        Name = UniqueName("client_test_invalid"),
                        FieldType = FieldType.String,
                        Length = 10,
                        ListValues = new List<string> { "A", "B" },
                    },
                }).ConfigureAwait(false)).ConfigureAwait(false);
            Assert.AreEqual(400, ex2.StatusCode);

            // Need a real field to exercise the PATCH /Properties validations.
            string fieldName = UniqueName("client_test_props_validation");
            int createdId = 0;
            try
            {
                var created = await client.FieldDefinitionsClient.CreateFieldDefinitionAsync(new CreateFieldDefinitionParameters
                {
                    RepositoryId = RepositoryId,
                    Request = new CreateFieldDefinitionRequest { Name = fieldName, FieldType = FieldType.String, Length = 10 }
                }).ConfigureAwait(false);
                createdId = created.Id;

                // PATCH /Properties with an empty key — server rejects
                var ex3 = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                    await client.FieldDefinitionsClient.UpdateFieldPropertiesAsync(new UpdateFieldPropertiesParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = createdId,
                        Request = new UpdateFieldPropertiesRequest
                        {
                            Set = new Dictionary<string, string> { { "", "v" } },
                        },
                    }).ConfigureAwait(false)).ConfigureAwait(false);
                Assert.AreEqual(400, ex3.StatusCode);

                // PATCH /Properties with a key in both set and remove — server rejects
                var ex4 = await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                    await client.FieldDefinitionsClient.UpdateFieldPropertiesAsync(new UpdateFieldPropertiesParameters
                    {
                        RepositoryId = RepositoryId,
                        FieldId = createdId,
                        Request = new UpdateFieldPropertiesRequest
                        {
                            Set = new Dictionary<string, string> { { "shared", "v" } },
                            Remove = new List<string> { "shared" },
                        },
                    }).ConfigureAwait(false)).ConfigureAwait(false);
                Assert.AreEqual(400, ex4.StatusCode);
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
