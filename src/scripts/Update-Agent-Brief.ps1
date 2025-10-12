<#
.SYNOPSIS
    Generates or checks the context digest for the Semantic RAG agent brief.

.DESCRIPTION
    Aggregates hashes and timestamps of the primary context artifacts used by development agents.
    Intended to run in scheduled jobs (generation mode) and by agents locally with -CheckOnly to confirm they
    are working against the latest brief.

.PARAMETER CheckOnly
    When specified, prints the current digest to the console without writing the digest file.

.PARAMETER OutputPath
    Relative path (from repo root) where the digest JSON should be persisted when not using -CheckOnly.
    Defaults to docs/reference/SemanticRag-Agent-Brief.digest.json.

.EXAMPLE
    pwsh src/scripts/Update-Agent-Brief.ps1 -CheckOnly

.EXAMPLE
    pwsh src/scripts/Update-Agent-Brief.ps1
#>
param(
    [switch]$CheckOnly,
    [string]$OutputPath = "docs/reference/SemanticRag-Agent-Brief.digest.json"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-RepoRoot {
    $scriptRoot = Split-Path -Path $PSScriptRoot -Parent
    $repoRoot = Resolve-Path -Path (Join-Path $scriptRoot "..") -ErrorAction Stop
    return $repoRoot.ProviderPath
}

function Get-FileDigestInfo {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Required artifact not found: $Path"
    }

    $hash = Get-FileHash -LiteralPath $Path -Algorithm SHA256
    $item = Get-Item -LiteralPath $Path

    [pscustomobject]@{
        Path            = ($hash.Path | Resolve-Path -Relative)
        HashAlgorithm   = $hash.Algorithm
        Hash            = $hash.Hash
        Length          = $item.Length
        LastWriteTimeUtc= $item.LastWriteTimeUtc
    }
}

$repoRoot = Get-RepoRoot
Push-Location -Path $repoRoot
try {
    $artifactPaths = @(
        "docs/reference/SemanticRag-Agent-Brief.md",
        "docs/templates/AgentWorkPackage.md",
        "docs/Unified-Semantic-RAG-Standards-Initiative.md",
        "docs/operations/AgentAssignmentRegister.csv",
        "docs/operations/AgentSyncLog.csv"
    )

    $fileInfo = foreach ($artifact in $artifactPaths) {
        Get-FileDigestInfo -Path $artifact
    }

    $aggregateInput = [System.Text.StringBuilder]::new()
    foreach ($file in $fileInfo) {
        [void]$aggregateInput.Append($file.Hash)
    }

    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($aggregateInput.ToString())
        $aggregateHash = [System.BitConverter]::ToString($sha.ComputeHash($bytes)).Replace("-", "").ToLowerInvariant()
    }
    finally {
        $sha.Dispose()
    }

    $digest = [pscustomobject]@{
        GeneratedOnUtc = [DateTime]::UtcNow
        AggregateHash  = $aggregateHash
        Files          = $fileInfo
    }

    if ($CheckOnly) {
        $digest | ConvertTo-Json -Depth 4
        return
    }

    $outputFullPath = Resolve-Path -Path (Join-Path $repoRoot $OutputPath) -ErrorAction SilentlyContinue
    if (-not $outputFullPath) {
        $outputDirectory = Split-Path -Path (Join-Path $repoRoot $OutputPath) -Parent
        if (-not (Test-Path -LiteralPath $outputDirectory)) {
            New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
        }
        $outputFullPath = Join-Path $repoRoot $OutputPath
    }

    $json = $digest | ConvertTo-Json -Depth 4
    Set-Content -LiteralPath $outputFullPath -Value $json -Encoding UTF8
    Write-Host "Agent brief digest updated at $OutputPath" -ForegroundColor Green
}
finally {
    Pop-Location
}
