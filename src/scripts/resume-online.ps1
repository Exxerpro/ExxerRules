#requires -Version 7.0
param([string]$Configuration = 'Debug')

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host "Restoring with online fallback..."
dotnet restore IndFusion.sln --configfile NuGet.online.config

Write-Host "Building ($Configuration)..."
dotnet build IndFusion.sln -c $Configuration --configfile NuGet.online.config

Write-Host "Running tests ($Configuration)..."
dotnet test IndFusion.sln -c $Configuration --configfile NuGet.online.config

Write-Host "Done."

