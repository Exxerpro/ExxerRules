# Agentic Execution Framework

## Problem Statement

Developers are prone to going off-rails when they read instructions and start coding without:
1. **Verifying against real code** before implementing
2. **Checking existing patterns** in the codebase
3. **Validating assumptions** against actual implementation
4. **Stopping to verify** before proceeding to next steps

## Solution: Grounded Agentic Histories

### 🎯 **Core Principles**

1. **Code-First Validation**: Every step must verify against existing code
2. **Pattern Recognition**: Identify existing patterns before creating new ones
3. **Incremental Verification**: Stop and validate at each checkpoint
4. **Real-World Constraints**: Ground all decisions in actual codebase capabilities
5. **Failure Detection**: Built-in mechanisms to detect when going off-rails

### 🔍 **Grounded Execution Pattern**

Each history follows this pattern:

```
1. EXPLORE → 2. ANALYZE → 3. PLAN → 4. IMPLEMENT → 5. VERIFY → 6. VALIDATE
```

#### **EXPLORE Phase**
- Search existing codebase for related implementations
- Identify existing patterns and conventions
- Map current state vs. desired state
- Document findings in execution log

#### **ANALYZE Phase**
- Analyze existing code patterns
- Identify dependencies and constraints
- Validate assumptions against real code
- Create grounded implementation plan

#### **PLAN Phase**
- Create detailed implementation plan based on real code
- Define verification checkpoints
- Identify potential failure points
- Set up monitoring and validation

#### **IMPLEMENT Phase**
- Implement following existing patterns
- Use existing abstractions where possible
- Follow established conventions
- Document decisions and rationale

#### **VERIFY Phase**
- Verify implementation against existing code
- Run tests and validation scripts
- Check for regressions
- Validate against requirements

#### **VALIDATE Phase**
- Validate against acceptance criteria
- Run integration tests
- Check performance and quality metrics
- Document lessons learned

## Enhanced History Template

### **History X: [Feature Name]**

#### **Context & Grounding**
- **Current State**: [What exists in codebase now]
- **Target State**: [What we want to achieve]
- **Constraints**: [Real constraints from existing code]
- **Dependencies**: [Actual dependencies from codebase analysis]

#### **EXPLORE Phase - Code Discovery**
```bash
# Mandatory exploration commands
find src/ -name "*.cs" -exec grep -l "related_pattern" {} \;
grep -r "existing_implementation" src/
codebase_search "How does X work in the current codebase?"
```

**Deliverables**:
- [ ] Code exploration report in `docs/execution/HistoryX-Exploration.md`
- [ ] Existing pattern analysis
- [ ] Current implementation gaps
- [ ] Dependency mapping

#### **ANALYZE Phase - Pattern Analysis**
```bash
# Analyze existing patterns
grep -r "class.*Service" src/ | head -20
grep -r "interface.*Service" src/ | head -20
codebase_search "What are the existing service patterns?"
```

**Deliverables**:
- [ ] Pattern analysis report
- [ ] Existing abstraction mapping
- [ ] Convention documentation
- [ ] Implementation strategy

#### **PLAN Phase - Grounded Planning**
**Deliverables**:
- [ ] Detailed implementation plan based on real code
- [ ] Verification checkpoint definitions
- [ ] Test strategy aligned with existing patterns
- [ ] Risk assessment and mitigation

#### **IMPLEMENT Phase - Pattern-Following Implementation**
**Checkpoints**:
- [ ] **Checkpoint 1**: Verify new code follows existing patterns
- [ ] **Checkpoint 2**: Validate against existing abstractions
- [ ] **Checkpoint 3**: Check for regressions
- [ ] **Checkpoint 4**: Validate integration points

#### **VERIFY Phase - Real-World Validation**
```bash
# Mandatory verification commands
dotnet build IndFusion.sln -c Release
dotnet test src/test/IndFusion.Analyzer.Tests/ -c Release
./src/scripts/Validate-Implementation.ps1 -History X
```

**Deliverables**:
- [ ] Build verification report
- [ ] Test execution report
- [ ] Performance validation
- [ ] Integration validation

#### **VALIDATE Phase - Acceptance Validation**
**Deliverables**:
- [ ] Acceptance criteria validation
- [ ] End-to-end testing
- [ ] Documentation updates
- [ ] Lessons learned document

#### **Exit Criteria**
- [ ] All exploration findings documented
- [ ] Implementation follows existing patterns
- [ ] All verification checkpoints passed
- [ ] Integration tests pass
- [ ] Performance within acceptable limits
- [ ] Documentation updated

## Grounding Mechanisms

### 🔍 **Code-First Validation Scripts**

