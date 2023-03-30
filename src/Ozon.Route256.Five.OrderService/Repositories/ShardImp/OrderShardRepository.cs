using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Repositories.ShardImp;

public class OrderShardRepository : IOrderRepository
{

    private readonly string _insertQuery = $@"
INSERT INTO __bucket__.""order""
    (date_create, 
    date_update, 
    state, 
    type, 
    shipments_count, 
    price, 
    weight,     
    address,
    customer_id, 
    region_id)
VALUES ( @{nameof(Order.DateCreate)},
@{nameof(Order.DateUpdate)},
@{nameof(Order.State)},
@{nameof(Order.Type)},
@{nameof(Order.CountProduct)},
@{nameof(Order.TotalSumm)},
@{nameof(Order.TotalWeight)},
@{nameof(Order.DeliveryAddress)}::jsonb,
@{nameof(Order.ClientId)},
@{nameof(Order.RegionId)})";


    public Task<Order?> FindAsync(long orderId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<Order[]> GetOrdersForClientByTimePerPageAsync(OrdersForClientByTimeRequest filters, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<Order[]> GetOrdersListByRegionsAndDateTimeAsync(DateTimeOffset dateStart, List<long> regionIds, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<Order[]> GetOrdersListWithFiltersByPageAsync(OrdersListWithFiltersRequest filters, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task InsertAsync(Order newOrder, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Order order, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
