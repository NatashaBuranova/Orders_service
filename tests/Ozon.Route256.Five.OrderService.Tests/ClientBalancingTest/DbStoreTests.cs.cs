using Ozon.Route256.Five.OrderService.ClientBalancing;

namespace OzonRoute256.Five.OrderService.Tests.ClientBalancingTest;

public class DbStoreTests
{
    private readonly DbStore _dbStore;

    public DbStoreTests()
    {
        _dbStore = new DbStore();
    }

    //[Fact]
    //public async Task GetNextEndpointAsync_Successful()
    //{
    //    //Arrange
    //    var dbEndpoints = new[]
    //        {
    //        new DbEndpoint("testHost1", DbReplicaType.Master),
    //        new DbEndpoint("testHost2", DbReplicaType.Master),
    //        };
    //    await _dbStore.UpdateEndpointsAsync(dbEndpoints);

    //    //Act
    //    var resultFirst = await _dbStore.GetNextEndpointAsync();
    //    var resultSecond = await _dbStore.GetNextEndpointAsync();
    //    var resultThird = await _dbStore.GetNextEndpointAsync();

    //    //Assert
    //    Assert.Equal(dbEndpoints[0].HostAndPort,resultFirst.HostAndPort);
    //    Assert.Equal(dbEndpoints[1].HostAndPort, resultSecond.HostAndPort);
    //    Assert.Equal(dbEndpoints[0].HostAndPort, resultThird.HostAndPort);
    //}
}