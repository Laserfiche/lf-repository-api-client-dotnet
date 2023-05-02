# Laserfiche Repository API Client .NET
[![NuGet version (Laserfiche.Repository.Api.Client)](https://img.shields.io/nuget/v/Laserfiche.Repository.Api.Client.svg?style=flat-square)](https://www.nuget.org/packages/Laserfiche.Repository.Api.Client)

Use the Laserfiche Repository API to access data in a Laserfiche repository. Import or export files, modify the repository folder structure, read and modify templates and field values, and more.

## Documentation
- [Laserfiche Developer Center](https://developer.laserfiche.com/)
- [Laserfiche Repository API Client .NET Library Documentation](https://laserfiche.github.io/lf-repository-api-client-dotnet/)

## Change Log
See CHANGELOG [here](https://github.com/Laserfiche/lf-repository-api-client-dotnet/blob/HEAD/CHANGELOG.md).

## How to contribute
Useful commands for building and testing the app.

### Generate the repository client
1. Download the `nswag` command line tool. Instructions can be found [here](https://github.com/RicoSuter/NSwag/wiki/CommandLine).
2. From the root directory of this Git repository, run the command `pwsh generate-client.ps1`

### Build, Test, and Package
See the [.github/workflows/main.yml](https://github.com/Laserfiche/lf-repository-api-client-dotnet/blob/HEAD/.github/workflows/main.yml).
