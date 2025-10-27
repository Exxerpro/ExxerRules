#Requires -Version 7.0

<#
.SYNOPSIS
    Validates that all pre-development gate requirements are met for the IndFusion Semantic RAG initiative.

.DESCRIPTION
    This script enforces the mandatory tracking requirements for the pre-development gate.
    It validates that all epics, stories, and sprint planning documents are created and properly structured.

.PARAMETER ProjectRoot
    The root directory of the IndFusion project. Defaults to the script's parent directory.

.PARAMETER OutputFile
    Path to write the validation report. Defaults to a timestamped file in the project root.

.EXAMPLE
    .\Validate-PreDevelopmentGate.ps1

.EXAMPLE
    .\Validate-PreDevelopmentGate.ps1 -ProjectRoot "C:\Projects\IndFusion" -OutputFile "C:\Reports\pre-dev-validation.json"
#>

param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$OutputFile = (Join-Path $ProjectRoot "docs\operations\due-diligence\pre-development-gate-validation-$(Get-Date -Format 'yyyyMMdd-HHmmss').json")
)

# Ensure output directory exists
$OutputDir = Split-Path -Parent $OutputFile
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

# Initialize validation results
$ValidationResults = @{
    Timestamp = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"
    Gate = "Pre-Development"
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
}

# Define required paths
$ProjectManagementRoot = Join-Path $ProjectRoot "docs\project-management"
$EpicsPath = Join-Path $ProjectManagementRoot "epics"
$StoriesPath = Join-Path $ProjectManagementRoot "stories"
$SprintsPath = Join-Path $ProjectManagementRoot "sprints"
$ProgressPath = Join-Path $ProjectManagementRoot "progress"
$TemplatesPath = Join-Path $ProjectManagementRoot "templates"

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

# Define expected progress files
$ExpectedProgressFiles = @(
    "epic-completion-tracker.md",
    "requirements-matrix.md",
    "milestone-dashboard.md"
)

# Define expected template files
$ExpectedTemplateFiles = @(
    "epic-template.md",
    "story-template.md",
    "sprint-template.md"
)

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

# Function to validate markdown file structure
function Test-MarkdownFile {
    param(
        [string]$FilePath,
        [string[]]$RequiredSections
    )
    
    if (-not (Test-Path $FilePath)) {
        return $false
    }
    
    $content = Get-Content $FilePath -Raw
    $missingSections = @()
    
    foreach ($section in $RequiredSections) {
        if ($content -notmatch $section) {
            $missingSections += $section
        }
    }
    
    return $missingSections.Count -eq 0
}

Write-Host "🔍 Validating Pre-Development Gate Requirements..." -ForegroundColor Cyan
Write-Host "Project Root: $ProjectRoot" -ForegroundColor Gray
Write-Host "Output File: $OutputFile" -ForegroundColor Gray
Write-Host ""

# Check 1: Project management directory structure
Write-Host "📁 Checking project management directory structure..." -ForegroundColor Yellow

$requiredDirs = @($ProjectManagementRoot, $EpicsPath, $StoriesPath, $SprintsPath, $ProgressPath, $TemplatesPath)
foreach ($dir in $requiredDirs) {
    if (Test-Path $dir) {
        Add-CheckResult -Name "Directory: $(Split-Path $dir -Leaf)" -Status "PASS" -Message "Directory exists: $dir"
    } else {
        Add-CheckResult -Name "Directory: $(Split-Path $dir -Leaf)" -Status "FAIL" -Message "Missing required directory: $dir"
    }
}

# Check 2: Epic files
Write-Host "📋 Checking epic files..." -ForegroundColor Yellow

foreach ($epic in $ExpectedEpics) {
    $epicFile = Join-Path $EpicsPath $epic.File
    $epicDir = Join-Path $StoriesPath "epic-$($epic.Id.Substring(1))"
    
    if (Test-Path $epicFile) {
        $requiredSections = @("Epic Information", "Epic Goal", "Success Criteria", "Stories", "Dependencies", "Risks and Mitigations")
        if (Test-MarkdownFile -FilePath $epicFile -RequiredSections $requiredSections) {
            Add-CheckResult -Name "Epic: $($epic.Id)" -Status "PASS" -Message "Epic file exists with required sections: $($epic.File)"
        } else {
            Add-CheckResult -Name "Epic: $($epic.Id)" -Status "WARNING" -Message "Epic file exists but missing required sections: $($epic.File)"
        }
    } else {
        Add-CheckResult -Name "Epic: $($epic.Id)" -Status "FAIL" -Message "Missing epic file: $($epic.File)"
    }
    
    # Check epic story directory
    if (Test-Path $epicDir) {
        Add-CheckResult -Name "Epic Stories Dir: $($epic.Id)" -Status "PASS" -Message "Epic stories directory exists: $epicDir"
    } else {
        Add-CheckResult -Name "Epic Stories Dir: $($epic.Id)" -Status "FAIL" -Message "Missing epic stories directory: $epicDir"
    }
}

