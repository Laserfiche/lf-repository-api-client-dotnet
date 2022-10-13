## 1.0.5

### Fixes
- Add missing `403` and `404` status codes to various APIs.
- Add missing property `EntryId` to `OperationProgress` type.
- Change `Entry` type to abstract. Should use the derived types like `Folder`, `Document`, `Shortcut`, and `RecordSeries`.
- Deprecate the `ServerSession` APIs. This applies to the following:
  - `ServerSessionClient.CreateServerSessionAsync`
  - `ServerSessionClient.RefreshServerSessionAsync`
  - `ServerSessionClient.InvalidateServerSessionAsync`
