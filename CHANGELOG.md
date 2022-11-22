## 1.1.0

### Fixes
- Fix `IEntriesClient.GetDocumentContentTypeAsync` return type from `Task` to `Task<HttpResponseHead>` to allow retrieving response headers.
- Fix `IEntriesClient.GetDocumentContentTypeAsync` from throwing an error when trying to deserialize the response from an error response code.
- Fix `ISimpleSearchesClient.CreateSimpleSearchOperationAsync` return type from `Task<ODataValueOfIListOfEntry>` to `Task<ODataValueContextOfIListOfEntry>` to more accurately represent the response. The `ODataValueContextOfIListOfEntry` type derives from the `ODataValueOfIListOfEntry` type.
- Fix `FuzzyType` enum to serialize to string values instead of numbers.
- Fix the error message when an `ApiException` is thrown and use the `ProblemDetails.Title` if possible.
- Add more properties to the `ProblemDetails` type to more accurately represent the response.
- **[BREAKING]** Remove the `ProblemDetails.Extensions` dictionary property. This property was always null.
- **[BREAKING]** Update the usage of `ApiException` thrown from error response. In general, the `ApiException` can be caught like this.
  ```c#
  try
  {
    await repositoryApiClient.EntriesClient.GetEntryAsync(...);
  }
  catch (ApiException e)
  {
    Console.Error.WriteLine(e.Message);
    Console.Error.WriteLine(e);
  }
  ```
  In the case for `IEntriesClient.ImportDocumentAsync` when a document is successfully imported but errors occur when setting metadata, an `ApiException` is thrown with the partial success import response. The partial success import response and the successfully imported entry id can be retrieved like this.
  ```c#
  try
  {
    await repositoryApiClient.EntriesClient.ImportDocumentAsync(...);
  }
  catch (ApiException e)
  {
    Console.Error.WriteLine(e.Message);
    Console.Error.WriteLine(e);
    CreateEntryResult partialSuccessResult = (CreateEntryResult)e.ProblemDetails.AdditionalProperties[typeof(CreateEntryResult).Name];
    int createdEntryId = partialSuccessResult.Operations.EntryCreate.EntryId;
  }
  ```

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
