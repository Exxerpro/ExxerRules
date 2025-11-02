namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for Optical Character Recognition (OCR) operations.
/// </summary>
public interface IOcrService
{
    /// <summary>
    /// Extracts text from image bytes using OCR.
    /// </summary>
    /// <param name="imageBytes">The image bytes to process.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The extracted text, or empty string if OCR fails.</returns>
    Task<string> ExtractTextAsync(byte[] imageBytes, CancellationToken cancellationToken = default);
}

