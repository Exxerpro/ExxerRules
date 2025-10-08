namespace IndTrace.Domain.UnitTests.CommonTests;
/// <summary>
/// Represents the ValueObjectTests.
/// </summary>

public class ValueObjectTests
{
    /// <summary>
    /// Executes Equals_GivenDifferentValues_ShouldReturnFalse operation.
    /// </summary>
    [Fact]
    public void Equals_GivenDifferentValues_ShouldReturnFalse()
    {
        var point1 = new Point(1, 2);
        var point2 = new Point(2, 1);

        Assert.False(point1.Equals(point2));
    }
    /// <summary>
    /// Executes Equals_GivenMatchingValues_ShouldReturnTrue operation.
    /// </summary>

    [Fact]
    public void Equals_GivenMatchingValues_ShouldReturnTrue()
    {
        var point1 = new Point(1, 2);
        var point2 = new Point(1, 2);

        Assert.True(point1.Equals(point2));
    }

    private class Point : ValueObject
    {
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        public int Y { get; set; }

        private Point()
        { }
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return X;
            yield return Y;
        }
    }
}
