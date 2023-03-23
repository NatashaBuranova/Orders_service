using Dapper;
using Ozon.Route256.Five.OrderService.Infrastructure;
using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Repositories.DataBaseImp;

public class RegionRepository : IRegionRepository
{
    private readonly string _getAllQuery = $@"
SELECT 
    id {nameof(Region.Id)}, 
    name {nameof(Region.Name)}, 
    stockcoordinate[0] {nameof(Region.StockLatitude)}, 
    stockcoordinate[1] {nameof(Region.StockLongitude)} 
FROM region";

    private readonly string _getByNameQuery = $@"
SELECT 
    id {nameof(Region.Id)}, 
    name {nameof(Region.Name)}, 
    stockcoordinate[0] {nameof(Region.StockLatitude)}, 
    stockcoordinate[1] {nameof(Region.StockLongitude)} 
FROM region where LOWER(name) = LOWER(@regionName)";

    private readonly string _getByIdQuery = $@"
SELECT EXISTS (SELECT 
    id {nameof(Region.Id)}, 
    name {nameof(Region.Name)}, 
    stockcoordinate[0] {nameof(Region.StockLatitude)}, 
    stockcoordinate[1] {nameof(Region.StockLongitude)} 
FROM region where id = @regionId)";

    private readonly IPostgresConnectionFactory _connectionFactory;

    public RegionRepository(IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Region?> FindAsync(string regionName, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<Region>(_getByNameQuery, new { regionName });
        return result;
    }

    public async Task<Region[]> GetAllAsync(CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        var result = await connection.QueryAsync<Region>(_getAllQuery);
        return result.ToArray();
    }

    public async Task<bool> IsExistsAsync(long regionId, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<bool>(_getByIdQuery, new { regionId });

        return result;
    }
}
