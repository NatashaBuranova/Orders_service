using Dapper;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Infrastructure;
using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Repositories.ShardImp;

public class OrderShardRepository : IOrderRepository
{

    private readonly string _insertQuery = $@"
INSERT INTO __bucket__.""order""
    ( id,
    date_create, 
    date_update, 
    state, 
    type, 
    shipments_count, 
    price, 
    weight,     
    address,
    customer_id, 
    region_id)
VALUES ( @{nameof(Order.Id)},
@{nameof(Order.DateCreate)},
@{nameof(Order.DateUpdate)},
@{nameof(Order.State)},
@{nameof(Order.Type)},
@{nameof(Order.CountProduct)},
@{nameof(Order.TotalSumm)},
@{nameof(Order.TotalWeight)},
@{nameof(Order.DeliveryAddress)}::jsonb,
@{nameof(Order.ClientId)},
@{nameof(Order.RegionId)})";

    private readonly string _updateQuery = $@"
UPDATE __bucket__.""order""
SET date_create = @{nameof(Order.DateCreate)}, 
date_update = @{nameof(Order.DateUpdate)}, 
state = @{nameof(Order.State)}, 
type = @{nameof(Order.Type)}, 
shipments_count = @{nameof(Order.CountProduct)}, 
price = @{nameof(Order.TotalSumm)}, 
weight = @{nameof(Order.TotalWeight)},     
address = @{nameof(Order.DeliveryAddress)}::jsonb,
customer_id = @{nameof(Order.ClientId)}, 
region_id = @{nameof(Order.RegionId)}
where ""order"".id = @{nameof(Order.Id)}";

    private readonly string _getOrdersForRegionByTime = $@"
SELECT 
    ""order"".id {nameof(Order.Id)}, 
    date_create {nameof(Order.DateCreate)}, 
    date_update {nameof(Order.DateUpdate)}, 
    state {nameof(Order.State)}, 
    type {nameof(Order.Type)}, 
    shipments_count {nameof(Order.CountProduct)}, 
    price {nameof(Order.TotalSumm)}, 
    weight {nameof(Order.TotalWeight)},     
    address as {nameof(Order.DeliveryAddress)},
    customer_id {nameof(Order.ClientId)}, 
    region_id {nameof(Order.RegionId)},
    customer.id {nameof(Models.Client.Id)},
    customer.first_name {nameof(Models.Client.FirstName)},
    customer.last_name {nameof(Models.Client.LastName)},
    customer.phone_number {nameof(Models.Client.Telephone)},
    region.id {nameof(Region.Id)},
    region.name {nameof(Region.Name)},
    region.stockcoordinate[0] {nameof(Region.StockLatitude)}, 
    region.stockcoordinate[1] {nameof(Region.StockLongitude)} 
FROM __bucket__.""order"" 
INNER JOIN __bucket__.customer ON ""order"".customer_id = customer.id
INNER JOIN __bucket__.region ON ""order"".region_id = region.id
where (( cardinality(@RegionIds) > 0 AND region_id = ANY(@RegionIds)) OR cardinality(@RegionIds)= 0) AND date_create >= @DateStart";

    private readonly string _getOrdersForRegionAndType = $@"
SELECT 
    ""order"".id {nameof(Order.Id)}, 
    date_create {nameof(Order.DateCreate)}, 
    date_update {nameof(Order.DateUpdate)}, 
    state {nameof(Order.State)}, 
    type {nameof(Order.Type)}, 
    shipments_count {nameof(Order.CountProduct)}, 
    price {nameof(Order.TotalSumm)}, 
    weight {nameof(Order.TotalWeight)},     
    address as {nameof(Order.DeliveryAddress)},
    customer_id {nameof(Order.ClientId)}, 
    region_id {nameof(Order.RegionId)},
    customer.id {nameof(Models.Client.Id)},
    customer.first_name {nameof(Models.Client.FirstName)},
    customer.last_name {nameof(Models.Client.LastName)},
    customer.phone_number {nameof(Models.Client.Telephone)},
    region.id {nameof(Region.Id)},
    region.name {nameof(Region.Name)},
    region.stockcoordinate[0] {nameof(Region.StockLatitude)}, 
    region.stockcoordinate[1] {nameof(Region.StockLongitude)} 
FROM __bucket__.""order"" 
INNER JOIN __bucket__.customer ON ""order"".customer_id = customer.id
INNER JOIN __bucket__.region ON ""order"".region_id = region.id
where (( cardinality(@RegionIds) > 0 AND region_id = ANY(@RegionIds)) OR cardinality(@RegionIds)= 0) AND
      (@Type is NULL OR type=@Type)";


    private readonly IShardConnectionFactory _connectionFactory;
    private readonly IShardingRule<long> _shardingRule;
    private readonly int _bucketsCount;

    public OrderShardRepository(IShardConnectionFactory connectionFactory,
        IShardingRule<long> shardingRule,
        IDbStore dbStore)
    {
        _connectionFactory = connectionFactory;
        _shardingRule = shardingRule;
        _bucketsCount = dbStore.BucketsCount;
    }

    public Task<Order?> FindAsync(long orderId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<Order[]> GetOrdersForClientByTimePerPageAsync(OrdersForClientByTimeRequest filters, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<Order[]> GetOrdersListByRegionsAndDateTimeAsync(DateTimeOffset dateStart, List<long> regionIds, CancellationToken token)
    {
        var orders = new List<Order>();

        foreach (var regionId in regionIds)
        {
            await using var connection = await _connectionFactory.GetConnectionByKeyAsync(regionId, token);
            var result = await connection.QueryAsync<Order, Models.Client, Region, Order>(_getOrdersForRegionByTime, (order, customer, region) =>
            {
                order.Client = customer;
                order.Region = region;
                return order;
            }, param: new { DateStart = dateStart, RegionIds = regionIds }, splitOn: "id,id");

            orders.AddRange(result);
        }

        return orders.ToArray();
    }

    public async Task<Order[]> GetOrdersListWithFiltersByPageAsync(OrdersListWithFiltersRequest filters, CancellationToken token)
    {
        var orders = new List<Order>();
        var bucketToIdsMap = filters.RegionFilterIds?
                 .Select(id => (BucketId: _shardingRule.GetBucketId(id, _bucketsCount), Id: id))
                 .GroupBy(x => x.BucketId)
                 .ToDictionary(
                     g => g.Key,
                     g => g.Select(x => x.Id).ToArray())
             ?? new Dictionary<int, long[]>();

        foreach (var (bucketId, idsInBucket) in bucketToIdsMap)
        {
            await using var connection = await _connectionFactory.GetConnectionByBucketAsync(bucketId, token);

            var result = await connection.QueryAsync<Order, Models.Client, Region, Order>(_getOrdersForRegionAndType, (order, customer, region) =>
            {
                order.Client = customer;
                order.Region = region;
                return order;
            },
            param: new { RegionIds = idsInBucket, Type = filters.TypeOrder }, splitOn: "id,id");

            orders.AddRange(orders);
        }

        if (filters.IsOrderByFilter) orders = orders.OrderBy(x => x.RegionId).ToList();
        var skip = filters.PageSize * (filters.CurrentPage - 1);

        return orders.Skip(skip).Take(filters.PageSize).ToArray();
    }

    public async Task InsertAsync(Order newOrder, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionByKeyAsync(newOrder.RegionId, token);
        await connection.ExecuteAsync(_insertQuery, newOrder);
    }

    public async Task UpdateAsync(Order order, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionByKeyAsync(order.RegionId, token);
        await connection.ExecuteAsync(_updateQuery, order);
    }
}
