# Changelog

## 2.2.0

### Features

- Add field definition administration methods: `CreateFieldDefinitionAsync`, `UpdateFieldDefinitionAsync`, `DeleteFieldDefinitionAsync`, `GetFieldListValuesAsync`, `ReplaceFieldListValuesAsync`, `GetFieldContainingTemplatesAsync`, `GetFieldAssignedEntryCountAsync`, `GetFieldPropertiesAsync`, `UpdateFieldPropertiesAsync`.
- Add destructive field operations: `MergeFieldsAsync` and `ChangeFieldTypeAsync`, both gated by an explicit `allowDataLoss` flag — a request that would lose data is rejected unless `allowDataLoss` is `true`.
- Add template definition administration methods: `CreateTemplateAsync`, `UpdateTemplateAsync`, `DeleteTemplateAsync`, `GetTemplateAssignedEntryCountAsync`, `GetTemplatePropertiesAsync`, `UpdateTemplatePropertiesAsync`, `AddTemplateFieldAsync`, `UpdateTemplateFieldPropertiesAsync`, `RemoveTemplateFieldAsync`, `MoveTemplateFieldAsync`.
- `GetEntryAsync` accepts opt-in `includeChildInfo` (folder entries — immediate-children counts: `hasChildren`, `childCount`, `folderCount`, `documentCount`, `shortcutCount`) and `includeTotalSize` (document entries — full stored size including page data, distinct from `electronicDocumentSize`). Both are omitted from the response unless requested.
- Add Dynamic Fields administration methods (REQ-ADMIN-008): `ListExternalTablesAsync`, `GetExternalTableAsync`, `ListExternalTableColumnsAsync` for external-table registrations, and `GetTemplateFormLogicRulesAsync` / `SetTemplateFormLogicRulesAsync` to get and replace a template's dynamic-field (form-logic) rules. External tables are read-only — they are provisioned out-of-band through Process Automation data management, so there are no register/update/unregister methods.
- New types: request/response DTOs for field and template definition administration, the Dynamic Fields external-table and form-logic-rule types, and the `childInfo` object on the entry response.

## 2.1.0

### Features

- Add electronic document methods: `UpdateDocumentAsync`, `UpdateDocumentUploadedPartsAsync`.
- Add page manipulation methods: `CreatePagesAsync`, `ReplacePagesAsync`, `WritePageAsync`, `ListPageInfosAsync`, `MovePagesAsync`, `CopyPagesAsync`, `RotateImagePageAsync`, `GetPageImageAsync`, `GetPageTextAsync`, `GenerateTextAsync`.
- `ListPageInfosAsync` returns a paginated `PageInfoCollectionResponse` (OData envelope with `Value`/`OdataCount`/`OdataNextLink`); accepts `Top`, `Select`, `Count`, `PageRange`, and the `Prefer: odata.maxpagesize=...` header. Default page size 150; clients follow `OdataNextLink` for further pages.
- Add check-in/check-out and lock methods: `LockDocumentAsync`, `UnlockDocumentAsync`, `GetDocumentLockInfoAsync`, `PutUnderVersionControlAsync`, `CheckOutDocumentAsync`, `CheckInDocumentAsync`, `UndoCheckOutAsync`.

### Breaking changes

- `ListDynamicFieldValuesAsync` return type narrowed from `Task<IDictionary<string, ICollection<string>>>` to `Task<IDictionary<string, IList<string>>>`. Compile-time only — runtime type is unchanged (`List<T>` underneath). Callers explicitly declaring the dictionary value as `ICollection<string>` need to update the declaration; callers using `var` or iterating with `foreach` are unaffected.

  **Why kept:** The narrowing is a side effect of adding `"responseArrayType": "IList"` to `nswag.json`, which standardizes the entire client surface on `IList<T>` for collection returns (preserving the codebase convention against NSwag 14.4's new `ICollection<T>` default). Reverting just this method would require either (a) dropping the setting and regressing every newly added method to `ICollection<T>` — inconsistent with the indexable assertion style used across the integration tests — or (b) carving a per-method exception in the liquid template, which is maintenance burden for an API where the runtime type is already `List<T>`. The benign compile-only break for one method is the better trade-off.

## 2.0.4

### Fixes

- Fix retry when locked

## 2.0.3

### Features

- Add additional methods that retry when entry is locked.

## 2.0.2

### Features

- Add retry when entry is locked. Defaults to 30 seconds. Allows setting by `EntriesClient.RetryIfLockedForTimeout`

## 2.0.1

### Features

- Add ability to update HttpClient timeout. Increased default timeout to 180 seconds.

### Chore & Maintenance

- Remove DateTime JSON serialization. Date/DateTime strings will not be converted by client.

## 2.0.0

### Features

- Add `StartTestTaskAsync` to `TasksClient` to allow starting a "mock" operation with a specified duration and outcome.

### Chore & Maintenance

- Update major version of package to `2.0.0` to avoid confusion with existing package `Laserfiche.Repository.Api.Client`
   - No breaking changes were included 
- Re-generated client using updated swagger.json to include any changes from the last year

## 1.0.2

### Chore & Maintenance

- Update version of `Laserfiche.Api.Client.Core` to `1.3.6`

## 1.0.1

### Chore & Maintenance

- Update the versions of `Moq`, `dotenv`, `MSTest.TestAdapter`, and `MSTest.TestFramework` due to dependency vulnerabilities
- Update version of `Laserfiche.Api.Client.Core` to `1.3.4`

## 1.0.0

### Features

- Initial release of the [Laserfiche.Repository.Api.Client.V2](https://www.nuget.org/packages/Laserfiche.Repository.Api.Client.V2) NuGet package. See the [migration guide](https://github.com/Laserfiche/lf-repository-api-client-dotnet/blob/HEAD/MIGRATION_GUIDE.md) for details on upgrading from the [Laserfiche.Repository.Api.Client](https://www.nuget.org/packages/Laserfiche.Repository.Api.Client) NuGet package.
