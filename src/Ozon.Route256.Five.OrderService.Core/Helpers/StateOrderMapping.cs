using Ozon.Route256.Five.OrderService.Core.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Core.Helpers;

public static class StateOrderMapping
{
    public static OrderState GetStateOrderFromName(string stateName)
    {
        OrderState state = OrderState.Cancelled;

        switch (stateName)
        {
            case "Created":
                state = OrderState.Created;
                break;
            case "SentToCustomer":
                state = OrderState.SentToCustomer;
                break;
            case "Delivered":
                state = OrderState.Delivered;
                break;
            case "Lost":
                state = OrderState.Lost;
                break;
            case "Cancelled":
                state = OrderState.Cancelled;
                break;
        }

        return state;
    }
}
