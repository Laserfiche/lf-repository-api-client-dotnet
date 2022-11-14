## Prerelease

### Fixes
- Fix `IEntriesClient.GetDocumentContentTypeAsync` return type from `Task` to `Task<HttpResponseHead>` to allow retrieving response headers.
- Fix `ISimpleSearchesClient.CreateSimpleSearchOperationAsync` return type from `Task<ODataValueOfIListOfEntry>` to `Task<ODataValueContextOfIListOfEntry>` to more accurately represent the response. The `ODataValueContextOfIListOfEntry` type derives from the `ODataValueOfIListOfEntry` type.
- Fix `FuzzyType` enum to serialize to string values instead of numbers.

## 1.0.5

### Fixes
- Add missing `403` and `404` status codes to various APIs.
- Add missing property `EntryId` to `OperationProgress` type.
- Change `Entry` type to abstract. Should use the derived types like `Folder`, `Document`, `Shortcut`, and `RecordSeries`.
- Deprecate the `ServerSession` APIs. This applies to the following:
  - `ServerSessionClient.CreateServerSessionAsync`
  - `ServerSessionClient.RefreshServerSessionAsync`
  - `ServerSessionClient.InvalidateServerSessionAsync`
- **[BREAKING]**: `IEntriesClient`
  - Rename `MoveOrRenameDocumentAsync` to `MoveOrRenameEntryAsync` to better represent its capability.
