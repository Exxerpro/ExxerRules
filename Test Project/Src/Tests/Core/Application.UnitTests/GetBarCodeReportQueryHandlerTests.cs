using IndTrace.Application.BarCodes.Queries.GetBarCodeDetail;
using IndTrace.Application.BarCodes.Queries.DataLoaders;
using IndTrace.Application.BarCodes.Queries.Mappers;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Interfaces;
using System.Reflection.Metadata;

namespace Application.UnitTests
{
    /// <summary>
    /// Unit tests for GetBarCodeReportQueryHandler using repository pattern
    /// </summary>
    public class GetBarCodeReportQueryHandlerTests
    {
        /// <summary>
        /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
        /// </summary>
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange
            var dataLoader = Substitute.For<IBarCodeDetailDataLoader>();
            var mapper = Substitute.For<IBarCodeDetailMapper>();
            var barCodeResult = Substitute.For<IBarCodeResult>();
            var logger = XUnitLogger.CreateLogger<IndTrace.Application.BarCodes.Queries.GetBarCodeDetail.GetBarCodeReportQueryHandler>();

            // Act
            var handler = new IndTrace.Application.BarCodes.Queries.GetBarCodeDetail.GetBarCodeReportQueryHandler(
                dataLoader,
                mapper,
                barCodeResult,
                logger);

            // Assert
            handler.ShouldNotBeNull();
        }

        /// <summary>
        /// Executes Handler_ShouldNotBeNull_WhenCreated operation.
        /// </summary>

        [Fact]
        public void Handler_ShouldNotBeNull_WhenCreated()
        {
            // Arrange
            var dataLoader = Substitute.For<IBarCodeDetailDataLoader>();
            var mapper = Substitute.For<IBarCodeDetailMapper>();
            var barCodeResult = Substitute.For<IBarCodeResult>();
            var logger = XUnitLogger.CreateLogger<IndTrace.Application.BarCodes.Queries.GetBarCodeDetail.GetBarCodeReportQueryHandler>();

            // Act
            var handler = new IndTrace.Application.BarCodes.Queries.GetBarCodeDetail.GetBarCodeReportQueryHandler(
                dataLoader,
                mapper,
                barCodeResult,
                logger);

            // Assert
            handler.ShouldNotBeNull();
        }
    }
}
