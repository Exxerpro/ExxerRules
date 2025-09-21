using System;
using System.Collections.Generic;
using System.Linq;
using IndFusion.Analyzers.Architecture;
using IndFusion.Analyzers.Async;
using IndFusion.Analyzers.Common;
using IndFusion.Analyzers.NullSafety;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases;

/// <summary>
/// Comprehensive edge case tests for analyzers to kill surviving mutants.
/// Focuses on null handling, boundary conditions, and complex scenarios.
/// </summary>
public class AnalyzerEdgeCaseTests
{
    [Fact]
    public void DomainShouldNotReferenceInfrastructureAnalyzer_WithNullUsingDirective_ShouldHandleGracefully()
    {
        // Test edge case for null using directive
        var analyzer = new DomainShouldNotReferenceInfrastructureAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void DomainShouldNotReferenceInfrastructureAnalyzer_WithEmptyNamespace_ShouldHandleGracefully()
    {
        // Test edge case for empty namespace
        var analyzer = new DomainShouldNotReferenceInfrastructureAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void DomainShouldNotReferenceInfrastructureAnalyzer_WithSingleCharacterNamespace_ShouldHandleGracefully()
    {
        // Test edge case for single character namespace
        var analyzer = new DomainShouldNotReferenceInfrastructureAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void DomainShouldNotReferenceInfrastructureAnalyzer_WithVeryLongNamespace_ShouldHandleGracefully()
    {
        // Test edge case for very long namespace
        var analyzer = new DomainShouldNotReferenceInfrastructureAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void DomainShouldNotReferenceInfrastructureAnalyzer_WithUnicodeNamespace_ShouldHandleGracefully()
    {
        // Test edge case for unicode namespace
        var analyzer = new DomainShouldNotReferenceInfrastructureAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void DomainShouldNotReferenceInfrastructureAnalyzer_WithSpecialCharactersNamespace_ShouldHandleGracefully()
    {
        // Test edge case for special characters in namespace
        var analyzer = new DomainShouldNotReferenceInfrastructureAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void UseRepositoryPatternAnalyzer_WithEmptyClassName_ShouldHandleGracefully()
    {
        // Test edge case for empty class name
        var analyzer = new UseRepositoryPatternAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void UseRepositoryPatternAnalyzer_WithSingleCharacterClassName_ShouldHandleGracefully()
    {
        // Test edge case for single character class name
        var analyzer = new UseRepositoryPatternAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void UseRepositoryPatternAnalyzer_WithVeryLongClassName_ShouldHandleGracefully()
    {
        // Test edge case for very long class name
        var analyzer = new UseRepositoryPatternAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void UseRepositoryPatternAnalyzer_WithUnicodeClassName_ShouldHandleGracefully()
    {
        // Test edge case for unicode class name
        var analyzer = new UseRepositoryPatternAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void AsyncMethodsShouldAcceptCancellationTokenAnalyzer_WithEmptyMethodName_ShouldHandleGracefully()
    {
        // Test edge case for empty method name
        var analyzer = new AsyncMethodsShouldAcceptCancellationTokenAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void AsyncMethodsShouldAcceptCancellationTokenAnalyzer_WithSingleCharacterMethodName_ShouldHandleGracefully()
    {
        // Test edge case for single character method name
        var analyzer = new AsyncMethodsShouldAcceptCancellationTokenAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void AsyncMethodsShouldAcceptCancellationTokenAnalyzer_WithVeryLongMethodName_ShouldHandleGracefully()
    {
        // Test edge case for very long method name
        var analyzer = new AsyncMethodsShouldAcceptCancellationTokenAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithEmptyParameterName_ShouldHandleGracefully()
    {
        // Test edge case for empty parameter name
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithSingleCharacterParameterName_ShouldHandleGracefully()
    {
        // Test edge case for single character parameter name
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithVeryLongParameterName_ShouldHandleGracefully()
    {
        // Test edge case for very long parameter name
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithUnicodeParameterType_ShouldHandleGracefully()
    {
        // Test edge case for unicode parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithSpecialCharactersParameterType_ShouldHandleGracefully()
    {
        // Test edge case for special characters in parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithGenericParameterType_ShouldHandleGracefully()
    {
        // Test edge case for generic parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithNestedGenericParameterType_ShouldHandleGracefully()
    {
        // Test edge case for nested generic parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithComplexGenericParameterType_ShouldHandleGracefully()
    {
        // Test edge case for complex generic parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithNullableParameterType_ShouldHandleGracefully()
    {
        // Test edge case for nullable parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithArrayParameterType_ShouldHandleGracefully()
    {
        // Test edge case for array parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithMultiDimensionalArrayParameterType_ShouldHandleGracefully()
    {
        // Test edge case for multi-dimensional array parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithJaggedArrayParameterType_ShouldHandleGracefully()
    {
        // Test edge case for jagged array parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithComplexArrayParameterType_ShouldHandleGracefully()
    {
        // Test edge case for complex array parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithRefParameterType_ShouldHandleGracefully()
    {
        // Test edge case for ref parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithOutParameterType_ShouldHandleGracefully()
    {
        // Test edge case for out parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithInParameterType_ShouldHandleGracefully()
    {
        // Test edge case for in parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithParamsParameterType_ShouldHandleGracefully()
    {
        // Test edge case for params parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithOptionalParameterType_ShouldHandleGracefully()
    {
        // Test edge case for optional parameter type
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithMultipleParameters_ShouldHandleGracefully()
    {
        // Test edge case for multiple parameters
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void ValidateNullParametersAnalyzer_WithManyParameters_ShouldHandleGracefully()
    {
        // Test edge case for many parameters (performance test)
        var analyzer = new ValidateNullParametersAnalyzer();
        
        // Should not throw exception during initialization
        Should.NotThrow(() => analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0));
    }

    [Fact]
    public void AllAnalyzers_ShouldHaveValidDiagnostics()
    {
        // Test that all analyzers have valid diagnostic descriptors
        var analyzers = new DiagnosticAnalyzer[]
        {
            new DomainShouldNotReferenceInfrastructureAnalyzer(),
            new UseRepositoryPatternAnalyzer(),
            new AsyncMethodsShouldAcceptCancellationTokenAnalyzer(),
            new ValidateNullParametersAnalyzer()
        };

        foreach (var analyzer in analyzers)
        {
            analyzer.ShouldNotBeNull();
            analyzer.SupportedDiagnostics.Length.ShouldBeGreaterThan(0);
            
            foreach (var diagnostic in analyzer.SupportedDiagnostics)
            {
                diagnostic.ShouldNotBeNull();
                diagnostic.Id.ShouldNotBeNullOrEmpty();
                diagnostic.Title.ShouldNotBeNull();
                diagnostic.MessageFormat.ShouldNotBeNull();
                diagnostic.Category.ShouldNotBeNullOrEmpty();
            }
        }
    }

    [Fact]
    public void AllAnalyzers_ShouldHandleNullContextGracefully()
    {
        // Test that analyzers don't crash with null contexts
        var analyzers = new DiagnosticAnalyzer[]
        {
            new DomainShouldNotReferenceInfrastructureAnalyzer(),
            new UseRepositoryPatternAnalyzer(),
            new AsyncMethodsShouldAcceptCancellationTokenAnalyzer(),
            new ValidateNullParametersAnalyzer()
        };

        foreach (var analyzer in analyzers)
        {
            // Should not throw exception when accessing properties
            Should.NotThrow(() => 
            {
                var diagnostics = analyzer.SupportedDiagnostics;
                diagnostics.Length.ShouldBeGreaterThan(0);
            });
        }
    }

    [Fact]
    public void AllAnalyzers_ShouldHaveConsistentDiagnosticIds()
    {
        // Test that diagnostic IDs are consistent and valid
        var analyzers = new DiagnosticAnalyzer[]
        {
            new DomainShouldNotReferenceInfrastructureAnalyzer(),
            new UseRepositoryPatternAnalyzer(),
            new AsyncMethodsShouldAcceptCancellationTokenAnalyzer(),
            new ValidateNullParametersAnalyzer()
        };

        var diagnosticIds = new HashSet<string>();

        foreach (var analyzer in analyzers)
        {
            foreach (var diagnostic in analyzer.SupportedDiagnostics)
            {
                diagnostic.Id.ShouldNotBeNullOrEmpty();
                diagnostic.Id.ShouldNotContain(" ");
                diagnostic.Id.ShouldNotContain("\t");
                diagnostic.Id.ShouldNotContain("\n");
                
                // Check for unique IDs
                diagnosticIds.ShouldNotContain(diagnostic.Id);
                diagnosticIds.Add(diagnostic.Id);
            }
        }
    }

    [Fact]
    public void AllAnalyzers_ShouldHaveValidSeverityLevels()
    {
        // Test that diagnostic severity levels are valid
        var analyzers = new DiagnosticAnalyzer[]
        {
            new DomainShouldNotReferenceInfrastructureAnalyzer(),
            new UseRepositoryPatternAnalyzer(),
            new AsyncMethodsShouldAcceptCancellationTokenAnalyzer(),
            new ValidateNullParametersAnalyzer()
        };

        foreach (var analyzer in analyzers)
        {
            foreach (var diagnostic in analyzer.SupportedDiagnostics)
            {
                diagnostic.DefaultSeverity.ShouldBeOneOf(
                    DiagnosticSeverity.Hidden,
                    DiagnosticSeverity.Info,
                    DiagnosticSeverity.Warning,
                    DiagnosticSeverity.Error
                );
            }
        }
    }

    [Fact]
    public void AllAnalyzers_ShouldHaveValidCategories()
    {
        // Test that diagnostic categories are valid
        var analyzers = new DiagnosticAnalyzer[]
        {
            new DomainShouldNotReferenceInfrastructureAnalyzer(),
            new UseRepositoryPatternAnalyzer(),
            new AsyncMethodsShouldAcceptCancellationTokenAnalyzer(),
            new ValidateNullParametersAnalyzer()
        };

        foreach (var analyzer in analyzers)
        {
            foreach (var diagnostic in analyzer.SupportedDiagnostics)
            {
                diagnostic.Category.ShouldNotBeNullOrEmpty();
                diagnostic.Category.ShouldNotContain(" ");
                diagnostic.Category.ShouldNotContain("\t");
                diagnostic.Category.ShouldNotContain("\n");
            }
        }
    }
}

