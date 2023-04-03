# Changelog

## 1.1.2

## Fixes
- Added `static async System.Threading.Tasks.Task<System.Collections.Generic.ICollection<RepositoryInfo>> GetSelfHostedRepositoryListAsync(baseUrl: string)` method to the `RepositoryApiClient` class that will enable self hosted users to get their repo list without a jwt.

## 1.1.1

### Features
- `CreateFromAccessKey` now has an optional parameter `scope` to specify the request scope(s) for the access token.

## 1.1.0

### Fixes
- Fix `IEntriesClient.GetDocumentContentTypeAsync` return type from `Task` to `Task<HttpResponseHead>` to allow retrieving response headers.
- Fix `IEntriesClient.GetDocumentContentTypeAsync` from throwing an error when trying to deserialize the response from an error response code.
- Fix `ISimpleSearchesClient.CreateSimpleSearchOperationAsync` return type from `Task<ODataValueOfIListOfEntry>` to `Task<ODataValueContextOfIListOfEntry>` to more accurately represent the response. The `ODataValueContextOfIListOfEntry` type derives from the `ODataValueOfIListOfEntry` type.
- Fix `FuzzyType` enum to serialize to string values instead of numbers.
- Fix the error message when an `ApiException` is thrown and use the `ProblemDetails.Title` if possible.
- Add more properties to the `ProblemDetails` type to more accurately represent the response.
- **[BREAKING]** Types `ApiException` and `ProblemDetails` have been moved to namespace `Laserfiche.Api.Client`.
- **[BREAKING]** Property `ProblemDetails.AdditionalProperties` has been removed. Use property `ProblemDetails.Extensions` instead.
- **[BREAKING]** Types of `ApiException<T>` has been removed. Use `ApiException` instead. The `ApiException` has a `ProblemDetails` property which may contain additional information.\
  `IEntriesClient.ImportDocumentAsync` API v1 can succeed in creating a document, but fail in setting some or all of its metadata components. To retrieve errors in the case of a partial success, inspect the content of the `ProblemDetails.Extensions`. See example below.
  ```c#
  try
  {
    await repositoryApiClient.EntriesClient.ImportDocumentAsync(...);
  }
  catch (ApiException e)
  {
    Console.Error.WriteLine(e.Message);
    Console.Error.WriteLine(e);
    if (e?.ProblemDetails?.Extensions?.TryGetValue(nameof(CreateEntryResult), out var value) == true && value is CreateEntryResult partialSuccessResult)
    {
        int? createdEntryId = partialSuccessResult?.Operations?.EntryCreate?.EntryId;
    }
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