```powershell
# src/scripts/Validate-Implementation.ps1
param(
    [Parameter(Mandatory=$true)]
    [string]$History
)

Write-Host "Validating History $History implementation..." -ForegroundColor Green

# 1. Check if implementation follows existing patterns
Write-Host "1. Checking pattern compliance..." -ForegroundColor Yellow
$patternViolations = Get-PatternViolations -Path "src/" -History $History
if ($patternViolations.Count -gt 0) {
    Write-Error "Pattern violations detected: $($patternViolations.Count)"
    $patternViolations | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    exit 1
}

# 2. Verify against existing abstractions
Write-Host "2. Verifying abstraction compliance..." -ForegroundColor Yellow
$abstractionViolations = Get-AbstractionViolations -Path "src/" -History $History
if ($abstractionViolations.Count -gt 0) {
    Write-Error "Abstraction violations detected: $($abstractionViolations.Count)"
    exit 1
}

# 3. Check for regressions
Write-Host "3. Checking for regressions..." -ForegroundColor Yellow
$regressions = Get-Regressions -Path "src/" -History $History
if ($regressions.Count -gt 0) {
    Write-Error "Regressions detected: $($regressions.Count)"
    exit 1
}

# 4. Validate integration points
Write-Host "4. Validating integration points..." -ForegroundColor Yellow
$integrationIssues = Get-IntegrationIssues -Path "src/" -History $History
if ($integrationIssues.Count -gt 0) {
    Write-Error "Integration issues detected: $($integrationIssues.Count)"
    exit 1
}

Write-Host "All validations passed!" -ForegroundColor Green
```

### 📊 **Execution Monitoring**

```powershell
# src/scripts/Monitor-Execution.ps1
param(
    [Parameter(Mandatory=$true)]
    [string]$History,
    [Parameter(Mandatory=$true)]
    [string]$Phase
)

$logFile = "docs/execution/History$History-$Phase.log"
$startTime = Get-Date

Write-Host "Starting $Phase for History $History..." -ForegroundColor Green
Write-Host "Logging to: $logFile" -ForegroundColor Yellow

# Log execution details
@"
Execution Log - History $History - Phase $Phase
Started: $startTime
User: $env:USERNAME
Working Directory: $(Get-Location)
"@ | Out-File -FilePath $logFile -Append

# Monitor for common off-rails indicators
$offRailsIndicators = @(
    "Creating new patterns without analyzing existing ones",
    "Implementing without running existing tests",
    "Adding dependencies without checking existing ones",
    "Creating new abstractions without understanding existing ones"
)

# Set up monitoring
$monitoringJob = Start-Job -ScriptBlock {
    param($logFile, $indicators)
    while ($true) {
        $content = Get-Content $logFile -Tail 10
        foreach ($indicator in $indicators) {
            if ($content -match $indicator) {
                Write-Warning "Potential off-rails indicator detected: $indicator"
            }
        }
        Start-Sleep 30
    }
} -ArgumentList $logFile, $offRailsIndicators

# Return monitoring job for cleanup
return $monitoringJob
```

### 🎯 **Checkpoint Validation**

```powershell
# src/scripts/Validate-Checkpoint.ps1
param(
    [Parameter(Mandatory=$true)]
    [string]$History,
    [Parameter(Mandatory=$true)]
    [string]$Checkpoint,
    [Parameter(Mandatory=$true)]
    [string]$ValidationType
)

$checkpointFile = "docs/execution/History$History-Checkpoint$Checkpoint.md"

switch ($ValidationType) {
    "PatternCompliance" {
        $violations = Get-PatternViolations -Path "src/" -History $History
        if ($violations.Count -eq 0) {
            Write-Host "✅ Pattern compliance validated" -ForegroundColor Green
            "✅ Pattern compliance validated at $(Get-Date)" | Out-File -FilePath $checkpointFile -Append
        } else {
            Write-Error "❌ Pattern violations detected: $($violations.Count)"
            "❌ Pattern violations detected: $($violations.Count) at $(Get-Date)" | Out-File -FilePath $checkpointFile -Append
            exit 1
        }
    }
    "AbstractionCompliance" {
        $violations = Get-AbstractionViolations -Path "src/" -History $History
        if ($violations.Count -eq 0) {
            Write-Host "✅ Abstraction compliance validated" -ForegroundColor Green
            "✅ Abstraction compliance validated at $(Get-Date)" | Out-File -FilePath $checkpointFile -Append
        } else {
            Write-Error "❌ Abstraction violations detected: $($violations.Count)"
            exit 1
        }
    }
    "IntegrationValidation" {
        $issues = Get-IntegrationIssues -Path "src/" -History $History
        if ($issues.Count -eq 0) {
            Write-Host "✅ Integration validation passed" -ForegroundColor Green
            "✅ Integration validation passed at $(Get-Date)" | Out-File -FilePath $checkpointFile -Append
        } else {
            Write-Error "❌ Integration issues detected: $($issues.Count)"
            exit 1
        }
    }
}
```

## Enhanced History Examples

### **History 2: Semantic RAG Fabric Foundations (Epic E1) - Enhanced**

#### **Context & Grounding**
- **Current State**: 
  - Existing MCP server in `src/code/IndFusion.Mcp.Server/`
  - Current analyzers in `src/code/IndFusion.Analyzer/`
  - Existing CLI tools in `src/code/IndFusion.Tools.Cli/`
- **Target State**: Add RAG capabilities with Qdrant, Neo4j, and Ollama integration
- **Constraints**: Must follow existing hexagonal architecture patterns
- **Dependencies**: Existing service registration patterns, configuration patterns

