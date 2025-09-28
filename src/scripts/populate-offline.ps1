#requires -Version 7.0
param(
  [string]$Source = 'https://api.nuget.org/v3/index.json',
  [string]$OutputSubPath = 'artifacts/nuget/offline'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host "Listing packages (dry run)..."
pwsh -NoLogo -File VS/fetch-packages.ps1 -Source $Source -OutputSubPath $OutputSubPath -DryRun

Write-Host "Downloading packages to $OutputSubPath ..."
pwsh -NoLogo -File VS/fetch-packages.ps1 -Source $Source -OutputSubPath $OutputSubPath -SkipDownloadIfExists

Write-Host "Verifying offline feed..."
pwsh -NoLogo -File VS/verify-offline-feed.ps1 -OfflinePath $OutputSubPath

Write-Host "Offline cache ready."
