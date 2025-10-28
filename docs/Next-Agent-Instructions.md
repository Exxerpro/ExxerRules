# Next Agent Instructions - Sprint 3 Ready

## 🎯 **Current Status**
- ✅ **Sprint 1**: TDD Foundation Complete (78/78 tests passing)
- ✅ **Sprint 2**: MCP Tooling Surface Complete (193/194 tests passing) 
- 🔄 **Sprint 3**: Ready to begin - Graph RAG Layer implementation

## 🚀 **Immediate Next Steps**

### 1. **Activate Plan Mode First**
```
Use sequential thinking to analyze requirements and create systematic implementation plan
```

### 2. **Sprint 3 Focus Areas**
- **Pattern Graph Query**: Implement `pattern_graph_query` MCP tool
- **Pattern Suggest**: Implement `pattern_suggest` MCP tool  
- **Graph RAG Layer**: Symbol/pattern graph builder with caching
- **Confidence Scoring**: Include confidence + provenance in suggestions

## 📋 **TDD Approach (Critical)**
- **Red-Green-Refactor**: Write failing tests first, make them pass, then refactor
- **ITDD-First**: Define interfaces and contracts before implementation
- **Real Implementation**: Replace mocks with actual functionality
- **Behavioral Tests**: Focus on "what the system does" not "how it does it"

## 🛠️ **Key Success Patterns from Sprint 2**

### **Sequential Thinking**
- Use for complex problem analysis
- Break down complex tasks into manageable steps
- Analyze SUT (System Under Test) and expectations systematically

### **ILogger Debugging**
- Add `Meziantou logger" statements for debugging
- Meziantou.Extensions.Logging.Xunit.v3
- _logger = XUnitLogger.CreateLogger(_testOutputHelper);
- Trace execution flow and inspect intermediate states
- Keep important debug statements after fixing issues

### **Test Infrastructure**
- Use `TestUtilities.GetSolutionPath()` for minimal test solutions
- Maintain proper test fixture management
- Use `Xunit.TestContext.Current.CancellationToken` consistently
- Ensure test data matches test expectations

### **Cache Management**
- Implement proper cache cleanup and invalidation
- Use clean state for reliable testing
- Delete `.ExxerFactor-Mcp` directory when cache issues occur

## 🏗️ **Architecture Compliance**
- **Hexagonal Architecture**: Maintain ports & adapters pattern
- **Result<T> Pattern**: Use for all error handling (no exceptions in normal flow)
- **MCP Tool Patterns**: Follow established patterns from Sprint 2
- **Roslyn Integration**: Use existing analyzer infrastructure

## 📁 **Key Files to Reference**
- `ExxerRules/docs/Unified-Semantic-RAG-Standards-Initiative.md` - Complete project documentation
- `ExxerRules/src/code/IndFusion.Mcp.Core/Tools/LintRunTool.cs` - MCP tool pattern example
- `ExxerRules/src/code/IndFusion.Mcp.Core/Services/LintingService.cs` - Service implementation pattern
- `ExxerRules/src/test/IndFusion.Mcp.Tests/Tools/TestUtilities.cs` - Test infrastructure utilities

## ⚡ **Quick Start Commands**
```bash
# Run tests to verify current state
dotnet test test/IndFusion.Mcp.Tests/IndFusion.Mcp.Tests.csproj --verbosity normal

# Check test results (should show 193/194 passing)
# Only 1 unrelated MoveMultipleMethodsTool test should be failing
```

## 🎯 **Success Criteria for Sprint 3**
- Implement `pattern_graph_query` MCP tool with comprehensive tests
- Implement `pattern_suggest` MCP tool with confidence scoring
- Maintain 99%+ test success rate
- Follow established TDD and architecture patterns
- Document all public APIs with XML comments

## 🚨 **Critical Reminders**
1. **Never add features while tests are failing**
2. **Always use sequential thinking for complex problems**
3. **Focus on making tests pass before adding new functionality**
4. **Use real implementations, not mocks**
5. **Maintain clean code with warnings-as-errors policy**

## 📞 **If Stuck**
1. Use sequential thinking to analyze the problem
2. Add console debugging to trace execution
3. Check test data matches test expectations
4. Verify cache is clean if seeing unexpected results
5. Reference Sprint 2 lessons learned in the main document

---

**Ready to proceed with Sprint 3! The foundation is solid and all patterns are established.**
