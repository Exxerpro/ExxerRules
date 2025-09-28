#requires -Version 7.0
param([string]$Configuration = 'Release')

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host "Restoring with default NuGet sources..."
dotnet restore IndFusion.sln

Write-Host "Building ($Configuration)..."
dotnet build IndFusion.sln -c $Configuration

Write-Host "Done."
