using Ozon.Route256.Five.OrderService.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;

public record PreOrderDto(long Id, OrderType Source, PreOrderCustomerDto Customer, List<PreOrderGoods> Goods);

public record PreOrderCustomerDto(int Id, PreOrderAdressDTO Address);

public record PreOrderAdressDTO(
    string Region,
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude);

public record PreOrderGoods(long Id, string Name, int Quantity, double Price, long Weight);
