using Ozon.Route256.Five.OrderService.Core.Helpers;
using Ozon.Route256.Five.OrderService.Core.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Tests.HelpersTest;

public class StateOrderMappingTests
{
    [Theory]
    [InlineData("Created", OrderState.Created)]
    [InlineData("SentToCustomer", OrderState.SentToCustomer)]
    [InlineData("Delivered", OrderState.Delivered)]
    [InlineData("Lost", OrderState.Lost)]
    [InlineData("Cancelled", OrderState.Cancelled)]
    public void GetStateOrderFromName_Returns_Correct_OrderState_For_StateName(string stateName, OrderState expectedState)
    {
        // Act
        OrderState result = StateOrderMapping.GetStateOrderFromName(stateName);

        // Assert
        Assert.Equal(expectedState, result);
    }
}
