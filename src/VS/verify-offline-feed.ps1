#requires -Version 7.0
param(
  [string]$OfflinePath = 'artifacts/nuget/offline'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$scriptDir = Split-Path -Parent $PSCommandPath
$srcDir = Resolve-Path (Join-Path $scriptDir '..') | Select-Object -ExpandProperty Path
$repoRoot = Resolve-Path (Join-Path $srcDir '..') | Select-Object -ExpandProperty Path
$propsPath = Join-Path $srcDir 'Directory.Packages.props'
$offlineDir = Join-Path $repoRoot $OfflinePath

if (-not (Test-Path $offlineDir)) {
  Write-Warning "Offline directory not found: $offlineDir"
}

function Get-PackagesFromDirectoryPackagesProps([string]$propsPath) {
  if (-not (Test-Path $propsPath)) { return @() }
  [xml]$xml = Get-Content -Path $propsPath -Raw
  $nodes = $xml.SelectNodes('//PackageVersion')
  $nodes | ForEach-Object { [pscustomobject]@{ Id = $_.Include; Version = $_.Version } }
}

$packages = Get-PackagesFromDirectoryPackagesProps -propsPath $propsPath
$missing = @()
foreach ($p in $packages) {
  $dirName = "{0}.{1}" -f $p.Id, $p.Version
  $pkgDir = Join-Path $offlineDir $dirName
  if (-not (Test-Path $pkgDir)) {
    $missing += $dirName
  }
}

Write-Host "Packages referenced: $($packages.Count)"
Write-Host "Packages present:   $($packages.Count - $missing.Count)"
Write-Host "Packages missing:   $($missing.Count)"

if ($missing.Count -gt 0) {
  Write-Host "Missing entries:" -ForegroundColor Yellow
  $missing | ForEach-Object { Write-Host " - $_" }
  exit 2
}
exit 0

