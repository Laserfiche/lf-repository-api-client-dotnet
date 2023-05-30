param(
  [Parameter(Mandatory=$true)][String]$input_folder,
  [Parameter(Mandatory=$true)][String]$output_folder,
  [Parameter(Mandatory=$true)][String]$client_filename
)
$ErrorActionPreference = "Stop"

$input_folder = $input_folder.TrimEnd("/", "\")
$output_folder = $output_folder.TrimEnd("/", "\")

# Generate the client
nswag run "$input_folder/nswag.json" /variables:OutputFilename=$client_filename

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

# Move the generated client to the output folder
Move-Item -Path $client_filepath -Destination "$output_folder/$client_filename" -Force
