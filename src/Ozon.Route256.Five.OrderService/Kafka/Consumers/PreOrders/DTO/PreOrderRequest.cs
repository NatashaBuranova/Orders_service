using Ozon.Route256.Five.OrderService.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders.DTO;

public record PreOrderRequest(long Id, OrderType Source, PreOrderCustomerRequest Customer, List<PreOrderGoodsRequest> Goods);
