using Dapper;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.Infrastructure;
using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Repositories.ShardImp;

public class RegionShardRepository : IRegionRepository
{

    private readonly string _getAllQuery = $@"
SELECT 
    id {nameof(Region.Id)}, 
    name {nameof(Region.Name)}, 
    stockcoordinate[0] {nameof(Region.StockLatitude)}, 
    stockcoordinate[1] {nameof(Region.StockLongitude)} 
FROM __bucket__.region";

    private readonly string _getByNameQuery = $@"
SELECT 
    id {nameof(Region.Id)}, 
    name {nameof(Region.Name)}, 
    stockcoordinate[0] {nameof(Region.StockLatitude)}, 
    stockcoordinate[1] {nameof(Region.StockLongitude)} 
FROM __bucket__.region where LOWER(name) = LOWER(@regionName)";

    private readonly string _IsExistQuery = @"SELECT EXISTS (SELECT * FROM __bucket__.region where id = @regionId)";

    private readonly IShardConnectionFactory _connectionFactory;
    private readonly IShardingRule<long> _shardingRule;
    private readonly int _bucketsCount;

    public RegionShardRepository(IShardConnectionFactory connectionFactory,
        IShardingRule<long> shardingRule,
        IDbStore dbStore)
    {
        _connectionFactory = connectionFactory;
        _shardingRule = shardingRule;
        _bucketsCount = dbStore.BucketsCount;
    }


    public async Task<Region?> FindAsync(string regionName, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<Region[]> GetAllAsync(CancellationToken token)
    {
        var regions = new List<Region>();

        for (var i = 0; i < _bucketsCount; i++)
        {
            await using var connection = await _connectionFactory.GetConnectionByBucketAsync(i, token);
            var result = await connection.QueryAsync<Region>(_getAllQuery);

            regions.AddRange(result);
        }

        return regions.ToArray();
    }

    public async Task<bool> IsExistsAsync(long regionId, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionByKeyAsync(regionId, token);
        var result = await connection.QueryFirstOrDefaultAsync<bool>(_IsExistQuery, new { regionId });
        return result;
    }
}
