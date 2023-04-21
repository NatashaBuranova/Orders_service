using Ozon.Route256.Five.OrderService.Core.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Core.Helpers;

public static class StateOrderMapping
{
    public static OrderState GetStateOrderFromName(string stateName) =>
        stateName switch
        {
            "Created" => OrderState.Created,
            "SentToCustomer" => OrderState.SentToCustomer,
            "Delivered" => OrderState.Delivered,
            "Lost" => OrderState.Lost,
            "Cancelled" => OrderState.Cancelled,
            _ => OrderState.Cancelled
        };
}
