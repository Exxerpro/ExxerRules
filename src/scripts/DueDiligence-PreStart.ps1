<#
.SYNOPSIS
    Executes pre-start due diligence checks before commencing a Semantic RAG delivery workstream.

.DESCRIPTION
    Validates repository access, offline dependencies, baseline telemetry, and assignment records.
    Produces a JSON summary for archival in docs/operations/due-diligence.

.PARAMETER WorkItemId
    Azure Boards work item identifier to associate with this due diligence run.

.PARAMETER OutputDirectory
    Relative directory to persist the JSON summary (defaults to docs/operations/due-diligence).
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$WorkItemId,
    [string]$OutputDirectory = "docs/operations/due-diligence"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-RepoRoot {
    $scriptRoot = Split-Path -Path $PSScriptRoot -Parent
    return (Resolve-Path -Path (Join-Path $scriptRoot "..") -ErrorAction Stop).ProviderPath
}

function Test-AndReport {
    param(
        [string]$Name,
        [bool]$Result,
        [string]$Details
    )

    [pscustomobject]@{
        Name    = $Name
        Success = $Result
        Details = $Details
    }
}

$repoRoot = Resolve-RepoRoot
Push-Location -Path $repoRoot
try {
    $checks = @()

    $statusLines = git status --porcelain
    $isClean = [string]::IsNullOrWhiteSpace(($statusLines -join [Environment]::NewLine))
    $statusDetails = if ($isClean) { "Working tree is clean." } else { "Pending changes detected:`n$($statusLines -join [Environment]::NewLine)" }
    $checks += Test-AndReport -Name "Repo Cleanliness" -Result $isClean -Details $statusDetails
    $offlinePath = "artifacts/nuget/offline"
    $checks += Test-AndReport -Name "Offline Feed" -Result (Test-Path -LiteralPath $offlinePath) -Details "Offline feed located at $offlinePath."

    $briefPath = "docs/reference/SemanticRag-Agent-Brief.md"
    $checks += Test-AndReport -Name "Agent Brief Exists" -Result (Test-Path -LiteralPath $briefPath) -Details $briefPath

    $assignmentCsv = "docs/operations/AgentAssignmentRegister.csv"
    if (Test-Path -LiteralPath $assignmentCsv) {
        $assignmentRows = Import-Csv -LiteralPath $assignmentCsv
        $assignmentMatch = @($assignmentRows | Where-Object { $_.WorkItemId -eq $WorkItemId })
        $hasEntry = $assignmentMatch.Length -gt 0
        $details = if ($hasEntry) { $assignmentMatch | ConvertTo-Json -Compress } else { "No entry located for $WorkItemId." }
        $checks += Test-AndReport -Name "Assignment Registered" -Result $hasEntry -Details $details
    }
    else {
        $checks += Test-AndReport -Name "Assignment Registered" -Result $false -Details "Assignment register not found."
    }

    $syncCsv = "docs/operations/AgentSyncLog.csv"
    $checks += Test-AndReport -Name "Sync Log Prepared" -Result (Test-Path -LiteralPath $syncCsv) -Details $syncCsv

    $dotnetInfo = & dotnet --info

    $summary = [pscustomobject]@{
        WorkItemId   = $WorkItemId
        GeneratedOn  = [DateTime]::UtcNow
        Commit       = (git rev-parse HEAD)
        Checks       = $checks
        DotNetInfo   = ($dotnetInfo -split [Environment]::NewLine)
    }

    $outputDirFull = Join-Path $repoRoot $OutputDirectory
    if (-not (Test-Path -LiteralPath $outputDirFull)) {
        New-Item -ItemType Directory -Path $outputDirFull -Force | Out-Null
    }

    $outputPath = Join-Path $outputDirFull "$($WorkItemId)-prestart.json"
    $summary | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $outputPath -Encoding UTF8

    Write-Host "Pre-start due diligence summary written to $outputPath" -ForegroundColor Green
}
finally {
    Pop-Location
}
