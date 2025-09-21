// =====================================================================
// Global Using Statements for IndFusion.Mcp.Tests
// =====================================================================

// =====================================================================
// Testing Frameworks
// =====================================================================
global using Xunit;
global using NSubstitute;

// =====================================================================
// Microsoft Roslyn & Code Analysis
// =====================================================================
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Editing;
global using Microsoft.CodeAnalysis.Formatting;
global using Microsoft.CodeAnalysis.Text;

// =====================================================================
// System & JSON
// =====================================================================
global using System.Text.Json;

// =====================================================================
// Protocol & Communication
// =====================================================================
global using ModelContextProtocol;

// =====================================================================
// IndFusion.Mcp Core Libraries
// =====================================================================
global using IndFusion.Mcp.Core.Move;
global using IndFusion.Mcp.Core.SyntaxRewriters;
global using IndFusion.Mcp.Core.SyntaxWalkers;
global using IndFusion.Mcp.Core.Tools;