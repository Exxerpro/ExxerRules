<#
.SYNOPSIS
    Executes post-delivery due diligence for Semantic RAG work items.

.DESCRIPTION
    Optionally runs regression tests, verifies guardrail artifacts, and writes a JSON report summarising
    completion status, metrics, and outstanding warnings.

.PARAMETER WorkItemId
    Azure Boards work item identifier to associate with this due diligence run.

.PARAMETER OutputDirectory
    Relative directory to persist the JSON summary (defaults to docs/operations/due-diligence).

.PARAMETER TestProject
    Optional test project path to execute for verification. Defaults to the analyzer test suite.

.PARAMETER SkipTests
    Skip executing dotnet test (useful if tests already ran in CI and logs are attached).
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$WorkItemId,
    [string]$OutputDirectory = "docs/operations/due-diligence",
    [string]$TestProject = "src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj",
    [switch]$SkipTests
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
    $testOutcome = $null

    if (-not $SkipTests) {
        Write-Host "Running dotnet test for $TestProject..." -ForegroundColor Cyan
        $testOutput = & dotnet test $TestProject -c Release
        $testSucceeded = $LASTEXITCODE -eq 0
        $tail = (($testOutput -split [Environment]::NewLine) | Select-Object -Last 10) -join "`n"
        $checks += Test-AndReport -Name "dotnet test" -Result $testSucceeded -Details $tail
        $testOutcome = $testOutput -split [Environment]::NewLine
    }
    else {
        $checks += Test-AndReport -Name "dotnet test" -Result $true -Details "Skipped on request."
    }

    $assignmentCsv = "docs/operations/AgentAssignmentRegister.csv"
    if (Test-Path -LiteralPath $assignmentCsv) {
        $assignmentRows = Import-Csv -LiteralPath $assignmentCsv
        $assignmentMatch = @($assignmentRows | Where-Object { $_.WorkItemId -eq $WorkItemId })
        $hasEntry = $assignmentMatch.Length -gt 0
        $details = if ($hasEntry) { $assignmentMatch | ConvertTo-Json -Compress } else { "No entry located for $WorkItemId." }
        $checks += Test-AndReport -Name "Assignment Register" -Result $hasEntry -Details $details
    }
    else {
        $checks += Test-AndReport -Name "Assignment Register" -Result $false -Details "Assignment register missing."
    }

    $syncCsv = "docs/operations/AgentSyncLog.csv"
    if (Test-Path -LiteralPath $syncCsv) {
        $syncRows = Import-Csv -LiteralPath $syncCsv
        $syncMatch = @($syncRows | Where-Object { $_.WorkItemId -eq $WorkItemId })
        $hasSync = $syncMatch.Length -gt 0
        $syncDetails = if ($hasSync) { $syncMatch | ConvertTo-Json -Compress } else { "No sync acknowledgement for $WorkItemId." }
        $checks += Test-AndReport -Name "Agent Sync Log" -Result $hasSync -Details $syncDetails
    }
    else {
        $checks += Test-AndReport -Name "Agent Sync Log" -Result $false -Details "Sync log missing."
    }

    $logPath = Join-Path $repoRoot "agent-trace/$WorkItemId.log"
    $checks += Test-AndReport -Name "Agent Trace Log" -Result (Test-Path -LiteralPath $logPath) -Details $logPath

    $digestPath = "docs/reference/SemanticRag-Agent-Brief.digest.json"
    $checks += Test-AndReport -Name "Agent Brief Digest" -Result (Test-Path -LiteralPath $digestPath) -Details $digestPath

    $summary = [pscustomobject]@{
        WorkItemId   = $WorkItemId
        CompletedOn  = [DateTime]::UtcNow
        Commit       = (git rev-parse HEAD)
        Checks       = $checks
        TestOutput   = $testOutcome
    }

    $outputDirFull = Join-Path $repoRoot $OutputDirectory
    if (-not (Test-Path -LiteralPath $outputDirFull)) {
        New-Item -ItemType Directory -Path $outputDirFull -Force | Out-Null
    }

    $outputPath = Join-Path $outputDirFull "$($WorkItemId)-postfinish.json"
    $summary | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $outputPath -Encoding UTF8

    Write-Host "Post-finish due diligence summary written to $outputPath" -ForegroundColor Green
}
finally {
    Pop-Location
}
