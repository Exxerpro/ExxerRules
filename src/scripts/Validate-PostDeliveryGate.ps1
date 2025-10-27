#Requires -Version 7.0

<#
.SYNOPSIS
    Validates that all post-delivery gate requirements are met for the IndFusion Semantic RAG initiative.

.DESCRIPTION
    This script enforces the mandatory tracking requirements for the post-delivery gate.
    It validates that all epics and stories are marked complete with proper evidence and documentation.

.PARAMETER ProjectRoot
    The root directory of the IndFusion project. Defaults to the script's parent directory.

.PARAMETER OutputFile
    Path to write the validation report. Defaults to a timestamped file in the project root.

.EXAMPLE
    .\Validate-PostDeliveryGate.ps1

.EXAMPLE
    .\Validate-PostDeliveryGate.ps1 -ProjectRoot "C:\Projects\IndFusion" -OutputFile "C:\Reports\post-delivery-validation.json"
#>

param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$OutputFile = (Join-Path $ProjectRoot "docs\operations\due-diligence\post-delivery-gate-validation-$(Get-Date -Format 'yyyyMMdd-HHmmss').json")
)

# Ensure output directory exists
$OutputDir = Split-Path -Parent $OutputFile
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

# Initialize validation results
$ValidationResults = @{
    Timestamp = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"
    Gate = "Post-Delivery"
    Status = "PASS"
    Errors = @()
    Warnings = @()
    Checks = @()
    Summary = @{
        TotalChecks = 0
        PassedChecks = 0
        FailedChecks = 0
        WarningChecks = 0
    }
    EpicCompletion = @{}
    StoryCompletion = @{}
}

# Define required paths
$ProjectManagementRoot = Join-Path $ProjectRoot "docs\project-management"
$EpicsPath = Join-Path $ProjectManagementRoot "epics"
$StoriesPath = Join-Path $ProjectManagementRoot "stories"
$SprintsPath = Join-Path $ProjectManagementRoot "sprints"
$ProgressPath = Join-Path $ProjectManagementRoot "progress"

# Define expected epics
$ExpectedEpics = @(
    @{ Id = "E1"; Title = "MCP Tool Integration Foundation"; File = "epic-1-mcp-tool-integration.md" },
    @{ Id = "E2"; Title = "Semantic Search Infrastructure"; File = "epic-2-semantic-search.md" },
    @{ Id = "E3"; Title = "Knowledge Graph and Pattern Tracking"; File = "epic-3-knowledge-graph.md" },
    @{ Id = "E4"; Title = "Cross-Repository Analytics"; File = "epic-4-cross-repo-analytics.md" },
    @{ Id = "E5"; Title = "Agent Governance and Telemetry"; File = "epic-5-agent-governance.md" }
)

# Define expected stories per epic
$ExpectedStories = @{
    "E1" = @(
        @{ Id = "1.1"; Title = "Analyzer MCP Tool Wrapper"; File = "story-1.1-analyzer-mcp-wrapper.md" },
        @{ Id = "1.2"; Title = "Refactoring Tool MCP Integration"; File = "story-1.2-refactoring-tool-integration.md" },
        @{ Id = "1.3"; Title = "MCP Tool Registry and Discovery"; File = "story-1.3-mcp-tool-registry.md" }
    )
    "E2" = @(
        @{ Id = "2.1"; Title = "Code Embedding Pipeline"; File = "story-2.1-code-embedding-pipeline.md" },
        @{ Id = "2.2"; Title = "Vector Search Implementation"; File = "story-2.2-vector-search-implementation.md" },
        @{ Id = "2.3"; Title = "Documentation and Knowledge Integration"; File = "story-2.3-documentation-integration.md" }
    )
    "E3" = @(
        @{ Id = "3.1"; Title = "Code Relationship Graph"; File = "story-3.1-code-relationship-graph.md" },
        @{ Id = "3.2"; Title = "Pattern and Diagnostic Mapping"; File = "story-3.2-pattern-diagnostic-mapping.md" },
        @{ Id = "3.3"; Title = "Pattern Evolution Tracking"; File = "story-3.3-pattern-evolution-tracking.md" }
    )
    "E4" = @(
        @{ Id = "4.1"; Title = "Repository Ingestion Pipeline"; File = "story-4.1-repository-ingestion-pipeline.md" },
        @{ Id = "4.2"; Title = "Standards Drift Detection"; File = "story-4.2-standards-drift-detection.md" },
        @{ Id = "4.3"; Title = "Cross-Repository Insights Dashboard"; File = "story-4.3-cross-repo-insights-dashboard.md" }
    )
    "E5" = @(
        @{ Id = "5.1"; Title = "Agent Telemetry Infrastructure"; File = "story-5.1-agent-telemetry-infrastructure.md" },
        @{ Id = "5.2"; Title = "Agent Guardrails and Validation"; File = "story-5.2-agent-guardrails-validation.md" },
        @{ Id = "5.3"; Title = "Agent Supervision Dashboard"; File = "story-5.3-agent-supervision-dashboard.md" }
    )
}

