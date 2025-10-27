using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;
using SortDirection = IndFusion.SemanticRag.Domain.Services.SortDirection;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Services;

/// <summary>
/// Unit tests for the semantic search service.
/// </summary>
public class SemanticSearchServiceTests
{
    private readonly ISemanticSearchService _searchService;
    private readonly ILogger<ISemanticSearchService> _logger;

    public SemanticSearchServiceTests()
    {
        _searchService = Substitute.For<ISemanticSearchService>();
        _logger = Substitute.For<ILogger<ISemanticSearchService>>();
    }

    public class SearchAsyncTests
    {
        [Fact]
        public async Task Should_ReturnSearchResponse_When_ValidQueryProvided()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var query = "test query";
            var options = new SemanticSearchOptions();
            var expectedResponse = CreateTestSearchResponse();

            searchService.SearchAsync(query, options, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticSearchResponse>.Success(expectedResponse));

            // Act
            var result = await searchService.SearchAsync(query, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedResponse);
            result.Value.Query.ShouldBe(query);
            result.Value.HasResults.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_ReturnFailure_When_SearchFails()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var query = "test query";
            var options = new SemanticSearchOptions();
            var errorMessage = "Search failed";

            searchService.SearchAsync(query, options, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticSearchResponse>.WithFailure(errorMessage));

            // Act
            var result = await searchService.SearchAsync(query, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticSearchResponse CreateTestSearchResponse()
        {
            var results = new[]
            {
                CreateTestSearchResult("doc-1", 0.9f),
                CreateTestSearchResult("doc-2", 0.8f)
            };

            return new SemanticSearchResponse(
                results,
                TotalCount: 2,
                Query: "test query",
                ProcessingTimeMs: 150,
                Context: null,
                Suggestions: new[] { "test suggestion" });
        }

        private static SemanticSearchResult CreateTestSearchResult(string documentId, float score)
        {
            var document = new SemanticDocument(
                documentId,
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);

            return new SemanticSearchResult(document, score, Array.Empty<string>());
        }
    }

    public class HybridSearchAsyncTests
    {
        [Fact]
        public async Task Should_ReturnHybridSearchResponse_When_ValidQueryProvided()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var query = "test query";
            var options = new SemanticSearchOptions();
            var expectedResponse = CreateTestSearchResponse();

            searchService.HybridSearchAsync(query, options, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticSearchResponse>.Success(expectedResponse));

            // Act
            var result = await searchService.HybridSearchAsync(query, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedResponse);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_HybridSearchFails()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var query = "test query";
            var options = new SemanticSearchOptions();
            var errorMessage = "Hybrid search failed";

            searchService.HybridSearchAsync(query, options, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticSearchResponse>.WithFailure(errorMessage));

            // Act
            var result = await searchService.HybridSearchAsync(query, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticSearchResponse CreateTestSearchResponse()
        {
            var results = new[]
            {
                CreateTestSearchResult("doc-1", 0.9f),
                CreateTestSearchResult("doc-2", 0.8f)
            };

            return new SemanticSearchResponse(
                results,
                TotalCount: 2,
                Query: "test query",
                ProcessingTimeMs: 200,
                Context: null);
        }

        private static SemanticSearchResult CreateTestSearchResult(string documentId, float score)
        {
            var document = new SemanticDocument(
                documentId,
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);

            return new SemanticSearchResult(document, score, Array.Empty<string>());
        }
    }

    public class FindSimilarDocumentsAsyncTests
    {
        [Fact]
        public async Task Should_ReturnSimilarDocuments_When_ValidDocumentIdProvided()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var documentId = "doc-1";
            var limit = 5;
            var expectedResults = new[]
            {
                CreateTestSearchResult("doc-2", 0.9f),
                CreateTestSearchResult("doc-3", 0.8f)
            };

            searchService.FindSimilarDocumentsAsync(documentId, limit, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticSearchResult>>.Success(expectedResults));

