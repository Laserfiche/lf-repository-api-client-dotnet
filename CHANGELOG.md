# Changelog

## 2.5.0

### Features

- Add alternate electronic document methods: `ListAlternateEdocsAsync`, `GetAlternateEdocInfoAsync`, `WriteAlternateEdocAsync`, `DeleteAlternateEdocAsync`, `WriteAltEdocUploadedPartsAsync`. A document can carry named binary streams alongside its primary electronic document, and these travel with it through copy, move, versioning and briefcase operations. A write is create-or-replace and repeating an identical request is safe; the primary electronic document, the pages and the metadata are never touched. Names are at most 15 characters from a restricted ASCII set and are matched exactly, with no case folding — a write whose name differs from an existing stream's only in letter case is refused with 409 rather than performed.
- `ExportEntryRequestPart.AlternateEdoc` and `AlternateEdocName` on both export request types, so a written stream can be read back. `ExportEntryAsync` and `StartExportEntryAsync` return the named stream through the same audited export flow used for the electronic document. Without these an alternate electronic document could be written through this client but never downloaded.
- `Document.HasAlternateEdocs` reports whether a document carries any, so a caller can tell whether to enumerate without a second call. It is populated on a single-entry `GetEntryAsync` and is `null` in listing results, where the value is not determined — `null` means "not determined here" rather than "none".
- Add page word location methods: `GetPageTextOffsetsAsync` returns the page text span covered by a rectangle drawn on a page image, as UTF-16 offsets plus the text; `ListPageWordLocationsAsync` returns every word of a page in reading order with its text offsets and its rectangle, plus the geometry those coordinates use. Coordinates are raw unrotated image pixels, not display pixels, and `textEnd` is exclusive — the same convention text-linked annotations use, so a span read here writes straight back.
- New types: request/response DTOs for the above, including `AlternateEdocInfoResponse`, `AlternateEdocInfoCollectionResponse`, `PageTextOffsetsResponse`, `PageWordLocation` and `PageWordLocationsResponse`.

### Breaking changes

- **Overwrite requests must carry their collection member.** `SetTagsAsync`, `SetLinksAsync`, `SetFieldsAsync` and the five access-control setters (`SetEntryAccessControlAsync`, `SetFieldAccessControlAsync`, `SetDefaultFieldAccessControlAsync`, `SetTemplateAccessControlAsync`, `SetDefaultTemplateAccessControlAsync`) are overwrite operations. A request whose body does not carry the collection member at all is now rejected with a `400` naming the expected member, instead of being applied as an empty collection. Previously such a request succeeded and cleared everything the entry, field or template had — and on the access-control routes, dropping an explicit Deny could let a trustee fall back to an inherited Allow, so a malformed request could widen access. Sending the member with an explicit empty collection is unchanged and remains the documented way to clear.

### Behavior changes

- `ExportEntryAsync` and `StartExportEntryAsync` now answer `400` up front, instead of failing inside the export service with a `500`, when `Part` is `Image` on a document whose pages carry no image data, or `Text` on a document whose pages carry no text. Text is extracted asynchronously after an import, so a document may briefly have pages and no text — poll `hasText` on `ListPageInfosAsync` and retry once it reports true. A document with no pages at all is not rejected here.
- The download link an export returns is single-use: the first GET returns the file and any later GET of the same link answers `404`, with no problem details, because that response comes from the download service rather than from the API. Save the content on the first download and start a new export if a download has to be retried.
- `MoveTemplateFieldAsync` returns `400` naming the valid range, instead of `500` with a raw framework message, when `newPosition` is past the last field.
- Operations the repository server does not support (error code `7002`) return `400` instead of `500`. The refusal is deterministic — the same request can never succeed against that server — so a `500` invited retries that could not help. The error code is unchanged, so the condition remains identifiable.
- `CreateAnnotationAsync` and `UpdateAnnotationAsync` responses now report the stored values for `PageNumber`, `Creator`, `CreatedTime` and `LastModifiedTime`. Previously the create response reported page 1 regardless of the page annotated, a null creator and `0001-01-01T00:00:00Z` timestamps, and the update response reported the pre-update `LastModifiedTime`. Stored data was never affected.
- `CreateTemplateAsync` is now all-or-nothing when given initial `fields`: if any initial field assignment fails the template is deleted before the error is returned, so a retry no longer fails with `409 Object already exists` for a name that was never successfully used.

## 2.4.0

### Features

- Add optional `FolderPath` request property and `AutoCreateFolderPath` query parameter to `ImportEntryAsync`, `StartImportUploadedPartsAsync`, and `CreateEntryAsync`. When `AutoCreateFolderPath` is `true`, any missing folders in `FolderPath` are created; when `false` (default), a missing folder returns 404.

## 2.3.0

### Features

