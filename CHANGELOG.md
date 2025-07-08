# Changelog

## 2.0.2

### Features

- Add ability to retry when entry is locked for CreateOrCopyEntryAsync and metadata operations. 

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
