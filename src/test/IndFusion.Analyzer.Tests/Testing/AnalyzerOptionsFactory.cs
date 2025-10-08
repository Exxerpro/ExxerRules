using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzer.Tests.Testing;

/// <summary>
/// Factory helpers to create AnalyzerOptions and AnalyzerConfigOptions for analyzer tests.
/// </summary>
public static class AnalyzerOptionsFactory
{
	public static AnalyzerOptions Create(params (string key, string value)[] globalOptions)
	{
		var provider = new InMemoryAnalyzerConfigOptionsProvider(globalOptions);
		return new AnalyzerOptions([], provider);
	}

	private sealed class InMemoryAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
	{
		private readonly AnalyzerConfigOptions _global;

		public InMemoryAnalyzerConfigOptionsProvider((string key, string value)[] options)
		{
			_global = new InMemoryAnalyzerConfigOptions(options);
		}

		public override AnalyzerConfigOptions GlobalOptions => _global;

		public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _global;

		public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _global;
	}

	private sealed class InMemoryAnalyzerConfigOptions : AnalyzerConfigOptions
	{
		private readonly Dictionary<string, string> _values;

		public InMemoryAnalyzerConfigOptions((string key, string value)[] options)
		{
			_values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (var (key, value) in options)
			{
				_values[key] = value;
			}
		}

		public override bool TryGetValue(string key, out string value) => _values.TryGetValue(key, out value!);
	}
}


