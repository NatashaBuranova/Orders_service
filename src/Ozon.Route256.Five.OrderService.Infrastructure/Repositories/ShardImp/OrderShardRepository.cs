using Dapper;
using Microsoft.Extensions.Configuration;
using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Infrastructure.ClientBalancing;
using Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection.ShardConnection;
using Ozon.Route256.Five.OrderService.Infrastructure.Repositories.DTO;

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
    customer.id {nameof(Client.Id)},
    customer.first_name {nameof(Client.FirstName)},
    customer.last_name {nameof(Client.LastName)},
    customer.phone_number {nameof(Client.Telephone)},
    region.id {nameof(Region.Id)},
    region.name {nameof(Region.Name)},
    region.stock_coordinate[0] {nameof(Region.StockLatitude)}, 
    region.stock_coordinate[1] {nameof(Region.StockLongitude)} 
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
    customer.id {nameof(Client.Id)},
    customer.first_name {nameof(Client.FirstName)},
    customer.last_name {nameof(Client.LastName)},
    customer.phone_number {nameof(Client.Telephone)},
    region.id {nameof(Region.Id)},
    region.name {nameof(Region.Name)},
    region.stock_coordinate[0] {nameof(Region.StockLatitude)}, 
    region.stock_coordinate[1] {nameof(Region.StockLongitude)} 
FROM __bucket__.""order"" 
INNER JOIN __bucket__.customer ON ""order"".customer_id = customer.id
INNER JOIN __bucket__.region ON ""order"".region_id = region.id
where (( cardinality(@RegionIds) > 0 AND region_id = ANY(@RegionIds)) OR cardinality(@RegionIds)= 0) AND
      (@Type is NULL OR type=@Type)";

    private readonly string _indexQuery = @"
select region_id from __bucket__.index_region_client
where client_id = @ClientId";

    private readonly string _getOrdersForClientByTime = $@"
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
    customer.id {nameof(Client.Id)},
    customer.first_name {nameof(Client.FirstName)},
    customer.last_name {nameof(Client.LastName)},
    customer.phone_number {nameof(Client.Telephone)},
    region.id {nameof(Region.Id)},
    region.name {nameof(Region.Name)},
    region.stock_coordinate[0] {nameof(Region.StockLatitude)}, 
    region.stock_coordinate[1] {nameof(Region.StockLongitude)} 
FROM __bucket__.""order"" 
INNER JOIN __bucket__.customer ON ""order"".customer_id = customer.id
INNER JOIN __bucket__.region ON ""order"".region_id = region.id
where customer_id = @ClientId AND date_create >= @DateStart";

    private readonly string _insertIndexOrderQuery = @"
INSERT into  __bucket__.index_region_order (
    region_id,
    order_id)
VALUES (
    @RegionId,
    @OrderId)";

    private readonly string _indexForOrderQuery = @"
select region_id from __bucket__.index_region_order
where order_id = @OrderId";

    private readonly string _getByIdQuery = $@"
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
    customer.id {nameof(Client.Id)},
    customer.first_name {nameof(Client.FirstName)},
    customer.last_name {nameof(Client.LastName)},
    customer.phone_number {nameof(Client.Telephone)},
    region.id {nameof(Region.Id)},
    region.name {nameof(Region.Name)},
    region.stock_coordinate[0] {nameof(Region.StockLatitude)}, 
    region.stock_coordinate[1] {nameof(Region.StockLongitude)} 
