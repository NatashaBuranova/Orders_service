﻿using Dapper;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Infrastructure;
using Ozon.Route256.Five.OrderService.Models;
using static Dapper.SqlMapper;

namespace Ozon.Route256.Five.OrderService.Repositories.DataBaseImp;

public class OrderRepository : IOrderRepository
{
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
    customer.id {nameof(Models.Client.Id)},
    customer.first_name {nameof(Models.Client.FirstName)},
    customer.last_name {nameof(Models.Client.LastName)},
    customer.phone_number {nameof(Models.Client.Telephone)},
    region.id {nameof(Region.Id)},
    region.name {nameof(Region.Name)},
    region.stockcoordinate[0] {nameof(Region.StockLatitude)}, 
    region.stockcoordinate[1] {nameof(Region.StockLongitude)} 
FROM ""order"" 
INNER JOIN customer ON ""order"".customer_id = customer.id
INNER JOIN region ON ""order"".region_id = region.id
where ""order"".id = @orderId";

    private readonly string _insertQuery = $@"
INSERT INTO ""order""
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

    private readonly string _updateQuery = $@"
UPDATE ""order""
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
    customer.id {nameof(Models.Client.Id)},
    customer.first_name {nameof(Models.Client.FirstName)},
    customer.last_name {nameof(Models.Client.LastName)},
    customer.phone_number {nameof(Models.Client.Telephone)},
    region.id {nameof(Region.Id)},
    region.name {nameof(Region.Name)},
    region.stockcoordinate[0] {nameof(Region.StockLatitude)}, 
    region.stockcoordinate[1] {nameof(Region.StockLongitude)} 
FROM ""order"" 
INNER JOIN customer ON ""order"".customer_id = customer.id
INNER JOIN region ON ""order"".region_id = region.id
where customer_id = @ClientId AND date_create >= @DateStart";

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
FROM ""order"" 
INNER JOIN customer ON ""order"".customer_id = customer.id
INNER JOIN region ON ""order"".region_id = region.id
where (( cardinality(@RegionIds) > 0 AND region_id = ANY(@RegionIds)) OR cardinality(@RegionIds)= 0) AND date_create >= @DateStart";

    private readonly string _getOrdersForRegionPerPage = $@"
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
FROM ""order"" 
INNER JOIN customer ON ""order"".customer_id = customer.id
INNER JOIN region ON ""order"".region_id = region.id
where (( cardinality(@RegionIds) > 0 AND region_id = ANY(@RegionIds)) OR cardinality(@RegionIds)= 0) AND
      (@Type is NULL OR type=@Type)
LIMIT @Take OFFSET @Skip";

    private readonly string _getOrdersForRegionPerPageOrderByRegion = $@"
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
FROM ""order"" 
INNER JOIN customer ON ""order"".customer_id = customer.id
INNER JOIN region ON ""order"".region_id = region.id
where (( cardinality(@RegionIds) > 0 AND region_id = ANY(@RegionIds)) OR cardinality(@RegionIds)= 0) AND
      (@Type is NULL OR type=@Type)
order by region_id
LIMIT @Take OFFSET @Skip";

    private readonly IPostgresConnectionFactory _connectionFactory;

    public OrderRepository(IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Order?> FindAsync(long orderId, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        var result = await connection.QueryAsync<Order, Models.Client, Region, Order>(_getByIdQuery, (order, customer, region) =>
        {
            order.Client = customer;
            order.Region = region;
            return order;
        }, param: new { orderId }, splitOn: "id,id");

        return result.FirstOrDefault();
    }

    public async Task<Order[]> GetOrdersForClientByTimeAsync(DateTimeOffset dateStart, int clientId, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        var result = await connection.QueryAsync<Order, Models.Client, Region, Order>(_getOrdersForClientByTime, (order, customer, region) =>
        {
            order.Client = customer;
            order.Region = region;
            return order;
        }, param: new { DateStart = dateStart, ClientId = clientId }, splitOn: "id,id");

        return result.ToArray();
    }

    public async Task<Order[]> GetOrdersListByRegionsAndDateTime(DateTimeOffset dateStart, List<long> regionIds, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        var result = await connection.QueryAsync<Order, Models.Client, Region, Order>(_getOrdersForRegionByTime, (order, customer, region) =>
        {
            order.Client = customer;
            order.Region = region;
            return order;
        }, param: new { DateStart = dateStart, RegionIds = regionIds }, splitOn: "id,id");

        return result.ToArray();
    }

    public async Task<Order[]> GetOrdersListWithFiltersByPageAsync(OrdersListWithFiltersRequest filters, CancellationToken token)
    {
        var skip = filters.OnPage * (filters.CurrentPage - 1);

        await using var connection = await _connectionFactory.GetConnectionAsync();

        var qeury = filters.IsOrderByFilter ? _getOrdersForRegionPerPageOrderByRegion : _getOrdersForRegionPerPage;
        var result = await connection.QueryAsync<Order, Models.Client, Region, Order>(qeury, (order, customer, region) =>
        {
            order.Client = customer;
            order.Region = region;
            return order;
        }, param: new { RegionIds = filters.RegionFilterIds, Type = filters.TypeOrder, Skip = skip, Take = filters.OnPage }, splitOn: "id,id");

        return result.ToArray();
    }

    public async Task InsertAsync(Order newOrder, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        await connection.ExecuteAsync(_insertQuery, newOrder);
    }

    public async Task UpdateAsync(Order order, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        await connection.ExecuteAsync(_updateQuery, order);
    }
}
