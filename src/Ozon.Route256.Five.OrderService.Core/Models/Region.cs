﻿namespace Ozon.Route256.Five.OrderService.Core.Models;

public class Region
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public double StockLatitude { get; set; }
    public double StockLongitude { get; set; }
}
