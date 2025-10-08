namespace Integration.Tests
{
    /// <summary>
    /// Represents the SpResolution.
    /// </summary>
    public class SpResolution : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
    {
        private readonly IServiceProvider serviceProvider;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fixture">The test host fixture.</param>

        public SpResolution(Integration.Tests.Infrastructure.TestHostFixture fixture)
        {
            this.serviceProvider = fixture.Services;
        }
        /// <summary>
        /// Executes TheProjectTest_ShouldHaveAccessToTheServiceProvider operation.
        /// </summary>

        [Fact]
        public void TheProjectTest_ShouldHaveAccessToTheServiceProvider()
        {
            // Arrange

            //Act

            // Assert

            serviceProvider.ShouldNotBeNull("Because we need access to all the services");
        }
    }
}
