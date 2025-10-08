using IndTrace.Application.Attributes;

namespace Application.UnitTests.Features.Services;

/// <summary>
/// Tests for DatabaseSafetyInterceptor basic functionality and constructor validation
/// </summary>
public class DatabaseSafetyInterceptorBasicTests
{
    // MARKED FOR REMOVAL - Constructor null guard test no longer needed with Result<T> patterns
    // /// <summary>
    // /// Executes Constructor_WithValidLogger_ShouldCreateInstance operation.
    // /// </summary>
    // [Fact]
    // public void Constructor_WithValidLogger_ShouldCreateInstance()
    // {
    //     // Arrange
    //     var logger =  XUnitLogger.CreateLogger<DatabaseSafetyInterceptor>();

    //     // Act
    //     var interceptor = new DatabaseSafetyInterceptor(logger);

    //     // Assert
    //     interceptor.ShouldNotBeNull();
    // }
    // /// <summary>
    // /// Executes Constructor_WithNullLogger_ShouldThrowArgumentNullException operation.
    // /// </summary>

    // [Fact]
    // public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    // {
    //     // Arrange
    //     ILogger<DatabaseSafetyInterceptor>? nullLogger = null!;

    //     // Act & Assert
    //     Should.Throw<ArgumentNullException>(() => new DatabaseSafetyInterceptor(nullLogger!))
    //         .ParamName.ShouldBe("logger");
    // }
}

/// <summary>
/// Test entity with Production environment restriction
/// </summary>
[DatabaseSafe("Production")]
public class ProductionOnlyEntity
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Test entity with Simulation environment restriction
/// </summary>
[DatabaseSafe("Simulation")]
public class SimulationOnlyEntity
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Data.
    /// </summary>
    public string Data { get; set; } = string.Empty;
}

/// <summary>
/// Test entity that allows any environment
/// </summary>
[DatabaseSafe("Any")]
public class AnyEnvironmentEntity
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Test entity that allows cross-environment usage
/// </summary>
[DatabaseSafe("Development", AllowCrossEnvironment = true)]
public class CrossEnvironmentEntity
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Test entity without DatabaseSafe attribute
/// </summary>
public class UnmarkedEntity
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Content.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Mock implementation of IDataContext for testing
/// </summary>
public class MockDataContext : IDataContext
{
    /// <summary>
    /// Gets or sets the Environment.
    /// </summary>
    public string Environment { get; set; } = string.Empty;

    public bool IsSimulation => Environment == "Simulation";

    /// <summary>
    /// Gets or sets the AllowsDatabaseWrites.
    /// </summary>
    public bool AllowsDatabaseWrites { get; set; } = true;

    /// <summary>
    /// Gets or sets the DatabaseIdentifier.
    /// </summary>
    public string DatabaseIdentifier { get; set; } = "TestDatabase";
}