# Function to add a check result
function Add-CheckResult {
    param(
        [string]$Name,
        [string]$Status,
        [string]$Message,
        [string]$Details = ""
    )
    
    $check = @{
        Name = $Name
        Status = $Status
        Message = $Message
        Details = $Details
        Timestamp = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"
    }
    
    $ValidationResults.Checks += $check
    $ValidationResults.Summary.TotalChecks++
    
    switch ($Status) {
        "PASS" { $ValidationResults.Summary.PassedChecks++ }
        "FAIL" { 
            $ValidationResults.Summary.FailedChecks++
            $ValidationResults.Status = "FAIL"
            $ValidationResults.Errors += $Message
        }
        "WARNING" { 
            $ValidationResults.Summary.WarningChecks++
            $ValidationResults.Warnings += $Message
        }
    }
}

# Function to check if epic is marked complete
function Test-EpicCompletion {
    param(
        [string]$FilePath
    )
    
    if (-not (Test-Path $FilePath)) {
        return @{ IsComplete = $false; Reason = "File not found" }
    }
    
    $content = Get-Content $FilePath -Raw
    
    # Check for completion indicators
    $completionIndicators = @(
        "Status.*Complete",
        "Status.*✅",
        "Status.*🟢",
        "Epic marked complete",
        "All stories completed"
    )
    
    $isComplete = $false
    foreach ($indicator in $completionIndicators) {
        if ($content -match $indicator) {
            $isComplete = $true
            break
        }
    }
    
    # Check for completion checklist
    $completionChecklist = @(
        "All stories marked complete",
        "Integration tests passing",
        "Documentation updated",
        "Code reviewed and merged",
        "Retrospective completed"
    )
    
    $checklistComplete = $true
    foreach ($item in $completionChecklist) {
        if ($content -notmatch "\[x\].*$item" -and $content -notmatch "✅.*$item") {
            $checklistComplete = $false
            break
        }
    }
    
    return @{
        IsComplete = $isComplete -and $checklistComplete
        Reason = if ($isComplete -and $checklistComplete) { "Complete" } else { "Incomplete checklist or status" }
        HasCompletionStatus = $isComplete
        HasCompleteChecklist = $checklistComplete
    }
}

# Function to check if story is marked complete
function Test-StoryCompletion {
    param(
        [string]$FilePath
    )
    
    if (-not (Test-Path $FilePath)) {
        return @{ IsComplete = $false; Reason = "File not found" }
    }
    
    $content = Get-Content $FilePath -Raw
    
    # Check for completion indicators
    $completionIndicators = @(
        "Status.*Complete",
        "Status.*✅",
        "Status.*🟢",
        "Story marked complete"
    )
    
    $isComplete = $false
    foreach ($indicator in $completionIndicators) {
        if ($content -match $indicator) {
            $isComplete = $true
            break
        }
    }
    
    # Check for completion checklist
    $completionChecklist = @(
        "All acceptance criteria met",
        "All tests passing",
        "Code reviewed and approved",
        "Documentation updated",
        "Deployment completed"
    )
    
    $checklistComplete = $true
    foreach ($item in $completionChecklist) {
        if ($content -notmatch "\[x\].*$item" -and $content -notmatch "✅.*$item") {
            $checklistComplete = $false
            break
        }
    }
    
    # Check for evidence
    $evidenceIndicators = @(
        "Test results attached",
        "Code review completed",
        "Evidence attached",
        "Screenshots",
        "Test coverage"
    )
    
    $hasEvidence = $false
    foreach ($indicator in $evidenceIndicators) {
        if ($content -match $indicator) {
            $hasEvidence = $true
            break
        }
    }
    
    return @{
        IsComplete = $isComplete -and $checklistComplete -and $hasEvidence
        Reason = if ($isComplete -and $checklistComplete -and $hasEvidence) { "Complete" } else { "Missing completion status, checklist, or evidence" }
        HasCompletionStatus = $isComplete
        HasCompleteChecklist = $checklistComplete
        HasEvidence = $hasEvidence
    }
}

Write-Host "🔍 Validating Post-Delivery Gate Requirements..." -ForegroundColor Cyan
Write-Host "Project Root: $ProjectRoot" -ForegroundColor Gray
Write-Host "Output File: $OutputFile" -ForegroundColor Gray
Write-Host ""

