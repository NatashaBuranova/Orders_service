namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders.DTO;

public record PreOrderGoodsRequest(long Id, string Name, int Quantity, double Price, long Weight);
