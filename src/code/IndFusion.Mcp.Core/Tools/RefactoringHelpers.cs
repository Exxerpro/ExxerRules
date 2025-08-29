using System.Text;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Caching.Memory;
using ModelContextProtocol;

namespace IndFusion.Mcp.Mcp.Core.Tools;

/// <summary>
/// Helper utilities for solution/file handling, Roslyn workspace access, caching,
/// and common transformations used by refactoring tools.
/// </summary>
public static class ExxerFactoringHelpers
{
    // MemoryCache is thread-safe and Solution objects from Roslyn are immutable.
    // This allows us to store and access Solution instances across threads
    // without additional locking or synchronization.
    /// <summary>Cache of loaded solutions keyed by solution path.</summary>
    public static MemoryCache SolutionCache = new(new MemoryCacheOptions());
    /// <summary>Cache of parsed syntax trees keyed by file path.</summary>
    public static MemoryCache SyntaxTreeCache = new(new MemoryCacheOptions());
    /// <summary>Cache of semantic models keyed by file path.</summary>
    public static MemoryCache ModelCache = new(new MemoryCacheOptions());

    /// <summary>
    /// Clears and recreates all in-memory caches for solutions, syntax trees and semantic models.
    /// </summary>
    public static void ClearAllCaches()
    {
        SolutionCache.Dispose();
        SolutionCache = new MemoryCache(new MemoryCacheOptions());
        SyntaxTreeCache.Dispose();
        SyntaxTreeCache = new MemoryCache(new MemoryCacheOptions());
        ModelCache.Dispose();
        ModelCache = new MemoryCache(new MemoryCacheOptions());
    }

    private static readonly Lazy<AdhocWorkspace> _workspace =
        new(() => new AdhocWorkspace());

    private static bool _msbuildRegistered;
    private static readonly object _msbuildLock = new();

    /// <summary>
    /// A shared <see cref="AdhocWorkspace"/> instance for formatting and analysis.
    /// </summary>
    public static AdhocWorkspace SharedWorkspace => _workspace.Value;

    private static void EnsureMsBuildRegistered()
    {
        if (_msbuildRegistered) return;
        lock (_msbuildLock)
        {
            if (_msbuildRegistered) return;
            MSBuildLocator.RegisterDefaults();
            _msbuildRegistered = true;
        }
    }

    /// <summary>
    /// Creates a new <see cref="MSBuildWorkspace"/> configured to load solutions and projects.
    /// </summary>
    /// <returns>A configured workspace instance.</returns>
    public static MSBuildWorkspace CreateWorkspace()
    {
        EnsureMsBuildRegistered();
        var host = MefHostServices.Create(MSBuildMefHostServices.DefaultAssemblies);
        var workspace = MSBuildWorkspace.Create(host);
        workspace.WorkspaceFailed += (_, e) =>
            Console.Error.WriteLine(e.Diagnostic.Message);
        return workspace;
    }

