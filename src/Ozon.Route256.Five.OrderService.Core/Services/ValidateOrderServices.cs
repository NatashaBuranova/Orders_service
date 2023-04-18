using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Core.Models.Enums;
using Ozon.Route256.Five.OrderService.Helpers;

namespace Ozon.Route256.Five.OrderService.Core.Services;

public class ValidateOrderServices : IValidateOrderServices
{
    private static readonly HashSet<OrderState> ForbiddenToCancelStates = new()
    {
        OrderState.Cancelled,
        OrderState.Delivered
    };

    public bool IsCanCancelOrder(OrderState state)
    {
        if (ForbiddenToCancelStates.Contains(state))
            return false;

        return true;
    }

    public bool IsOrderValid(Order order)
    {
        if (order.DeliveryAddress == null || order.Region == null)
            return false;

        var distance = DistanceCalculator.CalculateDistance(new Point(order.DeliveryAddress.Latitude, order.DeliveryAddress.Longitude),
            new Point(order.Region.StockLatitude, order.Region.StockLongitude));

        if (distance > 5)
            return false;

        return true;
    }
}
