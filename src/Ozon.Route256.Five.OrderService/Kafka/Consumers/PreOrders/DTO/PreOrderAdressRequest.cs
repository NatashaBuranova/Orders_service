namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders.DTO;

public record PreOrderAddressRequest(
    string Region,
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude);
