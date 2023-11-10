<#
.SYNOPSIS
Generates a client from a swagger json file using the nswag tool.

.PARAMETER input_folder
Path to the folder containing the necessary files for generating the client. The folder should contain the nswag configuration file and the swagger json file.

.PARAMETER output_folder
Path to the folder the client should be created in
#>
param(
  [Parameter(Mandatory=$true)][String]$input_folder,
  [Parameter(Mandatory=$true)][String]$output_folder
)
$ErrorActionPreference = "Stop"

$client_filename = 'TableClients.cs'

# Clean folder endings from input parameters
$input_folder = $input_folder.TrimEnd("/", "\")
$output_folder = $output_folder.TrimEnd("/", "\")

# Generate the client
nswag run "$input_folder/nswag.json"

# Remove long namespaces from class names in the generated client. This makes the generated documentation look nicer.
$client_filepath = "$input_folder/$client_filename" 
(Get-Content $client_filepath) `
  -replace "System\.CodeDom\.Compiler\.","" `
  -replace "System\.Collections\.Generic\.","" `
  -replace "System\.Globalization\.","" `
  -replace "System\.IO\.","" `
  -replace "System\.Linq\.","" `
  -replace "System\.Net\.Http\.Headers\.","" `
  -replace "System\.Net\.Http\.(?![\w\.]+;)","" `
  -replace "System\.Runtime\.Serialization\.","" `
  -replace "System\.Reflection\.","" `
  -replace "System\.Text\.","" `
  -replace "System\.Threading\.Tasks\.","" `
  -replace "System\.Threading\.CancellationToken","CancellationToken" `
  -replace "System\.(?![\w\.]+;)","" `
  | Out-File $client_filepath

# Fix line breaks in documentation by moving the <br/> tag from the beginning of new lines to the end of previous lines
(Get-Content $client_filepath -Raw) `
  -replace "(`r?`n\s+\/\/\/ )(<br\/>)","`${2}`${1}" `
  | Out-File $client_filepath

# Move the generated client to the output folder
Move-Item -Path $client_filepath -Destination "$output_folder/$client_filename" -Force
