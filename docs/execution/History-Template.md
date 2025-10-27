# History Execution Template

## History X: [Feature Name]

### Context & Grounding
- **Current State**: [What exists in codebase now]
- **Target State**: [What we want to achieve]
- **Constraints**: [Real constraints from existing code]
- **Dependencies**: [Actual dependencies from codebase analysis]

### EXPLORE Phase - Code Discovery

#### Exploration Commands
```bash
# Mandatory exploration commands
find src/ -name "*.cs" -exec grep -l "related_pattern" {} \;
grep -r "existing_implementation" src/
codebase_search "How does X work in the current codebase?"
```

#### Findings
- [ ] **Existing Patterns**: [List of existing patterns found]
- [ ] **Current Implementation**: [Description of current implementation]
- [ ] **Gaps Identified**: [List of gaps between current and target state]
- [ ] **Dependencies Mapped**: [List of dependencies and their locations]

#### Deliverables
- [ ] Code exploration report in `docs/execution/HistoryX-Exploration.md`
- [ ] Existing pattern analysis
- [ ] Current implementation gaps
- [ ] Dependency mapping

### ANALYZE Phase - Pattern Analysis

#### Analysis Commands
```bash
# Analyze existing patterns
grep -r "class.*Service" src/ | head -20
grep -r "interface.*Service" src/ | head -20
codebase_search "What are the existing service patterns?"
```

#### Analysis Results
- [ ] **Service Patterns**: [Analysis of existing service patterns]
- [ ] **Interface Patterns**: [Analysis of existing interface patterns]
- [ ] **Configuration Patterns**: [Analysis of existing configuration patterns]
- [ ] **Testing Patterns**: [Analysis of existing testing patterns]

#### Deliverables
- [ ] Pattern analysis report
- [ ] Convention documentation
- [ ] Implementation strategy

### PLAN Phase - Grounded Planning

#### Implementation Plan
- [ ] **Service Registration**: [Plan for service registration following existing patterns]
- [ ] **Configuration**: [Plan for configuration following existing patterns]
- [ ] **Adapters**: [Plan for adapter implementation following existing patterns]
- [ ] **Testing**: [Plan for testing following existing patterns]

#### Verification Checkpoints
- [ ] **Checkpoint 1**: [Description and validation criteria]
- [ ] **Checkpoint 2**: [Description and validation criteria]
- [ ] **Checkpoint 3**: [Description and validation criteria]
- [ ] **Checkpoint 4**: [Description and validation criteria]

#### Risk Assessment
- [ ] **Integration Risks**: [List of integration risks and mitigations]
- [ ] **Performance Risks**: [List of performance risks and mitigations]
- [ ] **Maintenance Risks**: [List of maintenance risks and mitigations]

### IMPLEMENT Phase - Pattern-Following Implementation

#### Checkpoint 1: [Checkpoint Name]
- [ ] **Validation**: `./src/scripts/Validate-Checkpoint.ps1 -History X -Checkpoint 1 -ValidationType PatternCompliance`
- [ ] **Status**: [Pass/Fail]
- [ ] **Notes**: [Any notes or issues]

#### Checkpoint 2: [Checkpoint Name]
- [ ] **Validation**: `./src/scripts/Validate-Checkpoint.ps1 -History X -Checkpoint 2 -ValidationType AbstractionCompliance`
- [ ] **Status**: [Pass/Fail]
- [ ] **Notes**: [Any notes or issues]

#### Checkpoint 3: [Checkpoint Name]
- [ ] **Validation**: `./src/scripts/Validate-Checkpoint.ps1 -History X -Checkpoint 3 -ValidationType IntegrationValidation`
- [ ] **Status**: [Pass/Fail]
- [ ] **Notes**: [Any notes or issues]

#### Checkpoint 4: [Checkpoint Name]
- [ ] **Validation**: `./src/scripts/Validate-Checkpoint.ps1 -History X -Checkpoint 4 -ValidationType BuildValidation`
- [ ] **Status**: [Pass/Fail]
- [ ] **Notes**: [Any notes or issues]

### VERIFY Phase - Real-World Validation

#### Verification Commands
```bash
# Mandatory verification commands
dotnet build IndFusion.sln -c Release
dotnet test src/test/IndFusion.Analyzer.Tests/ -c Release
./src/scripts/Validate-Implementation.ps1 -History X
```

#### Verification Results
- [ ] **Build Verification**: [Results of build verification]
- [ ] **Test Execution**: [Results of test execution]
- [ ] **Performance Validation**: [Results of performance validation]
- [ ] **Integration Validation**: [Results of integration validation]

#### Deliverables
- [ ] Build verification report
- [ ] Test execution report
- [ ] Performance validation
- [ ] Integration validation

### VALIDATE Phase - Acceptance Validation

#### Acceptance Criteria Validation
- [ ] **Criteria 1**: [Description and validation result]
- [ ] **Criteria 2**: [Description and validation result]
- [ ] **Criteria 3**: [Description and validation result]
- [ ] **Criteria 4**: [Description and validation result]

#### End-to-End Testing
- [ ] **Test Scenario 1**: [Description and result]
- [ ] **Test Scenario 2**: [Description and result]
- [ ] **Test Scenario 3**: [Description and result]

#### Documentation Updates
- [ ] **API Documentation**: [Status of API documentation updates]
- [ ] **User Documentation**: [Status of user documentation updates]
- [ ] **Developer Documentation**: [Status of developer documentation updates]

#### Lessons Learned
- [ ] **What Worked Well**: [List of things that worked well]
- [ ] **What Could Be Improved**: [List of things that could be improved]
- [ ] **Recommendations**: [Recommendations for future implementations]

### Exit Criteria
- [ ] All exploration findings documented
- [ ] Implementation follows existing patterns
- [ ] All verification checkpoints passed
- [ ] Integration tests pass
- [ ] Performance within acceptable limits
- [ ] Documentation updated
- [ ] Lessons learned documented

### Execution Log
- **Started**: [Start date and time]
- **Completed**: [Completion date and time]
- **Duration**: [Total duration]
- **Executed By**: [Name of person/agent who executed]
- **Log File**: [Path to execution log file]

### Monitoring
- **Monitoring Started**: [Date and time monitoring started]
- **Monitoring Stopped**: [Date and time monitoring stopped]
- **Off-Rails Indicators**: [List of any off-rails indicators detected]
- **Mitigation Actions**: [Actions taken to address off-rails indicators]

