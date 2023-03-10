﻿namespace Ozon.Route256.Five.OrderService.Models;

public class Adress
{
    public long RegionId { get; set; }
    public Region Region { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string Building { get; set; }
    public string Apartment { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