    /// <summary>
    /// Gets a cached solution or loads it from disk if not present.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the .sln file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The loaded solution instance.</returns>
    public static async Task<Solution> GetOrLoadSolution(
        string solutionPath,
        CancellationToken cancellationToken = default)
    {

        if (SolutionCache.TryGetValue(solutionPath, out Solution? cachedSolution))
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(solutionPath)!);
            return cachedSolution!;
        }
        using var workspace = CreateWorkspace();
        var solution = await workspace.OpenSolutionAsync(solutionPath, progress: null, cancellationToken);
        SolutionCache.Set(solutionPath, solution);
        Directory.SetCurrentDirectory(Path.GetDirectoryName(solutionPath)!);
        return solution;
    }

    // Solutions are immutable, so replacing the cached instance is safe even
    // when accessed concurrently by multiple threads.
    /// <summary>
    /// Updates the cached solution instance for the document's solution and triggers metrics refresh.
    /// </summary>
    /// <param name="updatedDocument">The updated Roslyn document.</param>
    public static void UpdateSolutionCache(Document updatedDocument)
    {
        var solutionPath = updatedDocument.Project.Solution.FilePath;
        if (!string.IsNullOrEmpty(solutionPath))
        {
            SolutionCache.Set(solutionPath!, updatedDocument.Project.Solution);
            if (!string.IsNullOrEmpty(updatedDocument.FilePath))
            {
                _ = MetricsProvider.RefreshFileMetrics(solutionPath!, updatedDocument.FilePath!);
            }
        }
    }

    /// <summary>
    /// Finds a document in a solution by its absolute file path.
    /// </summary>
    /// <param name="solution">The solution to search.</param>
    /// <param name="filePath">The absolute file path.</param>
    /// <returns>The matching document, or null if not found.</returns>
    public static Document? GetDocumentByPath(Solution solution, string filePath)
    {
        var normalizedPath = Path.GetFullPath(filePath);
        return solution.Projects
            .SelectMany(p => p.Documents)
            .FirstOrDefault(d => Path.GetFullPath(d.FilePath ?? "") == normalizedPath);
    }

    /// <summary>
    /// Parses a text range string in the format 'startLine:startColumn-endLine:endColumn'.
    /// </summary>
    /// <param name="range">The range string.</param>
    /// <param name="startLine">Parsed start line (1-based).</param>
    /// <param name="startColumn">Parsed start column (1-based).</param>
    /// <param name="endLine">Parsed end line (1-based).</param>
    /// <param name="endColumn">Parsed end column (1-based).</param>
    /// <returns>True if parsing succeeded; otherwise false.</returns>
    public static bool TryParseRange(string range, out int startLine, out int startColumn, out int endLine, out int endColumn)
    {
        startLine = startColumn = endLine = endColumn = 0;
        var parts = range.Split('-');
        if (parts.Length != 2) return false;
        var startParts = parts[0].Split(':');
        var endParts = parts[1].Split(':');
        if (startParts.Length != 2 || endParts.Length != 2) return false;
        return int.TryParse(startParts[0], out startLine) &&
               int.TryParse(startParts[1], out startColumn) &&
               int.TryParse(endParts[0], out endLine) &&
               int.TryParse(endParts[1], out endColumn);
    }

    /// <summary>
    /// Validates that a range is well-formed and within the bounds of the provided text.
    /// </summary>
    /// <param name="text">The source text.</param>
    /// <param name="startLine">Start line (1-based).</param>
    /// <param name="startColumn">Start column (1-based).</param>
    /// <param name="endLine">End line (1-based).</param>
    /// <param name="endColumn">End column (1-based).</param>
    /// <param name="error">Output error message when invalid.</param>
    /// <returns>True if the range is valid; otherwise false.</returns>
    public static bool ValidateRange(
        SourceText text,
        int startLine,
        int startColumn,
        int endLine,
        int endColumn,
        out string error)
    {
        error = string.Empty;
        if (startLine <= 0 || startColumn <= 0 || endLine <= 0 || endColumn <= 0)
        {
            error = "Error: Range values must be positive";
            return false;
        }
        if (startLine > endLine || (startLine == endLine && startColumn >= endColumn))
        {
            error = "Error: Range start must precede end";
            return false;
        }
        if (startLine > text.Lines.Count || endLine > text.Lines.Count)
        {
            error = "Error: Range exceeds file length";
            return false;
        }
        return true;
    }


    /// <summary>
    /// Applies a transformation to a single file on disk, preserving encoding and caches.
    /// </summary>
    /// <param name="filePath">Absolute file path to modify.</param>
    /// <param name="transform">Transformation that returns the new file text.</param>
    /// <param name="successMessage">Message to return upon success.</param>
    /// <returns>Success message or error message starting with "Error:".</returns>
    public static async Task<string> ApplySingleFileEdit(
        string filePath,
        Func<string, string> transform,
        string successMessage)
    {
        if (!File.Exists(filePath))
            throw new McpException($"Error: File {filePath} not found (current dir: {Directory.GetCurrentDirectory()})");

        var (sourceText, encoding) = await ReadFileWithEncodingAsync(filePath);
        var newText = transform(sourceText);

        if (newText.StartsWith("Error:"))
            return newText;

        await File.WriteAllTextAsync(filePath, newText, encoding);
        UpdateFileCaches(filePath, newText);
        return successMessage;
    }

    /// <summary>
    /// Finds a class declaration by name across all documents in the solution.
    /// </summary>
    /// <param name="solution">The solution to search.</param>
    /// <param name="className">The class identifier.</param>
    /// <param name="excludingFilePaths">Optional absolute paths to exclude.</param>
    /// <returns>The document containing the class, or null.</returns>
    public static async Task<Document?> FindClassInSolution(
        Solution solution,
        string className,
        params string[]? excludingFilePaths)
    {
        foreach (var doc in solution.Projects.SelectMany(p => p.Documents))
        {
            var docPath = doc.FilePath ?? string.Empty;
            if (excludingFilePaths != null && excludingFilePaths.Any(p => Path.GetFullPath(docPath) == Path.GetFullPath(p)))
                continue;

            var root = await doc.GetSyntaxRootAsync();
            if (root != null && root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .Any(c => c.Identifier.Text == className))
            {
                return doc;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds a type declaration (class, enum, delegate, etc.) by name across the solution.
    /// </summary>
    /// <param name="solution">The solution to search.</param>
    /// <param name="typeName">The type identifier.</param>
    /// <param name="excludingFilePaths">Optional absolute paths to exclude.</param>
    /// <returns>The document containing the type, or null.</returns>
    public static async Task<Document?> FindTypeInSolution(
        Solution solution,
        string typeName,
        params string[]? excludingFilePaths)
    {
        foreach (var doc in solution.Projects.SelectMany(p => p.Documents))
        {
            var docPath = doc.FilePath ?? string.Empty;
            if (excludingFilePaths != null && excludingFilePaths.Any(p => Path.GetFullPath(docPath) == Path.GetFullPath(p)))
                continue;

            var root = await doc.GetSyntaxRootAsync();
            if (root != null && root.DescendantNodes().Any(n =>
                    n is BaseTypeDeclarationSyntax bt && bt.Identifier.Text == typeName ||
                    n is EnumDeclarationSyntax en && en.Identifier.Text == typeName ||
                    n is DelegateDeclarationSyntax dd && dd.Identifier.Text == typeName))
            {
                return doc;
            }
        }

        return null;
    }

    /// <summary>
    /// Adds an existing source file as a document to the given project if not present.
    /// </summary>
    /// <param name="project">The target project.</param>
    /// <param name="filePath">Absolute path to the source file.</param>
    public static void AddDocumentToProject(Project project, string filePath)
    {
        if (project.Documents.Any(d =>
                Path.GetFullPath(d.FilePath ?? "") == Path.GetFullPath(filePath)))
            return;

        var text = SourceText.From(File.ReadAllText(filePath));
        var newDoc = project.AddDocument(Path.GetFileName(filePath), text, filePath: filePath);

        var solutionPath = project.Solution.FilePath;
        if (!string.IsNullOrEmpty(solutionPath))
        {
            SolutionCache.Set(solutionPath!, newDoc.Project.Solution);
        }
    }

    private static CSharpCompilation CreateCompilation(SyntaxTree tree)
    {
        var refs = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(p => MetadataReference.CreateFromFile(p));
        return CSharpCompilation.Create(
            "SingleFile",
            new[] { tree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    /// <summary>
    /// Retrieves a cached syntax tree for a file or parses it from disk if absent.
    /// </summary>
    /// <param name="filePath">Absolute path to the source file.</param>
    /// <returns>The syntax tree.</returns>
    public static async Task<SyntaxTree> GetOrParseSyntaxTreeAsync(string filePath)
    {
        if (SyntaxTreeCache.TryGetValue(filePath, out SyntaxTree? cached))
            return cached!;
        var (text, _) = await ReadFileWithEncodingAsync(filePath);
        var tree = CSharpSyntaxTree.ParseText(text);
        SyntaxTreeCache.Set(filePath, tree);
        return tree;
    }

    /// <summary>
    /// Retrieves or creates a semantic model for the specified file.
    /// </summary>
    /// <param name="filePath">Absolute path to the source file.</param>
    /// <returns>The semantic model.</returns>
    public static async Task<SemanticModel> GetOrCreateSemanticModelAsync(string filePath)
    {
        if (ModelCache.TryGetValue(filePath, out SemanticModel? cached))
            return cached!;
        var tree = await GetOrParseSyntaxTreeAsync(filePath);
        var compilation = CreateCompilation(tree);
        var model = compilation.GetSemanticModel(tree);
        ModelCache.Set(filePath, model);
        return model;
    }

    /// <summary>
    /// Updates in-memory caches after a file's contents have changed.
    /// </summary>
    /// <param name="filePath">Absolute path to the source file.</param>
    /// <param name="newText">New file contents.</param>
    public static void UpdateFileCaches(string filePath, string newText)
    {
        var tree = CSharpSyntaxTree.ParseText(newText);
        SyntaxTreeCache.Set(filePath, tree);
        var compilation = CreateCompilation(tree);
        var model = compilation.GetSemanticModel(tree);
        ModelCache.Set(filePath, model);
    }

    /// <summary>
    /// Reads a file and returns its text along with the detected encoding.
    /// </summary>
    /// <param name="filePath">Absolute path to the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the text and encoding.</returns>
    public static async Task<(string Text, Encoding Encoding)> ReadFileWithEncodingAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
        var encoding = DetectEncoding(bytes);
        var text = encoding.GetString(bytes);
        return (text, encoding);
    }

    /// <summary>
    /// Detects and returns the encoding of a file without loading it as text.
    /// </summary>
    /// <param name="filePath">Absolute path to the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detected encoding.</returns>
    public static async Task<Encoding> GetFileEncodingAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
        return DetectEncoding(bytes);
    }

    private static Encoding DetectEncoding(byte[] bytes)
    {
        if (bytes.Length >= 4)
        {
            if (bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF)
                return new UTF32Encoding(true, true);
            if (bytes[0] == 0xFF && bytes[1] == 0xFE && bytes[2] == 0x00 && bytes[3] == 0x00)
                return new UTF32Encoding(false, true);
        }
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return Encoding.UTF8;
        if (bytes.Length >= 2)
        {
            if (bytes[0] == 0xFE && bytes[1] == 0xFF)
                return Encoding.BigEndianUnicode;
            if (bytes[0] == 0xFF && bytes[1] == 0xFE)
                return Encoding.Unicode;
        }
        return Encoding.UTF8;
    }

    /// <summary>
    /// Writes text to a file with a specific encoding and updates caches.
    /// </summary>
    /// <param name="filePath">Absolute path to the file.</param>
    /// <param name="text">Text to write.</param>
    /// <param name="encoding">Encoding to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task WriteFileWithEncodingAsync(
        string filePath,
        string text,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        await File.WriteAllTextAsync(filePath, text, encoding, cancellationToken);
        UpdateFileCaches(filePath, text);
    }

    /// <summary>
    /// Executes a function using a Roslyn document when available or falls back to a single-file path.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file.</param>
    /// <param name="filePath">Absolute path to the C# file.</param>
    /// <param name="withSolution">Function to run when the document is found in the solution.</param>
    /// <param name="singleFile">Function to run when only a file path is available.</param>
    /// <returns>A message returned by the executed function.</returns>
    public static async Task<string> RunWithSolutionOrFile(
        string solutionPath,
        string filePath,
        Func<Document, Task<string>> withSolution,
        Func<string, Task<string>> singleFile)
    {
        var solution = await GetOrLoadSolution(solutionPath);
        var document = GetDocumentByPath(solution, filePath);
        if (document != null)
            return await withSolution(document);

        return await singleFile(filePath);
    }
}
