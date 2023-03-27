﻿using Ozon.Route256.Five.OrderService.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Models;

public class Order
{
    public long Id { get; set; }
    public int CountProduct { get; set; }
    public long TotalSumm { get; set; }
    public long TotalWeight { get; set; }
    public DateTimeOffset DateCreate { get; set; }
    public OrderState State { get; set; }
    public OrderType Type { get; set; }
    public long DeliveryAddressId { get; set; }
    public Address DeliveryAddress { get; set; } = new();
    public Client Client { get; set; }
    public int ClientId { get; set; }
    public Region Region { get; set; }
    public long RegionId { get; set; }
}