# Check 1: Epic completion
Write-Host "📋 Checking epic completion..." -ForegroundColor Yellow

foreach ($epic in $ExpectedEpics) {
    $epicFile = Join-Path $EpicsPath $epic.File
    $completion = Test-EpicCompletion -FilePath $epicFile
    
    $ValidationResults.EpicCompletion[$epic.Id] = $completion
    
    if ($completion.IsComplete) {
        Add-CheckResult -Name "Epic: $($epic.Id)" -Status "PASS" -Message "Epic marked complete with evidence: $($epic.Title)"
    } else {
        Add-CheckResult -Name "Epic: $($epic.Id)" -Status "FAIL" -Message "Epic not marked complete: $($epic.Title) - $($completion.Reason)"
    }
}

# Check 2: Story completion
Write-Host "📝 Checking story completion..." -ForegroundColor Yellow

foreach ($epicId in $ExpectedStories.Keys) {
    $epicStories = $ExpectedStories[$epicId]
    $epicDir = Join-Path $StoriesPath "epic-$($epicId.Substring(1))"
    
    foreach ($story in $epicStories) {
        $storyFile = Join-Path $epicDir $story.File
        $completion = Test-StoryCompletion -FilePath $storyFile
        
        $storyKey = "$epicId.$($story.Id)"
        $ValidationResults.StoryCompletion[$storyKey] = $completion
        
        if ($completion.IsComplete) {
            Add-CheckResult -Name "Story: $storyKey" -Status "PASS" -Message "Story marked complete with evidence: $($story.Title)"
        } else {
            Add-CheckResult -Name "Story: $storyKey" -Status "FAIL" -Message "Story not marked complete: $($story.Title) - $($completion.Reason)"
        }
    }
}

# Check 3: Progress tracking completion
Write-Host "📊 Checking progress tracking completion..." -ForegroundColor Yellow

$progressFiles = @(
    "epic-completion-tracker.md",
    "requirements-matrix.md",
    "milestone-dashboard.md"
)

foreach ($progressFile in $progressFiles) {
    $filePath = Join-Path $ProgressPath $progressFile
    
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Check if progress is marked complete
        if ($content -match "Complete.*100%" -or $content -match "All.*complete" -or $content -match "Project.*complete") {
            Add-CheckResult -Name "Progress: $progressFile" -Status "PASS" -Message "Progress tracking shows completion: $progressFile"
        } else {
            Add-CheckResult -Name "Progress: $progressFile" -Status "WARNING" -Message "Progress tracking may not show completion: $progressFile"
        }
    } else {
        Add-CheckResult -Name "Progress: $progressFile" -Status "FAIL" -Message "Missing progress tracking file: $progressFile"
    }
}

# Check 4: Sprint retrospectives
Write-Host "🏃 Checking sprint retrospectives..." -ForegroundColor Yellow

$sprintFiles = Get-ChildItem -Path $SprintsPath -Filter "*retrospective*.md" -ErrorAction SilentlyContinue
if ($sprintFiles.Count -gt 0) {
    $completedRetros = 0
    foreach ($sprintFile in $sprintFiles) {
        $content = Get-Content $sprintFile.FullName -Raw
        if ($content -match "Sprint.*complete" -or $content -match "Retrospective.*complete" -or $content -match "Action Items.*documented") {
            $completedRetros++
        }
    }
    
    if ($completedRetros -eq $sprintFiles.Count) {
        Add-CheckResult -Name "Sprint Retrospectives" -Status "PASS" -Message "All sprint retrospectives completed: $completedRetros/$($sprintFiles.Count)"
    } else {
        Add-CheckResult -Name "Sprint Retrospectives" -Status "WARNING" -Message "Some sprint retrospectives may be incomplete: $completedRetros/$($sprintFiles.Count)"
    }
} else {
    Add-CheckResult -Name "Sprint Retrospectives" -Status "WARNING" -Message "No sprint retrospective files found"
}

# Check 5: Final documentation
Write-Host "📖 Checking final documentation..." -ForegroundColor Yellow

$documentationChecks = @(
    @{ Name = "API Documentation"; Path = "docs\reference\api"; Pattern = "*.md" },
    @{ Name = "User Guide"; Path = "docs\user-guide"; Pattern = "*.md" },
    @{ Name = "Architecture Documentation"; Path = "docs\architecture"; Pattern = "*.md" },
    @{ Name = "Deployment Guide"; Path = "docs\deployment"; Pattern = "*.md" }
)

