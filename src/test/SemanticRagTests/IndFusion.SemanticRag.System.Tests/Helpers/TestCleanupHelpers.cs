namespace IndFusion.SemanticRag.System.Tests.Helpers;

/// <summary>
/// Helper methods for cleaning up test data between tests.
/// Provides isolation by clearing collections and database data.
/// </summary>
public static class TestCleanupHelpers
{
    /// <summary>
    /// Clears all points from a Qdrant collection, or deletes the collection if it exists.
    /// </summary>
    /// <param name="client">The Qdrant client instance.</param>
    /// <param name="collectionName">The name of the collection to clear.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    public static async Task ClearQdrantCollection(
        QdrantClient client,
        string collectionName,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));

        if (string.IsNullOrWhiteSpace(collectionName))
            return;

        try
        {
            // Check if collection exists
            var collections = await client.ListCollectionsAsync(cancellationToken: cancellationToken);
            if (collections == null || !collections.Contains(collectionName))
                return;

            // Delete the entire collection (more efficient than clearing points)
            await client.DeleteCollectionAsync(collectionName, cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            // Ignore errors during cleanup - collection may not exist or already deleted
        }
    }

    /// <summary>
    /// Clears all nodes and relationships from a Neo4j database.
    /// </summary>
    /// <param name="driver">The Neo4j driver instance.</param>
    /// <param name="database">The database name to clear. Defaults to "neo4j".</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    public static async Task ClearNeo4jDatabase(
        IDriver driver,
        string database = "neo4j",
        CancellationToken cancellationToken = default)
    {
        if (driver == null)
            throw new ArgumentNullException(nameof(driver));

        if (string.IsNullOrWhiteSpace(database))
            database = "neo4j";

        try
        {
            await using var session = driver.AsyncSession(o => o.WithDatabase(database));
            var result = await session.RunAsync("MATCH (n) DETACH DELETE n");
            await result.ConsumeAsync();
        }
        catch (Exception)
        {
            // Ignore errors during cleanup - database may be empty or not exist
        }
    }

    /// <summary>
    /// Clears all nodes and relationships matching a specific label from Neo4j.
    /// </summary>
    /// <param name="driver">The Neo4j driver instance.</param>
    /// <param name="label">The node label to clear.</param>
    /// <param name="database">The database name. Defaults to "neo4j".</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    public static async Task ClearNeo4jNodesByLabel(
        IDriver driver,
        string label,
        string database = "neo4j",
        CancellationToken cancellationToken = default)
    {
        if (driver == null)
            throw new ArgumentNullException(nameof(driver));

        if (string.IsNullOrWhiteSpace(label))
            return;

        try
        {
            await using var session = driver.AsyncSession(o => o.WithDatabase(database));
            var cypher = $"MATCH (n:{label}) DETACH DELETE n";
            var result = await session.RunAsync(cypher);
            await result.ConsumeAsync();
        }
        catch (Exception)
        {
            // Ignore errors during cleanup
        }
    }
}