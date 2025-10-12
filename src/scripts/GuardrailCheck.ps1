<#
.SYNOPSIS
    Validates guardrail compliance for a given work item before opening a pull request.

.DESCRIPTION
    Ensures staged changes live within the approved directories, required logs exist, and the agent
    sync register contains the latest acknowledgement for the work item.

.PARAMETER WorkItemId
    Azure Boards work item identifier (e.g., 12345).

.PARAMETER AllowedPaths
    Optional array of relative path prefixes that are permitted for this work item.
    Defaults to src/, docs/, agent-trace/, and build metadata files.

.PARAMETER AgentLogDirectory
    Relative directory containing agent execution logs. Defaults to agent-trace.

.PARAMETER AssignmentRegisterPath
    Path to the assignment register CSV. Defaults to docs/operations/AgentAssignmentRegister.csv.

.PARAMETER SyncLogPath
    Path to the sync acknowledgment CSV. Defaults to docs/operations/AgentSyncLog.csv.

.PARAMETER AllowUntracked
    Allows untracked files outside of AllowedPaths (useful for generated artifacts not yet reviewed).
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$WorkItemId,
    [string[]]$AllowedPaths = @("src/", "docs/", "agent-trace/", ".editorconfig", ".gitignore"),
    [string]$AgentLogDirectory = "agent-trace",
    [string]$AssignmentRegisterPath = "docs/operations/AgentAssignmentRegister.csv",
    [string]$SyncLogPath = "docs/operations/AgentSyncLog.csv",
    [switch]$AllowUntracked
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-RepoRoot {
    $scriptRoot = Split-Path -Path $PSScriptRoot -Parent
    $repoRoot = Resolve-Path -Path (Join-Path $scriptRoot "..") -ErrorAction Stop
    return $repoRoot.ProviderPath
}

function Get-GitStatus {
    param(
        [string]$RepoRoot
    )

    Push-Location -Path $RepoRoot
    try {
        $status = git status --porcelain
        return $status
    }
    finally {
        Pop-Location
    }
}

function Ensure-WithinGuardrails {
    param(
        [string[]]$StatusLines,
        [string[]]$Prefixes,
        [switch]$AllowUntracked
    )

    $violations = @()
    foreach ($line in $StatusLines) {
        if ([string]::IsNullOrWhiteSpace($line)) {
            continue
        }

        $statusCode = $line.Substring(0, 2)
        $path = $line.Substring(3).Trim()

        if ($statusCode -eq "??" -and $AllowUntracked) {
            continue
        }

        $isAllowed = $false
        foreach ($prefix in $Prefixes) {
            if ($path.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase)) {
                $isAllowed = $true
                break
            }
        }

        if (-not $isAllowed) {
            $violations += [pscustomobject]@{
                Status = $statusCode
                Path   = $path
            }
        }
    }

    if ($violations.Count -gt 0) {
        $message = "Guardrail violation detected:`n" + ($violations | Format-Table -AutoSize | Out-String)
        throw $message
    }
}

function Ensure-RegisterEntry {
    param(
        [string]$CsvPath,
        [string]$WorkItemId,
        [string]$RegisterName
    )

    if (-not (Test-Path -LiteralPath $CsvPath)) {
        throw "$RegisterName not found at $CsvPath"
    }

    $rows = Import-Csv -LiteralPath $CsvPath
    if (-not ($rows | Where-Object { $_.WorkItemId -eq $WorkItemId })) {
        throw "$RegisterName missing entry for work item $WorkItemId"
    }
}

$repoRoot = Resolve-RepoRoot
$statusLines = Get-GitStatus -RepoRoot $repoRoot
Ensure-WithinGuardrails -StatusLines $statusLines -Prefixes $AllowedPaths -AllowUntracked:$AllowUntracked

$logPath = Join-Path $repoRoot (Join-Path $AgentLogDirectory "$WorkItemId.log")
if (-not (Test-Path -LiteralPath $logPath)) {
    throw "Agent log missing: $logPath"
}

Ensure-RegisterEntry -CsvPath (Join-Path $repoRoot $AssignmentRegisterPath) -WorkItemId $WorkItemId -RegisterName "Assignment register"
Ensure-RegisterEntry -CsvPath (Join-Path $repoRoot $SyncLogPath) -WorkItemId $WorkItemId -RegisterName "Agent sync log"

Write-Host "Guardrail validation passed for work item $WorkItemId" -ForegroundColor Green