foreach ($docCheck in $documentationChecks) {
    $docPath = Join-Path $ProjectRoot $docCheck.Path
    if (Test-Path $docPath) {
        $docFiles = Get-ChildItem -Path $docPath -Filter $docCheck.Pattern -ErrorAction SilentlyContinue
        if ($docFiles.Count -gt 0) {
            Add-CheckResult -Name "Documentation: $($docCheck.Name)" -Status "PASS" -Message "Documentation exists: $($docCheck.Name) ($($docFiles.Count) files)"
        } else {
            Add-CheckResult -Name "Documentation: $($docCheck.Name)" -Status "WARNING" -Message "Documentation directory exists but no files found: $($docCheck.Name)"
        }
    } else {
        Add-CheckResult -Name "Documentation: $($docCheck.Name)" -Status "WARNING" -Message "Documentation directory not found: $($docCheck.Name)"
    }
}

# Check 6: Test coverage and results
Write-Host "🧪 Checking test coverage and results..." -ForegroundColor Yellow

$testPaths = @(
    "src\test\IndFusion.Analyzer.Tests\TestResults",
    "src\test\IndFusion.Mcp.Tests\TestResults",
    "TestResults"
)

$hasTestResults = $false
foreach ($testPath in $testPaths) {
    $fullPath = Join-Path $ProjectRoot $testPath
    if (Test-Path $fullPath) {
        $testFiles = Get-ChildItem -Path $fullPath -Filter "*.trx" -ErrorAction SilentlyContinue
        if ($testFiles.Count -gt 0) {
            $hasTestResults = $true
            break
        }
    }
}

if ($hasTestResults) {
    Add-CheckResult -Name "Test Results" -Status "PASS" -Message "Test results found in repository"
} else {
    Add-CheckResult -Name "Test Results" -Status "WARNING" -Message "No test result files found. Consider running tests and committing results."
}

# Generate summary
Write-Host ""
Write-Host "📋 Validation Summary:" -ForegroundColor Cyan
Write-Host "  Total Checks: $($ValidationResults.Summary.TotalChecks)" -ForegroundColor White
Write-Host "  Passed: $($ValidationResults.Summary.PassedChecks)" -ForegroundColor Green
Write-Host "  Failed: $($ValidationResults.Summary.FailedChecks)" -ForegroundColor Red
Write-Host "  Warnings: $($ValidationResults.Summary.WarningChecks)" -ForegroundColor Yellow

# Epic completion summary
Write-Host ""
Write-Host "📊 Epic Completion Summary:" -ForegroundColor Cyan
foreach ($epicId in $ValidationResults.EpicCompletion.Keys) {
    $completion = $ValidationResults.EpicCompletion[$epicId]
    $status = if ($completion.IsComplete) { "✅ Complete" } else { "❌ Incomplete" }
    Write-Host "  $epicId : $status" -ForegroundColor $(if ($completion.IsComplete) { "Green" } else { "Red" })
}

# Story completion summary
Write-Host ""
Write-Host "📝 Story Completion Summary:" -ForegroundColor Cyan
$totalStories = $ValidationResults.StoryCompletion.Count
$completedStories = ($ValidationResults.StoryCompletion.Values | Where-Object { $_.IsComplete }).Count
Write-Host "  Completed: $completedStories/$totalStories stories" -ForegroundColor $(if ($completedStories -eq $totalStories) { "Green" } else { "Yellow" })

if ($ValidationResults.Status -eq "PASS") {
    Write-Host ""
    Write-Host "✅ Post-Delivery Gate: PASSED" -ForegroundColor Green
    Write-Host "   Delivery is complete and ready for handover!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "❌ Post-Delivery Gate: FAILED" -ForegroundColor Red
    Write-Host "   Delivery cannot be considered complete until all requirements are met." -ForegroundColor Red
    
    if ($ValidationResults.Errors.Count -gt 0) {
        Write-Host ""
        Write-Host "🚨 Critical Issues:" -ForegroundColor Red
        foreach ($error in $ValidationResults.Errors) {
            Write-Host "   • $error" -ForegroundColor Red
        }
    }
    
    if ($ValidationResults.Warnings.Count -gt 0) {
        Write-Host ""
        Write-Host "⚠️  Warnings:" -ForegroundColor Yellow
        foreach ($warning in $ValidationResults.Warnings) {
            Write-Host "   • $warning" -ForegroundColor Yellow
        }
    }
}

# Save validation results
$ValidationResults | ConvertTo-Json -Depth 10 | Out-File -FilePath $OutputFile -Encoding UTF8

Write-Host ""
Write-Host "📄 Validation report saved to: $OutputFile" -ForegroundColor Gray

# Exit with appropriate code
if ($ValidationResults.Status -eq "PASS") {
    exit 0
} else {
    exit 1
}