# Check 3: Story files
Write-Host "📝 Checking story files..." -ForegroundColor Yellow

foreach ($epicId in $ExpectedStories.Keys) {
    $epicStories = $ExpectedStories[$epicId]
    $epicDir = Join-Path $StoriesPath "epic-$($epicId.Substring(1))"
    
    foreach ($story in $epicStories) {
        $storyFile = Join-Path $epicDir $story.File
        
        if (Test-Path $storyFile) {
            $requiredSections = @("Story Information", "User Story", "Acceptance Criteria", "Technical Requirements", "Definition of Done")
            if (Test-MarkdownFile -FilePath $storyFile -RequiredSections $requiredSections) {
                Add-CheckResult -Name "Story: $($epicId).$($story.Id)" -Status "PASS" -Message "Story file exists with required sections: $($story.File)"
            } else {
                Add-CheckResult -Name "Story: $($epicId).$($story.Id)" -Status "WARNING" -Message "Story file exists but missing required sections: $($story.File)"
            }
        } else {
            Add-CheckResult -Name "Story: $($epicId).$($story.Id)" -Status "FAIL" -Message "Missing story file: $($story.File)"
        }
    }
}

# Check 4: Progress tracking files
Write-Host "📊 Checking progress tracking files..." -ForegroundColor Yellow

foreach ($progressFile in $ExpectedProgressFiles) {
    $filePath = Join-Path $ProgressPath $progressFile
    
    if (Test-Path $filePath) {
        Add-CheckResult -Name "Progress File: $progressFile" -Status "PASS" -Message "Progress tracking file exists: $progressFile"
    } else {
        Add-CheckResult -Name "Progress File: $progressFile" -Status "FAIL" -Message "Missing progress tracking file: $progressFile"
    }
}

# Check 5: Template files
Write-Host "📄 Checking template files..." -ForegroundColor Yellow

foreach ($templateFile in $ExpectedTemplateFiles) {
    $filePath = Join-Path $TemplatesPath $templateFile
    
    if (Test-Path $filePath) {
        Add-CheckResult -Name "Template File: $templateFile" -Status "PASS" -Message "Template file exists: $templateFile"
    } else {
        Add-CheckResult -Name "Template File: $templateFile" -Status "FAIL" -Message "Missing template file: $templateFile"
    }
}

# Check 6: Sprint planning files
Write-Host "🏃 Checking sprint planning files..." -ForegroundColor Yellow

$sprintFiles = Get-ChildItem -Path $SprintsPath -Filter "*.md" -ErrorAction SilentlyContinue
if ($sprintFiles.Count -gt 0) {
    Add-CheckResult -Name "Sprint Planning" -Status "PASS" -Message "Sprint planning files exist: $($sprintFiles.Count) files"
} else {
    Add-CheckResult -Name "Sprint Planning" -Status "WARNING" -Message "No sprint planning files found. Consider creating initial sprint planning documents."
}

# Check 7: README file
Write-Host "📖 Checking project management README..." -ForegroundColor Yellow

$readmeFile = Join-Path $ProjectManagementRoot "README.md"
if (Test-Path $readmeFile) {
    Add-CheckResult -Name "Project Management README" -Status "PASS" -Message "Project management README exists"
} else {
    Add-CheckResult -Name "Project Management README" -Status "FAIL" -Message "Missing project management README file"
}

# Generate summary
Write-Host ""
Write-Host "📋 Validation Summary:" -ForegroundColor Cyan
Write-Host "  Total Checks: $($ValidationResults.Summary.TotalChecks)" -ForegroundColor White
Write-Host "  Passed: $($ValidationResults.Summary.PassedChecks)" -ForegroundColor Green
Write-Host "  Failed: $($ValidationResults.Summary.FailedChecks)" -ForegroundColor Red
Write-Host "  Warnings: $($ValidationResults.Summary.WarningChecks)" -ForegroundColor Yellow

if ($ValidationResults.Status -eq "PASS") {
    Write-Host ""
    Write-Host "✅ Pre-Development Gate: PASSED" -ForegroundColor Green
    Write-Host "   Development can proceed!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "❌ Pre-Development Gate: FAILED" -ForegroundColor Red
    Write-Host "   Development cannot begin until all requirements are met." -ForegroundColor Red
    
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