FROM __bucket__.""order"" 
INNER JOIN __bucket__.customer ON ""order"".customer_id = customer.id
INNER JOIN __bucket__.region ON ""order"".region_id = region.id
where ""order"".id = @orderId";

    private readonly IShardConnectionFactory _connectionFactory;
    private readonly IShardingRule<long> _shardingRule;
    private readonly int _bucketsCount;

    public OrderShardRepository(IShardConnectionFactory connectionFactory,
        IShardingRule<long> shardingRule,
        IConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _shardingRule = shardingRule;
        _bucketsCount = configuration.GetSection("DataBase:BacketCount").Get<int>(); 
    }

    public async Task<Order?> FindAsync(long orderId, CancellationToken token)
    {
        await using var connectionIndex = await _connectionFactory.GetConnectionByKeyAsync(orderId, token);
        var regionId = await connectionIndex.QueryFirstOrDefaultAsync<long>(_indexForOrderQuery, new { OrderId = orderId });

        await using var connection = await _connectionFactory.GetConnectionByKeyAsync(regionId, token);
        var result = await connection.QueryAsync<Order, Client, Region, Order>(_getByIdQuery, (order, customer, region) =>
        {
            order.Client = customer;
            order.Region = region;
            return order;
        }, param: new { orderId }, splitOn: "id,id");

        return result.FirstOrDefault();
    }

    public async Task<Order[]> GetOrdersForClientByTimePerPageAsync(OrdersForClientByTimeRepositoryRequest filters, CancellationToken token)
    {
        await using var connectionIndex = await _connectionFactory.GetConnectionByKeyAsync(filters.ClientId, token);
        var regionIds = await connectionIndex.QueryAsync<long>(_indexQuery, new { filters.ClientId });

        var bucketToIdsMap = regionIds?
                 .Select(id => (BucketId: _shardingRule.GetBucketId(id, _bucketsCount), Id: id))
                 .GroupBy(x => x.BucketId)
                 .ToDictionary(
                     g => g.Key,
                     g => g.Select(x => x.Id).ToArray())
             ?? new Dictionary<int, long[]>();

        var orders = new List<Order>();

        foreach (var (bucketId, idsInBucket) in bucketToIdsMap)
        {
            await using var connection = await _connectionFactory.GetConnectionByBucketAsync(bucketId, token);
            var result = await connection.QueryAsync<Order, Client, Region, Order>(_getOrdersForClientByTime, (order, customer, region) =>
            {
                order.Client = customer;
                order.Region = region;
                return order;
            }, param: new { DateStart = filters.StartPeriod, ClientId = filters.ClientId }, splitOn: "id,id");

            orders.AddRange(result);
        }

        var skip = filters.PageSize * (filters.CurrentPage - 1);

        return orders.Skip(skip).Take(filters.PageSize).ToArray();
    }

    public async Task<Order[]> GetOrdersListByRegionsAndDateTimeAsync(DateTimeOffset dateStart, List<long> regionIds, CancellationToken token)
    {
        var orders = new List<Order>();

        foreach (var regionId in regionIds)
        {
            await using var connection = await _connectionFactory.GetConnectionByKeyAsync(regionId, token);
            var result = await connection.QueryAsync<Order, Client, Region, Order>(_getOrdersForRegionByTime, (order, customer, region) =>
            {
                order.Client = customer;
                order.Region = region;
                return order;
            }, param: new { DateStart = dateStart, RegionIds = regionIds }, splitOn: "id,id");

            orders.AddRange(result);
        }

        return orders.ToArray();
    }

    public async Task<Order[]> GetOrdersListWithFiltersByPageAsync(OrdersListWithFiltersRepositoryRequest filters, CancellationToken token)
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

            var result = await connection.QueryAsync<Order, Client, Region, Order>(_getOrdersForRegionAndType, (order, customer, region) =>
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
        await using var connectionIndex = await _connectionFactory.GetConnectionByKeyAsync(newOrder.Id, token);
        await connectionIndex.ExecuteAsync(_insertIndexOrderQuery, new { RegionId = newOrder.RegionId, OrderId = newOrder.Id });

        await using var connection = await _connectionFactory.GetConnectionByKeyAsync(newOrder.RegionId, token);
        await connection.ExecuteAsync(_insertQuery, newOrder);
    }

    public async Task UpdateAsync(Order order, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionByKeyAsync(order.RegionId, token);
        await connection.ExecuteAsync(_updateQuery, order);
    }
}
