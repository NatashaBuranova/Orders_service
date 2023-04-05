using Ozon.Route256.Five.OrderService.Helpers;

namespace Ozon.Route256.Five.OrderService.Tests.Helpers;

public class DistanceCalculatorTests
{
    [Fact]
    public void CalculateDistance_ReturnsZero_WhenCalculatingDistanceBetweenSamePoint()
    {
        // Arrange
        var point = new Point(51.5074, -0.1278);

        // Act
        var distance = DistanceCalculator.CalculateDistance(point, point);

        // Assert
        Assert.Equal(0, distance);
    }

    [Fact]
    public void CalculateDistance_ReturnsCorrectDistance_WhenCalculatingDistanceBetweenTwoPoints()
    {
        // Arrange
        var point1 = new Point(51.5074, -0.1278);
        var point2 = new Point(48.8566, 2.3522);

        // Act
        var distance = DistanceCalculator.CalculateDistance(point1, point2);

        // Assert
        Assert.Equal(Math.Round(343.56, 2), Math.Round(distance, 2));
    }
}