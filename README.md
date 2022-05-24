# Laserfiche Repository API Client .NET

.NET API that enables easy and secure access to Laserfiche Repository. 

## Documentation

https://developer.laserfiche.com/

## How to contribute

Technically you could use any editors you like. But it's more convenient if you are using either Visual Studio Code or Visual Studio. Here is a few useful commands for building and testing the app.

### Generate the repository client

1. Download the `nswag` command line tool. Instructions can be found [here](https://github.com/RicoSuter/NSwag/wiki/CommandLine).
2. From the root directory of this Git repository, run the command `nswag run nswag.json`

### Build

`dotnet build --no-restore`

### Release Build

`dotnet build --configuration Release --no-restore`

### Make a Nuget Package

`dotnet pack` (in the same directly where the `.csproj` file resides)

### Add a Nuget Pacakge in a Local Directory

`dotnet add package {package_id} --source {absolute_path_to_folder_containing_nupkg}`
