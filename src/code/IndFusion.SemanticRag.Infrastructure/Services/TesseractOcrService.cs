using IndFusion.SemanticRag.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Tesseract;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Tesseract-based implementation of the OCR service.
/// </summary>
public class TesseractOcrService : IOcrService, IDisposable
{
    private readonly ILogger<TesseractOcrService> _logger;
    private readonly TesseractEngine _tesseractEngine;

    /// <summary>
    /// Initializes a new instance of the <see cref="TesseractOcrService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="dataPath">Path to Tesseract data files. Defaults to "./tessdata".</param>
    /// <param name="language">OCR language. Defaults to "eng".</param>
    public TesseractOcrService(
        ILogger<TesseractOcrService> logger,
        string? dataPath = null,
        string language = "eng")
    {
        _logger = logger;
        dataPath ??= @"./tessdata";
        
        try
        {
            _tesseractEngine = new TesseractEngine(dataPath, language, EngineMode.Default);
            _logger.LogDebug("Tesseract OCR engine initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Tesseract OCR engine with data path: {DataPath}", dataPath);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> ExtractTextAsync(byte[] imageBytes, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return string.Empty;
        }

        try
        {
            await Task.CompletedTask.ConfigureAwait(false);
            
            using var img = Pix.LoadFromMemory(imageBytes);
            using var page = _tesseractEngine.Process(img);
            
            var text = page.GetText();
            _logger.LogDebug("Extracted {CharacterCount} characters from image using OCR", text.Length);
            
            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from image using OCR");
            return string.Empty;
        }
    }

    /// <summary>
    /// Disposes of resources.
    /// </summary>
    public void Dispose()
    {
        _tesseractEngine?.Dispose();
        GC.SuppressFinalize(this);
    }
}

