namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public class OrderResponse
{
    public long Id { get; set; }
    public int CountProduct { get; set; }
    public long TotalSumm { get; set; }
    public long TotalWeight { get; set; }
    public DateTime DateCreate { get; set; }
    public int Status { get; set; }
    public Adress DeliveryAddress { get; set; } = new Adress();
    public long Telephone { get; set; }
    public Client Client { get; set; } = new Client();
}

public class Client
{
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
}

public class Adress
{
    public string? Region { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Building { get; set; }
    public string? Apartment { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

