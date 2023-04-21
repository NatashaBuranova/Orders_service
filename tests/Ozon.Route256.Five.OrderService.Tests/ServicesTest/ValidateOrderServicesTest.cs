using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Core.Models.Enums;
using Ozon.Route256.Five.OrderService.Core.Services;

namespace Ozon.Route256.Five.OrderService.Tests.ServicesTest;

public class ValidateOrderServicesTest
{
    [Fact]
    public void IsOrderValid_ValidOrder_ReturnTrue()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            DeliveryAddress = new Address { Latitude = 43.7384, Longitude = 7.4246 },
            Region = new Region { StockLatitude = 43.7102, StockLongitude = 7.4246 }
        };
        var service = new ValidateOrderServices();

        // Act
        var isValid = service.IsOrderValid(order);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsOrderValidMoreDistanceThen5КМReturnFslse()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            DeliveryAddress = new Address { Latitude = 55.7558, Longitude = 37.6173 },
            Region = new Region { StockLatitude = 59.9343, StockLongitude = 30.3351 }
        };
        var service = new ValidateOrderServices();

        // Act
        var isValid = service.IsOrderValid(order);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsCanCancelOrder_ReturnsTrue_ForCreatedState()
    {
        // Arrange
        var service = new ValidateOrderServices();
        OrderState state = OrderState.Created;

        // Act
        bool canCancel = service.IsCanCancelOrder(state);

        // Assert
        Assert.True(canCancel);
    }

    [Theory]
    [InlineData(OrderState.SentToCustomer)]
    [InlineData(OrderState.Lost)]
    [InlineData(OrderState.Created)]
    public void IsCanCancelOrder_ValidState_ReturnsTrue(OrderState state)
    {
        // Arrange
        var service = new ValidateOrderServices();

        // Act
        bool canCancel = service.IsCanCancelOrder(state);

        // Assert
        Assert.True(canCancel);
    }

    [Theory]
    [InlineData(OrderState.Delivered)]
    [InlineData(OrderState.Cancelled)]
    public void IsCanCancelOrder_NotValidState_ReturnsFalse(OrderState state)
    {
        // Arrange
        var service = new ValidateOrderServices();

        // Act
        bool canCancel = service.IsCanCancelOrder(state);

        // Assert
        Assert.False(canCancel);
    }
}
