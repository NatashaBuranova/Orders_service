﻿using Ozon.Route256.Five.OrderService.Helpers;
using Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;
using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Services;

public class SendNewOrder : ISendNewOrder
{
    private readonly INewOrderKafkaPublisher _newOrderKafkaPublisher;

    public SendNewOrder(INewOrderKafkaPublisher newOrderKafkaPublisher)
    {
        _newOrderKafkaPublisher = newOrderKafkaPublisher;
    }


    public async Task SendValidOrder(Order order, CancellationToken token)
    {
        if (IsOrderValid(order))
            await _newOrderKafkaPublisher.PublishToKafka(new NewOrderDTO(order.Id), token);
    }

    private static bool IsOrderValid(Order order)
    {
        if (order.DeliveryAddress == null || order.Region == null) return false;

        var distance = DistanceCalculator.CalculateDistance(new Point(order.DeliveryAddress.Latitude, order.DeliveryAddress.Latitude),
            new Point(order.Region.StockLatitude, order.Region.StockLongitude));

        if (distance > 5) return false;

        return true;
    }
}
