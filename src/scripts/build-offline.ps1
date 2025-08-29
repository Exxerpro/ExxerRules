#requires -Version 7.0
param([string]$Configuration = 'Release')

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host "Restoring strictly from offline feed..."
dotnet restore IndFusion.sln --configfile NuGet.config

Write-Host "Building ($Configuration) strictly offline..."
dotnet build IndFusion.sln -c $Configuration --configfile NuGet.config

Write-Host "Done."

