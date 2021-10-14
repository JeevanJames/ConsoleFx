# Download .NET Core 2.2.401 SDK and add to PATH
$urlCurrent = "https://download.visualstudio.microsoft.com/download/pr/4de548ef-9b51-4b1f-ae3a-60ebd6a6f2b5/01fce24fe286e7475fdbecc60f1daee5/dotnet-sdk-2.2.401-win-x64.zip"
$env:DOTNET_INSTALL_DIR = "$pwd\.dotnetsdk"
mkdir $env:DOTNET_INSTALL_DIR -Force | Out-Null
$tempFileCurrent = [System.IO.Path]::GetTempFileName()
(New-Object System.Net.WebClient).DownloadFile($urlCurrent, $tempFileCurrent)
Add-Type -AssemblyName System.IO.Compression.FileSystem; [System.IO.Compression.ZipFile]::ExtractToDirectory($tempFileCurrent, $env:DOTNET_INSTALL_DIR)
$env:Path = "$env:DOTNET_INSTALL_DIR;$env:Path"  