            // Act
            var result = await searchService.FindSimilarDocumentsAsync(documentId, limit, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBe(expectedResults);
            result.Value.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_FindingSimilarDocumentsFails()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var documentId = "doc-1";
            var errorMessage = "Similarity search failed";

            searchService.FindSimilarDocumentsAsync(documentId, 5, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticSearchResult>>.WithFailure(errorMessage));

            // Act
            var result = await searchService.FindSimilarDocumentsAsync(documentId, 5, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticSearchResult CreateTestSearchResult(string documentId, float score)
        {
            var document = new SemanticDocument(
                documentId,
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);

            return new SemanticSearchResult(document, score, Array.Empty<string>());
        }
    }

    public class FacetedSearchAsyncTests
    {
        [Fact]
        public async Task Should_ReturnFacetedSearchResponse_When_ValidQueryAndFacetsProvided()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var query = "test query";
            var facets = new[]
            {
                new SearchFacet("type", new[] { "code", "document" }),
                new SearchFacet("language", new[] { "csharp", "javascript" })
            };
            var options = new SemanticSearchOptions();
            var expectedResponse = CreateTestFacetedSearchResponse();

            searchService.FacetedSearchAsync(query, facets, options, Arg.Any<CancellationToken>())
                .Returns(Result<FacetedSearchResponse>.Success(expectedResponse));

            // Act
            var result = await searchService.FacetedSearchAsync(query, facets, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedResponse);
            result.Value.Query.ShouldBe(query);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_FacetedSearchFails()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var query = "test query";
            var facets = new[] { new SearchFacet("type", new[] { "code" }) };
            var options = new SemanticSearchOptions();
            var errorMessage = "Faceted search failed";

            searchService.FacetedSearchAsync(query, facets, options, Arg.Any<CancellationToken>())
                .Returns(Result<FacetedSearchResponse>.WithFailure(errorMessage));

            // Act
            var result = await searchService.FacetedSearchAsync(query, facets, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static FacetedSearchResponse CreateTestFacetedSearchResponse()
        {
            var results = new[]
            {
                CreateTestSearchResult("doc-1", 0.9f)
            };

            var facets = new Dictionary<string, IReadOnlyList<FacetValue>>
            {
                ["type"] = new[] { new FacetValue("code", 5), new FacetValue("document", 3) },
                ["language"] = new[] { new FacetValue("csharp", 4), new FacetValue("javascript", 2) }
            };

            return new FacetedSearchResponse(
                results,
                facets,
                TotalCount: 1,
                Query: "test query",
                ProcessingTimeMs: 100);
        }

        private static SemanticSearchResult CreateTestSearchResult(string documentId, float score)
        {
            var document = new SemanticDocument(
                documentId,
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);

            return new SemanticSearchResult(document, score, Array.Empty<string>());
        }
    }

    public class GetSuggestionsAsyncTests
    {
        [Fact]
        public async Task Should_ReturnSuggestions_When_ValidPartialQueryProvided()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var partialQuery = "test";
            var limit = 10;
            var expectedSuggestions = new[] { "test query", "test document", "test code" };

            searchService.GetSuggestionsAsync(partialQuery, limit, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<string>>.Success(expectedSuggestions));

            // Act
            var result = await searchService.GetSuggestionsAsync(partialQuery, limit, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBe(expectedSuggestions);
            result.Value.Count.ShouldBe(3);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_GettingSuggestionsFails()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var partialQuery = "test";
            var errorMessage = "Suggestions retrieval failed";

            searchService.GetSuggestionsAsync(partialQuery, 10, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<string>>.WithFailure(errorMessage));

            // Act
            var result = await searchService.GetSuggestionsAsync(partialQuery, 10, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }
    }

    public class AdvancedSearchAsyncTests
    {
        [Fact]
        public async Task Should_ReturnAdvancedSearchResponse_When_ValidRequestProvided()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var request = CreateTestAdvancedSearchRequest();
            var expectedResponse = CreateTestAdvancedSearchResponse();

            searchService.AdvancedSearchAsync(request, Arg.Any<CancellationToken>())
                .Returns(Result<AdvancedSearchResponse>.Success(expectedResponse));

            // Act
            var result = await searchService.AdvancedSearchAsync(request, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedResponse);
            result.Value.Query.ShouldBe(request.Query);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_AdvancedSearchFails()
        {
            // Arrange
            var searchService = Substitute.For<ISemanticSearchService>();
            var request = CreateTestAdvancedSearchRequest();
            var errorMessage = "Advanced search failed";

            searchService.AdvancedSearchAsync(request, Arg.Any<CancellationToken>())
                .Returns(Result<AdvancedSearchResponse>.WithFailure(errorMessage));

            // Act
            var result = await searchService.AdvancedSearchAsync(request, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static AdvancedSearchRequest CreateTestAdvancedSearchRequest()
        {
            var filters = new[]
            {
                new SearchFilter("type", FilterOperator.Equals, "code")
            };
            var sorting = new[]
            {
                new SearchSort("score", SortDirection.Descending)
            };
            var pagination = new PaginationOptions(1, 10);
            var options = new SemanticSearchOptions();

            return new AdvancedSearchRequest("test query", filters, sorting, pagination, options);
        }

        private static AdvancedSearchResponse CreateTestAdvancedSearchResponse()
        {
            var results = new[]
            {
                CreateTestSearchResult("doc-1", 0.9f)
            };

            var pageInfo = new PageInfo(1, 10, 1, 1);

            return new AdvancedSearchResponse(
                results,
                TotalCount: 1,
                pageInfo,
                Query: "test query",
                ProcessingTimeMs: 100);
        }

        private static SemanticSearchResult CreateTestSearchResult(string documentId, float score)
        {
            var document = new SemanticDocument(
                documentId,
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);

            return new SemanticSearchResult(document, score, Array.Empty<string>());
        }
    }
}

/// <summary>
/// Unit tests for semantic search options validation.
/// </summary>
public class SemanticSearchOptionsTests
{
    [Fact]
    public void Should_ValidateSuccessfully_When_ValidParametersProvided()
    {
        // Arrange
        var options = new SemanticSearchOptions(
            MaxResults: 10,
            SimilarityThreshold: 0.7f,
            IncludeHighlights: true,
            IncludeMetadata: true,
            EnableQueryExpansion: true,
            EnableContextRetrieval: true,
            SortBy: SearchSortBy.Relevance,
            Filters: null);

        // Act
        var result = options.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_ValidateFailure_When_MaxResultsIsZeroOrNegative(int maxResults)
    {
        // Arrange
        var options = new SemanticSearchOptions(MaxResults: maxResults);

        // Act
        var result = options.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("MaxResults must be greater than 0");
    }

    [Theory]
    [InlineData(-0.1f)]
    [InlineData(1.1f)]
    public void Should_ValidateFailure_When_SimilarityThresholdIsOutOfRange(float threshold)
    {
        // Arrange
        var options = new SemanticSearchOptions(SimilarityThreshold: threshold);

        // Act
        var result = options.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("SimilarityThreshold must be between 0.0 and 1.0");
    }
}

/// <summary>
/// Unit tests for search facet validation.
/// </summary>
public class SearchFacetTests
{
    [Fact]
    public void Should_ValidateSuccessfully_When_ValidParametersProvided()
    {
        // Arrange
        var facet = new SearchFacet("type", new[] { "code", "document" }, FacetOperator.Or);

        // Act
        var result = facet.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_ValidateFailure_When_NameIsNullOrEmpty(string name)
    {
        // Arrange
        var facet = new SearchFacet(name, new[] { "code" }, FacetOperator.Or);

        // Act
        var result = facet.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Facet name cannot be null or empty");
    }

    [Fact]
    public void Should_ValidateFailure_When_ValuesIsEmpty()
    {
        // Arrange
        var facet = new SearchFacet("type", Array.Empty<string>(), FacetOperator.Or);

        // Act
        var result = facet.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("At least one facet value must be specified");
    }
}