#requires -Version 7.0
param(
    [string]$Source = 'https://api.nuget.org/v3/index.json',
    [string]$OutputSubPath = 'artifacts/nuget/offline'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Ensure-NuGetExe {
    $nugetDir = Join-Path $env:TEMP 'nuget-cli'
    $nugetExe = Join-Path $nugetDir 'nuget.exe'
    if (-not (Test-Path $nugetDir)) { New-Item -ItemType Directory -Path $nugetDir | Out-Null }
    if (-not (Test-Path $nugetExe)) {
        Write-Host "Downloading nuget.exe ..."
        $nugetUrl = 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe'
        Invoke-WebRequest -Uri $nugetUrl -OutFile $nugetExe -UseBasicParsing
    }
    return $nugetExe
}

function Get-PackagesFromDirectoryPackagesProps([string]$propsPath) {
    if (-not (Test-Path $propsPath)) { return @() }
    [xml]$xml = Get-Content -Path $propsPath -Raw
    $nodes = $xml.SelectNodes('//PackageVersion')
    $nodes | ForEach-Object {
        [pscustomobject]@{ Id = $_.Include; Version = $_.Version }
    }
}

function Get-PackagesFromCsproj([string]$root) {
    $csprojFiles = Get-ChildItem -Path $root -Recurse -Filter '*.csproj'
    $results = @()
    foreach ($file in $csprojFiles) {
        try {
            [xml]$xml = Get-Content -Path $file.FullName -Raw
        } catch { continue }
        $nodes = $xml.SelectNodes('//PackageReference[@Include and @Version]')
        if ($nodes) {
            $nodes | ForEach-Object {
                [pscustomobject]@{ Id = $_.Include; Version = $_.Version }
            } | ForEach-Object { $results += $_ }
        }
    }
    return $results
}

function Install-PackageAndDependencies([string]$nugetExe, [string]$id, [string]$version, [string]$outDir, [string]$source) {
    $args = @(
        'install', $id,
        '-Version', $version,
        '-Source', $source,
        '-OutputDirectory', $outDir,
        '-DependencyVersion', 'Highest',
        '-DirectDownload',
        '-NonInteractive'
    )
    if ($version -match '-') { $args += '-Prerelease' }
    & $nugetExe @args | Write-Host
}

# Paths
$scriptDir = Split-Path -Parent $PSCommandPath
$srcDir = Resolve-Path (Join-Path $scriptDir '..') | Select-Object -ExpandProperty Path
$repoRoot = Resolve-Path (Join-Path $srcDir '..') | Select-Object -ExpandProperty Path
$propsPath = Join-Path $srcDir 'Directory.Packages.props'
$outputDir = Join-Path $repoRoot $OutputSubPath

New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

# Collect package list
$packages = @()
$packages += Get-PackagesFromDirectoryPackagesProps -propsPath $propsPath
$packages += Get-PackagesFromCsproj -root $srcDir

# De-duplicate by Id+Version
$packages = $packages | Group-Object Id, Version | ForEach-Object { $_.Group[0] }

Write-Host "Found $($packages.Count) package(s) to download."
$nugetExe = Ensure-NuGetExe

foreach ($pkg in $packages) {
    Write-Host "Downloading $($pkg.Id) $($pkg.Version) ..."
    Install-PackageAndDependencies -nugetExe $nugetExe -id $pkg.Id -version $pkg.Version -outDir $outputDir -source $Source
}

Write-Host "Done. Packages downloaded to: $outputDir"