#### **EXPLORE Phase - Code Discovery**
```bash
# Mandatory exploration commands
find src/ -name "*.cs" -exec grep -l "Service" {} \; | head -20
grep -r "interface.*Service" src/ | head -20
grep -r "class.*Adapter" src/ | head -20
codebase_search "How are services registered in the current codebase?"
codebase_search "What are the existing configuration patterns?"
codebase_search "How are external services integrated?"
```

**Deliverables**:
- [ ] **Code exploration report** in `docs/execution/History2-Exploration.md`
  - Existing service patterns analysis
  - Configuration pattern analysis
  - Adapter pattern analysis
  - Dependency injection patterns
- [ ] **Pattern inventory**:
  - Service registration patterns
  - Configuration patterns
  - Adapter patterns
  - Testing patterns

#### **ANALYZE Phase - Pattern Analysis**
```bash
# Analyze existing patterns
grep -r "AddScoped.*Service" src/
grep -r "IOptions" src/ | head -10
grep -r "Configuration" src/ | head -10
codebase_search "What are the existing service registration patterns?"
codebase_search "How are external services configured?"
```

**Deliverables**:
- [ ] **Pattern analysis report**:
  - Service registration pattern analysis
  - Configuration pattern analysis
  - Adapter pattern analysis
  - Testing pattern analysis
- [ ] **Convention documentation**:
  - Naming conventions
  - File organization conventions
  - Interface design conventions
  - Testing conventions

#### **PLAN Phase - Grounded Planning**
**Deliverables**:
- [ ] **Implementation plan** based on existing patterns:
  - Service registration following existing patterns
  - Configuration following existing patterns
  - Adapter implementation following existing patterns
  - Testing following existing patterns
- [ ] **Verification checkpoints**:
  - Checkpoint 1: Service registration follows existing patterns
  - Checkpoint 2: Configuration follows existing patterns
  - Checkpoint 3: Adapters follow existing patterns
  - Checkpoint 4: Tests follow existing patterns
- [ ] **Risk assessment**:
  - Integration risks
  - Performance risks
  - Maintenance risks

#### **IMPLEMENT Phase - Pattern-Following Implementation**
**Checkpoints**:
- [ ] **Checkpoint 1**: Verify service registration follows existing patterns
  ```bash
  ./src/scripts/Validate-Checkpoint.ps1 -History 2 -Checkpoint 1 -ValidationType PatternCompliance
  ```
- [ ] **Checkpoint 2**: Verify configuration follows existing patterns
  ```bash
  ./src/scripts/Validate-Checkpoint.ps1 -History 2 -Checkpoint 2 -ValidationType AbstractionCompliance
  ```
- [ ] **Checkpoint 3**: Verify adapters follow existing patterns
  ```bash
  ./src/scripts/Validate-Checkpoint.ps1 -History 2 -Checkpoint 3 -ValidationType PatternCompliance
  ```
- [ ] **Checkpoint 4**: Verify tests follow existing patterns
  ```bash
  ./src/scripts/Validate-Checkpoint.ps1 -History 2 -Checkpoint 4 -ValidationType IntegrationValidation
  ```

#### **VERIFY Phase - Real-World Validation**
```bash
# Mandatory verification commands
dotnet build IndFusion.sln -c Release
dotnet test src/test/IndFusion.Analyzer.Tests/ -c Release
./src/scripts/Validate-Implementation.ps1 -History 2
```

**Deliverables**:
- [ ] **Build verification report**
- [ ] **Test execution report**
- [ ] **Performance validation**
- [ ] **Integration validation**

#### **VALIDATE Phase - Acceptance Validation**
**Deliverables**:
- [ ] **Acceptance criteria validation**
- [ ] **End-to-end testing**
- [ ] **Documentation updates**
- [ ] **Lessons learned document**

#### **Exit Criteria**
- [ ] All exploration findings documented
- [ ] Implementation follows existing patterns
- [ ] All verification checkpoints passed
- [ ] Integration tests pass
- [ ] Performance within acceptable limits
- [ ] Documentation updated

## Implementation Guidelines

### 🚫 **Anti-Patterns to Avoid**

1. **Starting implementation without exploration**
2. **Creating new patterns without analyzing existing ones**
3. **Implementing without running existing tests**
4. **Adding dependencies without checking existing ones**
5. **Creating new abstractions without understanding existing ones**
6. **Skipping verification checkpoints**
7. **Not documenting decisions and rationale**

### ✅ **Best Practices**

1. **Always start with exploration**
2. **Follow existing patterns**
3. **Validate at each checkpoint**
4. **Document decisions and rationale**
5. **Run tests frequently**
6. **Check for regressions**
7. **Validate integration points**

### 🔧 **Tooling Support**

1. **Validation scripts** for each checkpoint
2. **Monitoring scripts** for off-rails detection
3. **Pattern analysis tools** for existing code
4. **Integration validation tools** for new code
5. **Documentation templates** for execution logs

This framework ensures that developers stay grounded in the real codebase and follow established patterns, preventing them from going off-rails during implementation.

---

**Last Updated**: 2024-01-XX  
**Updated By**: PM Agent