- Add Records Management methods, exposed on the new `RecordsManagementClient`: `GetEntryRecordsManagementPropertiesAsync`, `UpdateEntryRecordsManagementPropertiesAsync`, `GetEligibleRecordsAsync`, `GetIndependentRecordsAsync`, `GetAltRetentionEventsAsync`, `GetRecordSeriesPropertiesAsync`, `UpdateRecordSeriesPropertiesAsync`, `SetRecordEventAsync`, `RemoveRecordEventAsync`, `CreateRecordSeriesAsync`.
- Add unified access-rights/access-control methods for fields, templates, and entries, exposed on the new `AccessControlClient`: `GetFieldAccessControlAsync`/`SetFieldAccessControlAsync`, `GetFieldRightsAsync`, `GetDefaultFieldAccessControlAsync`/`SetDefaultFieldAccessControlAsync`, `GetEntryAccessControlAsync`/`SetEntryAccessControlAsync`, `GetEntryRightsAsync`, `GetSessionRightsAsync`, `GetTemplateAccessControlAsync`/`SetTemplateAccessControlAsync`, `GetTemplateRightsAsync`, `GetDefaultTemplateAccessControlAsync`/`SetDefaultTemplateAccessControlAsync`, `LookupTrusteesAsync`, `GetTrusteeSecurityAsync`.
- Add User Areas methods, exposed on the new `UserAreasClient`: recent documents/folders (`GetRecentDocumentsAsync`, `GetRecentFoldersAsync`), starred entries (`GetStarredEntriesAsync`, `StarEntriesAsync`, `UnstarEntriesAsync`), personal collections (`GetPersonalCollectionsAsync`, `CreatePersonalCollectionAsync`, `GetPersonalCollectionAsync`, `RenamePersonalCollectionAsync`, `DeletePersonalCollectionAsync`, `AddCollectionEntriesAsync`, `RemoveCollectionEntriesAsync`), and generic user areas (`GetUserAreasAsync`, `CreateUserAreaAsync`, `GetUserAreaAsync`, `UpdateUserAreaAsync`, `DeleteUserAreaAsync`, `GetUserAreaEntriesAsync`, `AddUserAreaEntriesAsync`, `RemoveUserAreaEntriesAsync`).
- Add Annotations & Stamps methods, exposed on the new `AnnotationsClient` and `StampsClient`: `ListDocumentAnnotationsAsync`, `ListPageAnnotationsAsync`, `CreateAnnotationAsync`, `GetAnnotationAsync`, `UpdateAnnotationAsync`, `DeleteAnnotationAsync`, `GetAnnotationAttachmentAsync`, `UploadAnnotationAttachmentAsync`, `UploadAnnotationImageAsync`, annotation reasons (`ListAnnotationReasonsAsync`, `CreateAnnotationReasonAsync`, `UpdateAnnotationReasonAsync`, `DeleteAnnotationReasonAsync`), and stamps (`ListStampsAsync`, `CreateStampAsync`, `GetStampAsync`, `UpdateStampAsync`, `DeleteStampAsync`, `GetStampImageAsync`).
- New types: request/response DTOs for all of the above, plus discriminated-union types `Annotation` (14 subtypes: Highlight, Redaction, Strikeout, Underline, Note, Attachment, TextBox, Bitmap, Line, Rectangle, Polyline, Callout, Stamp, FreeHand) and `RecordsManagementProperties` (`RecordProperties`/`RecordFolderProperties`).

## 2.2.0

### Features

- Add field definition administration methods: `CreateFieldDefinitionAsync`, `UpdateFieldDefinitionAsync`, `DeleteFieldDefinitionAsync`, `GetFieldListValuesAsync`, `ReplaceFieldListValuesAsync`, `GetFieldContainingTemplatesAsync`, `GetFieldAssignedEntryCountAsync`, `GetFieldPropertiesAsync`, `UpdateFieldPropertiesAsync`.
- Add destructive field operations: `MergeFieldsAsync` and `ChangeFieldTypeAsync`, both gated by an explicit `allowDataLoss` flag — a request that would lose data is rejected unless `allowDataLoss` is `true`.
- Add template definition administration methods: `CreateTemplateAsync`, `UpdateTemplateAsync`, `DeleteTemplateAsync`, `GetTemplateAssignedEntryCountAsync`, `GetTemplatePropertiesAsync`, `UpdateTemplatePropertiesAsync`, `AddTemplateFieldAsync`, `UpdateTemplateFieldPropertiesAsync`, `RemoveTemplateFieldAsync`, `MoveTemplateFieldAsync`.
- `GetEntryAsync` accepts opt-in `includeChildInfo` (folder entries — immediate-children counts: `hasChildren`, `childCount`, `folderCount`, `documentCount`, `shortcutCount`) and `includeTotalSize` (document entries — full stored size including page data, distinct from `electronicDocumentSize`). Both are omitted from the response unless requested.
- New types: request/response DTOs for field and template definition administration, and the `childInfo` object on the entry response.

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
